using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface ISqlServerCacheService
    {
        Task<object> GetCacheValueAsync(string key);
    }

}
