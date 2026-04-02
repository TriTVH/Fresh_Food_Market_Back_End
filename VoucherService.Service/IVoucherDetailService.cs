using VoucherService.Service.DTO;
using VoucherService.Service.DTO.Request;
using VoucherService.Service.DTO.Response;

namespace VoucherService.Service;

public interface IVoucherDetailService
{
    Task<ApiResponse<List<VoucherDetailResponse>>> GetAllAsync();
    Task<ApiResponse<List<VoucherDetailResponse>>> GetByVoucherIdAsync(int voucherId);
    Task<ApiResponse<VoucherDetailResponse>> GetByIdAsync(int id);
    Task<ApiResponse<bool>> UnAppliedVouchers(int orderId);
    Task<ApiResponse<List<VoucherDetailResponse>>> CreateAsync(CreateVoucherDetailRequest request);
}