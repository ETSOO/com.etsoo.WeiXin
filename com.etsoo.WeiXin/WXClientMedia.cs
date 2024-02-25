using com.etsoo.HTTP;
using com.etsoo.WeiXin.Dto;
using System.Net.Http.Headers;

namespace com.etsoo.WeiXin
{
    /// <summary>
    /// WeiXin Client - Media
    /// 微信客户端 - 媒体
    /// </summary>
    public partial class WXClient : HttpClientService, IWXClient
    {
        /// <summary>
        /// 下载临时媒体文件
        /// </summary>
        /// <param name="mediaId">Media id</param>
        /// <param name="saveStream">Save stream</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Task</returns>
        public async Task<string> DownloadMediaAsync(string mediaId, Stream saveStream, CancellationToken cancellationToken = default)
        {
            var accessToken = await GetAcessTokenAsync(cancellationToken);
            var api = $"{ApiUri}media/get?access_token={accessToken}&media_id={mediaId}";
            return await DownloadAsync(api, saveStream, cancellationToken);
        }

        /// <summary>
        /// 上传媒体文件
        /// </summary>
        /// <param name="fileName">文件名，如 file.png</param>
        /// <param name="bytes">字节数组</param>
        /// <param name="type">类型</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        public async Task<HttpClientResult<WXUploadMediaResult, WXApiError>> UploadMediaAsync(string fileName, byte[] bytes, WXMediaType? type = null, CancellationToken cancellationToken = default)
        {
            if (type == null)
            {
                var ext = Path.GetExtension(fileName).ToLower();
                var mediaType = MimeTypeMap.TryGetMimeType(ext);
                if (mediaType is null) throw new NullReferenceException(nameof(mediaType));

                type = mediaType switch
                {
                    string a when a.StartsWith("image/") => WXMediaType.image,
                    string a when a.StartsWith("audio/") => WXMediaType.voice,
                    string a when a.StartsWith("video/") => WXMediaType.video,
                    _ => WXMediaType.thumb
                };
            }

            var accessToken = await GetAcessTokenAsync(cancellationToken);
            var api = $"{ApiUri}media/upload?access_token={accessToken}&type={type}";

            using var content = new MultipartFormDataContent();

            var file = new ByteArrayContent(bytes);
            file.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = "media",
                // 微信服务器端需要文件名在引号中，否则失败
                FileName = $"\"{fileName}\""
            };
            // file.Headers.ContentType = new MediaTypeHeaderValue(mediaType);

            content.Add(file);

            // 默认会生成：boundary="792cf2f7-9b2f-417c-86f2-c0c7790f26b0"
            // 微信服务器不能处理引号，需要去掉，否则失败 "errcode":41005,"errmsg":"media data missing hint
            var boundary = content.Headers?.ContentType?.Parameters.FirstOrDefault(o => o.Name == "boundary");
            if (boundary is not null)
                boundary.Value = boundary.Value?.Replace("\"", string.Empty);

            return await PostAsync(api, content, "media_id",
                WeiXinJsonSerializerContext.Default.WXUploadMediaResult,
                WeiXinJsonSerializerContext.Default.WXApiError,
                cancellationToken);
        }
    }
}
