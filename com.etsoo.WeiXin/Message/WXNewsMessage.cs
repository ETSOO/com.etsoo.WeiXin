using com.etsoo.Utils;
using System.Xml;

namespace com.etsoo.WeiXin.Message
{
    /// <summary>
    /// 图文消息项目
    /// </summary>
    /// <param name="Title">图文消息标题</param>
    /// <param name="Description">图文消息描述</param>
    /// <param name="PicUrl">图片链接，支持JPG、PNG格式，较好的效果为大图360*200，小图200*200</param>
    /// <param name="Url">点击图文消息跳转链接</param>
    public record WXNewsItem(string Title, string Description, string PicUrl, string Url);

    /// <summary>
    /// 图文消息
    /// </summary>
    public class WXNewsMessage : WXNormalMessage
    {
        /// <summary>
        /// 回复图文消息
        /// </summary>
        /// <param name="writer">写入器</param>
        /// <param name="items">项目</param>
        /// <returns>任务</returns>
        public static async Task ReplyWithAsync(XmlWriter writer, IEnumerable<WXNewsItem> items)
        {
            await XmlUtils.WriteCDataAsync(writer, "MsgType", WXMessageType.news.ToString());

            await writer.WriteElementStringAsync(null, "ArticleCount", null, items.Count().ToString());

            await writer.WriteStartElementAsync(null, "Articles", null);

            foreach (var item in items)
            {
                await writer.WriteStartElementAsync(null, "item", null);

                await XmlUtils.WriteCDataAsync(writer, "Title", item.Title);
                await XmlUtils.WriteCDataAsync(writer, "Description", item.Description);
                await XmlUtils.WriteCDataAsync(writer, "PicUrl", item.PicUrl);
                await XmlUtils.WriteCDataAsync(writer, "Url", item.Url);

                await writer.WriteEndElementAsync();
            }

            await writer.WriteEndElementAsync();
        }

        /// <summary>
        /// 消息类型
        /// </summary>
        public override WXMessageType MsgType => WXMessageType.news;

        /// <summary>
        /// 构造函数
        /// </summary>
        public WXNewsMessage() : base()
        {
        }
    }
}
