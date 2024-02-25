using com.etsoo.Utils;
using System.Diagnostics.CodeAnalysis;
using System.Xml;
using System.Xml.Serialization;

namespace com.etsoo.WeiXin.Message
{
    /// <summary>
    /// 文本消息
    /// </summary>
    [XmlRoot("xml")]
    public class WXTextMessage : WXNormalMessage
    {
        /// <summary>
        /// 回复文本消息
        /// </summary>
        /// <param name="writer">写入器</param>
        /// <param name="content">内容</param>
        /// <returns>任务</returns>
        public static async Task ReplyWithAsync(XmlWriter writer, string content)
        {
            await XmlUtils.WriteCDataAsync(writer, "MsgType", WXMessageType.text.ToString());
            await XmlUtils.WriteCDataAsync(writer, "Content", content);
        }

        /// <summary>
        /// 消息类型
        /// </summary>
        public override WXMessageType MsgType => WXMessageType.text;

        /// <summary>
        /// 文本消息内容
        /// </summary>
        public required string Content { get; init; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public WXTextMessage() : base()
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dic">字典数据</param>
        [SetsRequiredMembers]
        public WXTextMessage(Dictionary<string, string> dic) : base(dic)
        {
            Content = dic["Content"];
        }
    }
}
