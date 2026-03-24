using VoucherService.Service.DTO;
using VoucherService.Service.DTO.Request;
using VoucherService.Service.DTO.Response;

namespace VoucherService.Service;

public interface IDiscountProgramService
{
    Task<ApiResponse<List<DiscountProgramResponse>>> GetAllAsync();
    Task<ApiResponse<DiscountProgramResponse>> GetByIdAsync(int id);
    Task<ApiResponse<DiscountProgramResponse>> CreateAsync(CreateDiscountProgramRequest request);
    Task<ApiResponse<DiscountProgramResponse>> UpdateAsync(UpdateDiscountProgramRequest request);
    Task<ApiResponse<object>> DeleteAsync(int id);
}