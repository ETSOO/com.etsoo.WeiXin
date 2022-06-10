using Microsoft.AspNetCore.Mvc;

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
    public record WXMessageCallbackInput(string Openid, string Timestamp, string Nonce, string Signature) : IWXCheckSignatureInput
    {
        /// <summary>
        /// Encrypt type, aes
        /// </summary>
        [FromQuery(Name = "encrypt_type")]
        public string? EncryptType { get; set; }

        /// <summary>
        /// Message signature
        /// </summary>
        [FromQuery(Name = "msg_signature")]
        public string? MsgSignature { get; set; }
    }
}
