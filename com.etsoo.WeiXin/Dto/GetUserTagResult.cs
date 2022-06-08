namespace com.etsoo.WeiXin.Dto
{
    /// <summary>
    /// 获取用户身上的标签列表结果
    /// </summary>
    /// <param name="TagidList">标签编号数组</param>
    public record GetUserTagResult(int[] TagidList);
}
