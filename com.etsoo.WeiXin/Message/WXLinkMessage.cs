using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace com.etsoo.WeiXin.Message
{
    /// <summary>
    /// 链接消息
    /// </summary>
    [XmlRoot("xml")]
    public class WXLinkMessage : WXNormalMessage
    {
        /// <summary>
        /// 消息类型
        /// </summary>
        public override WXMessageType MsgType => WXMessageType.link;

        /// <summary>
        /// 消息标题
        /// </summary>
        public required string Title { get; init; }

        /// <summary>
        /// 消息描述
        /// </summary>
        public required string Description { get; init; }

        /// <summary>
        /// 消息链接
        /// </summary>
        public required string Url { get; init; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public WXLinkMessage() : base()
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dic">字典数据</param>
        [SetsRequiredMembers]
        public WXLinkMessage(Dictionary<string, string> dic) : base(dic)
        {
            Title = dic["Title"];
            Description = dic["Description"];
            Url = dic["Url"];
        }
    }
}
