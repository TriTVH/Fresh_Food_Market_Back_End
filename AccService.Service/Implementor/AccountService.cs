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
    }
}
