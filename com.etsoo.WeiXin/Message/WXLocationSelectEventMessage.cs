using com.etsoo.Utils;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace com.etsoo.WeiXin.Message
{
    /// <summary>
    /// 位置信息
    /// </summary>
    public class WXLocationInfo
    {
        /// <summary>
        /// 地理位置纬度
        /// </summary>
        [XmlElement(ElementName = "Location_X")]
        public decimal LocationX { get; init; }

        /// <summary>
        /// 地理位置经度
        /// </summary>
        [XmlElement(ElementName = "Location_Y")]
        public decimal LocationY { get; init; }

        /// <summary>
        /// 地图缩放大小
        /// </summary>
        public required int Scale { get; init; }

        /// <summary>
        /// 地理位置信息
        /// </summary>
        public required string Label { get; init; }

        /// <summary>
        /// 朋友圈 POI 的名字，可能为空
        /// </summary>
        public string? Poiname { get; init; }
    }

    /// <summary>
    /// 弹出地理位置选择器的事件推送
    /// </summary>
    [XmlRoot("xml")]
    public class WXLocationSelectEventMessage : WXEventMessage
    {
        /// <summary>
        /// 事件类型
        /// </summary>
        public override WXEventType Event => WXEventType.location_select;

        /// <summary>
        /// 事件 KEY 值，与自定义菜单接口中 KEY 值对应
        /// </summary>
        public required string EventKey { get; init; }

        /// <summary>
        /// 发送的位置信息
        /// </summary>
        public required WXLocationInfo SendLocationInfo { get; init; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public WXLocationSelectEventMessage() : base()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dic">字典数据</param>
        [SetsRequiredMembers]
        public WXLocationSelectEventMessage(Dictionary<string, string> dic) : base(dic)
        {
            EventKey = dic["EventKey"];

            var info = XmlUtils.ParseXml(SharedUtils.GetStream($"<xml>{dic["SendLocationInfo"]}</xml>"));
            SendLocationInfo = new WXLocationInfo
            {
                LocationX = XmlUtils.GetValue<decimal>(info, "Location_X").GetValueOrDefault(),
                LocationY = XmlUtils.GetValue<decimal>(info, "Location_Y").GetValueOrDefault(),
                Scale = XmlUtils.GetValue<int>(info, "Scale").GetValueOrDefault(),
                Label = info["Label"],
                Poiname = XmlUtils.GetValue(info, "Poiname")
            };
        }
    }
}
