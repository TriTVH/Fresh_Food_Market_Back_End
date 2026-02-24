using AccService.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AccService.API.Controllers
{
    [Route("api/account")]
    public class AccountController : ControllerBase
    {
        public IAccountService _accountService;
        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }
        [HttpGet]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> GetAccounts()
        {
            var response = await _accountService.GetAccounts();
            if (response.Success)
            {
                return Ok(response);
            }
            else            {
                return StatusCode(response.StatusCode, response);
            }
        }
    }
}
