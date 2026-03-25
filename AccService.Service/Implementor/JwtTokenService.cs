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
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AccService.Service.Implementor
{
    public class JwtTokenService : IJwtTokenService
    {

        private IAccountRepository _accRepo;
        private IRefreshTokenRepository _refreshTokenRepo;

        public JwtTokenService(IAccountRepository accRepo, IRefreshTokenRepository refreshTokenRepo)
        {
            _accRepo = accRepo;
            _refreshTokenRepo = refreshTokenRepo;
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

                await _refreshTokenRepo.RevokeAllByUserIdAsync(user.AccountId);

                var refreshToken = GenerateRefreshToken();
                var refreshTokenExpiredAt = DateTime.UtcNow.AddDays(7);

                var refreshTokenEntity = new RefreshToken
                {
                    UserId = user.AccountId,
                    TokenHash = HashToken(refreshToken),
                    ExpiredAt = refreshTokenExpiredAt,
                    Revoked = false
                };

                await _refreshTokenRepo.CreateRefreshTokenAsync(refreshTokenEntity);

                return ApiResponse<AuthenticationToken>.Ok(new AuthenticationToken { ExpirationTime = expirationTimeStamp, Token = tokenString, RefreshTokenExpiredAt = refreshTokenExpiredAt, RefreshToken = refreshToken, Role = user.RoleId, Username = user.Username }, "Login Successfully", 200);
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
        private string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }
        public async Task<ApiResponse<AuthenticationToken>> RefreshTokenAsync(RefreshTokenRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.RefreshToken))
                {
                    return ApiResponse<AuthenticationToken>.Error(null, "Refresh token is required.", 400);
                }

                var tokenHash = HashToken(request.RefreshToken);
                var existingRefreshToken = await _refreshTokenRepo.GetValidRefreshTokenAsync(tokenHash);

                if (existingRefreshToken == null || existingRefreshToken.User == null)
                {
                    return ApiResponse<AuthenticationToken>.Error(null, "Invalid or expired refresh token.", 401);
                }

                if (!existingRefreshToken.User.IsActive)
                {
                    return ApiResponse<AuthenticationToken>.Error(null, "Account is inactive.", 403);
                }

                // Rotate refresh token: revoke token cũ, cấp token mới
                await _refreshTokenRepo.RevokeRefreshTokenAsync(existingRefreshToken.RefreshTokenId);

                var newAccessTokenExpiredAt = DateTime.UtcNow.AddMinutes(15);
                var newAccessToken = GenerateAccessToken(existingRefreshToken.User, newAccessTokenExpiredAt);

                var newRefreshToken = GenerateRefreshToken();
                var newRefreshTokenExpiredAt = DateTime.UtcNow.AddDays(7);

                var newRefreshTokenEntity = new RefreshToken
                {
                    UserId = existingRefreshToken.UserId,
                    TokenHash = HashToken(newRefreshToken),
                    ExpiredAt = newRefreshTokenExpiredAt,
                    Revoked = false
                };

                await _refreshTokenRepo.CreateRefreshTokenAsync(newRefreshTokenEntity);

                return ApiResponse<AuthenticationToken>.Ok(
                    new AuthenticationToken() { ExpirationTime = newAccessTokenExpiredAt, Token = newAccessToken, RefreshTokenExpiredAt = newRefreshTokenExpiredAt, RefreshToken = newRefreshToken },
                    "Refresh token successfully",
                    200);
            }
            catch (Exception ex)
            {
                return ApiResponse<AuthenticationToken>.Error(null, ex.Message, 500);
            }
        }
        private string GenerateAccessToken(Account user, DateTime expiredAt)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtExtension.SecurityKey));
            var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.AccountId.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Username ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Name, user.Username ?? string.Empty),
                new Claim("role", user.RoleId.ToString()),
                new Claim("phone", user.Phone ?? string.Empty)
            };

            var tokenOptions = new JwtSecurityToken(
                issuer: JwtExtension.Issuer,
                claims: claims,
                expires: expiredAt,
                signingCredentials: signingCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        }
        private string HashToken(string token)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(token);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
