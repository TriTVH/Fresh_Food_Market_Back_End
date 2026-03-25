using Microsoft.EntityFrameworkCore;
using VoucherService.Model;
using VoucherService.Model.DBContext;

namespace VoucherService.Repository.Implementor;

public class DiscountProgramRepository : IDiscountProgramRepository
{
    private readonly PromotionFfmContext _context;

    public DiscountProgramRepository(PromotionFfmContext context)
    {
        _context = context;
    }

    public async Task<List<DiscountProgram>> GetAllAsync()
    {
        return await _context.DiscountPrograms
            .OrderByDescending(x => x.CreatedDate)
            .ToListAsync();
    }

    public async Task<DiscountProgram?> GetByIdAsync(int id)
    {
        return await _context.DiscountPrograms.FirstOrDefaultAsync(x => x.ProgramId == id);
    }

    public async Task<DiscountProgram> CreateAsync(DiscountProgram entity)
    {
        var entry = await _context.DiscountPrograms.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entry.Entity;
    }

    public async Task<DiscountProgram> UpdateAsync(DiscountProgram entity)
    {
        var entry = _context.DiscountPrograms.Update(entity);
        await _context.SaveChangesAsync();
        return entry.Entity;
    }

    public async Task<bool> DeleteAsync(DiscountProgram entity)
    {
        _context.DiscountPrograms.Remove(entity);
        return await _context.SaveChangesAsync() > 0;
    }
}
