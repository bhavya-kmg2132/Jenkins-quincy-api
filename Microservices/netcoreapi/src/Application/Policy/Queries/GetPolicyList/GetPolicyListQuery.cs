using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Policy.Queries.GetPolicyList
{
    public class GetPolicyListQuery : IRequest<PolicyListVm>
    {
    }

    public class GetPolicyListQueryHandler : IRequestHandler<GetPolicyListQuery, PolicyListVm>
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IPolicyDataAccess _policyDataAccess;

        public GetPolicyListQueryHandler(IConfiguration configuration, ILogger logger,
            IMapper mapper, IPolicyDataAccess policyDataAccess)
        {
            _configuration = configuration;
            _logger = logger;
            _mapper = mapper;
            _policyDataAccess = policyDataAccess;
        }

        public async Task<PolicyListVm> Handle(GetPolicyListQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("GetPolicyListQuery.Handle - In Process");

            var entities = await _policyDataAccess.GetPolicyList();

            _logger.LogInformation("GetPolicyListQuery.Handle - Completed");
            return new PolicyListVm
            {
                PolicyList = _mapper.Map<List<Domain.Entities.Policy>, List<PolicyDto>>(entities)
            };
        }
    }
}
