using com.etsoo.HTTP;
using com.etsoo.WeiXin.Dto;
using com.etsoo.WeiXin.Message;
using System.Xml;

namespace com.etsoo.WeiXin
{
    /// <summary>
    /// Weixin client interface
    /// 微信客户端接口
    /// </summary>
    public interface IWXClient
    {
        /// <summary>
        /// 创建个性化自定义菜单
        /// </summary>
        /// <param name="json">菜单Json定义</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>操作结果</returns>
        Task<WXApiError?> CreateConditionalMenuAsync(string json, CancellationToken cancellationToken = default);

        /// <summary>
        /// 创建自定义菜单
        /// </summary>
        /// <param name="json">菜单Json定义</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>操作结果</returns>
        Task<WXApiError?> CreateMenuAsync(string json, CancellationToken cancellationToken = default);

        /// <summary>
        /// Check signature
        /// 检查签名
        /// </summary>
        /// <param name="input">Input data</param>
        /// <returns>Result</returns>
        ValueTask<bool> CheckSignatureAsync(IWXCheckSignatureInput input);

        /// <summary>
        /// Create signature
        /// 创建签名
        /// </summary>
        /// <param name="timestamp">Timestamp</param>
        /// <param name="nonce">Nonce</param>
        /// <returns>Result</returns>
        Task<string> CreateSignatureAsync(string timestamp, string nonce);

        /// <summary>
        /// Create Js API signature
        /// 创建 Js 接口签名
        /// </summary>
        /// <param name="url">Url without #</param>
        /// <returns>Result</returns>
        ValueTask<WXJsApiSignatureResult> CreateJsApiSignatureAsync(string url);

        /// <summary>
        /// Create Js Card API signature
        /// 创建 Js 卡券接口签名
        /// </summary>
        /// <param name="cardId">Card id / 卡券ID</param>
        /// <param name="code">Code / 指定的卡券code码，只能被领一次。自定义code模式的卡券必须填写，非自定义code和预存code模式的卡券不必填写</param>
        /// <param name="balance">Balance / 红包类型卡券，指定金额</param>
        /// <param name="openid">Open id / 指定领取者的openid，只有该用户能领取。bind_openid字段为true的卡券必须填写，bind_openid字段为false不必填写。</param>
        /// <returns>Result</returns>
        ValueTask<WXJsCardApiSignatureResult> CreateJsCardApiSignatureAsync(string cardId, string? code = null, decimal? balance = null, string? openid = null);

        /// <summary>
        /// 下载临时媒体文件
        /// </summary>
        /// <param name="mediaId">Media id</param>
        /// <param name="saveStream">Save stream</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Task</returns>
        Task<string> DownloadMediaAsync(string mediaId, Stream saveStream, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get Access Token
        /// 获取访问凭据
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        /// <exception cref="WXClientException"></exception>
        ValueTask<string> GetAcessTokenAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Get Js API ticket
        /// 获取脚本接口凭证
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="WXClientException"></exception>
        ValueTask<string> GetJsApiTicketAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Get Js API Card ticket
        /// 获取脚本卡券接口凭证
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="WXClientException"></exception>
        ValueTask<string> GetJsApiCardTicketAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Get subscribe template list
        /// 获取订阅模板列表
        /// </summary>
        /// <param name="saveStream">Save stream</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Task</returns>
        Task GetSubscribeTemplateListAsync(Stream saveStream, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get template list
        /// 获取模板列表
        /// </summary>
        /// <param name="saveStream">Save stream</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Task</returns>
        Task GetTemplateListAsync(Stream saveStream, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get custom menu
        /// 获取自定义菜单
        /// </summary>
        /// <param name="saveStream">Save stream</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Task</returns>
        Task GetMenuAsync(Stream saveStream, CancellationToken cancellationToken = default);

        /// <summary>
        /// Parse message
        /// 解析消息
        /// </summary>
        /// <typeparam name="T">Generic message type</typeparam>
        /// <param name="input">Input stream</param>
        /// <param name="rq">Request data</param>
        /// <returns>Message</returns>
        Task<(T?, Dictionary<string, string>)> ParseMessageAsync<T>(Stream input, WXMessageCallbackInput rq) where T : WXMessage;

        /// <summary>
        /// Parse message
        /// 解析消息
        /// </summary>
        /// <param name="input">Input stream</param>
        /// <param name="rq">Request data</param>
        /// <returns>Message and source dictionary</returns>
        Task<(WXMessage?, Dictionary<string, string>)> ParseMessageAsync(Stream input, WXMessageCallbackInput rq);

        /// <summary>
        /// Reply message
        /// 回复消息
        /// </summary>
        /// <param name="output">output stream</param>
        /// <param name="message">Message</param>
        /// <param name="func">Reply callback</param>
        /// <returns>Task</returns>
        Task ReplyMessageAsync(Stream output, WXMessage message, Func<XmlWriter, Task> func);

        /// <summary>
        /// 发送订阅通知
        /// </summary>
        /// <param name="input">输入的信息</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>操作结果</returns>
        Task<WXApiError?> SendSubscribeMessageAsync(WXSendMessageInput input, CancellationToken cancellationToken = default);

        /// <summary>
        /// 发送模板消息
        /// </summary>
        /// <param name="input">输入的信息</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>操作结果</returns>
        Task<WXApiError?> SendTemplateMessageAsync(WXSendMessageInput input, CancellationToken cancellationToken = default);

        /// <summary>
        /// 批量为用户打标签
        /// </summary>
        /// <param name="tagId">标签编号</param>
        /// <param name="openids">粉丝编号</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>结果</returns>
        Task<WXApiError?> BatchTagAsync(int tagId, IEnumerable<string> openids, CancellationToken cancellationToken = default);

        /// <summary>
        /// 批量为用户取消标签
        /// </summary>
        /// <param name="tagId">标签编号</param>
        /// <param name="openids">粉丝编号</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>结果</returns>
        Task<WXApiError?> BatchUntagAsync(int tagId, IEnumerable<string> openids, CancellationToken cancellationToken = default);

        /// <summary>
        /// 创建标签
        /// </summary>
        /// <param name="name">标签名</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>操作结果</returns>
        Task<HttpClientResult<WXCreateTagResult, WXApiError>> CreateTagAsync(string name, CancellationToken cancellationToken = default);

        /// <summary>
        /// 删除标签
        /// </summary>
        /// <param name="id">标签编号</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>操作结果</returns>
        Task<WXApiError?> DeleteTagAsync(long id, CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取标签列表
        /// </summary>
        /// <param name="saveStream">Save stream</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Task</returns>
        Task GetTagListAsync(Stream saveStream, CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取所有用户
        /// </summary>
        /// <param name="nextOpenId">拉取列表的最后一个用户的OPENID，为空标识从头开始</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>结果</returns>
        Task<HttpClientResult<WXUserListResult, WXApiError>> GetUserListAsync(string? nextOpenId, CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取用户身上的标签列表
        /// </summary>
        /// <param name="openid">编号</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>结果</returns>
        Task<HttpClientResult<GetUserTagResult, WXApiError>> GetUserTagAsync(string openid, CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取标签下的所有用户
        /// </summary>
        /// <param name="tagId">标签编号</param>
        /// <param name="nextOpenId">拉取列表的最后一个用户的OPENID，为空标识从头开始</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>结果</returns>
        Task<HttpClientResult<WXUserListResult, WXApiError>> GetTagUserListAsync(int tagId, string? nextOpenId, CancellationToken cancellationToken = default);

        /// <summary>
        /// 编辑标签
        /// </summary>
        /// <param name="id">编号</param>
        /// <param name="name">新标签名</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>操作结果</returns>
        Task<WXApiError?> UpdateTagAsync(long id, string name, CancellationToken cancellationToken = default);

        /// <summary>
        /// 上传媒体文件
        /// </summary>
        /// <param name="fileName">文件名，如 file.png</param>
        /// <param name="bytes">字节数组</param>
        /// <param name="type">类型</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        Task<HttpClientResult<WXUploadMediaResult, WXApiError>> UploadMediaAsync(string fileName, byte[] bytes, WXMediaType? type = null, CancellationToken cancellationToken = default);
    }
}
