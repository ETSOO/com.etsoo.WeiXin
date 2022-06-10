using com.etsoo.Utils;
using com.etsoo.WeiXinService;
using System.Net;
using System.Web;

namespace TestProject1
{
    [TestClass]
    public class WXServiceUtilsTests
    {
        [TestMethod]
        public async Task SendAsyncTests()
        {
            // ���͵�����
            var data = new LogAlertDto
            {
                Tokens = new string[] { "Your Token" },
                Service = "Seq",
                Id = "123",
                Level = "Warning",
                Message = "Hello, ����",
                Datetime = SharedUtils.TruncateDateTime(DateTime.Now)
            };

            // ��ϣ
            var (json, signature) = await ServiceUtils.SerializeAsync(data);
            var signUrl = HttpUtility.UrlEncode(signature);

            using var content = new StreamContent(json);
            content.Headers.Add("Content-Type", "application/json");

            using var client = new HttpClient();
            var response = await client.PostAsync("https://wechatapi.etsoo.com/api/Service/LogAlert/" + signUrl, content);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}