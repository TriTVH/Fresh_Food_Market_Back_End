using AuthService.Model;
using AuthService.Repository;
using AuthService.Service.DTO;
using AuthService.Service.DTO.RequestObject;
using AuthService.Service.DTO.ResponseObject;
using JwtConfiguration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Service.Implementor
{
    public class JwtTokenService : IJwtTokenService
    {

        private IAccountRepository _accRepo;

        public JwtTokenService(IAccountRepository accRepo)
        {
            _accRepo = accRepo;
        }

     
        public async Task<ApiResponse<AuthenticationToken>> GenerateAuthToken(LoginModel loginModel)
        {
            try
            {
                if(loginModel.Phone.Length != 10)
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
            } catch (Exception ex)
            {
                return ApiResponse<AuthenticationToken>.Error(null, ex.Message, 500);
            }
            
        }
    }
}
