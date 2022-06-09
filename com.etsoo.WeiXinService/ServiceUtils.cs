using System.Security.Cryptography;
using System.Text.Json;

namespace com.etsoo.WeiXinService
{
    /// <summary>
    /// WeiXin service utils
    /// 微信服务工具
    /// </summary>
    public static class ServiceUtils
    {
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
    }
}
