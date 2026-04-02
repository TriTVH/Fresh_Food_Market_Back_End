using Microsoft.EntityFrameworkCore;
using VoucherService.Model;
using VoucherService.Model.DBContext;

namespace VoucherService.Repository.Implementor;

public class VoucherRepository : IVoucherRepository
{
    private readonly PromotionFfmContext _context;

    public VoucherRepository(PromotionFfmContext context)
    {
        _context = context;
    }

    public async Task<List<Voucher>> GetAllAsync()
    {
        return await _context.Vouchers
            .OrderByDescending(v => v.CreatedDate)
            .ToListAsync();
    }

    public async Task<Voucher?> GetByIdAsync(int id)
    {
        return await _context.Vouchers.FirstOrDefaultAsync(v => v.VoucherId == id);
    }

    public async Task<Voucher?> GetByCodeAsync(string voucherCode)
    {
        return await _context.Vouchers.FirstOrDefaultAsync(v => v.VoucherCode == voucherCode);
    }

    public async Task<Voucher> CreateAsync(Voucher voucher)
    {
        var entry = await _context.Vouchers.AddAsync(voucher);
        await _context.SaveChangesAsync();
        return entry.Entity;
    }

    public async Task<Voucher> UpdateAsync(Voucher voucher)
    {
        var entry = _context.Vouchers.Update(voucher);
        await _context.SaveChangesAsync();
        return entry.Entity;
    }

    public async Task<bool> DeleteAsync(Voucher voucher)
    {
        _context.Vouchers.Remove(voucher);
        var affected = await _context.SaveChangesAsync();
        return affected > 0;
    }

    public Task<List<Voucher>> GetAllByOrderIdAsync(int orderId)
    {
        return _context.Vouchers.
             Include(v => v.VoucherDetails)
            .Where(v => v.VoucherDetails.Any(d => d.OrderId == orderId))
            .ToListAsync();
    }

    public async Task<List<Voucher>> GetByIdsAsync(List<int> ids)
    {
        return await _context.Vouchers
            .Where(v => ids.Contains(v.VoucherId))
            .ToListAsync();
    }

    public async Task UpdateRangeAsync(List<Voucher> vouchers)
    {
        _context.Vouchers.UpdateRange(vouchers);
        await _context.SaveChangesAsync();
    }

}