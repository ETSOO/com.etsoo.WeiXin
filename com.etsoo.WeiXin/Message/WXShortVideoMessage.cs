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
        public string MediaId { get; init; } = null!;

        /// <summary>
        /// 视频消息缩略图的媒体id，可以调用获取临时素材接口拉取数据
        /// </summary>
        public string ThumbMediaId { get; init; } = null!;

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
        public WXShortVideoMessage(Dictionary<string, string> dic) : base(dic)
        {
            if (dic is not null)
            {
                ThumbMediaId = dic["ThumbMediaId"];
                MediaId = dic["MediaId"];
            }
        }
    }
}
