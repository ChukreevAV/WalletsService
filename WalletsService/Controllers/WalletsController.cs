using Microsoft.AspNetCore.Mvc;

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

        /// <summary>Проверить существует ли аккаунт электронного кошелька</summary>
        /// <returns></returns>
        [HttpPost("CheckWallet")]
        public async Task<ActionResult<CheckAccountResult>> PostCheck()
        {
            var mes = InputMessage.ReadUserId(this);
            if (mes.Error != InputMessageError.None) return BadRequest(mes.Error.GetDescription());

            var result = new CheckAccountResult { isexist = _worker.CheckAccount(mes.UserId) };
            return await new ValueTask<ActionResult<CheckAccountResult>>(result);
        }

        /// <summary>Получить баланс электронного кошелька</summary>
        /// <returns></returns>
        [HttpPost("GetBalance")]
        public async Task<ActionResult<BalanceResult>> PostGetBalance()
        {
            var mes = InputMessage.ReadUserId(this);
            if (mes.Error != InputMessageError.None) return BadRequest(mes.Error.GetDescription());

            var result = _worker.GetBalance(mes.UserId);
            if (result == null) return BadRequest(InputMessageError.WalletNotFound.GetDescription());

            return await new ValueTask<ActionResult<BalanceResult>>(result);
        }

        /// <summary>Получить общее количество и суммы операций пополнения за выбранный месяц</summary>
        /// <returns></returns>
        [HttpPost("GetMonth")]
        public async Task<ActionResult<MonthResult>> PostGetMonth()
        {
            var message = InputMessage.ReadMessaged(this); 
            if (message.Error != InputMessageError.None) return BadRequest(message.Error.GetDescription());

            var data = message.ParseJson<MonthMessage>();

            if (data == null) return BadRequest(InputMessageError.Json.GetDescription());

            var result = _worker.GetMonthResult(message.UserId, data.month);

            if (result == null) return BadRequest(InputMessageError.WalletNotFound.GetDescription());

            return await new ValueTask<ActionResult<MonthResult>>(result);
        }

        /// <summary>Получить общее количество и суммы операций пополнения за текущий месяц</summary>
        /// <returns></returns>
        [HttpPost("GetCurrentMonth")]
        public async Task<ActionResult<MonthResult>> PostGetCurrentMonth()
        {
            var mes = InputMessage.ReadUserId(this);
            if (mes.Error != InputMessageError.None) return BadRequest(mes.Error.GetDescription());

            var result = _worker.GetMonthResult(mes.UserId, DateTime.Now.Month);

            if (result == null) return BadRequest(InputMessageError.WalletNotFound.GetDescription());

            return await new ValueTask<ActionResult<MonthResult>>(result);
        }

        /// <summary>Пополнение электронного кошелька</summary>
        /// <returns></returns>
        [HttpPost("ReplenishWallet")]
        public Task<ActionResult> PostReplenishWallet()
        {
            var message = InputMessage.ReadMessaged(this);
            if (message.Error != InputMessageError.None) return Task.FromResult<ActionResult>(BadRequest(message.Error.GetDescription()));

            var mes = message.ParseJson<ReplenishWalletMessage>();

            if (mes == null) return Task.FromResult<ActionResult>(BadRequest(InputMessageError.Json.GetDescription()));

            try
            {
                _worker.AddOperation(message.UserId, mes.value);
            }
            catch (ArgumentException)
            {
                return Task.FromResult<ActionResult>(BadRequest("Limit is exceeded"));
            }

            Response.StatusCode = 200;
            return Task.FromResult<ActionResult>(Content("Complete"));
        }

        [HttpGet]
        public string Get()
        {
            return "test";
        }
    }
}