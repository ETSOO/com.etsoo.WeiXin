using com.etsoo.Utils;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace com.etsoo.WeiXin.Message
{
    /// <summary>
    /// 点击菜单事件消息
    /// </summary>
    [XmlRoot("xml")]
    public class WXTemplateSendEventMessage : WXEventMessage
    {
        /// <summary>
        /// 事件类型
        /// </summary>
        public override WXEventType Event => WXEventType.TEMPLATESENDJOBFINISH;

        /// <summary>
        /// 消息id，64位整型
        /// </summary>
        public required long MsgID { get; init; }

        /// <summary>
        /// 发送状态
        /// </summary>
        public required string Status { get; init; }

        /// <summary>
        /// 是否成功
        /// </summary>
        [XmlIgnore]
        public bool Success => Status == "success";

        /// <summary>
        /// 构造函数
        /// </summary>
        public WXTemplateSendEventMessage() : base()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dic">字典数据</param>
        [SetsRequiredMembers]
        public WXTemplateSendEventMessage(Dictionary<string, string> dic) : base(dic)
        {
            MsgID = XmlUtils.GetValue<long>(dic, "MsgID").GetValueOrDefault();
            Status = dic["Status"];
        }
    }
}
