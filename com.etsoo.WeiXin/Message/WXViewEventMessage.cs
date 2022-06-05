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
        public string EventKey { get; init; } = null!;

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
        public WXViewEventMessage(Dictionary<string, string> dic) : base(dic)
        {
            if (dic is not null)
            {
                EventKey = dic["EventKey"];
            }
        }
    }
}
