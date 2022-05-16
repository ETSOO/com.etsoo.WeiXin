namespace com.etsoo.WeiXin.Dto
{
    /// <summary>
    /// WeiXin client exception
    /// 微信客户端异常
    /// </summary>
    public class WXClientException : Exception
    {
        /// <summary>
        /// Error
        /// 错误
        /// </summary>
        public WXApiError Error { get; }

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="error">Error</param>
        public WXClientException(WXApiError error) : base(error.Errmsg)
        {
            Error = error;
        }
    }
}
