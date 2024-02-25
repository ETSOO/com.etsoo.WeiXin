using System.Text.Json.Serialization;

namespace com.etsoo.WeiXinService
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
    [JsonSerializable(typeof(EventAlertDto))]
    [JsonSerializable(typeof(LogAlertDto))]
    public partial class WXServiceJsonSerializerContext : JsonSerializerContext
    {
    }
}
