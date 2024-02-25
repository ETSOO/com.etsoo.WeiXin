using com.etsoo.WeiXinService;
using System.Text.Json;

namespace TestProject1
{
    [TestClass]
    public class ServiceUtilsTests
    {
        [TestMethod]
        public async Task SerializeAsyncTests()
        {
            // Arrange
            var message = new LogAlertDto
            {
                Host = "Test",
                Tokens = ["oCkMJj86v6J_auePAut2p0AIQy5s"],
                Service = "Seq",
                Id = "1234",
                Level ="Error",
                Message = "≥ˆœ÷“Ï≥£",
                Datetime = DateTime.Now
            };

            // Act
            var (ms, signature) = await ServiceUtils.SerializeAsync(message, WXServiceJsonSerializerContext.Default.LogAlertDto);

            var newMessage = await JsonSerializer.DeserializeAsync(ms, WXServiceJsonSerializerContext.Default.LogAlertDto);
            Assert.IsNotNull(newMessage);

            var result = await ServiceUtils.CheckSignatureAsync(newMessage, signature, WXServiceJsonSerializerContext.Default.LogAlertDto);

            // Assert
            Assert.IsTrue(result);
        }
    }
}