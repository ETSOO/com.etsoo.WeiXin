using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace com.etsoo.WeiXin.Message
{
    /// <summary>
    /// 扫码事件消息
    /// </summary>
    [XmlRoot("xml")]
    public class WXScanEventMessage : WXEventMessage
    {
        /// <summary>
        /// 事件类型
        /// </summary>
        public override WXEventType Event => WXEventType.SCAN;

        /// <summary>
        /// 事件 KEY 值，是一个32位无符号整数，即创建二维码时的二维码scene_id
        /// </summary>
        public required string EventKey { get; init; }

        /// <summary>
        /// 二维码的ticket，可用来换取二维码图片
        /// </summary>
        public required string Ticket { get; init; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public WXScanEventMessage() : base()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dic">字典数据</param>
        [SetsRequiredMembers]
        public WXScanEventMessage(Dictionary<string, string> dic) : base(dic)
        {
            EventKey = dic["EventKey"];
            Ticket = dic["Ticket"];
        }
    }
}
