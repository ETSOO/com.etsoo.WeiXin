using com.etsoo.Utils;
using System.Xml.Serialization;

namespace com.etsoo.WeiXin.Message
{
    /// <summary>
    /// 订阅事件消息
    /// </summary>
    [XmlRoot("xml")]
    public class WXSubscribeEventMessage : WXEventMessage
    {
        /// <summary>
        /// 事件类型
        /// </summary>
        public override WXEventType Event => WXEventType.subscribe;

        /// <summary>
        /// 事件 KEY 值，qrscene_为前缀，后面为二维码的参数值
        /// </summary>
        public string? EventKey { get; init; }

        /// <summary>
        /// 场景值
        /// </summary>
        [XmlIgnore]
        public string? SceneId => EventKey != null && EventKey.StartsWith("qrscene_") ? EventKey.Substring(8) : null;

        /// <summary>
        /// 二维码的ticket，可用来换取二维码图片
        /// </summary>
        public string? Ticket { get; init; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public WXSubscribeEventMessage() : base()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dic">字典数据</param>
        public WXSubscribeEventMessage(Dictionary<string, string> dic) : base(dic)
        {
            if (dic is not null)
            {
                EventKey = XmlUtils.GetValue(dic, "EventKey");
                Ticket = XmlUtils.GetValue(dic, "Ticket");
            }
        }
    }
}
