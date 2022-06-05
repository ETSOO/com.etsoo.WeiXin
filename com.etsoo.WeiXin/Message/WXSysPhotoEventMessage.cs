using com.etsoo.Utils;
using System.Xml.Serialization;

namespace com.etsoo.WeiXin.Message
{
    /// <summary>
    /// 发送的图片信息
    /// </summary>
    [XmlRoot("SendPicsInfo")]
    public class WXSendPicsInfo
    {
        /// <summary>
        /// 创建图片信息对象
        /// </summary>
        /// <param name="dic">字典数据</param>
        /// <returns>信息对象</returns>
        public static WXSendPicsInfo Create(Dictionary<string, string> dic)
        {
            var info = XmlUtils.ParseXml(SharedUtils.GetStream($"<xml>{dic["SendPicsInfo"]}</xml>"), 1);
            var count = XmlUtils.GetValue<int>(info, "Count").GetValueOrDefault();
            var items = XmlUtils.GetList(XmlUtils.GetValue(info, "PicList")).Select(item => new WXSendPicsInfoItem { PicMd5Sum = item["PicMd5Sum"] });
            return new WXSendPicsInfo { Count = count, PicList = items.ToArray() };
        }

        /// <summary>
        /// 发送的图片数量
        /// </summary>
        public int Count { get; init; }

        /// <summary>
        /// 图片列表
        /// </summary>
        public WXSendPicsInfoItem[] PicList { get; init; } = null!;
    }

    /// <summary>
    /// 发送的图片项目
    /// </summary>
    [XmlRoot("item")]
    public class WXSendPicsInfoItem
    {
        /// <summary>
        /// 图片的MD5值，开发者若需要，可用于验证接收到图片
        /// </summary>
        public string PicMd5Sum { get; init; } = null!;
    }

    /// <summary>
    /// 弹出系统拍照发图的事件推送
    /// </summary>
    [XmlRoot("xml")]
    public class WXSysPhotoEventMessage : WXEventMessage
    {
        /// <summary>
        /// 事件类型
        /// </summary>
        public override WXEventType Event => WXEventType.pic_sysphoto;

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
        public WXSysPhotoEventMessage() : base()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dic">字典数据</param>
        public WXSysPhotoEventMessage(Dictionary<string, string> dic) : base(dic)
        {
            if (dic is not null)
            {
                EventKey = dic["EventKey"];
                SendPicsInfo = WXSendPicsInfo.Create(dic);
            }
        }
    }
}
