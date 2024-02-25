using com.etsoo.HTTP;
using com.etsoo.WeiXin.Dto;

namespace com.etsoo.WeiXin
{
    /// <summary>
    /// WeiXin Client - Template
    /// 微信客户端 - 模板
    /// </summary>
    public partial class WXClient : HttpClientService, IWXClient
    {
        /// <summary>
        /// Get subscribe template list
        /// 获取订阅模板列表
        /// </summary>
        /// <param name="saveStream">Save stream</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Task</returns>
        public async Task GetSubscribeTemplateListAsync(Stream saveStream, CancellationToken cancellationToken = default)
        {
            var accessToken = await GetAcessTokenAsync(cancellationToken);
            var api = $"https://api.weixin.qq.com/wxaapi/newtmpl/gettemplate?access_token={accessToken}";
            await GetAsync(api, saveStream, cancellationToken);
        }

        /// <summary>
        /// Get template list
        /// 获取消息模板
        /// </summary>
        /// <param name="saveStream">Save stream</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Task</returns>
        public async Task GetTemplateListAsync(Stream saveStream, CancellationToken cancellationToken = default)
        {
            var accessToken = await GetAcessTokenAsync(cancellationToken);
            var api = $"{ApiUri}template/get_all_private_template?access_token={accessToken}";
            await GetAsync(api, saveStream, cancellationToken);
        }

        /// <summary>
        /// 发送订阅通知
        /// </summary>
        /// <param name="input">输入的信息</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>操作结果</returns>
        public async Task<WXApiError?> SendSubscribeMessageAsync(WXSendMessageInput input, CancellationToken cancellationToken = default)
        {
            var accessToken = await GetAcessTokenAsync(cancellationToken);
            var api = $"{ApiUri}message/subscribe/bizsend?access_token={accessToken}";

            return await PostAsync(api, input,
                WeiXinJsonSerializerContext.Default.WXSendMessageInput,
                WeiXinJsonSerializerContext.Default.WXApiError, true, cancellationToken);
        }

        /// <summary>
        /// 发送模板消息
        /// </summary>
        /// <param name="input">输入的信息</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>操作结果</returns>
        public async Task<WXApiError?> SendTemplateMessageAsync(WXSendMessageInput input, CancellationToken cancellationToken = default)
        {
            var accessToken = await GetAcessTokenAsync(cancellationToken);
            var api = $"{ApiUri}message/template/send?access_token={accessToken}";

            return await PostAsync(api, input,
                WeiXinJsonSerializerContext.Default.WXSendMessageInput,
                WeiXinJsonSerializerContext.Default.WXApiError, true, cancellationToken);
        }
    }
}
