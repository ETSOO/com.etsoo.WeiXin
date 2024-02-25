using com.etsoo.Utils;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace com.etsoo.WeiXin.Message
{
    /// <summary>
    /// 上报位置事件消息
    /// </summary>
    [XmlRoot("xml")]
    public class WXLocationEventMessage : WXEventMessage
    {
        /// <summary>
        /// 事件类型
        /// </summary>
        public override WXEventType Event => WXEventType.LOCATION;

        /// <summary>
        /// 地理位置纬度
        /// </summary>
        public required decimal Latitude { get; init; }

        /// <summary>
        /// 地理位置经度
        /// </summary>
        public required decimal Longitude { get; init; }

        /// <summary>
        /// 地理位置精度
        /// </summary>
        public required decimal Precision { get; init; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public WXLocationEventMessage() : base()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dic">字典数据</param>
        [SetsRequiredMembers]
        public WXLocationEventMessage(Dictionary<string, string> dic) : base(dic)
        {
            Latitude = XmlUtils.GetValue<decimal>(dic, "Latitude").GetValueOrDefault();
            Longitude = XmlUtils.GetValue<decimal>(dic, "Longitude").GetValueOrDefault();
            Precision = XmlUtils.GetValue<decimal>(dic, "Precision").GetValueOrDefault();
        }
    }
}
