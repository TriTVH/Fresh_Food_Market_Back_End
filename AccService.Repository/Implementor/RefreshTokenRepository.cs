using AccService.Model;
using AccService.Model.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccService.Repository.Implementor
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {

        private readonly AccountMgmtFfmContext _context;

        public RefreshTokenRepository(AccountMgmtFfmContext context)
        {
            _context = context;
        }

        public async Task<RefreshToken> CreateRefreshTokenAsync(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();
            return refreshToken;
        }

        public async Task<RefreshToken?> GetValidRefreshTokenAsync(string tokenHash)
        {
            return await _context.RefreshTokens
                .Include(x => x.User)
                .FirstOrDefaultAsync(x =>
                    x.TokenHash == tokenHash &&
                    !x.Revoked &&
                    x.ExpiredAt > DateTime.UtcNow);
        }

        public async Task RevokeRefreshTokenAsync(int refreshTokenId)
        {
            var token = await _context.RefreshTokens.FirstOrDefaultAsync(x => x.RefreshTokenId == refreshTokenId);
            if (token != null)
            {
                token.Revoked = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task RevokeAllByUserIdAsync(int userId)
        {
            var tokens = await _context.RefreshTokens
                .Where(x => x.UserId == userId && !x.Revoked)
                .ToListAsync();

            foreach (var token in tokens)
            {
                token.Revoked = true;
            }

            await _context.SaveChangesAsync();
        }

    }
}
