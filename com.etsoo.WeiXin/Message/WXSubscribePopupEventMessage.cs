using com.etsoo.Utils;
using System.Xml.Serialization;

namespace com.etsoo.WeiXin.Message
{
    /// <summary>
    /// 用户操作订阅通知弹窗事件项目
    /// </summary>
    [XmlRoot("List")]
    public class WXSubscribeMsgPopupEventItem
    {
        /// <summary>
        /// 模板 id
        /// </summary>
        public string TemplateId { get; init; } = null!;

        /// <summary>
        /// 用户点击行为（同意/accept、取消/reject 发送通知）
        /// </summary>
        public string SubscribeStatusString { get; init; } = null!;

        /// <summary>
        /// 场景，1 = 弹窗来自 H5 页面, 2 = 弹窗来自图文消息
        /// </summary>
        public int PopupScene { get; init; }
    }

    /// <summary>
    /// 用户操作订阅通知弹窗
    /// </summary>
    [XmlRoot("xml")]
    public class WXSubscribePopupEventMessage : WXEventMessage
    {
        /// <summary>
        /// 事件类型
        /// </summary>
        public override WXEventType Event => WXEventType.subscribe_msg_popup_event;

        /// <summary>
        /// 细节
        /// </summary>
        public WXSubscribeMsgPopupEventItem[] SubscribeMsgPopupEvent { get; init; } = null!;

        /// <summary>
        /// 构造函数
        /// </summary>
        public WXSubscribePopupEventMessage() : base()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dic">字典数据</param>
        public WXSubscribePopupEventMessage(Dictionary<string, string> dic) : base(dic)
        {
            if (dic is not null)
            {
                SubscribeMsgPopupEvent = XmlUtils.GetList(dic["SubscribeMsgPopupEvent"]).Select(item => new WXSubscribeMsgPopupEventItem
                {
                    TemplateId = item["TemplateId"],
                    SubscribeStatusString = item["SubscribeStatusString"],
                    PopupScene = XmlUtils.GetValue<int>(item, "PopupScene").GetValueOrDefault()
                }).ToArray();
            }
        }
    }
}
