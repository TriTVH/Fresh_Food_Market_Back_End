using VoucherService.Model;
using VoucherService.Repository;
using VoucherService.Service.DTO;
using VoucherService.Service.DTO.Request;
using VoucherService.Service.DTO.Response;

namespace VoucherService.Service.Implementor;

public class VoucherDetailService : IVoucherDetailService
{
    private readonly IVoucherDetailRepository _voucherDetailRepository;
    private readonly IVoucherRepository _voucherRepository;

    public VoucherDetailService(
        IVoucherDetailRepository voucherDetailRepository,
        IVoucherRepository voucherRepository)
    {
        _voucherDetailRepository = voucherDetailRepository;
        _voucherRepository = voucherRepository;
    }

    public async Task<ApiResponse<List<VoucherDetailResponse>>> GetAllAsync()
    {
        var items = await _voucherDetailRepository.GetAllAsync();
        return ApiResponse<List<VoucherDetailResponse>>.Ok(items.Select(Map).ToList());
    }

    public async Task<ApiResponse<List<VoucherDetailResponse>>> GetByVoucherIdAsync(int voucherId)
    {
        var items = await _voucherDetailRepository.GetByVoucherIdAsync(voucherId);
        return ApiResponse<List<VoucherDetailResponse>>.Ok(items.Select(Map).ToList());
    }

    public async Task<ApiResponse<VoucherDetailResponse>> GetByIdAsync(int id)
    {
        var item = await _voucherDetailRepository.GetByIdAsync(id);
        if (item == null)
            return ApiResponse<VoucherDetailResponse>.Error("Voucher detail not found", 404);

        return ApiResponse<VoucherDetailResponse>.Ok(Map(item));
    }

    public async Task<ApiResponse<VoucherDetailResponse>> CreateAsync(CreateVoucherDetailRequest request)
    {
        var voucher = await _voucherRepository.GetByIdAsync(request.VoucherId);
        if (voucher == null)
            return ApiResponse<VoucherDetailResponse>.Error("Voucher not found", 404);

        if (voucher.MaxUsage.HasValue && voucher.CurrentUsage.GetValueOrDefault() >= voucher.MaxUsage.Value)
            return ApiResponse<VoucherDetailResponse>.Error("Voucher has reached max usage", 400);

        var detail = new VoucherDetail
        {
            OrderId = request.OrderId,
            VoucherId = request.VoucherId,
            AppliedDate = DateTime.UtcNow
        };

        var created = await _voucherDetailRepository.CreateAsync(detail);

        voucher.CurrentUsage = voucher.CurrentUsage.GetValueOrDefault() + 1;
        voucher.UpdatedDate = DateTime.UtcNow;
        await _voucherRepository.UpdateAsync(voucher);

        return ApiResponse<VoucherDetailResponse>.Ok(Map(created), "Voucher detail created successfully", 201);
    }

    private static VoucherDetailResponse Map(VoucherDetail x)
    {
        return new VoucherDetailResponse
        {
            VoucherDetailId = x.VoucherDetailId,
            OrderId = x.OrderId,
            VoucherId = x.VoucherId,
            VoucherCode = x.Voucher?.VoucherCode ?? string.Empty,
            AppliedDate = x.AppliedDate
        };
    }
}