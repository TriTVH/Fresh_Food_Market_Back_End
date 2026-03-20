using Microsoft.EntityFrameworkCore;
using VoucherService.Model;
using VoucherService.Model.DBContext;

namespace VoucherService.Repository.Implementor;

public class VoucherRepository : IVoucherRepository
{
    private readonly VoucherMgmtFfmContext _context;

    public VoucherRepository(VoucherMgmtFfmContext context)
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
}