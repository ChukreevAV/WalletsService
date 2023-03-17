using System.Net;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc.Testing;

using Newtonsoft.Json;

using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

using WalletsService.Controllers.Nested;
using WalletsService.MiddleLayer.Nested;

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

        [TestMethod] public async Task TestCheckWallet()
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

        private BalanceResult? GetBalanceResult(HttpResponseMessage? mes)
        {
            var httpContent = mes?.Content;
            var asyncContent = httpContent?.ReadAsStringAsync().Result;
            return JsonNode.Parse(asyncContent).Deserialize<BalanceResult>();
        }

        [TestMethod] public async Task TestGetBalance()
        {
            var url1 = "Wallets/GetBalance";
            _httpClient.DefaultRequestHeaders.Add("X-UserId", _userGuid1);
            var response1 = await _httpClient.PostAsync(url1, null);
            var b1 = GetBalanceResult(response1);
            Assert.IsTrue(b1.balance == 2000);
        }

        private string GetHashString(Stream stream)
        {
            using (var sha1 = new SHA1Managed())
            {
                var hash = sha1.ComputeHash(stream);
                var sb = new StringBuilder(hash.Length * 2);

                foreach (var b in hash)
                {
                    // can be "x2" if you want lowercase
                    sb.Append(b.ToString("X2"));
                }

                return sb.ToString();
            }
        }

        private MonthResult? GetMonthResult(HttpResponseMessage? mes)
        {
            var httpContent = mes?.Content;
            var asyncContent = httpContent?.ReadAsStringAsync().Result;
            return JsonNode.Parse(asyncContent).Deserialize<MonthResult>();
        }

        [TestMethod] public async Task TestGetMonth()
        {
            var url1 = "Wallets/GetMonth";
            var mes = new MonthMessage { month = DateTime.Now.Month };

            var json = JsonConvert.SerializeObject(mes);
            var data = new StringContent(json, Encoding.UTF8);

            _httpClient.DefaultRequestHeaders.Add("X-UserId", _userGuid1);
            _httpClient.DefaultRequestHeaders
                .Add("X-Digest", GetHashString(data.ReadAsStream()));
            var response1 = await _httpClient.PostAsync(url1, data);
            Assert.IsTrue(response1.StatusCode == HttpStatusCode.OK);
            var m1 = GetMonthResult(response1);
            Assert.IsNotNull(m1);
        }

        [TestMethod] public async Task TestGetCurrentMonth()
        {
            var url1 = "Wallets/GetCurrentMonth";
            _httpClient.DefaultRequestHeaders.Add("X-UserId", _userGuid1);
            var response1 = await _httpClient.PostAsync(url1, null);
            Assert.IsTrue(response1.StatusCode == HttpStatusCode.OK);
            var m1 = GetMonthResult(response1);
            Assert.IsNotNull(m1);
        }

        [TestMethod] public async Task TestReplenishWallet()
        {
            var url1 = "Wallets/ReplenishWallet";
            _httpClient.DefaultRequestHeaders.Add("X-UserId", _userGuid1);
            var mes = new ReplenishWalletMessage { value = 11.22 };

            var json = JsonConvert.SerializeObject(mes);
            var data = new StringContent(json, Encoding.UTF8);

            _httpClient.DefaultRequestHeaders
                .Add("X-Digest", GetHashString(data.ReadAsStream()));
            var response1 = await _httpClient.PostAsync(url1, data);

            var url2 = "Wallets/GetBalance";
            var response2 = await _httpClient.PostAsync(url2, null);
            var b1 = GetBalanceResult(response2);
            Assert.IsTrue(b1.balance > 2000);
        }

        [TestMethod] public async Task TestReplenishWalletLimit()
        {
            var url1 = "Wallets/ReplenishWallet";
            _httpClient.DefaultRequestHeaders.Add("X-UserId", _userGuid1);
            var mes = new ReplenishWalletMessage { value = 100000 };

            var json = JsonConvert.SerializeObject(mes);
            var data = new StringContent(json, Encoding.UTF8);

            _httpClient.DefaultRequestHeaders
                .Add("X-Digest", GetHashString(data.ReadAsStream()));
            var response1 = await _httpClient.PostAsync(url1, data);
            Assert.IsTrue(response1.StatusCode == HttpStatusCode.BadRequest);

            var url2 = "Wallets/GetBalance";
            var response2 = await _httpClient.PostAsync(url2, null);
            var b1 = GetBalanceResult(response2);
            Assert.IsTrue(b1.balance == 2000);
        }
    }
}