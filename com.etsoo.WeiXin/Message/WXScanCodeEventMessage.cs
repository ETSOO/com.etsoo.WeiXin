﻿using com.etsoo.Utils;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace com.etsoo.WeiXin.Message
{
    /// <summary>
    /// 扫码事件信息
    /// </summary>
    public class WXScanCodeInfo
    {
        /// <summary>
        /// 扫描类型，一般是qrcode
        /// </summary>
        public string ScanType { get; init; } = null!;

        /// <summary>
        /// 扫描结果，即二维码对应的字符串信息
        /// 如果 ScanType = 'barcode'，返回 EAN_13, **** 形式
        /// </summary>
        public string ScanResult { get; init; } = null!;
    }

    /// <summary>
    /// 扫码推事件的事件推送
    /// </summary>
    [XmlRoot("xml")]
    public class WXScanCodeEventMessage : WXEventMessage
    {
        /// <summary>
        /// 事件类型
        /// </summary>
        public override WXEventType Event => WXEventType.scancode_push;

        /// <summary>
        /// 事件 KEY 值，由开发者在创建菜单时设定
        /// </summary>
        public required string EventKey { get; init; }

        /// <summary>
        /// 扫码信息
        /// </summary>
        public required WXScanCodeInfo ScanCodeInfo { get; init; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public WXScanCodeEventMessage() : base()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dic">字典数据</param>
        [SetsRequiredMembers]
        public WXScanCodeEventMessage(Dictionary<string, string> dic) : base(dic)
        {
            EventKey = dic["EventKey"];

            var info = XmlUtils.ParseXml(SharedUtils.GetStream($"<xml>{dic["ScanCodeInfo"]}</xml>"), 1);
            ScanCodeInfo = new WXScanCodeInfo { ScanType = info["ScanType"], ScanResult = info["ScanResult"] };
        }
    }
}
