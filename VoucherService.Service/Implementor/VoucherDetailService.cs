using System.Runtime.ConstrainedExecution;
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

    public async Task<ApiResponse<List<VoucherDetailResponse>>> CreateAsync(CreateVoucherDetailRequest request)
    {
        List<int> voucherIds = request.VoucherIds.Distinct().ToList();

        List<VoucherDetailResponse> result = new List<VoucherDetailResponse>();

        var now = DateTime.UtcNow;

        decimal totalDiscountAmount = 0;

        foreach (var voucherId in voucherIds)
        {
            var voucher = await _voucherRepository.GetByIdAsync(voucherId);
            if(voucher == null)
            {
                return ApiResponse<List<VoucherDetailResponse>>.Error($"Voucher {voucherId} does not exist", 400, null);
            }
            var error = ValidateSingleVoucher(voucher, request.totalAmountOrder, now);
            if (error != null)
            {
                return ApiResponse<List<VoucherDetailResponse>>.Error(error, 400);
            }
            var discountAmount = CalculateDiscountAmount(voucher, request.totalAmountOrder);
            var remainingAmount = request.totalAmountOrder - totalDiscountAmount;

            if (remainingAmount < 0)
            {
                return ApiResponse<List<VoucherDetailResponse>>.Error(
                    "Total discount amount has exceeded the total order amount",
                    400
                );
            }

            // không cho voucher hiện tại làm tổng discount vượt quá totalAmountOrder
            if (discountAmount > remainingAmount)
            {
                discountAmount = remainingAmount;
            }

            var voucherDetail = new VoucherDetail
            {
                OrderId = request.OrderId,
                VoucherId = voucher.VoucherId,
                DiscountAmount = discountAmount,
                AppliedDate = DateTime.UtcNow
            };

            var created = await _voucherDetailRepository.CreateAsync(voucherDetail);

            var response = new VoucherDetailResponse
            {
                VoucherDetailId = created.VoucherDetailId,
                OrderId = created.OrderId,
                VoucherCode = created.Voucher.VoucherCode,
                DiscountAmount = created.DiscountAmount,
                VoucherId = created.VoucherId,
                AppliedDate = created.AppliedDate
            };

            result.Add(response);
            totalDiscountAmount += discountAmount;

            voucher.CurrentUsage += 1;
            await _voucherRepository.UpdateAsync(voucher);
        }

     

        return ApiResponse<List<VoucherDetailResponse>>.Ok(result, "Apply voucher ids successfully", 201);
    }

    private string? ValidateSingleVoucher(Voucher voucher, decimal totalAmountOrder, DateTime now)
    {
        if (!string.Equals(voucher.Status, "ACTIVE", StringComparison.OrdinalIgnoreCase))
            return $"Voucher {voucher.VoucherCode} is not active";
        if (voucher.MaxUsage.HasValue && voucher.CurrentUsage.HasValue &&
            voucher.CurrentUsage.Value >= voucher.MaxUsage.Value)
        {
            return $"Voucher {voucher.VoucherCode} has reached max usage";
        }
        var type = voucher.TypeDiscountTime.Trim().ToUpperInvariant();
        if (type == "FIXED")
        {
            if (now > voucher.ToDate.Value)
                return $"Voucher {voucher.VoucherCode} is expired";
        }
        if (voucher.ValidFrom.HasValue && totalAmountOrder < voucher.ValidFrom.Value)
            return $"Voucher {voucher.VoucherCode} requires minimum order value of {voucher.ValidFrom.Value}";

        return null;
    }
    private decimal CalculateDiscountAmount(Voucher voucher, decimal totalAmountOrder)
    {
        if (!voucher.DiscountPercentage.HasValue || voucher.DiscountPercentage.Value <= 0)
            return 0;

        decimal discount = totalAmountOrder * voucher.DiscountPercentage.Value / 100m;

        if (voucher.DiscountMax.HasValue && voucher.DiscountMax.Value > 0 && discount > voucher.DiscountMax.Value)
        {
            discount = voucher.DiscountMax.Value;
        }

        return Math.Round(discount, 2);
    }
    private VoucherDetailResponse Map(VoucherDetail x)
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