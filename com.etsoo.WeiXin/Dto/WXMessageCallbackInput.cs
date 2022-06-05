namespace com.etsoo.WeiXin.Dto
{
    /// <summary>
    /// WeiXin message callback input data
    /// 微信消息回调输入数据
    /// </summary>
    /// <param name="Openid">Open id</param>
    /// <param name="Timestamp">Timestamp</param>
    /// <param name="Nonce">Nonce</param>
    /// <param name="Signature">Signature</param>
    /// <param name="EncryptType">Encrypt type, aes</param>
    /// <param name="MsgSignature">Message signature</param>
    public record WXMessageCallbackInput(string Openid, string Timestamp, string Nonce, string Signature, string? EncryptType, string? MsgSignature) : IWXCheckSignatureInput;
}
