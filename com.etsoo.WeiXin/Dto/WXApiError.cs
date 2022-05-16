namespace com.etsoo.WeiXin.Dto
{
    /// <summary>
    /// WeiXin Api Error
    /// 微信接口错误
    /// </summary>
    public class WXApiError
    {
        /// <summary>
        /// Error code
        /// 错误代码
        /// </summary>
        public int Errcode { get; init; }

        /// <summary>
        /// Error message
        /// 错误信息
        /// </summary>
        public string Errmsg { get; init; } = null!;
    }
}
