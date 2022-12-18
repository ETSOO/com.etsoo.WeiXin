using com.etsoo.HTTP;
using com.etsoo.Utils;
using com.etsoo.Utils.Crypto;
using com.etsoo.Utils.Serialization;
using com.etsoo.WeiXin.Dto;
using com.etsoo.WeiXin.Message;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace com.etsoo.WeiXin
{
    /// <summary>
    /// WeiXin Client
    /// 微信客户端
    /// </summary>
    public partial class WXClient : HttpClientService, IWXClient
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

        // Json序列号特例选项
        private static readonly JsonSerializerOptions JsonOutOptions = new()
        {
            WriteIndented = false,
            PropertyNamingPolicy = new JsonSnakeNamingPolicy(),
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

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
        /// <param name="options">Options</param>
        /// <param name="secureManager">Secure manager</param>
        public WXClient(HttpClient client, WXClientOptions options, Func<string, string, string>? secureManager = null) : base(client)
        {
            if (secureManager == null)
            {
                AppId = options.AppId;
                appSecret = options.AppSecret;
                token = options.Token;
                aesKey = options.EncodingAESKey;
            }
            else
            {
                AppId = CryptographyUtils.UnsealData("AppId", options.AppId, secureManager);
                appSecret = CryptographyUtils.UnsealData("AppSecret", options.AppSecret, secureManager);
                token = CryptographyUtils.UnsealData("Token", options.Token, secureManager);
                aesKey = CryptographyUtils.UnsealData("EncodingAESKey", options.EncodingAESKey, secureManager);
            }

            Options.PropertyNamingPolicy = new JsonSnakeNamingPolicy();
        }

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="client">Client</param>
        /// <param name="section">Configuration section</param>
        /// <param name="secureManager">Secure manager</param>
        [ActivatorUtilitiesConstructor]
        public WXClient(HttpClient client, IOptions<WXClientOptions> options, Func<string, string, string>? secureManager = null)
            : this(client, options.Value, secureManager)
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
            var data = new[]
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
            var data = new SortedDictionary<string, string>
            {
                ["url"] = url
            };

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
            var data = new List<string>();
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
    }
}
