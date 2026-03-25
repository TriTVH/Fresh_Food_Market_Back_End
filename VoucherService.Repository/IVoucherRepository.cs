using VoucherService.Model;

namespace VoucherService.Repository;

public interface IVoucherRepository
{
    Task<List<Voucher>> GetAllAsync();
    Task<Voucher?> GetByIdAsync(int id);
    Task<Voucher?> GetByCodeAsync(string voucherCode);
    Task<Voucher> CreateAsync(Voucher voucher);
    Task<Voucher> UpdateAsync(Voucher voucher);
    Task<bool> DeleteAsync(Voucher voucher);
}