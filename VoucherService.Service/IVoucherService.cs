using VoucherService.Service.DTO;
using VoucherService.Service.DTO.Request;
using VoucherService.Service.DTO.Response;

namespace VoucherService.Service;

public interface IVoucherService
{
    Task<ApiResponse<List<VoucherResponse>>> GetAllAsync();
    Task<ApiResponse<VoucherResponse>> GetByIdAsync(int id);
    Task<ApiResponse<VoucherResponse>> CreateAsync(CreateVoucherRequest request);
    Task<ApiResponse<VoucherResponse>> UpdateAsync(UpdateVoucherRequest request);
    Task<ApiResponse<object>> DeleteAsync(int id);
}
