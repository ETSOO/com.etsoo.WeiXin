namespace com.etsoo.WeiXin.Dto
{
    /// <summary>
    /// WeiXin check signature input data
    /// 微信检查签名输入数据
    /// </summary>
    /// <param name="Signature">Signature</param>
    /// <param name="Timestamp">Timestamp</param>
    /// <param name="Nonce">Nonce</param>
    /// <param name="Echostr">Echo string</param>
    public record WXCheckSignatureInput(string Signature, string Timestamp, string Nonce, string Echostr);
}
