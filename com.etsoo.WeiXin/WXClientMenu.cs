using com.etsoo.HTTP;
using com.etsoo.Utils.String;
using com.etsoo.WeiXin.Dto;

namespace com.etsoo.WeiXin
{
    /// <summary>
    /// WeiXin Client - Menu
    /// 微信客户端 - 菜单
    /// </summary>
    public partial class WXClient : HttpClientService, IWXClient
    {
        /// <summary>
        /// 创建个性化自定义菜单
        /// </summary>
        /// <param name="json">菜单Json定义</param>
        /// <returns>操作结果</returns>
        public async Task<WXApiError?> CreateConditionalMenuAsync(string json)
        {
            var accessToken = await GetAcessTokenAsync();
            var api = $"{ApiUri}menu/addconditional?access_token={accessToken}";
            return await SendAsync<WXApiError>(api, CreateJsonStringContent(json));
        }

        /// <summary>
        /// 创建自定义菜单
        /// </summary>
        /// <param name="json">菜单Json定义</param>
        /// <returns>操作结果</returns>
        public async Task<WXApiError?> CreateMenuAsync(string json)
        {
            var accessToken = await GetAcessTokenAsync();
            var api = $"{ApiUri}menu/create?access_token={accessToken}";
            return await SendAsync<WXApiError>(api, CreateJsonStringContent(json));
        }

        /// <summary>
        /// 删除个性化自定义菜单
        /// </summary>
        /// <param name="menuId">菜单编号</param>
        /// <returns>操作结果</returns>
        public async Task<WXApiError?> DeleteConditonalMenuAsync(string menuId)
        {
            var accessToken = await GetAcessTokenAsync();
            var api = $"{ApiUri}menu/delconditional?access_token={accessToken}";
            var json = StringUtils.WriteJson((writer) => writer.WriteString("menuid", menuId));
            return await SendAsync<WXApiError>(api, CreateJsonStringContent(json));
        }

        /// <summary>
        /// 删除自定义菜单
        /// </summary>
        /// <returns>操作结果</returns>
        public async Task<WXApiError?> DeleteMenuAsync()
        {
            var accessToken = await GetAcessTokenAsync();
            var api = $"{ApiUri}menu/delete?access_token={accessToken}";
            return await GetAsync<WXApiError>(api);
        }

        /// <summary>
        /// 获取自定义菜单
        /// </summary>
        /// <param name="saveStream">Save stream</param>
        /// <returns>Task</returns>
        public async Task GetMenuAsync(Stream saveStream)
        {
            var accessToken = await GetAcessTokenAsync();
            var api = $"{ApiUri}menu/get?access_token={accessToken}";
            await GetAsync(api, saveStream);
        }
    }
}
