using VoucherService.Model;

namespace VoucherService.Repository;

public interface IDiscountProgramRepository
{
    Task<List<DiscountProgram>> GetAllAsync();
    Task<DiscountProgram?> GetByIdAsync(int id);
    Task<DiscountProgram> CreateAsync(DiscountProgram entity);
    Task<DiscountProgram> UpdateAsync(DiscountProgram entity);
    Task<bool> DeleteAsync(DiscountProgram entity);
}
