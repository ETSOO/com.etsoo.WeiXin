using System.Diagnostics.CodeAnalysis;

namespace com.etsoo.WeiXin.Dto
{
    /// <summary>
    /// Weixin get Js api ticket result
    /// 微信获取Js接口调用凭证结果
    /// </summary>
    public record WXJsApiTokenResult : WXApiError
    {
        /// <summary>
        /// Ticket
        /// 凭证
        /// </summary>
        public string? Ticket { get; init; }

        /// <summary>
        /// Ticket valid seconds
        /// 凭证有效时间，单位：秒
        /// </summary>
        [NotNullIfNotNull(nameof(Ticket))]
        public int? ExpiresIn { get; init; }
    }
}
