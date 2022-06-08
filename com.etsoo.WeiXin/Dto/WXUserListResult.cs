namespace com.etsoo.WeiXin.Dto
{
    /// <summary>
    /// 用户列表数据对象
    /// </summary>
    /// <param name="Openid">Openid array</param>
    public record WXUserListData(string[] Openid);

    /// <summary>
    /// 用户列表结果
    /// </summary>
    /// <param name="Total">关注该公众账号的总用户数</param>
    /// <param name="Count">拉取的 OPENID 个数，最大值为10000</param>
    /// <param name="Data">列表数据</param>
    /// <param name="NextOpenid">拉取列表的最后一个用户的OPENID</param>
    public record WXUserListResult(int Total, int Count, WXUserListData Data, string NextOpenid);
}
