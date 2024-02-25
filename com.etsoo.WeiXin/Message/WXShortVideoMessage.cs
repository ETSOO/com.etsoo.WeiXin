using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace com.etsoo.WeiXin.Message
{
    /// <summary>
    /// 小视频消息
    /// </summary>
    [XmlRoot("xml")]
    public class WXShortVideoMessage : WXNormalMessage
    {
        /// <summary>
        /// 消息类型
        /// </summary>
        public override WXMessageType MsgType => WXMessageType.shortvideo;

        /// <summary>
        /// 小视频消息媒体id，可以调用获取临时素材接口拉取数据
        /// </summary>
        public required string MediaId { get; init; }

        /// <summary>
        /// 视频消息缩略图的媒体id，可以调用获取临时素材接口拉取数据
        /// </summary>
        public required string ThumbMediaId { get; init; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public WXShortVideoMessage() : base()
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dic">字典数据</param>
        [SetsRequiredMembers]
        public WXShortVideoMessage(Dictionary<string, string> dic) : base(dic)
        {
            ThumbMediaId = dic["ThumbMediaId"];
            MediaId = dic["MediaId"];
        }
    }
}
