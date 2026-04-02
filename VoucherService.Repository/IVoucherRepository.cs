using VoucherService.Model;

namespace VoucherService.Repository;

public interface IVoucherRepository
{
    Task<List<Voucher>> GetAllAsync();
    Task<List<Voucher>> GetAllByOrderIdAsync(int orderId);
    Task<Voucher?> GetByIdAsync(int id);
    Task<Voucher?> GetByCodeAsync(string voucherCode);
    Task<Voucher> CreateAsync(Voucher voucher);
    Task<Voucher> UpdateAsync(Voucher voucher);
    Task<bool> DeleteAsync(Voucher voucher);

    Task<List<Voucher>> GetByIdsAsync(List<int> ids);
    Task UpdateRangeAsync(List<Voucher> vouchers);
}