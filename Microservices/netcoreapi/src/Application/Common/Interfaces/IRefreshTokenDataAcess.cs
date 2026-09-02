using System.Threading.Tasks;
using Domain.Common;

namespace Application.Common.Interfaces
{
    public interface IRefreshTokenDataAccess
    {
        Task<RefreshToken> GenerateRefreshToken(string userId);
        Task<RefreshToken> GetStoredTokenByRefreshToken(string token);
        Task RevokeAsync(string token);
        Task RevokeAllAsync(string userId);
        Task<RefreshToken> RotateAsync(RefreshToken oldToken);
    }
}
