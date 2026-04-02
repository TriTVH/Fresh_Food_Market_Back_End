using VoucherService.Model;

namespace VoucherService.Repository;

public interface IVoucherDetailRepository
{
    Task<List<VoucherDetail>> GetAllAsync();
    Task<List<VoucherDetail>> GetByVoucherIdAsync(int voucherId);
    Task<List<VoucherDetail>> GetByOrderIdAsync(int userId);
    Task<VoucherDetail?> GetByIdAsync(int id);
    Task<VoucherDetail> CreateAsync(VoucherDetail detail);
    Task<int> SaveChangesAsync();
    Task UpdateRangeAsync(List<VoucherDetail> voucherDetails);
}