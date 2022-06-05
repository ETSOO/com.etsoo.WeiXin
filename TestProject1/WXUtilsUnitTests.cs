using com.etsoo.WeiXin;

namespace TestProject1
{
    [TestClass]
    public class WXUtilsUnitTests
    {
        [TestMethod]
        public async Task CreateSignatureAsyncDicTests()
        {
            // Arrange
            var dic = new SortedDictionary<string, string>
            {
                ["noncestr"]="Wm3WZYTPz0wzccnW",
                ["jsapi_ticket"]="sM4AOVdWfPE4DxkXGEs8VMCPGGVi4C3VM0P37wVUCFvkVAy_90u5h9nbSlYy3-Sl-HhTdfl2fzFy1AOcHKP7qg",
                ["timestamp"]="1414587457",
                ["url"]="http://mp.weixin.qq.com?params=value"
            };

            // Act
            var signature = await WXUtils.CreateSignatureAsync(dic);

            // Assert
            Assert.AreEqual("0f9de62fce790f9a083d5c99e95740ceb90c27ed", signature);
        }

        [TestMethod]
        public async Task CreateSignatureAsyncDataTests()
        {
            // Arrange
            var data = new SortedSet<string>
            {
                "1434008071",
                "1404896688",
                "pjZ8Yt1XGILfi-FUsewpnnolGgZk",
                "ojZ8YtyVyr30HheH3CM73y7h4jJE",
                "123"
            };

            // Act
            var signature = await WXUtils.CreateSignatureAsync(data);

            // Assert
            Assert.AreEqual("f137ab68b7f8112d20ee528ab6074564e2796250", signature);
        }
    }
}