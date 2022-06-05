using com.etsoo.Utils;
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
        public int Scale { get; init; }

        /// <summary>
        /// 地理位置信息
        /// </summary>
        public string Label { get; init; } = null!;

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
        public string EventKey { get; init; } = null!;

        /// <summary>
        /// 发送的位置信息
        /// </summary>
        public WXLocationInfo SendLocationInfo { get; init; } = null!;

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
        public WXLocationSelectEventMessage(Dictionary<string, string> dic) : base(dic)
        {
            if (dic is not null)
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
}
