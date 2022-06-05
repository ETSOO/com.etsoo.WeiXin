using com.etsoo.Utils;
using System.Xml.Serialization;

namespace com.etsoo.WeiXin.Message
{
    /// <summary>
    /// 位置消息
    /// </summary>
    [XmlRoot("xml")]
    public class WXLocationMessage : WXNormalMessage
    {
        /// <summary>
        /// 消息类型
        /// </summary>
        public override WXMessageType MsgType => WXMessageType.location;

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
        /// 构造函数
        /// </summary>
        public WXLocationMessage() : base()
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dic">字典数据</param>
        public WXLocationMessage(Dictionary<string, string> dic) : base(dic)
        {
            if (dic is not null)
            {
                LocationX = XmlUtils.GetValue<decimal>(dic, "Location_X").GetValueOrDefault();
                LocationY = XmlUtils.GetValue<decimal>(dic, "Location_Y").GetValueOrDefault();
                Scale = XmlUtils.GetValue<int>(dic, "Scale").GetValueOrDefault();
                Label = dic["Label"];
            }
        }
    }
}
