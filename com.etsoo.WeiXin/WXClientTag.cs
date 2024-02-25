using com.etsoo.HTTP;
using com.etsoo.Utils.String;
using com.etsoo.WeiXin.Dto;

namespace com.etsoo.WeiXin
{
    /// <summary>
    /// WeiXin Client - Tag
    /// 微信客户端 - 标签
    /// </summary>
    public partial class WXClient : HttpClientService, IWXClient
    {
        private static HttpContent CreateBatchTagContent(int tagId, IEnumerable<string> openids)
        {
            var json = StringUtils.WriteJson((writer) =>
            {
                writer.WritePropertyName("openid_list");
                writer.WriteStartArray();
                foreach (var openid in openids)
                {
                    writer.WriteStringValue(openid);
                }
                writer.WriteEndArray();
                writer.WriteNumber("tagid", tagId);
            });
            return CreateJsonStringContent(json);
        }

        /// <summary>
        /// 批量为用户打标签
        /// </summary>
        /// <param name="tagId">标签编号</param>
        /// <param name="openids">粉丝编号</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>结果</returns>
        public async Task<WXApiError?> BatchTagAsync(int tagId, IEnumerable<string> openids, CancellationToken cancellationToken = default)
        {
            var accessToken = await GetAcessTokenAsync(cancellationToken);
            var api = $"{ApiUri}tags/members/batchtagging?access_token={accessToken}";
            var response = await Client.PostAsync(api, CreateBatchTagContent(tagId, openids), cancellationToken);
            return await ResponseToAsync(response, WeiXinJsonSerializerContext.Default.WXApiError, cancellationToken);
        }

        /// <summary>
        /// 批量为用户取消标签
        /// </summary>
        /// <param name="tagId">标签编号</param>
        /// <param name="openids">粉丝编号</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>结果</returns>
        public async Task<WXApiError?> BatchUntagAsync(int tagId, IEnumerable<string> openids, CancellationToken cancellationToken = default)
        {
            var accessToken = await GetAcessTokenAsync(cancellationToken);
            var api = $"{ApiUri}tags/members/batchuntagging?access_token={accessToken}";
            var response = await Client.PostAsync(api, CreateBatchTagContent(tagId, openids), cancellationToken);
            return await ResponseToAsync(response, WeiXinJsonSerializerContext.Default.WXApiError, cancellationToken);
        }

        /// <summary>
        /// 创建标签
        /// </summary>
        /// <param name="name">标签名</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>操作结果</returns>
        public async Task<HttpClientResult<WXCreateTagResult, WXApiError>> CreateTagAsync(string name, CancellationToken cancellationToken = default)
        {
            var accessToken = await GetAcessTokenAsync(cancellationToken);
            var api = $"{ApiUri}tags/create?access_token={accessToken}";
            var json = StringUtils.WriteJson((writer) =>
            {
                writer.WritePropertyName("tag");
                writer.WriteStartObject();
                writer.WriteString("name", name);
                writer.WriteEndObject();
            });
            return await PostAsync(api, CreateJsonStringContent(json), "tag",
                WeiXinJsonSerializerContext.Default.WXCreateTagResult,
                WeiXinJsonSerializerContext.Default.WXApiError,
                cancellationToken);
        }

        /// <summary>
        /// 删除标签
        /// </summary>
        /// <param name="id">标签编号</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>操作结果</returns>
        public async Task<WXApiError?> DeleteTagAsync(long id, CancellationToken cancellationToken = default)
        {
            var accessToken = await GetAcessTokenAsync(cancellationToken);
            var api = $"{ApiUri}tags/delete?access_token={accessToken}";
            var json = StringUtils.WriteJson((writer) =>
            {
                writer.WritePropertyName("tag");
                writer.WriteStartObject();
                writer.WriteNumber("id", id);
                writer.WriteEndObject();
            });
            var response = await Client.PostAsync(api, CreateJsonStringContent(json), cancellationToken);
            return await ResponseToAsync(response, WeiXinJsonSerializerContext.Default.WXApiError, cancellationToken);
        }

        /// <summary>
        /// 获取标签列表
        /// </summary>
        /// <param name="saveStream">Save stream</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Task</returns>
        public async Task GetTagListAsync(Stream saveStream, CancellationToken cancellationToken = default)
        {
            var accessToken = await GetAcessTokenAsync(cancellationToken);
            var api = $"{ApiUri}tags/get?access_token={accessToken}";
            await GetAsync(api, saveStream, cancellationToken);
        }

        /// <summary>
        /// 获取所有用户
        /// </summary>
        /// <param name="nextOpenId">拉取列表的最后一个用户的OPENID，为空标识从头开始</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>结果</returns>
        public async Task<HttpClientResult<WXUserListResult, WXApiError>> GetUserListAsync(string? nextOpenId, CancellationToken cancellationToken = default)
        {
            var accessToken = await GetAcessTokenAsync(cancellationToken);
            var api = $"{ApiUri}user/get?access_token={accessToken}";
            if (!string.IsNullOrEmpty(nextOpenId)) api += $"&next_openid={nextOpenId}";
            return await GetAsync(api, "count",
                WeiXinJsonSerializerContext.Default.WXUserListResult,
                WeiXinJsonSerializerContext.Default.WXApiError,
                cancellationToken);
        }

        /// <summary>
        /// 获取用户身上的标签列表
        /// </summary>
        /// <param name="openid">编号</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>结果</returns>
        public async Task<HttpClientResult<GetUserTagResult, WXApiError>> GetUserTagAsync(string openid, CancellationToken cancellationToken = default)
        {
            var accessToken = await GetAcessTokenAsync(cancellationToken);
            var api = $"{ApiUri}tags/getidlist?access_token={accessToken}";
            var json = StringUtils.WriteJson((writer) =>
            {
                writer.WriteString("openid", openid);
            });
            return await PostAsync(api, CreateJsonStringContent(json), "tagid_list",
                WeiXinJsonSerializerContext.Default.GetUserTagResult,
                WeiXinJsonSerializerContext.Default.WXApiError,
                cancellationToken);
        }

        /// <summary>
        /// 获取标签下的所有用户
        /// </summary>
        /// <param name="tagId">标签编号</param>
        /// <param name="nextOpenId">拉取列表的最后一个用户的OPENID，为空标识从头开始</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>结果</returns>
        public async Task<HttpClientResult<WXUserListResult, WXApiError>> GetTagUserListAsync(int tagId, string? nextOpenId, CancellationToken cancellationToken = default)
        {
            var accessToken = await GetAcessTokenAsync(cancellationToken);
            var api = $"{ApiUri}user/tag/get?access_token={accessToken}";
            var json = StringUtils.WriteJson((writer) =>
            {
                writer.WriteNumber("tagid", tagId);
                if (!string.IsNullOrEmpty(nextOpenId)) writer.WriteString("next_openid", nextOpenId);
            });
            return await PostAsync(api, CreateJsonStringContent(json), "count",
                WeiXinJsonSerializerContext.Default.WXUserListResult,
                WeiXinJsonSerializerContext.Default.WXApiError,
                cancellationToken);
        }

        /// <summary>
        /// 编辑标签
        /// </summary>
        /// <param name="id">编号</param>
        /// <param name="name">新标签名</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>操作结果</returns>
        public async Task<WXApiError?> UpdateTagAsync(long id, string name, CancellationToken cancellationToken = default)
        {
            var accessToken = await GetAcessTokenAsync(cancellationToken);
            var api = $"{ApiUri}tags/update?access_token={accessToken}";
            var json = StringUtils.WriteJson((writer) =>
            {
                writer.WritePropertyName("tag");
                writer.WriteStartObject();
                writer.WriteNumber("id", id);
                writer.WriteString("name", name);
                writer.WriteEndObject();
            });
            var response = await Client.PostAsync(api, CreateJsonStringContent(json), cancellationToken);
            return await ResponseToAsync(response, WeiXinJsonSerializerContext.Default.WXApiError, cancellationToken);
        }
    }
}
