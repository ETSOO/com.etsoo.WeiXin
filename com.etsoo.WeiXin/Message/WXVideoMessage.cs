using System.Xml;
using System.Xml.Serialization;

namespace com.etsoo.WeiXin.Message
{
    /// <summary>
    /// 视频消息
    /// </summary>
    [XmlRoot("xml")]
    public class WXVideoMessage : WXNormalMessage
    {
        /// <summary>
        /// 回复视频消息
        /// </summary>
        /// <param name="writer">写入器</param>
        /// <param name="mediaId">通过素材管理中的接口上传多媒体文件，得到的id</param>
        /// <param name="title">视频消息的标题</param>
        /// <param name="description">视频消息的描述</param>
        /// <returns>任务</returns>
        public static async Task ReplyWithAsync(XmlWriter writer, string mediaId, string? title = null, string? description = null)
        {
            await writer.WriteStartElementAsync(null, "MsgType", null);
            await writer.WriteCDataAsync(WXMessageType.video.ToString());
            await writer.WriteEndElementAsync();

            await writer.WriteStartElementAsync(null, "Video", null);

            await writer.WriteStartElementAsync(null, "MediaId", null);
            await writer.WriteCDataAsync(mediaId);
            await writer.WriteEndElementAsync();

            if (!string.IsNullOrEmpty(title))
            {
                await writer.WriteStartElementAsync(null, "Title", null);
                await writer.WriteCDataAsync(title);
                await writer.WriteEndElementAsync();
            }

            if (!string.IsNullOrEmpty(description))
            {
                await writer.WriteStartElementAsync(null, "Description", null);
                await writer.WriteCDataAsync(description);
                await writer.WriteEndElementAsync();
            }

            await writer.WriteEndElementAsync();
        }

        /// <summary>
        /// 消息类型
        /// </summary>
        public override WXMessageType MsgType => WXMessageType.video;

        /// <summary>
        /// 视频消息媒体id，可以调用获取临时素材接口拉取数据。
        /// </summary>
        public required string MediaId { get; init; }

        /// <summary>
        /// 视频消息缩略图的媒体id，可以调用多媒体文件下载接口拉取数据
        /// </summary>
        public required string ThumbMediaId { get; init; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public WXVideoMessage() : base()
        {
        }
    }
}
