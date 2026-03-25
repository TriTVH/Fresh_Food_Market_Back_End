using AccService.Model;
using AccService.Repository;
using AccService.Service.DTO;
using AccService.Service.DTO.RequestObject;
using AccService.Service.DTO.ResponseObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccService.Service.Implementor
{
    public class AccountService : IAccountService
    {
        public IAccountRepository _accountRepository;
        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<ApiResponse<List<AccountDTO>>> GetAccounts()
        {
            try
            {
                var accounts = await _accountRepository.GetAccounts();

                var accountDTOs = accounts.Select(a => new AccountDTO
                {
                    AccountId = a.AccountId,
                    RoleName = a.Role.RoleName,
                    Email = a.Email,
                    Phone = a.Phone,
                    Username = a.Username,
                    IsActive = a.IsActive,
                    AvatarUrl = a.AvatarUrl,
                    CreatedDate = a.CreatedDate,
                    UpdatedDate = a.UpdatedDate
                }).ToList();

                return ApiResponse<List<AccountDTO>>.Ok(accountDTOs);

            }
            catch (Exception ex)
            {
                return ApiResponse<List<AccountDTO>>.Error(null, ex.Message, 500);
            }
        }

        public async Task<ApiResponse<object>> CreateSupplierAccountAsync(CreateSupplierAccountRequest request)
        {
            if (await _accountRepository.CheckPhoneExistsAsync(request.Phone))
            {
                return new ApiResponse<object>(false, "Phone number already exists in the system.", null, 409);
            }

            if (await _accountRepository.CheckUsernameExistsAsync(request.Username))
            {
                return new ApiResponse<object>(false, "Username already exists.", null, 409);
            }

            var newAccount = new Account
            {
                Username = request.Username,
                Phone = request.Phone,
                Email = request.Email,
                Password = request.Password, 
                RoleId = 3,
                SupplierId = request.SupplierId,
                IsActive = true,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            };

            var createdAccount = await _accountRepository.CreateAccountAsync(newAccount);

            var responseData = new
            {
                createdAccount.AccountId,
                createdAccount.Username,
                createdAccount.Phone
            };

            return new ApiResponse<object>(true, "Supplier account created successfully.", responseData, 201);
        }
    }
}
