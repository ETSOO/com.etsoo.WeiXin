using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;

namespace com.etsoo.WeiXin.Auth
{
    /// <summary>
    /// Wechat auth client options
    /// 微信授权客户端选项
    /// </summary>
    public record WechatAuthClientOptions
    {
        /// <summary>
        /// Gateway, default is 'https://api.weixin.qq.com'
        /// API网关
        /// </summary>
        [Url]
        public string? Gateway { get; set; }

        /// <summary>
        /// Application Id
        /// 应用唯一标识
        /// </summary>
        [Required]
        public required string AppId { get; set; }

        /// <summary>
        /// Application secret
        /// 应用密钥AppSecret，在微信开放平台提交应用审核通过后获得
        /// </summary>
        [Required]
        public required string AppSecret { get; set; }

        /// <summary>
        /// Authorized redirect URIs for the server side application
        /// </summary>
        [Url]
        public string? ServerRedirectUrl { get; set; }

        /// <summary>
        /// Authorized redirect URIs for the script side application
        /// </summary>
        [Url]
        public string? ScriptRedirectUrl { get; set; }
    }

    [OptionsValidator]
    public partial class ValidateWechatAuthClientOptions : IValidateOptions<WechatAuthClientOptions>
    {
    }
}
