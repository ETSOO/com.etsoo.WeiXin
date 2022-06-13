using com.etsoo.WeiXinService;
using System.Net;

namespace TestProject1
{
    [TestClass]
    public class WXServiceUtilsTests
    {
        [TestMethod]
        public async Task SendLogAlertAsyncTests()
        {
            // 发送的数据
            var data = new LogAlertDto
            {
                Tokens = new string[] { "Your Token" },
                Service = "Seq",
                Id = "123",
                Level = "Warning",
                Message = "Hello",
                Datetime = DateTime.Now
            };

            // ServiceUtils.ServiceApi = "https://localhost:7206/api";

            using var client = new HttpClient();
            var response = await ServiceUtils.SendLogAlertAsync(data, client);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [TestMethod]
        public async Task SendEventAlertAsyncTests()
        {
            // 发送的数据
            var data = new EventAlertDto
            {
                Tokens = new string[] { "Your Token" },
                Id = "123",
                Description = "Description",
                Status = "Status",
                Remark = "Remark",
                Datetime = DateTime.Now
            };

            using var client = new HttpClient();
            var response = await ServiceUtils.SendEventAlertAsync(data, client);

            var result = await response.Content.ReadAsStringAsync();

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}