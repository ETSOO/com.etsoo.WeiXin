using com.etsoo.Utils;
using System.Xml.Serialization;

namespace com.etsoo.WeiXin.Message
{
    /// <summary>
    /// 发送订阅通知项目
    /// </summary>
    [XmlRoot("List")]
    public class WXSubscribeSendEventItem
    {
        /// <summary>
        /// 模板 id
        /// </summary>
        public string TemplateId { get; init; } = null!;

        /// <summary>
        /// 消息 id
        /// </summary>
        public string MsgID { get; init; } = null!;

        /// <summary>
        /// 推送结果状态码（0表示成功）
        /// </summary>
        public int ErrorCode { get; init; }

        /// <summary>
        /// 推送结果状态码文字含义
        /// </summary>
        public string ErrorStatus { get; init; } = null!;
    }

    /// <summary>
    /// 发送订阅通知
    /// </summary>
    [XmlRoot("xml")]
    public class WXSubscribeSendEventMessage : WXEventMessage
    {
        /// <summary>
        /// 事件类型
        /// </summary>
        public override WXEventType Event => WXEventType.subscribe_msg_sent_event;

        /// <summary>
        /// 细节
        /// </summary>
        public WXSubscribeSendEventItem[] SubscribeMsgSentEvent { get; init; } = null!;

        /// <summary>
        /// 构造函数
        /// </summary>
        public WXSubscribeSendEventMessage() : base()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dic">字典数据</param>
        public WXSubscribeSendEventMessage(Dictionary<string, string> dic) : base(dic)
        {
            if (dic is not null)
            {
                SubscribeMsgSentEvent = XmlUtils.GetList(dic["SubscribeMsgSentEvent"]).Select(item => new WXSubscribeSendEventItem
                {
                    TemplateId = item["TemplateId"],
                    MsgID = item["MsgID"],
                    ErrorCode = XmlUtils.GetValue<int>(item, "ErrorCode").GetValueOrDefault(),
                    ErrorStatus = item["ErrorStatus"]
                }).ToArray();
            }
        }
    }
}
