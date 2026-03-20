using Microsoft.EntityFrameworkCore;
using VoucherService.Model;
using VoucherService.Model.DBContext;

namespace VoucherService.Repository.Implementor;

public class VoucherDetailRepository : IVoucherDetailRepository
{
    private readonly VoucherMgmtFfmContext _context;

    public VoucherDetailRepository(VoucherMgmtFfmContext context)
    {
        _context = context;
    }

    public async Task<List<VoucherDetail>> GetAllAsync()
    {
        return await _context.VoucherDetails
            .Include(x => x.Voucher)
            .OrderByDescending(x => x.AppliedDate)
            .ToListAsync();
    }

    public async Task<List<VoucherDetail>> GetByVoucherIdAsync(int voucherId)
    {
        return await _context.VoucherDetails
            .Include(x => x.Voucher)
            .Where(x => x.VoucherId == voucherId)
            .OrderByDescending(x => x.AppliedDate)
            .ToListAsync();
    }

    public async Task<VoucherDetail?> GetByIdAsync(int id)
    {
        return await _context.VoucherDetails
            .Include(x => x.Voucher)
            .FirstOrDefaultAsync(x => x.VoucherDetailId == id);
    }

    public async Task<VoucherDetail> CreateAsync(VoucherDetail detail)
    {
        var entry = await _context.VoucherDetails.AddAsync(detail);
        await _context.SaveChangesAsync();
        return entry.Entity;
    }

    public Task<int> SaveChangesAsync()
    {
        return _context.SaveChangesAsync();
    }
}