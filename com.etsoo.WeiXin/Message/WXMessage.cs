using com.etsoo.Utils;
using System.Xml;
using System.Xml.Serialization;

namespace com.etsoo.WeiXin.Message
{
    /// <summary>
    /// 微信消息
    /// </summary>
    public abstract class WXMessage
    {
        /// <summary>
        /// 消息类型
        /// </summary>
        public abstract WXMessageType MsgType { get; }

        /// <summary>
        /// 接收方账号
        /// </summary>
        public string ToUserName { get; init; } = null!;

        /// <summary>
        /// 发送方帐号（一个OpenID）
        /// </summary>
        public string FromUserName { get; init; } = null!;

        /// <summary>
        /// 消息创建时间Unix毫秒数
        /// </summary>
        public long CreateTime { get; init; }

        /// <summary>
        /// 消息创建时间
        /// </summary>
        [XmlIgnore]
        public DateTime CreateTimeUTC => SharedUtils.UnixSecondsToUTC(CreateTime);

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dic">字典数据</param>
        public WXMessage(Dictionary<string, string>? dic = null)
        {
            if (dic is not null)
            {
                ToUserName = dic["ToUserName"];
                FromUserName = dic["FromUserName"];
                CreateTime = long.Parse(dic["CreateTime"]);
            }
        }

        /// <summary>
        /// 被动回复用户信息
        /// </summary>
        /// <param name="output">输出流</param>
        /// <param name="func">回复的信息调用</param>
        /// <returns>任务</returns>
        public async Task ReplyAsync(Stream output, Func<XmlWriter, Task> func)
        {
            await using var writer = XmlWriter.Create(output, new XmlWriterSettings { Async = true, OmitXmlDeclaration = true });

            await writer.WriteStartElementAsync(null, "xml", null);

            await XmlUtils.WriteCDataAsync(writer, "ToUserName", FromUserName);
            await XmlUtils.WriteCDataAsync(writer, "FromUserName", ToUserName);
            await writer.WriteElementStringAsync(null, "CreateTime", null, SharedUtils.UTCToUnixSeconds().ToString());

            await func(writer);

            await writer.WriteEndElementAsync();
        }
    }

    /// <summary>
    /// 普通消息
    /// </summary>
    public abstract class WXNormalMessage : WXMessage
    {
        /// <summary>
        /// 消息id，64位整型
        /// </summary>
        public long MsgId { get; init; }

        /// <summary>
        /// 消息的数据ID（消息如果来自文章时才有）
        /// </summary>
        public string? MsgDataId { get; init; }

        /// <summary>
        /// 多图文时第几篇文章，从1开始（消息如果来自文章时才有）
        /// </summary>
        public int? Idx { get; init; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dic">字典数据</param>
        public WXNormalMessage(Dictionary<string, string>? dic = null) : base(dic)
        {
            if (dic is not null)
            {
                MsgId = XmlUtils.GetValue<long>(dic, "MsgId").GetValueOrDefault();
                MsgDataId = XmlUtils.GetValue(dic, "MsgDataId");
                Idx = XmlUtils.GetValue<int>(dic, "Idx");
            }
        }
    }
}
