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
        /// Get server auth URL, for back-end processing
        /// 获取服务器授权URL，用于后端处理
        /// </summary>
        /// <param name="state">Specifies any string value that your application uses to maintain state between your authorization request and the authorization server's response</param>
        /// <param name="scope">A space-delimited list of scopes that identify the resources that your application could access on the user's behalf</param>
        /// <param name="offline">Set to true if your application needs to refresh access tokens when the user is not present at the browser</param>
        /// <param name="loginHint">Set the parameter value to an email address or sub identifier, which is equivalent to the user's Wechat ID</param>
        /// <returns>URL</returns>
        public string GetServerAuthUrl(string state, string scope, bool offline = false, string? loginHint = null)
        {
            var url = GetAuthUrl(_options.ServerRedirectUrl, "code", scope, state, loginHint);
            return url;
        }

        /// <summary>
        /// Get script auth URL, for front-end page
        /// 获取脚本授权URL，用于前端页面
        /// </summary>
        /// <param name="state">Specifies any string value that your application uses to maintain state between your authorization request and the authorization server's response</param>
        /// <param name="scope">A space-delimited list of scopes that identify the resources that your application could access on the user's behalf</param>
        /// <param name="loginHint">Set the parameter value to an email address or sub identifier, which is equivalent to the user's Wechat ID</param>
        /// <returns>URL</returns>
        public string GetScriptAuthUrl(string state, string scope, string? loginHint = null)
        {
            return GetAuthUrl(_options.ScriptRedirectUrl, "token", scope, state, loginHint);
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
        /// <param name="code">Authorization code</param>
        /// <returns>Token data</returns>
        public async ValueTask<WechatTokenData?> CreateTokenAsync(string code)
        {
            var api = $"{_gateway}/sns/oauth2/access_token?appid={_options.AppId}&secret={_options.AppSecret}&code={code}&grant_type=authorization_code";
            var response = await _client.GetAsync(api);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync(WechatAuthJsonSerializerContext.Default.WechatTokenData);
        }

        /// <summary>
        /// Refresh the access token with refresh token
        /// 用刷新令牌获取访问令牌
        /// </summary>
        /// <param name="refreshToken">Refresh token</param>
        /// <returns>Result</returns>
        public async Task<WechatTokenData?> RefreshTokenAsync(string refreshToken)
        {
            var api = $"{_gateway}/sns/oauth2/refresh_token?appid={_options.AppId}&grant_type=refresh_token&refresh_token={refreshToken}";
            var response = await _client.GetAsync(api);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync(WechatAuthJsonSerializerContext.Default.WechatTokenData);
        }

        /// <summary>
        /// Get user info
        /// 获取用户信息
        /// </summary>
        /// <param name="tokenData">Token data</param>
        /// <returns>Result</returns>
        public async ValueTask<WechatUserInfo?> GetUserInfoAsync(WechatTokenData tokenData)
        {
            var api = $"{_gateway}/sns/userinfo?access_token={tokenData.AccessToken}&openid={tokenData.OpenId}";
            var response = await _client.GetAsync(api);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync(WechatAuthJsonSerializerContext.Default.WechatUserInfo);
        }

        /// <summary>
        /// Get user info from callback request
        /// 从回调请求获取用户信息
        /// </summary>
        /// <param name="request">Callback request</param>
        /// <param name="state">Request state</param>
        /// <returns>Action result & user information</returns>
        public ValueTask<(IActionResult result, AuthUserInfo? userInfo)> GetUserInfoAsync(HttpRequest request, string state)
        {
            return GetUserInfoAsync(request, s => s == state);
        }

        /// <summary>
        /// Get user info from callback request
        /// 从回调请求获取用户信息
        /// </summary>
        /// <param name="request">Callback request</param>
        /// <param name="stateCallback">Callback to verify request state</param>
        /// <returns>Action result & user information</returns>
        public async ValueTask<(IActionResult result, AuthUserInfo? userInfo)> GetUserInfoAsync(HttpRequest request, Func<string, bool> stateCallback)
        {
            var (result, tokenData) = await ValidateAuthAsync(request, stateCallback);
            AuthUserInfo? userInfo = null;
            if (result.Ok && tokenData != null)
            {
                var data = await GetUserInfoAsync(tokenData);
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
        /// <returns>Action result & Token data</returns>
        public async Task<(IActionResult result, WechatTokenData? tokenData)> ValidateAuthAsync(HttpRequest request, Func<string, bool> stateCallback)
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
                        tokenData = await CreateTokenAsync(code);
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
