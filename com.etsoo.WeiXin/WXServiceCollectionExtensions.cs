using com.etsoo.WeiXin.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace com.etsoo.WeiXin
{
    /// <summary>
    /// Wechat service collection extensions
    /// 微信服务集合扩展
    /// </summary>
    public static class WXServiceCollectionExtensions
    {
        /// <summary>
        /// Add Wechat client
        /// 添加微信客户端
        /// </summary>
        /// <param name="services">Services</param>
        /// <param name="configuration">configuration</param>
        /// <returns>Services</returns>
        public static IServiceCollection AddWechatClient(this IServiceCollection services, IConfigurationSection configuration)
        {
            services.AddSingleton<IValidateOptions<WXClientOptions>, ValidateWXClientOptions>();
            services.AddOptions<WXClientOptions>().Bind(configuration).ValidateOnStart();
            services.AddHttpClient<IWXClient, WXClient>();
            return services;
        }

        /// <summary>
        /// Add Wechat auth client
        /// 添加微信授权客户端
        /// </summary>
        /// <param name="services">Services</param>
        /// <param name="configuration">configuration</param>
        /// <returns>Services</returns>
        public static IServiceCollection AddWechatAuthClient(this IServiceCollection services, IConfigurationSection configuration)
        {
            services.AddSingleton<IValidateOptions<WechatAuthClientOptions>, ValidateWechatAuthClientOptions>();
            services.AddOptions<WechatAuthClientOptions>().Bind(configuration).ValidateOnStart();
            services.AddHttpClient<IWechatAuthClient, WechatAuthClient>();
            return services;
        }
    }
}
