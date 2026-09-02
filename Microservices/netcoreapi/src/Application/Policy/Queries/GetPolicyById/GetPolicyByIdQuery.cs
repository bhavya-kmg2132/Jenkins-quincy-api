using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Policy.Queries.GetPolicyById
{
    public class GetPolicyByIdQuery : IRequest<PolicyDto>
    {
        public string Id { get; set; }
    }

    public class GetPolicyByIdHandler : IRequestHandler<GetPolicyByIdQuery, PolicyDto>
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IPolicyDataAccess _policyDataAccess;
        private readonly IFieldPermissionService _fieldPermissions;

        public GetPolicyByIdHandler(IConfiguration configuration, ILogger logger,
            IMapper mapper, IPolicyDataAccess policyDataAccess, IFieldPermissionService fieldPermissions)
        {
            _configuration = configuration;
            _logger = logger;
            _mapper = mapper;
            _policyDataAccess = policyDataAccess;
            _fieldPermissions = fieldPermissions;
        }

        public async Task<PolicyDto> Handle(GetPolicyByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("GetPolicyByIdQuery.Handle - In Process");

            var entity = await _policyDataAccess.GetPolicyById(request.Id);

            // Mask fields the current user is not permitted to view before mapping to DTO.
            // [FieldPermission(view:...)] attributes on Policy entity properties drive this.
            if (entity != null)
                await _fieldPermissions.ApplyViewPermissionsAsync(entity);

            _logger.LogInformation("GetPolicyByIdQuery.Handle - Completed");
            return _mapper.Map<PolicyDto>(entity);
        }
    }
}
