using AccService.Model;
using AccService.Repository;
using AccService.Repository.Implementor;
using AccService.Service.DTO;
using AccService.Service.DTO.RequestObject;
using AccService.Service.DTO.ResponseObject;
using JwtConfiguration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AccService.Service.Implementor
{
    public class JwtTokenService : IJwtTokenService
    {

        private IAccountRepository _accRepo;

        public JwtTokenService(IAccountRepository accRepo)
        {
            _accRepo = accRepo;
        }

        public async Task<ApiResponse<AccountDTO>> CreateBuyerAccountAsync(RegisterModel request)
        {
            try
            {
                var username = await GenerateUniqueUsernameAsync(request.Phone);
                var newAccount = new Account
                {
                    Phone = request.Phone,
                    Username = username,
                    Password = request.Password,
                    RoleId = 2,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                };
                var createdAccount = await _accRepo.CreateAccountAsync(newAccount);

                if (createdAccount == null)
                {
                    return ApiResponse<AccountDTO>.Error(null, "Account with the same phone number already exists", 400);
                }

                return ApiResponse<AccountDTO>.Ok(new AccountDTO
                {
                    AccountId = createdAccount.AccountId,
                    Phone = request.Phone,
                    Username = username,
                    RoleName = "CUSTOMER",
                    IsActive = true,
                    CreatedDate = createdAccount.CreatedDate,
                    UpdatedDate = createdAccount.UpdatedDate
                }, "Customer account created successfully", 201);

            }
            catch (Exception ex)
            {
                return ApiResponse<AccountDTO>.Error(null, ex.Message, 500);
            }
        }

        public async Task<ApiResponse<AuthenticationToken>> GenerateAuthToken(LoginModel loginModel)
        {
            try
            {
                if (loginModel.Phone.Length != 10)
                {
                    return ApiResponse<AuthenticationToken>.Error(null, "The Phone field is not a valid phone number.", 400);
                }
                var user = await _accRepo.IsValidUserCredential(loginModel.Phone, loginModel.Password);
                if (user is null)
                {
                    return ApiResponse<AuthenticationToken>.Error(null, "Invalid email or password!!!", 400);
                }
                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtExtension.SecurityKey));
                var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
                var expirationTimeStamp = DateTime.Now.AddMinutes(15);

                var claims = new List<Claim>
                {
                  new Claim(JwtRegisteredClaimNames.Name, user.Username),
                  new Claim("role", user.RoleId.ToString()),
                };
                var tokenOptions = new JwtSecurityToken(
                issuer: JwtExtension.Issuer,
                claims: claims,
                expires: expirationTimeStamp,
                signingCredentials: signingCredentials
                );
                var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
                return ApiResponse<AuthenticationToken>.Ok(new AuthenticationToken(tokenString, expirationTimeStamp), "Login Successfully", 200);
            }
            catch (Exception ex)
            {
                return ApiResponse<AuthenticationToken>.Error(null, ex.Message, 500);
            }

        }

        private async Task<string> GenerateUniqueUsernameAsync(string phone)
        {
            var baseUsername = $"user_{phone.Substring(Math.Max(0, phone.Length - 4))}";
            var username = baseUsername;

            var random = new Random();

            while (await _accRepo.IsUsernameExistsAsync(username))
            {
                username = $"{baseUsername}{random.Next(100, 999)}";
            }

            return username;
        }

    }
}
