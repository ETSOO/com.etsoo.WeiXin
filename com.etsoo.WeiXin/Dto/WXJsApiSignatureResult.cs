namespace com.etsoo.WeiXin.Dto
{
    /// <summary>
    /// Weixin get Js api signature result
    /// 微信获取Js接口调用签名结果
    /// </summary>
    public class WXJsApiSignatureResult
    {
        /// <summary>
        /// App id
        /// 程序编号
        /// </summary>
        public string AppId { get; init; } = null!;

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
        /// Signature
        /// 签名
        /// </summary>
        public string Signature { get; init; } = null!;
    }
}
