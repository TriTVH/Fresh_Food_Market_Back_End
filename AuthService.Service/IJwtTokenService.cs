using AuthService.Service.DTO;
using AuthService.Service.DTO.RequestObject;
using AuthService.Service.DTO.ResponseObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Service
{
    public interface IJwtTokenService
    {
        Task<ApiResponse<AuthenticationToken>> GenerateAuthToken(LoginModel loginModel);
    }
}
