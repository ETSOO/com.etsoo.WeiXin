using com.etsoo.Utils;
using System.Diagnostics.CodeAnalysis;
using System.Xml;
using System.Xml.Serialization;

namespace com.etsoo.WeiXin.Message
{
    /// <summary>
    /// 语音消息
    /// </summary>
    [XmlRoot("xml")]
    public class WXVoiceMessage : WXNormalMessage
    {
        /// <summary>
        /// 回复语音消息
        /// </summary>
        /// <param name="writer">写入器</param>
        /// <param name="mediaId">通过素材管理中的接口上传多媒体文件，得到的id</param>
        /// <returns>任务</returns>
        public static async Task ReplyWithAsync(XmlWriter writer, string mediaId)
        {
            await XmlUtils.WriteCDataAsync(writer, "MsgType", WXMessageType.voice.ToString());

            await writer.WriteStartElementAsync(null, "Voice", null);

            await XmlUtils.WriteCDataAsync(writer, "MediaId", mediaId);

            await writer.WriteEndElementAsync();
        }

        /// <summary>
        /// 消息类型
        /// </summary>
        public override WXMessageType MsgType => WXMessageType.voice;

        /// <summary>
        /// 语音格式，如amr，speex等
        /// </summary>
        public required string Format { get; init; }

        /// <summary>
        /// 语音消息媒体id，可以调用获取临时素材接口拉取数据
        /// </summary>
        public required string MediaId { get; init; }

        /// <summary>
        /// 通语音识别后，用户每次发送语音给公众号时，微信会在推送的语音消息 XML 数据包中，增加一个 Recognition 字段
        /// </summary>
        public string? Recognition { get; init; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public WXVoiceMessage() : base()
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dic">字典数据</param>
        [SetsRequiredMembers]
        public WXVoiceMessage(Dictionary<string, string> dic) : base(dic)
        {
            Format = dic["Format"];
            MediaId = dic["MediaId"];
            Recognition = XmlUtils.GetValue(dic, "Recognition");
        }
    }
}
