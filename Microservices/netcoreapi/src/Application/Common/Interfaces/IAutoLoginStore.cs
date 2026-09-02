using Domain.Dto;

namespace Application.Common.Interfaces
{
    public interface IAutoLoginStore
    {
        string MintKey(string userId);
        AutoLoginRecord ValidateAndConsume(string key);
    }
}
