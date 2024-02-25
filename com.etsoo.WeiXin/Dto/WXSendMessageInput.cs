namespace com.etsoo.WeiXin.Dto
{
    /// <summary>
    /// 发送消息的小程序信息
    /// </summary>
    /// <param name="Appid">程序编号</param>
    /// <param name="Pagepath">跳转的路径</param>
    public record WXSendMiniprogram(string Appid, string Pagepath);

    /// <summary>
    /// 发送消息的数据项目
    /// </summary>
    public record WXSendMessageDataItem(string Value, string? Color = null);

    /// <summary>
    /// 发送的消息
    /// </summary>
    public record WXSendMessageInput
    {
        /// <summary>
        /// 接收者（用户）的 openid
        /// </summary>
        public required string Touser { get; init; }

        /// <summary>
        /// 所需下发的订阅模板id
        /// </summary>
        public required string TemplateId { get; init; }

        /// <summary>
        /// 订阅消息跳转网页时填写
        /// </summary>
        public string? Page { get; init; }

        /// <summary>
        /// 模板消息跳转网页是Url
        /// </summary>
        public string? Url { get; init; }

        /// <summary>
        /// 跳转小程序信息
        /// </summary>
        public WXSendMiniprogram? Miniprogram { get; set; }

        /// <summary>
        /// 模板内容
        /// </summary>
        public required Dictionary<string, WXSendMessageDataItem> Data { get; init; }
    }
}
