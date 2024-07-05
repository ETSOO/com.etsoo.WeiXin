namespace com.etsoo.WeiXin.Auth
{
    /// <summary>
    /// Wechat OAuth2 user information
    /// 微信 OAuth2 用户信息
    /// </summary>
    public record WechatUserInfo
    {
        /// <summary>
        /// Normal user's unique identifier, unique to the current developer account
        /// 普通用户的标识，对当前开发者帐号唯一
        /// </summary>
        public required string OpenId { get; init; }

        /// <summary>
        /// Nickname of the user
        /// 普通用户昵称
        /// </summary>
        public required string Nickname { get; init; }

        /// <summary>
        /// Gender, 1 for male, 2 for female
        /// 普通用户性别，1为男性，2为女性
        /// </summary>
        public byte? Sex { get; init; }

        /// <summary>
        /// Country, such as CN for China
        /// 国家，如中国为CN
        /// </summary>
        public string? Country { get; init; }

        /// <summary>
        /// Province of the user's profile
        /// 普通用户个人资料填写的省份
        /// </summary>
        public string? Province { get; init; }

        /// <summary>
        /// City of the user's profile
        /// 普通用户个人资料填写的城市
        /// </summary>
        public string? City { get; init; }

        /// <summary>
        /// User's profile picture, the last number represents the square avatar size (with 0, 46, 64, 96, 132 values to choose from, 0 represents 640*640 square avatar), this item is empty when the user has no avatar
        /// 用户头像，最后一个数值代表正方形头像大小（有0、46、64、96、132数值可选，0代表640*640正方形头像），用户没有头像时该项为空
        /// </summary>
        public string? HeadImgUrl { get; init; }

        /// <summary>
        /// Only when the website application has obtained the userinfo authorization of the user, this field will appear.
        /// 当且仅当该网站应用已获得该用户的userinfo授权时，才会出现该字段。
        /// </summary>
        public string? UnionId { get; init; }
    }
}
