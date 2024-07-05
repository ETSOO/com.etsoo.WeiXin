namespace com.etsoo.WeiXin.Auth
{
    /// <summary>
    /// Wechat token data
    /// 微信令牌数据
    /// </summary>
    public record WechatTokenData
    {
        /// <summary>
        /// A token that can be sent to a Wechat API for access
        /// 令牌可以发送到 Google API 以获取访问权限
        /// </summary>
        public required string AccessToken { get; init; }

        /// <summary>
        /// The remaining lifetime of the access token in seconds
        /// 访问令牌的剩余生存时间（以秒为单位）
        /// </summary>
        public required int ExpiresIn { get; init; }

        /// <summary>
        /// Refresh token for user
        /// 用户刷新access_token
        /// </summary>
        public required string RefreshToken { get; init; }

        /// <summary>
        /// Wechat user unique identifier
        /// 微信用户唯一标识
        /// </summary>
        public required string OpenId { get; init; }

        /// <summary>
        /// Scope of user authorization, separated by commas (,)
        /// 用户授权的作用域，使用逗号（,）分隔
        /// </summary>
        public required string Scope { get; init; }

        /// <summary>
        /// Only when the website application has obtained the userinfo authorization of the user, this field will appear.
        /// 当且仅当该网站应用已获得该用户的userinfo授权时，才会出现该字段。
        /// </summary>
        public string? UnionId { get; init; }
    }
}
