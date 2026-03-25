using VoucherService.Model;
using VoucherService.Repository;
using VoucherService.Service.DTO;
using VoucherService.Service.DTO.Request;
using VoucherService.Service.DTO.Response;

namespace VoucherService.Service.Implementor;

public class VoucherService : IVoucherService
{
    private readonly IVoucherRepository _voucherRepository;

    public VoucherService(IVoucherRepository voucherRepository)
    {
        _voucherRepository = voucherRepository;
    }

    public async Task<ApiResponse<List<VoucherResponse>>> GetAllAsync()
    {
        var items = await _voucherRepository.GetAllAsync();
        var result = items.Select(MapToResponse).ToList();
        return ApiResponse<List<VoucherResponse>>.Ok(result);
    }

    public async Task<ApiResponse<VoucherResponse>> GetByIdAsync(int id)
    {
        var voucher = await _voucherRepository.GetByIdAsync(id);
        if (voucher == null)
            return ApiResponse<VoucherResponse>.Error("Voucher not found", 404);

        return ApiResponse<VoucherResponse>.Ok(MapToResponse(voucher));
    }

    public async Task<ApiResponse<VoucherResponse>> CreateAsync(CreateVoucherRequest request)
    {
        if (request.FromDate > request.ToDate)
            return ApiResponse<VoucherResponse>.Error("from_date must be less than or equal to to_date", 400);

        var existing = await _voucherRepository.GetByCodeAsync(request.VoucherCode.Trim());
        if (existing != null)
            return ApiResponse<VoucherResponse>.Error("Voucher code already exists", 409);

        var voucher = new Voucher
        {
            AccountId = request.AccountId,
            VoucherCode = request.VoucherCode.Trim(),
            VoucherName = request.VoucherName,
            Description = request.Description,
            DiscountPercentage = request.DiscountPercentage,
            DiscountAmount = request.DiscountAmount,
            TypeDiscountTime = request.TypeDiscountTime,
            MaxUsage = request.MaxUsage,
            CurrentUsage = 0,
            ValidFrom = request.ValidFrom,
            FromDate = request.FromDate,
            ToDate = request.ToDate,
            DiscountFor = request.DiscountFor,
            Status = request.Status,
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow
        };

        var created = await _voucherRepository.CreateAsync(voucher);
        return ApiResponse<VoucherResponse>.Ok(MapToResponse(created), "Voucher created successfully", 201);
    }

    public async Task<ApiResponse<VoucherResponse>> UpdateAsync(UpdateVoucherRequest request)
    {
        if (request.FromDate > request.ToDate)
            return ApiResponse<VoucherResponse>.Error("from_date must be less than or equal to to_date", 400);

        var voucher = await _voucherRepository.GetByIdAsync(request.VoucherId);
        if (voucher == null)
            return ApiResponse<VoucherResponse>.Error("Voucher not found", 404);

        var sameCodeVoucher = await _voucherRepository.GetByCodeAsync(request.VoucherCode.Trim());
        if (sameCodeVoucher != null && sameCodeVoucher.VoucherId != request.VoucherId)
            return ApiResponse<VoucherResponse>.Error("Voucher code already exists", 409);

        voucher.AccountId = request.AccountId;
        voucher.VoucherCode = request.VoucherCode.Trim();
        voucher.VoucherName = request.VoucherName;
        voucher.Description = request.Description;
        voucher.DiscountPercentage = request.DiscountPercentage;
        voucher.DiscountAmount = request.DiscountAmount;
        voucher.TypeDiscountTime = request.TypeDiscountTime;
        voucher.MaxUsage = request.MaxUsage;
        voucher.CurrentUsage = request.CurrentUsage;
        voucher.ValidFrom = request.ValidFrom;
        voucher.FromDate = request.FromDate;
        voucher.ToDate = request.ToDate;
        voucher.DiscountFor = request.DiscountFor;
        voucher.Status = request.Status;
        voucher.UpdatedDate = DateTime.UtcNow;

        var updated = await _voucherRepository.UpdateAsync(voucher);
        return ApiResponse<VoucherResponse>.Ok(MapToResponse(updated), "Voucher updated successfully");
    }

    public async Task<ApiResponse<object>> DeleteAsync(int id)
    {
        var voucher = await _voucherRepository.GetByIdAsync(id);
        if (voucher == null)
            return ApiResponse<object>.Error("Voucher not found", 404);

        var deleted = await _voucherRepository.DeleteAsync(voucher);
        if (!deleted)
            return ApiResponse<object>.Error("Delete failed", 500);

        return ApiResponse<object>.Ok(null, "Voucher deleted successfully");
    }

    private static VoucherResponse MapToResponse(Voucher voucher)
    {
        return new VoucherResponse
        {
            VoucherId = voucher.VoucherId,
            AccountId = voucher.AccountId,
            VoucherCode = voucher.VoucherCode,
            VoucherName = voucher.VoucherName,
            Description = voucher.Description,
            DiscountPercentage = voucher.DiscountPercentage,
            DiscountAmount = voucher.DiscountAmount,
            TypeDiscountTime = voucher.TypeDiscountTime,
            MaxUsage = voucher.MaxUsage,
            CurrentUsage = voucher.CurrentUsage,
            ValidFrom = voucher.ValidFrom,
            FromDate = voucher.FromDate,
            ToDate = voucher.ToDate,
            DiscountFor = voucher.DiscountFor,
            Status = voucher.Status,
            CreatedDate = voucher.CreatedDate,
            UpdatedDate = voucher.UpdatedDate
        };
    }
}
