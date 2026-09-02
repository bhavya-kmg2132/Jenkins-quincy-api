using NetAuth.Contract.DataContract.Entities;

namespace Application.Common.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(IdentityUser user);
    }
}
