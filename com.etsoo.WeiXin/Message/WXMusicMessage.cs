using com.etsoo.Utils;
using System.Xml;

namespace com.etsoo.WeiXin.Message
{
    /// <summary>
    /// 音乐消息
    /// </summary>
    public class WXMusicMessage : WXNormalMessage
    {
        /// <summary>
        /// 回复音乐消息
        /// </summary>
        /// <param name="writer">写入器</param>
        /// <param name="thumbMediaId">缩略图的媒体id，通过素材管理中的接口上传多媒体文件，得到的id</param>
        /// <param name="musicURL">音乐链接</param>
        /// <param name="hqMusicUrl">高质量音乐链接，WIFI环境优先使用该链接播放音乐</param>
        /// <param name="title">音乐标题</param>
        /// <param name="description">音乐描述</param>
        /// <returns>任务</returns>
        public static async Task ReplyWithAsync(XmlWriter writer, string thumbMediaId, string musicURL, string? hqMusicUrl = null, string? title = null, string? description = null)
        {
            await XmlUtils.WriteCDataAsync(writer, "MsgType", WXMessageType.music.ToString());

            await writer.WriteStartElementAsync(null, "Music", null);

            await XmlUtils.WriteCDataAsync(writer, "ThumbMediaId", thumbMediaId);
            await XmlUtils.WriteCDataAsync(writer, "MusicUrl", musicURL);

            if (!string.IsNullOrEmpty(hqMusicUrl))
            {
                await XmlUtils.WriteCDataAsync(writer, "HQMusicUrl", hqMusicUrl);
            }

            if (!string.IsNullOrEmpty(title))
            {
                await XmlUtils.WriteCDataAsync(writer, "Title", title);
            }

            if (!string.IsNullOrEmpty(description))
            {
                await XmlUtils.WriteCDataAsync(writer, "Description", description);
            }

            await writer.WriteEndElementAsync();
        }

        /// <summary>
        /// 消息类型
        /// </summary>
        public override WXMessageType MsgType => WXMessageType.music;

        /// <summary>
        /// 构造函数
        /// </summary>
        public WXMusicMessage() : base()
        {
        }
    }
}
