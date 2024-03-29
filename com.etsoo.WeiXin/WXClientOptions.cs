﻿namespace com.etsoo.WeiXin
{
    /// <summary>
    /// Weixin client options
    /// 微信客户端配置
    /// </summary>
    public class WXClientOptions
    {
        /// <summary>
        /// Name
        /// 名称
        /// </summary>
        public const string Name = "WX";

        /// <summary>
        /// Section name
        /// 配置区块名
        /// </summary>
        public const string SectionName = "EtsooProxy:WeiXin";

        /// <summary>
        /// App id
        /// 程序编号
        /// </summary>
        public required string AppId { get; init; }

        /// <summary>
        /// App secret
        /// 程序密钥
        /// </summary>
        public required string AppSecret { get; init; }

        /// <summary>
        /// Token, used for signature check
        /// 令牌，用于签名验证
        /// </summary>
        public string? Token { get; init; }

        /// <summary>
        /// Encoding AES key
        /// AES 编码密钥
        /// </summary>
        public string? EncodingAESKey { get; init; }
    }
}
