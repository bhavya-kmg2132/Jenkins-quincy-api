using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface IPostGreCacheService
    {
        Task<object> GetCacheValueAsync(string key);
    }

}
