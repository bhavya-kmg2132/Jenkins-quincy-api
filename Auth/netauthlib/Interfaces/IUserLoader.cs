using NetAuth.Domain.Dto;

namespace NetAuth.Interfaces
{
    internal interface IUserLoader
    {
        Task<List<UserDto>> LoadUsersFromDbAsync(string paramUserId);
    }
}
