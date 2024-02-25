using com.etsoo.Utils;
using System.Diagnostics.CodeAnalysis;
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
        public required decimal LocationX { get; init; }

        /// <summary>
        /// 地理位置经度
        /// </summary>
        [XmlElement(ElementName = "Location_Y")]
        public required decimal LocationY { get; init; }

        /// <summary>
        /// 地图缩放大小
        /// </summary>
        public required int Scale { get; init; }

        /// <summary>
        /// 地理位置信息
        /// </summary>
        public required string Label { get; init; }

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
        [SetsRequiredMembers]
        public WXLocationMessage(Dictionary<string, string> dic) : base(dic)
        {
            LocationX = XmlUtils.GetValue<decimal>(dic, "Location_X").GetValueOrDefault();
            LocationY = XmlUtils.GetValue<decimal>(dic, "Location_Y").GetValueOrDefault();
            Scale = XmlUtils.GetValue<int>(dic, "Scale").GetValueOrDefault();
            Label = dic["Label"];
        }
    }
}
