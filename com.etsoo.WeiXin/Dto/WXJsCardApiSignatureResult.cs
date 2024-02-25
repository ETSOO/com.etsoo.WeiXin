namespace com.etsoo.WeiXin.Dto
{
    /// <summary>
    /// Weixin get Js card api signature result
    /// 微信获取Js卡券接口调用签名结果
    /// </summary>
    public record WXJsCardApiSignatureResult
    {
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
        /// Sign type
        /// 签名类型
        /// </summary>
        public required string SignType { get; init; }

        /// <summary>
        /// Card signature
        /// 卡券签名
        /// </summary>
        public required string CardSign { get; init; }
    }
}
