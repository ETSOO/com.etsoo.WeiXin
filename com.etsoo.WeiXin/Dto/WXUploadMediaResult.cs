using System.Text.Json.Serialization;

namespace com.etsoo.WeiXin.Dto
{
    /// <summary>
    /// WeiXin upload media file result
    /// 微信上传媒体文件结果
    /// </summary>
    public class WXUploadMediaResult
    {
        /// <summary>
        /// 媒体文件类型
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public WXMediaType Type { get; init; }

        /// <summary>
        /// 媒体文件标识
        /// </summary>
        public string MediaId { get; init; } = null!;

        /// <summary>
        /// 上传时间戳
        /// </summary>
        public long CreatedAt { get; init; }
    }
}
