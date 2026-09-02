using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface IEventDbInitialSetUpDataAccess
    {
        Task<bool> Add();
    }
}
