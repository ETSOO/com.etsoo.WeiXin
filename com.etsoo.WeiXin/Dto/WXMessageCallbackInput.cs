using System.Text.Json.Serialization;

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
        public string? EncryptType { get; set; }

        /// <summary>
        /// Encrypt type in snake case
        /// </summary>
        [JsonPropertyName("encrypt_type")]
        public string? EncryptTypeSnake
        {
            get { return EncryptType; }
            set { EncryptType = value; }
        }

        /// <summary>
        /// Message signature
        /// </summary>
        public string? MsgSignature { get; set; }

        /// <summary>
        /// Message signature in snake case
        /// </summary>
        [JsonPropertyName("msg_signature")]
        public string? MsgSignatureSnake
        {
            get { return MsgSignature; }
            set { MsgSignature = value; }
        }
    }
}
