using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface IMainDbInitialSetUpDataAccess
    {
        Task<bool> Add();
    }
}
