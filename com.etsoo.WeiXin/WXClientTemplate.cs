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
        /// <returns>Task</returns>
        public async Task GetSubscribeTemplateListAsync(Stream saveStream)
        {
            var accessToken = await GetAcessTokenAsync();
            var api = $"https://api.weixin.qq.com/wxaapi/newtmpl/gettemplate?access_token={accessToken}";
            await GetAsync(api, saveStream);
        }

        /// <summary>
        /// Get template list
        /// 获取消息模板
        /// </summary>
        /// <param name="saveStream">Save stream</param>
        /// <returns>Task</returns>
        public async Task GetTemplateListAsync(Stream saveStream)
        {
            var accessToken = await GetAcessTokenAsync();
            var api = $"{ApiUri}template/get_all_private_template?access_token={accessToken}";
            await GetAsync(api, saveStream);
        }

        /// <summary>
        /// 发送订阅通知
        /// </summary>
        /// <param name="input">输入的信息</param>
        /// <returns>操作结果</returns>
        public async Task<WXApiError?> SendSubscribeMessageAsync(WXSendMessageInput input)
        {
            var accessToken = await GetAcessTokenAsync();
            var api = $"{ApiUri}message/subscribe/bizsend?access_token={accessToken}";

            return await PostAsync<WXSendMessageInput, WXApiError>(api, input, JsonOutOptions, true);
        }

        /// <summary>
        /// 发送模板消息
        /// </summary>
        /// <param name="input">输入的信息</param>
        /// <returns>操作结果</returns>
        public async Task<WXApiError?> SendTemplateMessageAsync(WXSendMessageInput input)
        {
            var accessToken = await GetAcessTokenAsync();
            var api = $"{ApiUri}message/template/send?access_token={accessToken}";

            return await PostAsync<WXSendMessageInput, WXApiError>(api, input, JsonOutOptions, true);
        }
    }
}
