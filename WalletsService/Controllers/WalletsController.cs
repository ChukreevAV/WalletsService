using Microsoft.AspNetCore.Mvc;
using WalletsService.Controllers.Nested;
using WalletsService.MiddleLayer;

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

        [HttpPost("CheckWallet")]
        public async Task<ActionResult<CheckAccountResult>> PostCheck()
        {
            var userGuid = GetUserGuid();

            if (userGuid == Guid.Empty)
            {
                Response.StatusCode = 400;
                return Content("X-UserId error");
            }

            var result = new CheckAccountResult { isexist = _worker.CheckAccount(userGuid) };
            return await new ValueTask<ActionResult<CheckAccountResult>>(result);
        }

        [HttpGet] public string Get()
        {
            return "test";
        }
    }
}