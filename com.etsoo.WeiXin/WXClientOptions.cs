namespace com.etsoo.WeiXin
{
    /// <summary>
    /// Weixin client options, options are dynamic, cannot set as readonly and 'init' / 'required' only
    /// 微信客户端选项，选项是动态的，不能设置为只读和仅初始化和必需的
    /// </summary>
    public record WXClientOptions
    {
        /// <summary>
        /// App id
        /// 程序编号
        /// </summary>
        public string AppId { get; set; } = string.Empty;

        /// <summary>
        /// App secret
        /// 程序密钥
        /// </summary>
        public string AppSecret { get; set; } = string.Empty;

        /// <summary>
        /// API URI
        /// API 地址
        /// </summary>
        public string? ApiUri { get; set; }

        /// <summary>
        /// Token, used for signature check
        /// 令牌，用于签名验证
        /// </summary>
        public string? Token { get; set; }

        /// <summary>
        /// Encoding AES key
        /// AES 编码密钥
        /// </summary>
        public string? EncodingAESKey { get; set; }
    }
}
