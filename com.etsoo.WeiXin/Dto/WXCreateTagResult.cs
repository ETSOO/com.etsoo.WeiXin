namespace com.etsoo.WeiXin.Dto
{
    /// <summary>
    /// 标签结果数据
    /// </summary>
    /// <param name="Id">标签id，由微信分配</param>
    /// <param name="Name">标签名</param>
    public record WXCreateTagData(long Id, string Name);

    /// <summary>
    /// 标签结果
    /// </summary>
    public record WXCreateTagResult(WXCreateTagData Tag);
}
