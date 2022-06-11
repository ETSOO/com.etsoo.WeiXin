using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Web;

namespace com.etsoo.WeiXinService
{
    /// <summary>
    /// WeiXin service utils
    /// 微信服务工具
    /// </summary>
    public static class ServiceUtils
    {
        /// <summary>
        /// 到最大长度
        /// </summary>
        /// <param name="input">Input string</param>
        /// <param name="maxLength">Max length</param>
        /// <returns>Result</returns>
        public static string ToMaxLength(this string input, int maxLength)
        {
            return input[..Math.Min(input.Length, maxLength)];
        }

        /// <summary>
        /// 服务接口地址
        /// </summary>
        public static string ServiceApi = "https://wechatapi.etsoo.com/api";

        /// <summary>
        /// Json serialize/deserialize options
        /// Json序列化参数
        /// </summary>
        public static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

        /// <summary>
        /// Async check message signature
        /// 异步验证消息签名
        /// </summary>
        /// <typeparam name="T">Generic message type</typeparam>
        /// <param name="message">Message</param>
        /// <param name="signature">Signature to check</param>
        /// <returns>Result</returns>
        public static async Task<bool> CheckSignatureAsync<T>(T message, string signature)
        {
            var (_, signatureNew) = await SerializeAsync(message);
            return signatureNew.Equals(signature);
        }

        /// <summary>
        /// Async serialize message
        /// 异步序列号消息
        /// </summary>
        /// <typeparam name="T">Generic message type</typeparam>
        /// <param name="message">Message</param>
        /// <returns>Json & signature</returns>
        public static async Task<(Stream json, string signature)> SerializeAsync<T>(T message)
        {
            // Json
            var ms = new MemoryStream();
            await JsonSerializer.SerializeAsync(ms, message, JsonOptions);
            ms.Position = 0;

            // Signature
            using var sha = SHA256.Create();
            var shaBytes = await sha.ComputeHashAsync(ms);
            var sign = Convert.ToBase64String(shaBytes);
            ms.Position = 0;

            // Return
            return (ms, sign);
        }

        /// <summary>
        /// Create Json stream content
        /// 创建Json流内容
        /// </summary>
        /// <param name="json">Json</param>
        /// <returns>Content</returns>
        public static HttpContent CreateJsonStreamContent(Stream json)
        {
            var content = new StreamContent(json);

            var headerValue = new MediaTypeHeaderValue("application/json")
            {
                CharSet = Encoding.UTF8.WebName
            };
            content.Headers.ContentType = headerValue;

            return content;
        }

        /// <summary>
        /// 格式化时间，删除秒数，UTC
        /// </summary>
        /// <param name="input">Input datetime</param>
        /// <returns>Result</returns>
        public static DateTime FormatDateTime(DateTime input)
        {
            input = input.ToUniversalTime();
            return input.AddTicks(-(input.Ticks % TimeSpan.TicksPerSecond));
        }

        /// <summary>
        /// 发送日志提醒
        /// </summary>
        /// <param name="data">Message data</param>
        /// <param name="client">Client to send</param>
        /// <returns>Response message</returns>
        public static async Task<HttpResponseMessage> SendLogAlertAsync(LogAlertDto data, HttpClient client)
        {
            // 哈希
            var (json, signature) = await SerializeAsync(data);
            var signUrl = HttpUtility.UrlEncode(signature);

            // 内容
            using var content = CreateJsonStreamContent(json);

            // 返回
            return await client.PostAsync($"{ServiceApi}/Service/LogAlert/?signature={signUrl}", content);
        }

        /// <summary>
        /// 发送事件提醒
        /// </summary>
        /// <param name="data">Message data</param>
        /// <param name="client">Client to send</param>
        /// <returns>Response message</returns>
        public static async Task<HttpResponseMessage> SendEventAlertAsync(EventAlertDto data, HttpClient client)
        {
            // 哈希
            var (json, signature) = await SerializeAsync(data);
            var signUrl = HttpUtility.UrlEncode(signature);

            // 内容
            using var content = CreateJsonStreamContent(json);

            // 返回
            return await client.PostAsync($"{ServiceApi}/Service/EventAlert/?signature={signUrl}", content);
        }
    }
}
