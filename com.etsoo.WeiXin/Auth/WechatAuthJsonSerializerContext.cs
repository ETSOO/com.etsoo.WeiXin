using System.Text.Json.Serialization;

namespace com.etsoo.WeiXin.Auth
{
    /// <summary>
    /// JSON serializer context
    /// JSON 序列化器上下文
    /// </summary>
    [JsonSourceGenerationOptions(
        PropertyNamingPolicy = JsonKnownNamingPolicy.SnakeCaseLower,
        DictionaryKeyPolicy = JsonKnownNamingPolicy.SnakeCaseLower,
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    )]
    [JsonSerializable(typeof(WechatTokenData))]
    [JsonSerializable(typeof(WechatUserInfo))]
    public partial class WechatAuthJsonSerializerContext : JsonSerializerContext
    {
    }
}
