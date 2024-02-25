using com.etsoo.Utils;
using System.Diagnostics.CodeAnalysis;
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
        public required int Count { get; init; }

        /// <summary>
        /// 图片列表
        /// </summary>
        public required WXSendPicsInfoItem[] PicList { get; init; }
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
        public required string PicMd5Sum { get; init; }
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
        public required string EventKey { get; init; }

        /// <summary>
        /// 发送的图片信息
        /// </summary>
        public required WXSendPicsInfo SendPicsInfo { get; init; }

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
        [SetsRequiredMembers]
        public WXSysPhotoEventMessage(Dictionary<string, string> dic) : base(dic)
        {
            EventKey = dic["EventKey"];
            SendPicsInfo = WXSendPicsInfo.Create(dic);
        }
    }
}
