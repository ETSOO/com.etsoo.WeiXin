namespace com.etsoo.WeiXin.Dto
{
    /// <summary>
    /// WeiXin message callback input data
    /// 微信消息回调输入数据
    /// </summary>
    /// <param name="Timestamp">Timestamp</param>
    /// <param name="Nonce">Nonce</param>
    /// <param name="EncryptType">Encrypt type, aes</param>
    /// <param name="MsgSignature">Message signature</param>
    public record WXMessageCallbackInput(string Timestamp, string Nonce, string? EncryptType, string? MsgSignature);
}
