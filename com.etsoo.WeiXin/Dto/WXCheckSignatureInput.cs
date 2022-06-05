namespace com.etsoo.WeiXin.Dto
{
    /// <summary>
    /// WeiXin check signature data interface
    /// 微信检查签名数据接口
    /// </summary>
    public interface IWXCheckSignatureInput
    {
        /// <summary>
        /// Signature
        /// 签名
        /// </summary>
        string Signature { get; }

        /// <summary>
        /// Timestamp
        /// 时间戳
        /// </summary>
        string Timestamp { get; }

        /// <summary>
        /// Nonce
        /// 随机数
        /// </summary>
        string Nonce { get; }
    }

    /// <summary>
    /// WeiXin check signature input data
    /// 微信检查签名输入数据
    /// </summary>
    /// <param name="Signature">Signature</param>
    /// <param name="Timestamp">Timestamp</param>
    /// <param name="Nonce">Nonce</param>
    /// <param name="Echostr">Echo string</param>
    public record WXCheckSignatureInput(string Signature, string Timestamp, string Nonce, string Echostr) : IWXCheckSignatureInput;
}
