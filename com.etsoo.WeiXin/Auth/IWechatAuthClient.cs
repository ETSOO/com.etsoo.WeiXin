using com.etsoo.ApiModel.Auth;
using com.etsoo.Utils.Actions;
using Microsoft.AspNetCore.Http;

namespace com.etsoo.WeiXin.Auth
{
    /// <summary>
    /// Wechat auth client interface
    /// 微信授权客户端接口
    /// </summary>
    public interface IWechatAuthClient : IAuthClient
    {
        /// <summary>
        /// Create access token from authorization code
        /// 从授权码创建访问令牌
        /// </summary>
        /// <param name="action">Request action</param>
        /// <param name="code">Authorization code</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Token data</returns>
        ValueTask<WechatTokenData?> CreateTokenAsync(string action, string code, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get user info
        /// 获取用户信息
        /// </summary>
        /// <param name="tokenData">Token data</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        ValueTask<WechatUserInfo?> GetUserInfoAsync(WechatTokenData tokenData, CancellationToken cancellationToken = default);

        /// <summary>
        /// Refresh the access token with refresh token
        /// 用刷新令牌获取访问令牌
        /// </summary>
        /// <param name="refreshToken">Refresh token</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        Task<WechatTokenData?> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);

        /// <summary>
        /// Validate auth callback
        /// 验证认证回调
        /// </summary>
        /// <param name="request">Callback request</param>
        /// <param name="stateCallback">Callback to verify request state</param>
        /// <param name="action">Request action</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Action result & Token data & actual state</returns>
        Task<(IActionResult result, WechatTokenData? tokenData, string? state)> ValidateAuthAsync(HttpRequest request, Func<string, bool> stateCallback, string? action = null, CancellationToken cancellationToken = default);
    }
}
