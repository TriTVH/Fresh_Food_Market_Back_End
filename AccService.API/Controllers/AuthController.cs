using AccService.Service;
using AccService.Service.DTO.RequestObject;
using AccService.Service.Implementor;
using Microsoft.AspNetCore.Mvc;

namespace AccService.API.Controllers
{
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
       public IJwtTokenService _jwtTokenService;
       public AuthController(IJwtTokenService jwtTokenService)
        {
            _jwtTokenService = jwtTokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel registerModel)
        {
            var response = await _jwtTokenService.CreateBuyerAccountAsync(registerModel);
            
            if (response.Success)
            {
                return Ok(response);
            }
            else
            {
                return StatusCode(response.StatusCode, response);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            var response = await _jwtTokenService.GenerateAuthToken(loginModel);
            if (response.Success)
            {
                return Ok(response);
            }
            else
            {
                return StatusCode(response.StatusCode, response);
            }
        }
    }
}
