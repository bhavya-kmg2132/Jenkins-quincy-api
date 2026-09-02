using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.UnlistedRequestAndQueryName.Queries.GetUnlistedRequestAndQueryName
{
    /// <summary>
    /// class GetUnlistedRequestAndQueryNameListQuery extends the IRequest interface of MediatR
    /// </summary>
    public class GetUnlistedRequestAndQueryNameListQuery : IRequest<UnlistedRequestAndQueryNameListVm>
    {

    }

    /// <summary>
    /// For Creating handler for the above request , created GetAcitivityQueryHandler class
    ///that implements the IRequestHandler interface as shown below.
    /// </summary>
    public class GetGetAllUnlistedPermissionsQueryHandler : IRequestHandler<GetUnlistedRequestAndQueryNameListQuery, UnlistedRequestAndQueryNameListVm>
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IUserDataAccess _dataAccess;
        private readonly IIdentityManager _identityManager;
        private readonly IMapper _mapper;

        /// <summary>
        /// Instantiates GetAcitivityQueryHandler class
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        /// <param name="mapper"></param>
        public GetGetAllUnlistedPermissionsQueryHandler(IConfiguration configuration, ILogger logger, IMapper mapper, IUserDataAccess dataAccess, IIdentityManager identityManager)
        {
            this._configuration = configuration;
            this._logger = logger;
            this._dataAccess = dataAccess;
            _mapper = mapper;
            _identityManager = identityManager;
        }

        /// <summary>
        /// Handler will recieve request ,process it and will return the response. 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<UnlistedRequestAndQueryNameListVm> Handle(GetUnlistedRequestAndQueryNameListQuery query, CancellationToken cancellationToken)
        {

            List<UnlistedRequestAndQueryNameDto> GetAllUnlistedPermissionsListData = new List<UnlistedRequestAndQueryNameDto>();
            UnlistedRequestAndQueryNameListVm GetAllUnlistedPermissionsVm = new UnlistedRequestAndQueryNameListVm();
            foreach (System.Type type in Assembly.GetExecutingAssembly()
           .GetTypes()
           .Where(mytype => mytype.GetInterfaces().Contains(typeof(IBaseRequest))))
            {
                if (!type.Name.Contains("TodoItem") && !type.Name.Contains("TodoLists") && !type.Name.Contains("Todos"))
                {
                    UnlistedRequestAndQueryNameDto GetAllUnlistedPermissionsDto = new UnlistedRequestAndQueryNameDto
                    {
                        Description = type.Name,
                    };
                    GetAllUnlistedPermissionsListData.Add(GetAllUnlistedPermissionsDto);
                }
            }

            #region now this section is not in use as netauth is not seprate api now onwards
            //var urlList = await _dataAccess.GetInternalApiUrl();
            //foreach (var url in urlList)
            //{
            //    var permissionsByUrl = await _dataAccess.GetPermissionsByUrl(url);
            //    foreach(var item in permissionsByUrl)
            //    {
            //        GetAllUnlistedPermissionsListData.Add(item);
            //    }
            //}

            #endregion

            List<UnlistedRequestAndQueryNameDto> getAllUnListedPermissions = new List<UnlistedRequestAndQueryNameDto>();
            var dbPermissions = await _identityManager.GetPermissionsAsync();
            foreach (var item in GetAllUnlistedPermissionsListData)
            {
                var list = dbPermissions.Exists(x => x.PermissionValue.Equals(item.Description));
                if (!list)
                {
                    getAllUnListedPermissions.Add(item);
                }
            }
            GetAllUnlistedPermissionsVm.UnlistedRequestAndQueryNames = getAllUnListedPermissions.DistinctBy(x => x.Description).ToList(); ;
            return GetAllUnlistedPermissionsVm;
        }
    }
}

