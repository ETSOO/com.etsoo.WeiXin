using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace com.etsoo.WeiXin.Message
{
    /// <summary>
    /// 取消订阅事件消息
    /// </summary>
    [XmlRoot("xml")]
    public class WXUnsubscribeEventMessage : WXEventMessage
    {
        /// <summary>
        /// 事件类型
        /// </summary>
        public override WXEventType Event => WXEventType.unsubscribe;

        /// <summary>
        /// 构造函数
        /// </summary>
        public WXUnsubscribeEventMessage() : base()
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dic">字典数据</param>
        [SetsRequiredMembers]
        public WXUnsubscribeEventMessage(Dictionary<string, string> dic) : base(dic)
        {
        }
    }
}
