using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Models;

namespace Application.Common.Mappings
{
    public static class MappingExtensions
    {
        public static Task<PaginatedList<TDestination>> PaginatedListAsync<TDestination>(this IQueryable<TDestination> queryable, int pageNumber, int pageSize, int totalRecord)
            => PaginatedList<TDestination>.CreateAsync(queryable, pageNumber, pageSize, totalRecord);

        public static List<TDestination> ProjectToListAsync<TDestination>(this IQueryable queryable, IConfigurationProvider configuration)
            => queryable.ProjectTo<TDestination>(configuration).ToList();
    }
}
