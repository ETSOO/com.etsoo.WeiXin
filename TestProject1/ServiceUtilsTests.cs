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
                Tokens = new[] { "oCkMJj86v6J_auePAut2p0AIQy5s" },
                Service = "Seq",
                Id = "1234",
                Level ="Error",
                Message = "≥ˆœ÷“Ï≥£",
                Datetime = DateTime.Now
            };

            // Act
            var (ms, signature) = await ServiceUtils.SerializeAsync(message);

            var newMessage = await JsonSerializer.DeserializeAsync<LogAlertDto>(ms, ServiceUtils.JsonOptions);
            var result = await ServiceUtils.CheckSignatureAsync(newMessage, signature);

            // Assert
            Assert.IsTrue(result);
        }
    }
}