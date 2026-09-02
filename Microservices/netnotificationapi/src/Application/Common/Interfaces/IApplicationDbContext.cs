using System.Threading;
using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        //   DbSet<TodoList> TodoLists { get; set; }

        //  DbSet<TodoItem> TodoItems { get; set; }

        //DbSet<Domain.Entities.Acme> AcmeProduct { get; set; }


        Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken());
    }
}
