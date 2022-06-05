using System.Xml.Serialization;

namespace com.etsoo.WeiXin.Message
{
    /// <summary>
    /// 弹出微信相册发图器的事件推送
    /// </summary>
    [XmlRoot("xml")]
    public class WXWeiXinPhotoEventMessage : WXEventMessage
    {
        /// <summary>
        /// 事件类型
        /// </summary>
        public override WXEventType Event => WXEventType.pic_weixin;

        /// <summary>
        /// 事件 KEY 值，与自定义菜单接口中 KEY 值对应
        /// </summary>
        public string EventKey { get; init; } = null!;

        /// <summary>
        /// 发送的图片信息
        /// </summary>
        public WXSendPicsInfo SendPicsInfo { get; init; } = null!;

        /// <summary>
        /// 构造函数
        /// </summary>
        public WXWeiXinPhotoEventMessage() : base()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dic">字典数据</param>
        public WXWeiXinPhotoEventMessage(Dictionary<string, string> dic) : base(dic)
        {
            if (dic is not null)
            {
                EventKey = dic["EventKey"];
                SendPicsInfo = WXSendPicsInfo.Create(dic);
            }
        }
    }
}
