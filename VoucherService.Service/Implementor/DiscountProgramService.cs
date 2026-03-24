using VoucherService.Model;
using VoucherService.Repository;
using VoucherService.Service.DTO;
using VoucherService.Service.DTO.Request;
using VoucherService.Service.DTO.Response;

namespace VoucherService.Service.Implementor;

public class DiscountProgramService : IDiscountProgramService
{
    private readonly IDiscountProgramRepository _repo;

    public DiscountProgramService(IDiscountProgramRepository repo)
    {
        _repo = repo;
    }

    public async Task<ApiResponse<List<DiscountProgramResponse>>> GetAllAsync()
    {
        var items = await _repo.GetAllAsync();
        return ApiResponse<List<DiscountProgramResponse>>.Ok(items.Select(Map).ToList());
    }

    public async Task<ApiResponse<DiscountProgramResponse>> GetByIdAsync(int id)
    {
        var item = await _repo.GetByIdAsync(id);
        if (item == null) return ApiResponse<DiscountProgramResponse>.Error("Discount program not found", 404);
        return ApiResponse<DiscountProgramResponse>.Ok(Map(item));
    }

    public async Task<ApiResponse<DiscountProgramResponse>> CreateAsync(CreateDiscountProgramRequest request)
    {
        if (request.FromDate > request.ToDate)
            return ApiResponse<DiscountProgramResponse>.Error("fromDate must be <= toDate", 400);

        var entity = new DiscountProgram
        {
            DiscountProduct = request.DiscountProduct,
            DiscountFor = request.DiscountFor,
            TypeDiscount = request.TypeDiscount,
            DiscountPercentage = request.DiscountPercentage,
            FromDate = request.FromDate,
            ToDate = request.ToDate,
            ValidTo = request.ValidTo,
            ValidFrom = request.ValidFrom,
            MaxUsage = request.MaxUsage,
            CurrentUsage = 0,
            Status = request.Status,
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow
        };

        var created = await _repo.CreateAsync(entity);
        return ApiResponse<DiscountProgramResponse>.Ok(Map(created), "Created", 201);
    }

    public async Task<ApiResponse<DiscountProgramResponse>> UpdateAsync(UpdateDiscountProgramRequest request)
    {
        if (request.FromDate > request.ToDate)
            return ApiResponse<DiscountProgramResponse>.Error("fromDate must be <= toDate", 400);

        var item = await _repo.GetByIdAsync(request.ProgramId);
        if (item == null) return ApiResponse<DiscountProgramResponse>.Error("Discount program not found", 404);

        item.DiscountProduct = request.DiscountProduct;
        item.DiscountFor = request.DiscountFor;
        item.TypeDiscount = request.TypeDiscount;
        item.DiscountPercentage = request.DiscountPercentage;
        item.FromDate = request.FromDate;
        item.ToDate = request.ToDate;
        item.ValidTo = request.ValidTo;
        item.ValidFrom = request.ValidFrom;
        item.MaxUsage = request.MaxUsage;
        item.CurrentUsage = request.CurrentUsage;
        item.Status = request.Status;
        item.UpdatedDate = DateTime.UtcNow;

        var updated = await _repo.UpdateAsync(item);
        return ApiResponse<DiscountProgramResponse>.Ok(Map(updated), "Updated");
    }

    public async Task<ApiResponse<object>> DeleteAsync(int id)
    {
        var item = await _repo.GetByIdAsync(id);
        if (item == null) return ApiResponse<object>.Error("Discount program not found", 404);

        var ok = await _repo.DeleteAsync(item);
        if (!ok) return ApiResponse<object>.Error("Delete failed", 500);

        return ApiResponse<object>.Ok(null, "Deleted");
    }

    private static DiscountProgramResponse Map(DiscountProgram x) => new()
    {
        ProgramId = x.ProgramId,
        DiscountProduct = x.DiscountProduct,
        DiscountFor = x.DiscountFor,
        TypeDiscount = x.TypeDiscount,
        DiscountPercentage = x.DiscountPercentage,
        FromDate = x.FromDate,
        ToDate = x.ToDate,
        ValidTo = x.ValidTo,
        ValidFrom = x.ValidFrom,
        MaxUsage = x.MaxUsage,
        CurrentUsage = x.CurrentUsage,
        Status = x.Status,
        CreatedDate = x.CreatedDate,
        UpdatedDate = x.UpdatedDate
    };
}