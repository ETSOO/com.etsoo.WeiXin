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
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>操作结果</returns>
        public async Task<WXApiError?> CreateConditionalMenuAsync(string json, CancellationToken cancellationToken = default)
        {
            var accessToken = await GetAcessTokenAsync(cancellationToken);
            var api = $"{ApiUri}menu/addconditional?access_token={accessToken}";
            return await SendAsync(api, CreateJsonStringContent(json), WeiXinJsonSerializerContext.Default.WXApiError, null, cancellationToken);
        }

        /// <summary>
        /// 创建自定义菜单
        /// </summary>
        /// <param name="json">菜单Json定义</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>操作结果</returns>
        public async Task<WXApiError?> CreateMenuAsync(string json, CancellationToken cancellationToken = default)
        {
            var accessToken = await GetAcessTokenAsync(cancellationToken);
            var api = $"{ApiUri}menu/create?access_token={accessToken}";
            return await SendAsync(api, CreateJsonStringContent(json), WeiXinJsonSerializerContext.Default.WXApiError, null, cancellationToken);
        }

        /// <summary>
        /// 删除个性化自定义菜单
        /// </summary>
        /// <param name="menuId">菜单编号</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>操作结果</returns>
        public async Task<WXApiError?> DeleteConditonalMenuAsync(string menuId, CancellationToken cancellationToken = default)
        {
            var accessToken = await GetAcessTokenAsync(cancellationToken);
            var api = $"{ApiUri}menu/delconditional?access_token={accessToken}";
            var json = StringUtils.WriteJson((writer) => writer.WriteString("menuid", menuId));
            return await SendAsync(api, CreateJsonStringContent(json), WeiXinJsonSerializerContext.Default.WXApiError, null, cancellationToken);
        }

        /// <summary>
        /// 删除自定义菜单
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>操作结果</returns>
        public async Task<WXApiError?> DeleteMenuAsync(CancellationToken cancellationToken = default)
        {
            var accessToken = await GetAcessTokenAsync(cancellationToken);
            var api = $"{ApiUri}menu/delete?access_token={accessToken}";
            return await GetAsync(api, WeiXinJsonSerializerContext.Default.WXApiError, cancellationToken);
        }

        /// <summary>
        /// 获取自定义菜单
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <param name="saveStream">Save stream</param>
        /// <returns>Task</returns>
        public async Task GetMenuAsync(Stream saveStream, CancellationToken cancellationToken = default)
        {
            var accessToken = await GetAcessTokenAsync(cancellationToken);
            var api = $"{ApiUri}menu/get?access_token={accessToken}";
            await GetAsync(api, saveStream, cancellationToken);
        }
    }
}
