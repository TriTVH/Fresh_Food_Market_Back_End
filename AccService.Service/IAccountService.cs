using AccService.Service.DTO;
using AccService.Service.DTO.RequestObject;
using AccService.Service.DTO.ResponseObject;

namespace AccService.Service
{
    public interface IAccountService
    {
        Task<ApiResponse<List<AccountDTO>>> GetAccounts();
        Task<ApiResponse<object>> CreateSupplierAccountAsync(CreateSupplierAccountRequest request);
    }
}
