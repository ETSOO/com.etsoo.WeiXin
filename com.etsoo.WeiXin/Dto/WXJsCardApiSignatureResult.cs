namespace com.etsoo.WeiXin.Dto
{
    /// <summary>
    /// Weixin get Js card api signature result
    /// 微信获取Js卡券接口调用签名结果
    /// </summary>
    public class WXJsCardApiSignatureResult
    {
        /// <summary>
        /// Nonce string
        /// 随机字符串
        /// </summary>
        public string NonceStr { get; init; } = null!;

        /// <summary>
        /// Timestamp
        /// 时间戳
        /// </summary>
        public string Timestamp { get; init; } = null!;

        /// <summary>
        /// Sign type
        /// 签名类型
        /// </summary>
        public string SignType { get; init; } = null!;

        /// <summary>
        /// Card signature
        /// 卡券签名
        /// </summary>
        public string CardSign { get; init; } = null!;
    }
}
