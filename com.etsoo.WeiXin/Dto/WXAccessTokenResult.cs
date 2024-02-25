namespace com.etsoo.WeiXin.Dto
{
    /// <summary>
    /// WeiXin get access token API result
    /// 微信获取访问令牌接口结果
    /// </summary>
    public record WXAccessTokenResult
    {
        /// <summary>
        /// Access token
        /// 访问令牌
        /// </summary>
        public required string AccessToken { get; init; }

        /// <summary>
        /// Token valid seconds
        /// 凭证有效时间，单位：秒
        /// </summary>
        public int ExpiresIn { get; init; }
    }
}
