﻿using com.etsoo.Utils;
using System.Xml.Serialization;

namespace com.etsoo.WeiXin.Message
{
    /// <summary>
    /// 用户管理订阅通知项目
    /// </summary>
    [XmlRoot("List")]
    public class WXSubscribeManageEventItem
    {
        /// <summary>
        /// 模板 id
        /// </summary>
        public string TemplateId { get; init; } = null!;

        /// <summary>
        /// 用户点击行为（仅推送用户拒收/reject通知）
        /// </summary>
        public string SubscribeStatusString { get; init; } = null!;
    }

    /// <summary>
    /// 用户管理订阅通知
    /// </summary>
    [XmlRoot("xml")]
    public class WXSubscribeManageEventMessage : WXEventMessage
    {
        /// <summary>
        /// 事件类型
        /// </summary>
        public override WXEventType Event => WXEventType.subscribe_msg_change_event;

        /// <summary>
        /// 细节
        /// </summary>
        public WXSubscribeManageEventItem[] SubscribeMsgChangeEvent { get; init; } = null!;

        /// <summary>
        /// 构造函数
        /// </summary>
        public WXSubscribeManageEventMessage() : base()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dic">字典数据</param>
        public WXSubscribeManageEventMessage(Dictionary<string, string> dic) : base(dic)
        {
            if (dic is not null)
            {
                SubscribeMsgChangeEvent = XmlUtils.GetList(dic["SubscribeMsgChangeEvent"]).Select(item => new WXSubscribeManageEventItem
                {
                    TemplateId = item["TemplateId"],
                    SubscribeStatusString = item["SubscribeStatusString"]
                }).ToArray();
            }
        }
    }
}
