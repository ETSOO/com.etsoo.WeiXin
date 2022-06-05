using com.etsoo.WeiXin.Dto;
using com.etsoo.WeiXin.Message;
using System.Xml;

namespace com.etsoo.WeiXin
{
    /// <summary>
    /// Weixin client interface
    /// 微信客户端接口
    /// </summary>
    public interface IWXClient
    {
        /// <summary>
        /// Check signature
        /// 检查签名
        /// </summary>
        /// <param name="input">Input data</param>
        /// <returns>Result</returns>
        ValueTask<bool> CheckSignatureAsync(IWXCheckSignatureInput input);

        /// <summary>
        /// Create signature
        /// 创建签名
        /// </summary>
        /// <param name="timestamp">Timestamp</param>
        /// <param name="nonce">Nonce</param>
        /// <returns>Result</returns>
        Task<string> CreateSignatureAsync(string timestamp, string nonce);

        /// <summary>
        /// Create Js API signature
        /// 创建 Js 接口签名
        /// </summary>
        /// <param name="url">Url without #</param>
        /// <returns>Result</returns>
        ValueTask<WXJsApiSignatureResult> CreateJsApiSignatureAsync(string url);

        /// <summary>
        /// Create Js Card API signature
        /// 创建 Js 卡券接口签名
        /// </summary>
        /// <param name="cardId">Card id / 卡券ID</param>
        /// <param name="code">Code / 指定的卡券code码，只能被领一次。自定义code模式的卡券必须填写，非自定义code和预存code模式的卡券不必填写</param>
        /// <param name="balance">Balance / 红包类型卡券，指定金额</param>
        /// <param name="openid">Open id / 指定领取者的openid，只有该用户能领取。bind_openid字段为true的卡券必须填写，bind_openid字段为false不必填写。</param>
        /// <returns>Result</returns>
        ValueTask<WXJsCardApiSignatureResult> CreateJsCardApiSignatureAsync(string cardId, string? code = null, decimal? balance = null, string? openid = null);

        /// <summary>
        /// Get Access Token
        /// 获取访问凭据
        /// </summary>
        /// <returns>Result</returns>
        /// <exception cref="WXClientException"></exception>
        ValueTask<string> GetAcessTokenAsync();

        /// <summary>
        /// Get Js API ticket
        /// 获取脚本接口凭证
        /// </summary>
        /// <returns>Result</returns>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="WXClientException"></exception>
        ValueTask<string> GetJsApiTicketAsync();

        /// <summary>
        /// Get Js API Card ticket
        /// 获取脚本卡券接口凭证
        /// </summary>
        /// <returns>Result</returns>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="WXClientException"></exception>
        ValueTask<string> GetJsApiCardTicketAsync();

        /// <summary>
        /// Parse message
        /// 解析消息
        /// </summary>
        /// <typeparam name="T">Generic message type</typeparam>
        /// <param name="input">Input stream</param>
        /// <param name="rq">Request data</param>
        /// <returns>Message</returns>
        Task<T?> ParseMessageAsync<T>(Stream input, WXMessageCallbackInput rq) where T : WXMessage;

        /// <summary>
        /// Parse message
        /// 解析消息
        /// </summary>
        /// <param name="input">Input stream</param>
        /// <param name="rq">Request data</param>
        /// <returns>Message</returns>
        Task<WXMessage?> ParseMessageAsync(Stream input, WXMessageCallbackInput rq);

        /// <summary>
        /// Reply message
        /// 回复消息
        /// </summary>
        /// <param name="output">output stream</param>
        /// <param name="message">Message</param>
        /// <param name="func">Reply callback</param>
        /// <returns>Task</returns>
        Task ReplyMessageAsync(Stream output, WXMessage message, Func<XmlWriter, Task> func);

        /// <summary>
        /// 发送订阅通知
        /// </summary>
        /// <param name="input">输入的信息</param>
        /// <returns>操作结果</returns>
        Task<WXApiError?> SendMessageAsync(WXSendMessageInput input);
    }
}
