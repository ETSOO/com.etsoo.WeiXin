using com.etsoo.Utils;
using System.Xml.Serialization;

namespace com.etsoo.WeiXin.Message
{
    /// <summary>
    /// 扫码推事件且弹出“消息接收中”提示框的事件推送
    /// </summary>
    [XmlRoot("xml")]
    public class WXScanCodeWaitEventMessage : WXEventMessage
    {
        /// <summary>
        /// 事件类型
        /// </summary>
        public override WXEventType Event => WXEventType.scancode_waitmsg;

        /// <summary>
        /// 事件 KEY 值，由开发者在创建菜单时设定
        /// </summary>
        public string EventKey { get; init; } = null!;

        /// <summary>
        /// 扫码信息
        /// </summary>
        public WXScanCodeInfo ScanCodeInfo { get; init; } = null!;

        /// <summary>
        /// 构造函数
        /// </summary>
        public WXScanCodeWaitEventMessage() : base()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dic">字典数据</param>
        public WXScanCodeWaitEventMessage(Dictionary<string, string> dic) : base(dic)
        {
            if (dic is not null)
            {
                EventKey = dic["EventKey"];

                var info = XmlUtils.ParseXml(SharedUtils.GetStream($"<xml>{dic["ScanCodeInfo"]}</xml>"), 1);
                ScanCodeInfo = new WXScanCodeInfo { ScanType = info["ScanType"], ScanResult = info["ScanResult"] };
            }
        }
    }
}
