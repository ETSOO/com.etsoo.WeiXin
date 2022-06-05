using com.etsoo.HTTP;
using com.etsoo.Utils;
using com.etsoo.Utils.Crypto;
using com.etsoo.WeiXin.Dto;
using com.etsoo.WeiXin.Message;
using com.etsoo.WeiXin.Support;
using Microsoft.Extensions.Configuration;
using System.Xml;

namespace com.etsoo.WeiXin
{
    /// <summary>
    /// WeiXin Client
    /// 微信客户端
    /// </summary>
    public class WXClient : HttpClientService, IWXClient
    {
        // 获取消息解析错误
        private static (WXMessage?, Dictionary<string, string>) ParseMessageError(Dictionary<string, string> dic, string title)
        {
            dic["EtsooError"] = title;
            return (null, dic);
        }

        /// <summary>
        /// Api base uri
        /// 接口基本地址
        /// </summary>
        public const string ApiUri = "https://api.weixin.qq.com/cgi-bin/";

        /// <summary>
        /// The globally unique interface calling credentials of the official account
        /// 公众号的全局唯一接口调用凭据
        /// https://developers.weixin.qq.com/doc/offiaccount/Basic_Information/Get_access_token.html
        /// </summary>
        private static string? AccessToken;

        // Invoke credential latest valid time
        // 调用凭据最晚有效时间
        private static DateTime? AccessTokenExpired;

        /// <summary>
        /// The globally unique ticket for Js API call
        /// Js API调用的全局唯一票据
        /// https://developers.weixin.qq.com/doc/offiaccount/OA_Web_Apps/JS-SDK.html#62
        /// </summary>
        private static string? JsApiTicket;
        private static DateTime? JsApiTicketExpired;

        /// <summary>
        /// The globally unique ticket for Js API Card call
        /// Js API卡券调用的全局唯一票据
        /// https://developers.weixin.qq.com/doc/offiaccount/OA_Web_Apps/JS-SDK.html#4
        /// </summary>
        private static string? JsApiCardTicket;
        private static DateTime? JsApiCardTicketExpired;

        /// <summary>
        /// App id
        /// 程序编号
        /// </summary>
        protected string AppId { get; }

        // App secret
        private readonly string appSecret;

        // Token
        private readonly string? token;

        // EncodingAESKey
        private readonly string? aesKey;

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="client">Client</param>
        /// <param name="appId">App id</param>
        /// <param name="appSecret">App secret</param>
        /// <param name="token">Token, used for signature check</param>
        /// <param name="aesKey">Encoding AES key</param>
        public WXClient(HttpClient client, string appId, string appSecret, string? token = null, string? aesKey = null) : base(client)
        {
            AppId = appId;
            this.appSecret = appSecret;
            this.token = token;
            this.aesKey = aesKey;

            Options.PropertyNamingPolicy = new WXClientJsonNamingPolicy();
        }

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="client">Client</param>
        /// <param name="section">Configuration section</param>
        /// <param name="secureManager">Secure manager</param>
        public WXClient(HttpClient client, IConfigurationSection section, Func<string, string>? secureManager = null)
            : this(client
                  , CryptographyUtils.UnsealData(section.GetValue<string>("AppId"), secureManager)
                  , CryptographyUtils.UnsealData(section.GetValue<string>("AppSecret"), secureManager)
                  , CryptographyUtils.UnsealData(section.GetValue<string>("Token"), secureManager)
                  , CryptographyUtils.UnsealData(section.GetValue<string>("EncodingAESKey"), secureManager))
        {

        }

        /// <summary>
        /// Check signature
        /// 检查签名
        /// </summary>
        /// <param name="input">Input data</param>
        /// <returns>Result</returns>
        public async ValueTask<bool> CheckSignatureAsync(IWXCheckSignatureInput input)
        {
            // Validate token
            if (string.IsNullOrEmpty(token)) return false;

            // Signature
            var signature = await CreateSignatureAsync(input.Timestamp, input.Nonce);

            // Result
            return signature == input.Signature;
        }

        /// <summary>
        /// Create signature
        /// 创建签名
        /// </summary>
        /// <param name="timestamp">Timestamp</param>
        /// <param name="nonce">Nonce</param>
        /// <returns>Result</returns>
        public async Task<string> CreateSignatureAsync(string timestamp, string nonce)
        {
            // Source
            var data = new SortedSet<string>
            {
                token!,
                timestamp,
                nonce
            };

            // Signature
            return await WXUtils.CreateSignatureAsync(data);
        }

        /// <summary>
        /// Create Js API signature
        /// 创建 Js 接口签名
        /// </summary>
        /// <param name="url">Url without #</param>
        /// <returns>Result</returns>
        public async ValueTask<WXJsApiSignatureResult> CreateJsApiSignatureAsync(string url)
        {
            // Source data
            var data = new SortedDictionary<string, string>();
            data["url"] = url;

            // Nonce
            var nonce = CryptographyUtils.CreateRandString(RandStringKind.DigitAndLetter, 16).ToString();
            data["noncestr"] = nonce;

            // Timestamp
            var timestamp = SharedUtils.UTCToJsMiliseconds().ToString();
            data["timestamp"] = timestamp;

            // Get ticket
            var ticket = await GetJsApiTicketAsync();
            data["jsapi_ticket"] = ticket;

            // Signature
            var signature = await WXUtils.CreateSignatureAsync(data);

            // Return
            return new WXJsApiSignatureResult
            {
                AppId = AppId,
                NonceStr = nonce,
                Timestamp = timestamp,
                Signature = signature
            };
        }

        /// <summary>
        /// Create Js Card API signature
        /// 创建 Js 卡券接口签名
        /// </summary>
        /// <param name="cardId">Card id / 卡券ID</param>
        /// <param name="code">Code / 指定的卡券code码，只能被领一次。自定义code模式的卡券必须填写，非自定义code和预存code模式的卡券不必填写</param>
        /// <param name="balance">Balance / 红包类型卡券，指定金额</param>
        /// <param name="openid">Open id / 指定领取者的openid，只有该用户能领取。bind_openid字段为true的卡券必须填写，bind_openid字段为false不必填写。</param>
        /// <returns>Result</returns>
        public async ValueTask<WXJsCardApiSignatureResult> CreateJsCardApiSignatureAsync(string cardId, string? code = null, decimal? balance = null, string? openid = null)
        {
            // Source data
            var data = new SortedSet<string>();
            if (!string.IsNullOrEmpty(cardId)) data.Add(cardId);
            if (!string.IsNullOrEmpty(code)) data.Add(code);
            if (balance.HasValue) data.Add(balance.Value.ToString());
            if (!string.IsNullOrEmpty(openid)) data.Add(openid);

            // Nonce
            var nonce = CryptographyUtils.CreateRandString(RandStringKind.DigitAndLetter, 16).ToString();
            data.Add(nonce);

            // Timestamp
            var timestamp = SharedUtils.UTCToJsMiliseconds().ToString();
            data.Add(timestamp);

            // Get ticket
            var ticket = await GetJsApiCardTicketAsync();
            data.Add(ticket);

            // Signature
            var signature = await WXUtils.CreateSignatureAsync(data);

            // Return
            return new WXJsCardApiSignatureResult
            {
                NonceStr = nonce,
                Timestamp = timestamp,
                SignType = "SHA1",
                CardSign = signature
            };
        }

        /// <summary>
        /// 下载临时媒体文件
        /// </summary>
        /// <param name="mediaId">Media id</param>
        /// <param name="saveStream">Save stream</param>
        /// <returns>Task</returns>
        public async Task DownloadMediaAsync(string mediaId, Stream saveStream)
        {
            var accessToken = await GetAcessTokenAsync();
            var api = $"{ApiUri}media/get?access_token={accessToken}&media_id={mediaId}";
            await DownloadAsync(api, saveStream);
        }

        /// <summary>
        /// Get Access Token
        /// 获取访问凭据
        /// </summary>
        /// <returns>Result</returns>
        /// <exception cref="WXClientException"></exception>
        public async ValueTask<string> GetAcessTokenAsync()
        {
            if (AccessToken == null || AccessTokenExpired == null || AccessTokenExpired.Value <= DateTime.Now)
            {
                var api = $"{ApiUri}token?grant_type=client_credential&appid={AppId}&secret={appSecret}";
                var result = await GetAsync<WXAccessTokenResult, WXApiError>(api, "access_token");
                if (result.Success)
                {
                    AccessToken = result.Data.AccessToken;
                    AccessTokenExpired = DateTime.Now.AddSeconds(result.Data.ExpiresIn);
                    return AccessToken;
                }
                else
                {
                    throw new WXClientException(result.Error);
                }
            }

            return AccessToken;
        }

        /// <summary>
        /// Get Js API ticket
        /// 获取脚本接口凭证
        /// </summary>
        /// <returns>Result</returns>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="WXClientException"></exception>
        public async ValueTask<string> GetJsApiTicketAsync()
        {
            if (JsApiTicket == null || JsApiTicketExpired == null || JsApiTicketExpired.Value <= DateTime.Now)
            {
                var accessToken = await GetAcessTokenAsync();
                var api = $"{ApiUri}ticket/getticket?access_token={accessToken}&type=jsapi";

                var result = await GetAsync<WXJsApiTokenResult>(api);
                if (result == null)
                {
                    throw new NullReferenceException();
                }

                if (result.Ticket != null)
                {
                    JsApiTicket = result.Ticket;
                    JsApiTicketExpired = DateTime.Now.AddSeconds(result.ExpiresIn!.Value);
                    return JsApiTicket;
                }
                else
                {
                    throw new WXClientException(result);
                }
            }

            return JsApiTicket;
        }

        /// <summary>
        /// Get Js API Card ticket
        /// 获取脚本卡券接口凭证
        /// </summary>
        /// <returns>Result</returns>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="WXClientException"></exception>
        public async ValueTask<string> GetJsApiCardTicketAsync()
        {
            if (JsApiCardTicket == null || JsApiCardTicketExpired == null || JsApiCardTicketExpired.Value <= DateTime.Now)
            {
                var accessToken = await GetAcessTokenAsync();
                var api = $"{ApiUri}ticket/getticket?access_token={accessToken}&type=wx_card";

                var result = await GetAsync<WXJsApiTokenResult>(api);
                if (result == null)
                {
                    throw new NullReferenceException();
                }

                if (result.Ticket != null)
                {
                    JsApiCardTicket = result.Ticket;
                    JsApiCardTicketExpired = DateTime.Now.AddSeconds(result.ExpiresIn!.Value);
                    return JsApiCardTicket;
                }
                else
                {
                    throw new WXClientException(result);
                }
            }

            return JsApiCardTicket;
        }

        /// <summary>
        /// Parse message
        /// 解析消息
        /// </summary>
        /// <typeparam name="T">Generic message type</typeparam>
        /// <param name="input">Input stream</param>
        /// <param name="rq">Request data</param>
        /// <returns>Message</returns>
        public async Task<(T?, Dictionary<string, string>)> ParseMessageAsync<T>(Stream input, WXMessageCallbackInput rq) where T : WXMessage
        {
            var (message, dic) = await ParseMessageAsync(input, rq);
            if (message == null) return (null, dic);
            var target = message as T;
            if (target == null)
            {
                dic["EtsooError"] = "Message type transformation failed";
                return (null, dic);
            }
            return (target, dic);
        }

        /// <summary>
        /// Parse message
        /// 解析消息
        /// </summary>
        /// <param name="input">Input stream</param>
        /// <param name="rq">Request data</param>
        /// <returns>Message</returns>
        public async Task<(WXMessage?, Dictionary<string, string>)> ParseMessageAsync(Stream input, WXMessageCallbackInput rq)
        {
            // 验证签名，确定是否来自微信
            if (await CheckSignatureAsync(rq) is false) return (null, new Dictionary<string, string>());

            // 第一层数据
            var dic = await XmlUtils.ParseXmlAsync(input);

            // 是否加密
            if (dic.TryGetValue("Encrypt", out var encrypt))
            {
                // 必须传递了消息签名，且定义了Encoding AES key
                if (rq.MsgSignature == null || token == null || aesKey == null)
                    return ParseMessageError(dic, "No token or AES key");

                // 验证签名
                var signData = new SortedSet<string>
                {
                    token, rq.Timestamp, rq.Nonce, encrypt
                };
                var signResult = await WXUtils.CreateSignatureAsync(signData);
                if (signResult != rq.MsgSignature)
                    return ParseMessageError(dic, "Message signature not verified");

                // 解密
                var bytes = await WXUtils.MessageDecryptAsync(encrypt, aesKey);
                if (bytes == null)
                    return ParseMessageError(dic, "Message decryption failed");

                // 读取解密的第一层数据
                dic = await XmlUtils.ParseXmlAsync(SharedUtils.GetStream(bytes));
            }

            // 解析为消息对象
            var msgType = XmlUtils.GetValue<WXMessageType>(dic, "MsgType");
            if (msgType is not null)
            {
                // 是否为事件
                if (msgType == WXMessageType.@event)
                {
                    var eventType = XmlUtils.GetValue<WXEventType>(dic, "Event");
                    if (eventType is null) return ParseMessageError(dic, "No key event data");

                    return eventType switch
                    {
                        WXEventType.subscribe => (new WXSubscribeEventMessage(dic), dic),
                        WXEventType.unsubscribe => (new WXUnsubscribeEventMessage(dic), dic),
                        WXEventType.SCAN => (new WXScanEventMessage(dic), dic),
                        WXEventType.LOCATION => (new WXLocationEventMessage(dic), dic),
                        WXEventType.CLICK => (new WXClickEventMessage(dic), dic),
                        WXEventType.VIEW => (new WXViewEventMessage(dic), dic),
                        WXEventType.scancode_push => (new WXScanCodeEventMessage(dic), dic),
                        WXEventType.scancode_waitmsg => (new WXScanCodeWaitEventMessage(dic), dic),
                        WXEventType.pic_sysphoto => (new WXSysPhotoEventMessage(dic), dic),
                        WXEventType.pic_photo_or_album => (new WXAlbumPhotoEventMessage(dic), dic),
                        WXEventType.pic_weixin => (new WXWeiXinPhotoEventMessage(dic), dic),
                        WXEventType.location_select => (new WXLocationSelectEventMessage(dic), dic),
                        WXEventType.view_miniprogram => (new WXViewMiniprogramEventMessage(dic), dic),
                        WXEventType.subscribe_msg_popup_event => (new WXSubscribePopupEventMessage(dic), dic),
                        WXEventType.subscribe_msg_change_event => (new WXSubscribeManageEventMessage(dic), dic),
                        WXEventType.subscribe_msg_sent_event => (new WXSubscribeSendEventMessage(dic), dic),
                        _ => ParseMessageError(dic, "Event type not covered")
                    };
                }
                else
                {
                    return msgType switch
                    {
                        WXMessageType.text => (new WXTextMessage(dic), dic),
                        WXMessageType.image => (new WXImageMessage(dic), dic),
                        WXMessageType.voice => (new WXVoiceMessage(dic), dic),
                        WXMessageType.shortvideo => (new WXShortVideoMessage(dic), dic),
                        WXMessageType.location => (new WXLocationMessage(dic), dic),
                        WXMessageType.link => (new WXLinkMessage(dic), dic),
                        _ => ParseMessageError(dic, "MsgType not covered")
                    };
                }
            }

            return ParseMessageError(dic, "No MsgType field or not identified");
        }

        /// <summary>
        /// Reply message
        /// 回复消息
        /// </summary>
        /// <param name="output">output stream</param>
        /// <param name="message">Message</param>
        /// <param name="func">Reply callback</param>
        /// <returns>Task</returns>
        public async Task ReplyMessageAsync(Stream output, WXMessage message, Func<XmlWriter, Task> func)
        {
            // Target stream
            var isDirect = aesKey is null;
            var ts = isDirect ? output : SharedUtils.GetStream();

            // Output
            await message.ReplyAsync(ts, func);

            // Encrypt
            if (isDirect is false && token is not null && aesKey is not null)
            {
                ts.Position = 0;

                var source = await SharedUtils.StreamToStringAsync(ts);
                var encrypt = await WXUtils.MessageEncryptAsync(source, aesKey, AppId);

                var nonce = CryptographyUtils.CreateRandString(RandStringKind.DigitAndLetter, 10).ToString();
                var timestamp = SharedUtils.UTCToUnixSeconds().ToString();

                var signData = new SortedSet<string>
                {
                    token, timestamp, nonce, encrypt
                };
                var signResult = await WXUtils.CreateSignatureAsync(signData);

                await using var writer = XmlWriter.Create(output, new XmlWriterSettings { Async = true, OmitXmlDeclaration = true });

                await writer.WriteStartElementAsync(null, "xml", null);

                await XmlUtils.WriteCDataAsync(writer, "Encrypt", encrypt);
                await XmlUtils.WriteCDataAsync(writer, "MsgSignature", signResult);
                await XmlUtils.WriteCDataAsync(writer, "TimeStamp", timestamp);
                await XmlUtils.WriteCDataAsync(writer, "Nonce", nonce);

                await writer.WriteEndElementAsync();
            }
        }

        /// <summary>
        /// 发送订阅通知
        /// </summary>
        /// <param name="input">输入的信息</param>
        /// <returns>操作结果</returns>
        public async Task<WXApiError?> SendMessageAsync(WXSendMessageInput input)
        {
            var accessToken = await GetAcessTokenAsync();
            var api = $"{ApiUri}message/subscribe/bizsend?access_token={accessToken}";

            return await PostAsync<WXSendMessageInput, WXApiError>(api, input);
        }
    }
}
