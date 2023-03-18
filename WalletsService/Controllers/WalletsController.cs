using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Nodes;
using System.Text;
using System.Text.Json;
using WalletsService.Controllers.Nested;
using WalletsService.MiddleLayer;
using WalletsService.MiddleLayer.Nested;

namespace WalletsService.Controllers
{
    [ApiController, Route("[controller]")]
    public class WalletsController : ControllerBase
    {
        private readonly IWorker _worker;

        public WalletsController(IWorker worker)
        {
            _worker = worker;
        }

        private Guid GetUserGuid()
        {
            var headers = Request.Headers;
            var userId = headers["X-UserId"];
            if (userId.Count <= 0) return Guid.Empty;
            return Guid.TryParse(userId[0], out var userGuid) ? userGuid : Guid.Empty;
        }

        private string GetDigest()
        {
            var headers = Request.Headers;
            var digest = headers["X-Digest"];
            return digest.Count <= 0 ? string.Empty : digest[0] ?? string.Empty;
        }

        private const string UserIdError = "X-UserId error";
        private const string DigestError = "X-Digest error";
        private const string JsonError = "Json error";
        private const string WalletNotFound = "Wallet not found";
                               
        /// <summary>Проверить существует ли аккаунт электронного кошелька</summary>
        /// <returns></returns>
        [HttpPost("CheckWallet")]
        public async Task<ActionResult<CheckAccountResult>> PostCheck()
        {
            var userGuid = GetUserGuid();

            if (userGuid == Guid.Empty) return BadRequest(UserIdError);

            var result = new CheckAccountResult { isexist = _worker.CheckAccount(userGuid) };
            return await new ValueTask<ActionResult<CheckAccountResult>>(result);
        }

        /// <summary>Получить баланс электронного кошелька</summary>
        /// <returns></returns>
        [HttpPost("GetBalance")]
        public async Task<ActionResult<BalanceResult>> PostGetBalance()
        {
            var userGuid = GetUserGuid();

            if (userGuid == Guid.Empty) return BadRequest(UserIdError);

            var result = _worker.GetBalance(userGuid);

            if (result == null) return BadRequest(WalletNotFound);

            return await new ValueTask<ActionResult<BalanceResult>>(result);
        }

        private bool CompareHash(byte[] rawData, string testHash)
        {
            using var sha1 = new SHA1Managed();
            var hash = sha1.ComputeHash(rawData);
            var sb = new StringBuilder(hash.Length * 2);

            foreach (var b in hash)
            {
                sb.Append(b.ToString("X2"));
            }

            var h1 = sb.ToString();
            return testHash == h1;
        }

        private async Task<byte[]?> GetMessage()
        {
            byte[]? rawData;

            using (var ms = new MemoryStream())
            {
                await Request.Body.CopyToAsync(ms);
                ms.Position = 0;
                rawData = ms.ToArray();
            }

            var digest = GetDigest();
            var compareHash = CompareHash(rawData, digest);

            return !compareHash ? null : rawData;
        }

        /// <summary>Получить общее количество и суммы операций пополнения за выбранный месяц</summary>
        /// <returns></returns>
        [HttpPost("GetMonth")]
        public async Task<ActionResult<MonthResult>> PostGetMonth()
        {
            var userGuid = GetUserGuid();

            if (userGuid == Guid.Empty) return BadRequest(UserIdError);

            var rawData = await GetMessage();

            if (rawData == null) return BadRequest(DigestError);

            var mes = JsonNode.Parse(rawData).Deserialize<MonthMessage>();

            if (mes == null) return BadRequest(JsonError);

            var result = _worker.GetMonthResult(userGuid, mes.month);

            if (result == null) return BadRequest(WalletNotFound);

            return await new ValueTask<ActionResult<MonthResult>>(result);
        }

        /// <summary>Получить общее количество и суммы операций пополнения за текущий месяц</summary>
        /// <returns></returns>
        [HttpPost("GetCurrentMonth")]
        public async Task<ActionResult<MonthResult>> PostGetCurrentMonth()
        {
            var userGuid = GetUserGuid();

            if (userGuid == Guid.Empty) return BadRequest(UserIdError);

            var result = _worker.GetMonthResult(userGuid, DateTime.Now.Month);

            if (result == null) return BadRequest(WalletNotFound);

            return await new ValueTask<ActionResult<MonthResult>>(result);
        }

        /// <summary>Пополнение электронного кошелька</summary>
        /// <returns></returns>
        [HttpPost("ReplenishWallet")]
        public async Task<ActionResult> PostReplenishWallet()
        {
            var userGuid = GetUserGuid();

            if (userGuid == Guid.Empty) return BadRequest(UserIdError);

            var rawData = await GetMessage();

            if (rawData == null) return BadRequest(DigestError);

            var mes = JsonNode.Parse(rawData).Deserialize<ReplenishWalletMessage>();

            if (mes == null) return BadRequest(JsonError);

            try
            {
                _worker.AddOperation(userGuid, mes.value);
            }
            catch (ArgumentException)
            {
                return BadRequest("Limit is exceeded");
            }

            Response.StatusCode = 200;
            return Content("Complete");
        }

        [HttpGet]
        public string Get()
        {
            return "test";
        }
    }
}