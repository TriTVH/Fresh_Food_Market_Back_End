using AccService.Service;
using AccService.Service.DTO.RequestObject;
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
        [HttpPost("supplierAcc")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> CreateSupplierAccount([FromBody] CreateSupplierAccountRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(new { Success = false, StatusCode = 400, Message = "Validation failed", Errors = errors });
            }

            var response = await _accountService.CreateSupplierAccountAsync(request);

            if (response.Success)
            {
                return StatusCode(201, response); 
            }
            else
            {
                return StatusCode(response.StatusCode, response); 
            }
        }
    }
}
