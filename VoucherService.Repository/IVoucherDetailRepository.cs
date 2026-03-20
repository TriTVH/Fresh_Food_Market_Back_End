using VoucherService.Model;

namespace VoucherService.Repository;

public interface IVoucherDetailRepository
{
    Task<List<VoucherDetail>> GetAllAsync();
    Task<List<VoucherDetail>> GetByVoucherIdAsync(int voucherId);
    Task<VoucherDetail?> GetByIdAsync(int id);
    Task<VoucherDetail> CreateAsync(VoucherDetail detail);
    Task<int> SaveChangesAsync();
}