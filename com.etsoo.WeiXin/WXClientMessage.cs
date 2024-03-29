﻿using com.etsoo.HTTP;
using com.etsoo.Utils;
using com.etsoo.Utils.Crypto;
using com.etsoo.Utils.String;
using com.etsoo.WeiXin.Dto;
using com.etsoo.WeiXin.Message;
using System.Xml;

namespace com.etsoo.WeiXin
{
    /// <summary>
    /// WeiXin Client - Message
    /// 微信客户端 - 消息
    /// </summary>
    public partial class WXClient : HttpClientService, IWXClient
    {
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
            if (await CheckSignatureAsync(rq) is false) return (null, []);

            // 第一层数据
            var dic = await XmlUtils.ParseXmlAsync(input);

            // 是否加密
            if (dic.TryGetValue("Encrypt", out var encrypt))
            {
                // 必须传递了消息签名，且定义了Encoding AES key
                if (rq.MsgSignature == null || token == null || aesKey == null)
                {
                    var errorItems = new List<string>();
                    if (rq.MsgSignature == null) errorItems.Add("Message signature");
                    if (token == null) errorItems.Add("Token");
                    if (aesKey == null) errorItems.Add("AES Key");
                    return ParseMessageError(dic, string.Join(", ", errorItems) + " required");
                }

                // 验证签名
                var signData = new[]
                {
                    token, rq.Timestamp, rq.Nonce, encrypt
                };

                var signResult = await WXUtils.CreateSignatureAsync(signData);
                if (signResult != rq.MsgSignature)
                {
                    var errorItems = new string[]
                    {
                        $"AppId: {StringUtils.HideData(AppId)}",
                        $"Token: {StringUtils.HideData(token)}",
                        $"Timestamp: {StringUtils.HideData(rq.Timestamp)}",
                        $"Nonce: {StringUtils.HideData(rq.Nonce)}"
                    };
                    return ParseMessageError(dic, "Message signature not verified with " + string.Join(", ", errorItems));
                }

                // 解密
                var bytes = await WXUtils.MessageDecryptAsync(encrypt, aesKey);
                if (bytes == null)
                {
                    return ParseMessageError(dic, "Message decryption failed with AES key: " + StringUtils.HideData(aesKey));
                }

                // 读取解密的第一层数据
                dic = await XmlUtils.ParseXmlAsync(SharedUtils.GetStream(bytes));
            }

            // 解析为消息对象
            var msgType = XmlUtils.GetValue<WXMessageType>(dic, "MsgType");
            if (msgType is not null)
            {
                WXMessage? message;

                // 是否为事件
                if (msgType == WXMessageType.@event)
                {
                    var eventType = XmlUtils.GetValue<WXEventType>(dic, "Event");
                    if (eventType is null) return ParseMessageError(dic, "No key event data");

                    message = eventType switch
                    {
                        WXEventType.subscribe => new WXSubscribeEventMessage(dic),
                        WXEventType.unsubscribe => new WXUnsubscribeEventMessage(dic),
                        WXEventType.SCAN => new WXScanEventMessage(dic),
                        WXEventType.LOCATION => new WXLocationEventMessage(dic),
                        WXEventType.CLICK => new WXClickEventMessage(dic),
                        WXEventType.VIEW => new WXViewEventMessage(dic),
                        WXEventType.scancode_push => new WXScanCodeEventMessage(dic),
                        WXEventType.scancode_waitmsg => new WXScanCodeWaitEventMessage(dic),
                        WXEventType.pic_sysphoto => new WXSysPhotoEventMessage(dic),
                        WXEventType.pic_photo_or_album => new WXAlbumPhotoEventMessage(dic),
                        WXEventType.pic_weixin => new WXWeiXinPhotoEventMessage(dic),
                        WXEventType.location_select => new WXLocationSelectEventMessage(dic),
                        WXEventType.view_miniprogram => new WXViewMiniprogramEventMessage(dic),
                        WXEventType.TEMPLATESENDJOBFINISH => new WXTemplateSendEventMessage(dic),
                        WXEventType.subscribe_msg_popup_event => new WXSubscribePopupEventMessage(dic),
                        WXEventType.subscribe_msg_change_event => new WXSubscribeManageEventMessage(dic),
                        WXEventType.subscribe_msg_sent_event => new WXSubscribeSendEventMessage(dic),
                        _ => null
                    };

                    if (message == null) return ParseMessageError(dic, "Event type not covered");
                }
                else
                {
                    message = msgType switch
                    {
                        WXMessageType.text => new WXTextMessage(dic),
                        WXMessageType.image => new WXImageMessage(dic),
                        WXMessageType.voice => new WXVoiceMessage(dic),
                        WXMessageType.shortvideo => new WXShortVideoMessage(dic),
                        WXMessageType.location => new WXLocationMessage(dic),
                        WXMessageType.link => new WXLinkMessage(dic),
                        _ => null
                    };

                    if (message == null) return ParseMessageError(dic, "MsgType not covered");
                }

                return (message, dic);
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

                var signData = new[]
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
    }
}
