using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace com.etsoo.WeiXin.Message
{
    /// <summary>
    /// 点击菜单事件消息
    /// </summary>
    [XmlRoot("xml")]
    public class WXViewMiniprogramEventMessage : WXEventMessage
    {
        /// <summary>
        /// 事件类型
        /// </summary>
        public override WXEventType Event => WXEventType.view_miniprogram;

        /// <summary>
        /// 事件 KEY 值，跳转的小程序路径
        /// </summary>
        public required string EventKey { get; init; }

        /// <summary>
        /// 菜单ID，如果是个性化菜单，则可以通过这个字段，知道是哪个规则的菜单被点击了
        /// </summary>
        public required string MenuId { get; init; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public WXViewMiniprogramEventMessage() : base()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dic">字典数据</param>
        [SetsRequiredMembers]
        public WXViewMiniprogramEventMessage(Dictionary<string, string> dic) : base(dic)
        {
            EventKey = dic["EventKey"];
            MenuId = dic["MenuId"];
        }
    }
}
