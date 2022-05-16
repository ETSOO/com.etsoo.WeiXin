using com.etsoo.WeiXin.Dto;

namespace com.etsoo.WeiXin
{
    /// <summary>
    /// Weixin client interface
    /// 微信客户端接口
    /// </summary>
    public interface IWXClient
    {
        /// <summary>
        /// Create Js API signature
        /// 创建 Js 接口签名
        /// </summary>
        /// <param name="url">Url without #</param>
        /// <returns>Result</returns>
        ValueTask<WXJsApiSignatureResult> CreateJsApiSignature(string url);

        /// <summary>
        /// Get Access Token
        /// 获取访问凭据
        /// </summary>
        /// <returns>Result</returns>
        /// <exception cref="WXClientException"></exception>
        ValueTask<string> GetAcessTokenAsync();

        /// <summary>
        /// Get Js API ticket
        /// 获取脚本接口凭证
        /// </summary>
        /// <returns>Result</returns>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="WXClientException"></exception>
        ValueTask<string> GetJsApiTicketAsync();
    }
}
