using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface IInitialSetUpDataAccess
    {
        Task<bool> Add();

    }
}

