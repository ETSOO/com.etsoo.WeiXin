using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace com.etsoo.WeiXin.Message
{
    /// <summary>
    /// 浏览链接菜单事件消息
    /// </summary>
    [XmlRoot("xml")]
    public class WXViewEventMessage : WXEventMessage
    {
        /// <summary>
        /// 事件类型
        /// </summary>
        public override WXEventType Event => WXEventType.VIEW;

        /// <summary>
        /// 事件 KEY 值，设置的跳转URL
        /// </summary>
        public required string EventKey { get; init; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public WXViewEventMessage() : base()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dic">字典数据</param>
        [SetsRequiredMembers]
        public WXViewEventMessage(Dictionary<string, string> dic) : base(dic)
        {
            EventKey = dic["EventKey"];
        }
    }
}
