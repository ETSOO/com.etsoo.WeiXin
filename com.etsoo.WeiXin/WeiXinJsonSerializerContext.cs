using com.etsoo.WeiXin.Dto;
using System.Text.Json.Serialization;

namespace com.etsoo.WeiXin
{
    /// <summary>
    /// JSON serializer context
    /// JSON 序列化器上下文
    /// </summary>
    [JsonSourceGenerationOptions(
        PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
        DictionaryKeyPolicy = JsonKnownNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    )]
    [JsonSerializable(typeof(WXAccessTokenResult))]
    [JsonSerializable(typeof(WXApiError))]
    [JsonSerializable(typeof(WXCreateTagResult))]
    [JsonSerializable(typeof(WXSendMessageInput))]
    [JsonSerializable(typeof(WXUploadMediaResult))]
    [JsonSerializable(typeof(WXUserListResult))]
    [JsonSerializable(typeof(GetUserTagResult))]
    public partial class WeiXinJsonSerializerContext : JsonSerializerContext
    {
    }
}
