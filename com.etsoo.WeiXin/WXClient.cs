using com.etsoo.Utils.Crypto;
using com.etsoo.Utils.Localization;
using com.etsoo.Utils.Net;
using com.etsoo.Utils.String;
using com.etsoo.WeiXin.Dto;
using com.etsoo.WeiXin.Support;
using Microsoft.Extensions.Configuration;

namespace com.etsoo.WeiXin
{
    /// <summary>
    /// WeiXin Client
    /// 微信客户端
    /// </summary>
    public class WXClient : HttpClientService
    {
        /// <summary>
        /// Api base uri
        /// 接口基本地址
        /// </summary>
        public const string ApiUri = "https://api.weixin.qq.com/cgi-bin/";

        /// <summary>
        /// The globally unique interface calling credentials of the official account
        /// 公众号的全局唯一接口调用凭据
        /// https://developers.weixin.qq.com/doc/offiaccount/Basic_Information/Get_access_token.html
        /// </summary>
        private static string? AccessToken;

        // Invoke credential latest valid time
        // 调用凭据最晚有效时间
        private static DateTime? AccessTokenExpired;

        /// <summary>
        /// The globally unique ticket for Js API call
        /// Js API调用的全局唯一票据
        /// https://developers.weixin.qq.com/doc/offiaccount/OA_Web_Apps/JS-SDK.html#62
        /// </summary>
        private static string? JsApiTicket;
        private static DateTime? JsApiTicketExpired;


        /// <summary>
        /// App id
        /// 程序编号
        /// </summary>
        protected string AppId { get; }

        // App secret
        private readonly string appSecret;

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="client">Client</param>
        /// <param name="appId">App id</param>
        /// <param name="appSecret">App secret</param>
        public WXClient(HttpClient client, string appId, string appSecret) : base(client)
        {
            AppId = appId;
            this.appSecret = appSecret;

            Options.PropertyNamingPolicy = new WXClientJsonNamingPolicy();
        }

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="client">Client</param>
        /// <param name="section">Configuration section</param>
        /// <param name="secureManager">Secure manager</param>
        public WXClient(HttpClient client, IConfigurationSection section, Func<string, string>? secureManager = null)
            : this(client
                  , CryptographyUtils.UnsealData(section.GetValue<string>("AppId"), secureManager)
                  , CryptographyUtils.UnsealData(section.GetValue<string>("AppSecret"), secureManager))
        {

        }

        /// <summary>
        /// Create Js API signature
        /// 创建 Js 接口签名
        /// </summary>
        /// <param name="url">Url without #</param>
        /// <returns>Result</returns>
        public async ValueTask<WXJsApiSignatureResult> CreateJsApiSignature(string url)
        {
            // Source data
            var data = new SortedDictionary<string, string>();
            data["url"] = url;

            // Nonce
            var nonce = CryptographyUtils.CreateRandString(RandStringKind.DigitAndLetter, 16).ToString();
            data["noncestr"] = nonce;

            // Timestamp
            var timestamp = LocalizationUtils.UTCToJsMiliseconds().ToString();
            data["timestamp"] = timestamp;

            // Get ticket
            var ticket = await GetJsApiTicketAsync();
            data["jsapi_ticket"] = ticket;

            // Signature
            var signature = await CreateSignatureAsync(data);

            // Return
            return new WXJsApiSignatureResult
            {
                AppId = AppId,
                NonceStr = nonce,
                Timestamp = timestamp,
                Signature = signature
            };
        }

        /// <summary>
        /// Create SHA1 signature
        /// 创建SHA1签名
        /// </summary>
        /// <param name="data">Source data</param>
        /// <returns>Result</returns>
        protected async Task<string> CreateSignatureAsync(SortedDictionary<string, string> data)
        {
            var source = data.JoinAsString().TrimEnd('&');
            var signatureBytes = await CryptographyUtils.SHA1Async(source);
            return Convert.ToHexString(signatureBytes).ToLower();
        }

        /// <summary>
        /// Get Access Token
        /// 获取访问凭据
        /// </summary>
        /// <returns>Result</returns>
        /// <exception cref="WXClientException"></exception>
        public async ValueTask<string> GetAcessTokenAsync()
        {
            if (AccessToken == null || AccessTokenExpired == null || AccessTokenExpired.Value <= DateTime.Now)
            {
                var api = $"{ApiUri}token?grant_type=client_credential&appid={AppId}&secret={appSecret}";
                var result = await GetAsync<WXAccessTokenResult, WXApiError>(api, "access_token");
                if (result.Success)
                {
                    AccessToken = result.Data.AccessToken;
                    AccessTokenExpired = DateTime.Now.AddSeconds(result.Data.ExpiresIn);
                    return AccessToken;
                }
                else
                {
                    throw new WXClientException(result.Error);
                }
            }

            return AccessToken;
        }

        /// <summary>
        /// Get Js API ticket
        /// 获取脚本接口凭证
        /// </summary>
        /// <returns>Result</returns>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="WXClientException"></exception>
        public async ValueTask<string> GetJsApiTicketAsync()
        {
            if (JsApiTicket == null || JsApiTicketExpired == null || JsApiTicketExpired.Value <= DateTime.Now)
            {
                var accessToken = await GetAcessTokenAsync();
                var api = $"{ApiUri}ticket/getticket?access_token={accessToken}&type=jsapi";

                var result = await GetAsync<WXJsApiTokenResult>(api);
                if (result == null)
                {
                    throw new NullReferenceException();
                }

                if (result.Ticket != null)
                {
                    JsApiTicket = result.Ticket;
                    JsApiTicketExpired = DateTime.Now.AddSeconds(result.ExpiresIn!.Value);
                    return JsApiTicket;
                }
                else
                {
                    throw new WXClientException(result);
                }
            }

            return JsApiTicket;
        }
    }
}
