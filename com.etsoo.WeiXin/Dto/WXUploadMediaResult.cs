using System.Text.Json.Serialization;

namespace com.etsoo.WeiXin.Dto
{
    /// <summary>
    /// WeiXin upload media file result
    /// 微信上传媒体文件结果
    /// </summary>
    public record WXUploadMediaResult
    {
        /// <summary>
        /// 媒体文件类型
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter<WXMediaType>))]
        public required WXMediaType Type { get; init; }

        /// <summary>
        /// 媒体文件标识
        /// </summary>
        public required string MediaId { get; init; }

        /// <summary>
        /// 上传时间戳
        /// </summary>
        public required long CreatedAt { get; init; }
    }
}
