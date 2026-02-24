using AccService.Service.DTO;
using AccService.Service.DTO.RequestObject;
using AccService.Service.DTO.ResponseObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccService.Service
{
    public interface IJwtTokenService
    {
        Task<ApiResponse<AccountDTO>> CreateBuyerAccountAsync(RegisterModel request);
        Task<ApiResponse<AuthenticationToken>> GenerateAuthToken(LoginModel loginModel);
    }
}
