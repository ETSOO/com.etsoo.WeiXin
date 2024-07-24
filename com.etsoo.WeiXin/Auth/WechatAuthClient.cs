using com.etsoo.ApiModel.Auth;
using com.etsoo.Utils.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Web;

namespace com.etsoo.WeiXin.Auth
{
    /// <summary>
    /// Wechat auth client
    /// https://cloud.tencent.com/developer/article/1447723
    /// https://developers.weixin.qq.com/doc/oplatform/Website_App/WeChat_Login/Wechat_Login.html
    /// 微信授权客户端
    /// </summary>
    public class WechatAuthClient : IWechatAuthClient
    {
        private const string SignScope = "snsapi_login";

        private readonly HttpClient _client;
        private readonly WechatAuthClientOptions _options;
        private readonly ILogger _logger;

        private readonly string _gateway;

        public WechatAuthClient(HttpClient client, WechatAuthClientOptions options, ILogger logger)
        {
            _client = client;
            _options = options;
            _logger = logger;

            _gateway = options.Gateway ?? "https://api.weixin.qq.com";
        }

        [ActivatorUtilitiesConstructor]
        public WechatAuthClient(HttpClient client, IOptions<WechatAuthClientOptions> options, ILogger<WechatAuthClient> logger)
            : this(client, options.Value, logger)
        {

        }

        /// <summary>
        /// Get log in URL
        /// 获取登录URL
        /// </summary>
        /// <param name="state">Request state</param>
        /// <param name="loginHint">Login hint</param>
        /// <returns>URL</returns>
        public string GetLogInUrl(string state, string? loginHint = null)
        {
            return GetServerAuthUrl(AuthExtentions.LogInAction, state, SignScope, false, loginHint);
        }

        /// <summary>
        /// Get sign up URL
        /// 获取注册URL
        /// </summary>
        /// <param name="state">Request state</param>
        /// <returns>URL</returns>
        public string GetSignUpUrl(string state)
        {
            return GetServerAuthUrl(AuthExtentions.SignUpAction, state, SignScope);
        }

        /// <summary>
        /// Get server auth URL, for back-end processing
        /// 获取服务器授权URL，用于后端处理
        /// </summary>
        /// <param name="action">Action of the request</param>
        /// <param name="state">Specifies any string value that your application uses to maintain state between your authorization request and the authorization server's response</param>
        /// <param name="scope">A space-delimited list of scopes that identify the resources that your application could access on the user's behalf</param>
        /// <param name="offline">Set to true if your application needs to refresh access tokens when the user is not present at the browser</param>
        /// <param name="loginHint">Set the parameter value to an email address or sub identifier, which is equivalent to the user's Wechat ID</param>
        /// <returns>URL</returns>
        public string GetServerAuthUrl(string action, string state, string scope, bool offline = false, string? loginHint = null)
        {
            var url = GetAuthUrl($"{_options.ServerRedirectUrl}/{action}", "code", scope, state, loginHint);
            return url;
        }

        /// <summary>
        /// Get script auth URL, for front-end page
        /// 获取脚本授权URL，用于前端页面
        /// </summary>
        /// <param name="action">Action of the request</param>
        /// <param name="state">Specifies any string value that your application uses to maintain state between your authorization request and the authorization server's response</param>
        /// <param name="scope">A space-delimited list of scopes that identify the resources that your application could access on the user's behalf</param>
        /// <param name="loginHint">Set the parameter value to an email address or sub identifier, which is equivalent to the user's Wechat ID</param>
        /// <returns>URL</returns>
        public string GetScriptAuthUrl(string action, string state, string scope, string? loginHint = null)
        {
            return GetAuthUrl($"{_options.ScriptRedirectUrl}/{action}", "token", scope, state, loginHint);
        }

        /// <summary>
        /// Get auth URL
        /// 获取授权URL
        /// </summary>
        /// <param name="redirectUrl">The value must exactly match one of the authorized redirect URIs for the OAuth 2.0 client, which you configured in your client's API Console</param>
        /// <param name="responseType">Set the parameter value to 'code' for web server applications or 'token' for SPA</param>
        /// <param name="scope">应用授权作用域，拥有多个作用域用逗号（,）分隔，网页应用目前仅填写snsapi_login即可</param>
        /// <param name="state">Specifies any string value that your application uses to maintain state between your authorization request and the authorization server's response</param>
        /// <param name="loginHint">Set the parameter value to an email address or sub identifier, which is equivalent to the user's Google ID</param>
        /// <returns>URL</returns>
        /// <exception cref="ArgumentNullException">Parameter 'redirectUrl' is required</exception>
        public string GetAuthUrl(string? redirectUrl, string responseType, string scope, string state, string? loginHint = null)
        {
            if (string.IsNullOrEmpty(redirectUrl))
            {
                throw new ArgumentNullException(nameof(redirectUrl));
            }

            return $"{_gateway}/connect/qrconnect?appid={_options.AppId}&redirect_uri={HttpUtility.UrlEncode(redirectUrl)}&response_type={responseType}&scope={scope}&state={HttpUtility.UrlEncode(state)}#wechat_redirect";
        }

        /// <summary>
        /// Create access token from authorization code
        /// 从授权码创建访问令牌
        /// </summary>
        /// <param name="action">Request action</param>
        /// <param name="code">Authorization code</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Token data</returns>
        public async ValueTask<WechatTokenData?> CreateTokenAsync(string action, string code, CancellationToken cancellationToken = default)
        {
            var api = $"{_gateway}/sns/oauth2/access_token?appid={_options.AppId}&secret={_options.AppSecret}&code={code}&grant_type=authorization_code";
            var response = await _client.GetAsync(api, cancellationToken);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync(WechatAuthJsonSerializerContext.Default.WechatTokenData, cancellationToken);
        }

        /// <summary>
        /// Refresh the access token with refresh token
        /// 用刷新令牌获取访问令牌
        /// </summary>
        /// <param name="refreshToken">Refresh token</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        public async Task<WechatTokenData?> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
        {
            var api = $"{_gateway}/sns/oauth2/refresh_token?appid={_options.AppId}&grant_type=refresh_token&refresh_token={refreshToken}";
            var response = await _client.GetAsync(api, cancellationToken);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync(WechatAuthJsonSerializerContext.Default.WechatTokenData, cancellationToken);
        }

        /// <summary>
        /// Get user info
        /// 获取用户信息
        /// </summary>
        /// <param name="tokenData">Token data</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        public async ValueTask<WechatUserInfo?> GetUserInfoAsync(WechatTokenData tokenData, CancellationToken cancellationToken = default)
        {
            var api = $"{_gateway}/sns/userinfo?access_token={tokenData.AccessToken}&openid={tokenData.OpenId}";
            var response = await _client.GetAsync(api, cancellationToken);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync(WechatAuthJsonSerializerContext.Default.WechatUserInfo, cancellationToken);
        }

        /// <summary>
        /// Get user info from callback request
        /// 从回调请求获取用户信息
        /// </summary>
        /// <param name="request">Callback request</param>
        /// <param name="state">Request state</param>
        /// <param name="action">Request action</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Action result & user information</returns>
        public ValueTask<(IActionResult result, AuthUserInfo? userInfo)> GetUserInfoAsync(HttpRequest request, string state, string? action = null, CancellationToken cancellationToken = default)
        {
            return GetUserInfoAsync(request, s => s == state, action, cancellationToken);
        }

        /// <summary>
        /// Get user info from callback request
        /// 从回调请求获取用户信息
        /// </summary>
        /// <param name="request">Callback request</param>
        /// <param name="stateCallback">Callback to verify request state</param>
        /// <param name="action">Request action</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Action result & user information</returns>
        public async ValueTask<(IActionResult result, AuthUserInfo? userInfo)> GetUserInfoAsync(HttpRequest request, Func<string, bool> stateCallback, string? action = null, CancellationToken cancellationToken = default)
        {
            var (result, tokenData) = await ValidateAuthAsync(request, stateCallback, action, cancellationToken);
            AuthUserInfo? userInfo = null;
            if (result.Ok && tokenData != null)
            {
                var data = await GetUserInfoAsync(tokenData, cancellationToken);
                if (data == null)
                {
                    result = new ActionResult
                    {
                        Type = "NoDataReturned",
                        Field = "userinfo"
                    };
                }
                else
                {
                    userInfo = new AuthUserInfo
                    {
                        OpenId = data.OpenId,
                        Name = data.Nickname,
                        Picture = data.HeadImgUrl
                    };
                }
            }

            return (result, userInfo);
        }

        /// <summary>
        /// Validate auth callback
        /// 验证认证回调
        /// </summary>
        /// <param name="request">Callback request</param>
        /// <param name="stateCallback">Callback to verify request state</param>
        /// <param name="action">Request action</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Action result & Token data</returns>
        public async Task<(IActionResult result, WechatTokenData? tokenData)> ValidateAuthAsync(HttpRequest request, Func<string, bool> stateCallback, string? action = null, CancellationToken cancellationToken = default)
        {
            IActionResult result;
            WechatTokenData? tokenData = null;

            if (request.Query.TryGetValue("error", out var error))
            {
                result = new ActionResult
                {
                    Type = "AccessDenied",
                    Field = error
                };
            }
            else if (request.Query.TryGetValue("state", out var actualState) && request.Query.TryGetValue("code", out var codeSource))
            {
                var code = codeSource.ToString();
                if (!stateCallback(actualState.ToString()))
                {
                    result = new ActionResult
                    {
                        Type = "AccessDenied",
                        Field = "state"
                    };
                }
                else if (string.IsNullOrEmpty(code))
                {
                    result = new ActionResult
                    {
                        Type = "NoDataReturned",
                        Field = "code"
                    };
                }
                else
                {
                    try
                    {
                        action ??= request.GetRequestAction();
                        tokenData = await CreateTokenAsync(action, code, cancellationToken);
                        if (tokenData == null)
                        {
                            result = new ActionResult
                            {
                                Type = "NoDataReturned",
                                Field = "token"
                            };
                        }
                        else
                        {
                            result = ActionResult.Success;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Create token failed");
                        result = ActionResult.From(ex);
                    }
                }
            }
            else
            {
                // 若用户禁止授权，则重定向后不会带上code参数，仅会带上state参数
                result = new ActionResult
                {
                    Type = "NoDataReturned",
                    Field = "code"
                };
            }

            return (result, tokenData);
        }
    }
}
