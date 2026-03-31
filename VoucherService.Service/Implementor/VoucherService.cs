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
        var validationError = ValidateCreateVoucher(request);
        if (validationError != null)
            return ApiResponse<VoucherResponse>.Error(validationError, 400);

        var existing = await _voucherRepository.GetByCodeAsync(request.VoucherCode.Trim());
        if (existing != null)
            return ApiResponse<VoucherResponse>.Error("Voucher code already exists", 409);

        var voucher = new Voucher
        {
            VoucherCode = request.VoucherCode.Trim(),
            VoucherName = request.VoucherName?.Trim(),
            Description = request.Description,
            DiscountMax = request.DiscountMax,
            DiscountPercentage = request.DiscountPercentage,
            TypeDiscountTime = request.TypeDiscountTime?.Trim().ToUpperInvariant(),
            MaxUsage = request.MaxUsage,
            CurrentUsage = 0,
            ValidFrom = request.ValidFrom,
            FromDate = request.FromDate,
            ToDate = request.ToDate,
            Status = "PENDING",
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow
        };

        var created = await _voucherRepository.CreateAsync(voucher);
        return ApiResponse<VoucherResponse>.Ok(MapToResponse(created), "Voucher created successfully", 201);
    }
    private string? ValidateCreateVoucher(CreateVoucherRequest request)
    {
        if (request == null)
            return "Request is required";

        if (string.IsNullOrWhiteSpace(request.VoucherCode))
            return "Voucher code is required";

        if (request.VoucherCode.Trim().Length > 50)
            return "Voucher code must not exceed 50 characters";

        if (!string.IsNullOrWhiteSpace(request.VoucherName) && request.VoucherName.Length > 200)
            return "Voucher name must not exceed 200 characters";

        if (request.DiscountMax.HasValue && request.DiscountMax.Value < 0)
            return "discount_max must be greater than or equal to 0";

        if (request.DiscountPercentage.HasValue && request.DiscountPercentage.Value < 0)
            return "discount_percentage must be greater than or equal to 0";

        if (request.DiscountPercentage.HasValue && request.DiscountPercentage.Value > 100)
            return "discount_percentage must be less than or equal to 100";

        if (request.ValidFrom.HasValue && request.ValidFrom.Value < 0)
            return "valid_from must be greater than or equal to 0";

        if (request.MaxUsage.HasValue && request.MaxUsage.Value < 0)
            return "max_usage must be greater than or equal to 0";

        if (request.FromDate.HasValue && request.ToDate.HasValue
            && request.FromDate.Value > request.ToDate.Value)
        {
            return "from_date must be less than or equal to to_date";
        }

        if (!string.IsNullOrWhiteSpace(request.TypeDiscountTime))
        {
            var type = request.TypeDiscountTime.Trim().ToUpperInvariant();

            if (type != "FIXED" && type != "NO_FIXED")
                return "type_discount_time must be FIXED or NO_FIXED";
        }

        // Nếu có discount_percentage, valid_from, discount_max
        // thì discount tại ngưỡng tối thiểu phải nhỏ hơn discount_max
        if (request.DiscountPercentage.HasValue &&
            request.ValidFrom.HasValue &&
            request.DiscountMax.HasValue)
        {
            var discountMin = request.DiscountPercentage.Value * request.ValidFrom.Value / 100m;

            if (discountMin >= request.DiscountMax.Value)
                return "discount_percentage * valid_from / 100 must be less than discount_max";
        }

        return null;
    }

    public async Task<ApiResponse<VoucherResponse>> UpdateAsync(UpdateVoucherRequest request)
    {
        var validationError = ValidateUpdateVoucher(request);
        if (validationError != null)
            return ApiResponse<VoucherResponse>.Error(validationError, 400);

        var voucher = await _voucherRepository.GetByIdAsync(request.VoucherId);
        if (voucher == null)
            return ApiResponse<VoucherResponse>.Error("Voucher not found", 404);

        var sameCodeVoucher = await _voucherRepository.GetByCodeAsync(request.VoucherCode.Trim());
        if (sameCodeVoucher != null && sameCodeVoucher.VoucherId != request.VoucherId)
            return ApiResponse<VoucherResponse>.Error("Voucher code already exists", 409);

        voucher.VoucherCode = request.VoucherCode.Trim();
        voucher.VoucherName = request.VoucherName;
        voucher.Description = request.Description;
        voucher.DiscountPercentage = request.DiscountPercentage;
        voucher.TypeDiscountTime = request.TypeDiscountTime;
        voucher.MaxUsage = request.MaxUsage;
        voucher.CurrentUsage = request.CurrentUsage;
        voucher.ValidFrom = request.ValidFrom;
        voucher.FromDate = request.FromDate;
        voucher.ToDate = request.ToDate;
        voucher.Status = request.Status;
        voucher.UpdatedDate = DateTime.UtcNow;

        var updated = await _voucherRepository.UpdateAsync(voucher);
        return ApiResponse<VoucherResponse>.Ok(MapToResponse(updated), "Voucher updated successfully");
    }
    private string? ValidateUpdateVoucher(UpdateVoucherRequest request)
    {
        if (request == null)
            return "Request is required";

        if (string.IsNullOrWhiteSpace(request.VoucherCode))
            return "Voucher code is required";

        if (request.VoucherCode.Trim().Length > 50)
            return "Voucher code must not exceed 50 characters";

        if (!string.IsNullOrWhiteSpace(request.VoucherName) && request.VoucherName.Length > 200)
            return "Voucher name must not exceed 200 characters";

        if (request.DiscountMax.HasValue && request.DiscountMax.Value < 0)
            return "discount_max must be greater than or equal to 0";

        if (request.DiscountPercentage.HasValue && request.DiscountPercentage.Value < 0)
            return "discount_percentage must be greater than or equal to 0";
        if (request.DiscountPercentage.HasValue && request.DiscountPercentage.Value > 100)
            return "discount_percentage must be less than or equal to 100";
        if (request.ValidFrom.HasValue && request.ValidFrom.Value < 0)
            return "valid_from must be greater than or equal to 0";

        if (request.MaxUsage.HasValue && request.MaxUsage.Value < 0)
            return "max_usage must be greater than or equal to 0";

        if (request.FromDate.HasValue && request.ToDate.HasValue
            && request.FromDate.Value > request.ToDate.Value)
        {
            return "from_date must be less than or equal to to_date";
        }

        if (!string.IsNullOrWhiteSpace(request.TypeDiscountTime))
        {
            var type = request.TypeDiscountTime.Trim().ToUpperInvariant();

            if (type != "FIXED" && type != "NO_FIXED")
                return "type_discount_time must be FIXED or NO_FIXED";
            
            if (type == "FIXED")
            {
                if (!request.FromDate.HasValue || !request.ToDate.HasValue)
                    return "FIXED type requires from_date and to_date";
            }
        }

        // Nếu có discount_percentage, valid_from, discount_max
        // thì discount tại ngưỡng tối thiểu phải nhỏ hơn discount_max
        if (request.DiscountPercentage.HasValue &&
            request.ValidFrom.HasValue &&
            request.DiscountMax.HasValue)
        {
            var discountMin = request.DiscountPercentage.Value * request.ValidFrom.Value / 100m;

            if (discountMin >= request.DiscountMax.Value)
                return "discount_percentage * valid_from / 100 must be less than discount_max";
        }

        return null;
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

    private VoucherResponse MapToResponse(Voucher voucher)
    {
        return new VoucherResponse
        {
            VoucherId = voucher.VoucherId,
            VoucherCode = voucher.VoucherCode,
            VoucherName = voucher.VoucherName,
            Description = voucher.Description,
            DiscountPercentage = voucher.DiscountPercentage,
            TypeDiscountTime = voucher.TypeDiscountTime,
            MaxUsage = voucher.MaxUsage,
            CurrentUsage = voucher.CurrentUsage,
            ValidFrom = voucher.ValidFrom,
            FromDate = voucher.FromDate,
            ToDate = voucher.ToDate,
            Status = voucher.Status,
            CreatedDate = voucher.CreatedDate,
            UpdatedDate = voucher.UpdatedDate
        };
    }
}
