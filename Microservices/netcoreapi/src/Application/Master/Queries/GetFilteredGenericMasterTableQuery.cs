using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
//using Application.Master.GenericMasterListSql.Queries.GetGenericMasterListSqlList;
using Application.Master.Master.Queries.GetFilteredGenericMasterTable;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Master.Queries.GetFilteredGenericMasterTable
{
    public class GetFilteredGenericMasterTableQuery : IRequest<List<GetFilteredGenericMasterTableQueryDto>>
    {
        public List<string> Type { get; set; }

        public List<string> Group { get; set; }
    }

    public class GetFilteredGenericMasterTableQueryHandler : IRequestHandler<GetFilteredGenericMasterTableQuery, List<GetFilteredGenericMasterTableQueryDto>>
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IMasterDataAccess _dataAccess;
        private readonly IMapper _mapper;

        public GetFilteredGenericMasterTableQueryHandler(IConfiguration configuration, ILogger logger, IMapper mapper, IMasterDataAccess dataAccess)
        {
            this._configuration = configuration;
            this._logger = logger;
            this._dataAccess = dataAccess;
            _mapper = mapper;
        }

        public async Task<List<GetFilteredGenericMasterTableQueryDto>> Handle(GetFilteredGenericMasterTableQuery request, CancellationToken cancellationToken)
        {
            var typeSet = new HashSet<string>(request.Type ?? Enumerable.Empty<string>());
            var groupSet = new HashSet<string>(request.Group ?? Enumerable.Empty<string>());

            var response = await _dataAccess.GetFilterGenericMasterList(typeSet.ToList(), groupSet.ToList());

            if (!typeSet.Any() && !groupSet.Any())
            {
                return response
                    .GroupBy(x => new { x.Type, x.Group })
                    .Select(g => new GetFilteredGenericMasterTableQueryDto
                    {
                        Type = g.Key.Type,
                        Group = g.Key.Group,
                        Data = g.ToList()
                    })
                    .ToList();
            }

            var typeGroups = new Dictionary<string, List<Domain.Entities.GenericMasterList>>();
            var groupGroups = new Dictionary<string, List<Domain.Entities.GenericMasterList>>();

            foreach (var item in response)
            {
                if (!string.IsNullOrEmpty(item.Type) && typeSet.Contains(item.Type))
                {
                    if (!typeGroups.TryGetValue(item.Type, out var list))
                    {
                        list = new List<Domain.Entities.GenericMasterList>();
                        typeGroups[item.Type] = list;
                    }
                    list.Add(item);
                }

                if (!string.IsNullOrEmpty(item.Group) && groupSet.Contains(item.Group))
                {
                    if (!groupGroups.TryGetValue(item.Group, out var list))
                    {
                        list = new List<Domain.Entities.GenericMasterList>();
                        groupGroups[item.Group] = list;
                    }
                    list.Add(item);
                }
            }

            var result = new List<GetFilteredGenericMasterTableQueryDto>();

            result.AddRange(
                typeGroups.Select(g => new GetFilteredGenericMasterTableQueryDto
                {
                    Type = g.Key,
                    Group = null,
                    Data = g.Value
                })
            );

            result.AddRange(
                groupGroups.Select(g => new GetFilteredGenericMasterTableQueryDto
                {
                    Type = null,
                    Group = g.Key,
                    Data = g.Value
                })
            );

            return result;
        }





    }
}