using com.etsoo.Utils.String;
using System.Text.Json;

namespace com.etsoo.WeiXin.Support
{
    /// <summary>
    /// WeiXin client Json naming policy
    /// 微信客户端Json命名策略
    /// </summary>
    internal class WXClientJsonNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name)
        {
            return name.ToSnakeCase().ToString();
        }
    }
}
