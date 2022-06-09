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
        public string Touser { get; init; } = null!;

        /// <summary>
        /// 所需下发的订阅模板id
        /// </summary>
        public string TemplateId { get; init; } = null!;

        /// <summary>
        /// 跳转网页时填写
        /// </summary>
        public string? Page { get; init; }

        /// <summary>
        /// 跳转小程序信息
        /// </summary>
        public WXSendMiniprogram? Miniprogram { get; set; }

        /// <summary>
        /// 模板内容
        /// </summary>
        public Dictionary<string, WXSendMessageDataItem> Data { get; init; } = null!;
    }
}
