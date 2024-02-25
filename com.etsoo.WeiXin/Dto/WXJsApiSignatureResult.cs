namespace com.etsoo.WeiXin.Dto
{
    /// <summary>
    /// Weixin get Js api signature result
    /// 微信获取Js接口调用签名结果
    /// </summary>
    public record WXJsApiSignatureResult
    {
        /// <summary>
        /// App id
        /// 程序编号
        /// </summary>
        public required string AppId { get; init; }

        /// <summary>
        /// Nonce string
        /// 随机字符串
        /// </summary>
        public required string NonceStr { get; init; }

        /// <summary>
        /// Timestamp
        /// 时间戳
        /// </summary>
        public required string Timestamp { get; init; }

        /// <summary>
        /// Signature
        /// 签名
        /// </summary>
        public required string Signature { get; init; }
    }
}
