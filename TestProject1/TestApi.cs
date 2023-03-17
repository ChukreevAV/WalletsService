using Microsoft.AspNetCore.Mvc.Testing;

using System.Text.Json;
using System.Text.Json.Nodes;

using WalletsService.Controllers.Nested;

namespace TestProject1
{
    [TestClass] public class TestApi
    {
        private HttpClient _httpClient = null!;

        private readonly string _userGuid1 = "50e28c3f-4267-4c5d-9b70-e19970a0086d";

        [TestInitialize]
        public void Setup()
        {
            var application = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services => {});
                });

            _httpClient = application.CreateClient();
        }

        [TestMethod] public async Task TestGetMethod()
        {
            var url1 = "Wallets";
            var response = await _httpClient.GetAsync(url1);
            var httpContent = response.Content;
            var asyncContent = httpContent.ReadAsStringAsync().Result;
            Assert.IsTrue(asyncContent == "test");
        }

        [TestMethod] public async Task TestMethod3()
        {
            var url1 = "Wallets/CheckWallet";
            _httpClient.DefaultRequestHeaders.Add("X-UserId", _userGuid1);
            var response = await _httpClient.PostAsync(url1, null);
            var httpContent = response.Content;
            var asyncContent = httpContent.ReadAsStringAsync().Result;
            Assert.IsNotNull(asyncContent);
            var checkAccountResult = JsonNode.Parse(asyncContent).Deserialize<CheckAccountResult>();
            Assert.IsTrue(checkAccountResult.isexist);
        }
    }
}