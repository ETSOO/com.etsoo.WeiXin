using com.etsoo.Utils;
using System.Xml;
using System.Xml.Serialization;

namespace com.etsoo.WeiXin.Message
{
    /// <summary>
    /// 图片消息
    /// </summary>
    [XmlRoot("xml")]
    public class WXImageMessage : WXNormalMessage
    {
        /// <summary>
        /// 回复文本消息
        /// </summary>
        /// <param name="writer">写入器</param>
        /// <param name="mediaId">通过素材管理中的接口上传多媒体文件，得到的id</param>
        /// <returns>任务</returns>
        public static async Task ReplyWithAsync(XmlWriter writer, string mediaId)
        {
            await XmlUtils.WriteCDataAsync(writer, "MsgType", WXMessageType.image.ToString());

            await writer.WriteStartElementAsync(null, "Image", null);

            await XmlUtils.WriteCDataAsync(writer, "MediaId", mediaId);

            await writer.WriteEndElementAsync();
        }

        /// <summary>
        /// 消息类型
        /// </summary>
        public override WXMessageType MsgType => WXMessageType.image;

        /// <summary>
        /// 图片链接（由系统生成）
        /// </summary>
        public string PicUrl { get; init; } = null!;

        /// <summary>
        /// 图片消息媒体id，可以调用获取临时素材接口拉取数据
        /// </summary>
        public string MediaId { get; init; } = null!;

        /// <summary>
        /// 构造函数
        /// </summary>
        public WXImageMessage() : base()
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dic">字典数据</param>
        public WXImageMessage(Dictionary<string, string> dic) : base(dic)
        {
            if (dic is not null)
            {
                PicUrl = dic["PicUrl"];
                MediaId = dic["MediaId"];
            }
        }
    }
}
