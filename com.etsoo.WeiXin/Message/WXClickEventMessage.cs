using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace com.etsoo.WeiXin.Message
{
    /// <summary>
    /// 点击菜单事件消息
    /// </summary>
    [XmlRoot("xml")]
    public class WXClickEventMessage : WXEventMessage
    {
        /// <summary>
        /// 事件类型
        /// </summary>
        public override WXEventType Event => WXEventType.CLICK;

        /// <summary>
        /// 事件 KEY 值，与自定义菜单接口中 KEY 值对应
        /// </summary>
        public required string EventKey { get; init; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public WXClickEventMessage() : base()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dic">字典数据</param>
        [SetsRequiredMembers]
        public WXClickEventMessage(Dictionary<string, string> dic) : base(dic)
        {
            EventKey = dic["EventKey"];
        }
    }
}
