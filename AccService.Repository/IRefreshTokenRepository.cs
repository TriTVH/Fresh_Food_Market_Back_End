using AccService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccService.Repository
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken> CreateRefreshTokenAsync(RefreshToken refreshToken);
        Task<RefreshToken?> GetValidRefreshTokenAsync(string tokenHash);
        Task RevokeRefreshTokenAsync(int refreshTokenId);
        Task RevokeAllByUserIdAsync(int userId);
    }
}
