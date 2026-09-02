using Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.SystemManager.UpdateActionPermissionEndPoint
{
    /// <summary>
    /// class UpdateActionPermissionEndPointRequest extends the IRequest interface of MediatR
    /// </summary>
    public class UpdateActionPermissionEndPointRequest : IRequest<UpdateActionPermissionEndPointVm>
    {
        public List<UpdateActionPermissionEndPointDto> Items { get; set; } = new List<UpdateActionPermissionEndPointDto>();
    }

    /// <summary>
    /// For Creating handler for the above request , created UpdateActionPermissionEndPointRequestHandler class
    /// that implements the IRequestHandler interface as shown below.
    /// </summary>
    public class UpdateActionPermissionEndPointRequestHandler : IRequestHandler<UpdateActionPermissionEndPointRequest, UpdateActionPermissionEndPointVm>
    {
        private const string ModuleLookupType = "Module";
        private const string ModuleLookupName = "PermissionModule";
        private const string PermissionSetLookupType = "PermissionSet";
        private const string PermissionSetLookupName = "PermissionView";
        private const string PermissionType = "ACTION";

        private readonly ILogger _logger;
        private readonly IUserDataAccess _dataAccess;
        private readonly IIdentityManager _identityManager;
        private readonly ICurrentUserService _currentUserService;
        private readonly IConfiguration _configuration;

        public UpdateActionPermissionEndPointRequestHandler(ILogger logger, IUserDataAccess dataAccess, IIdentityManager identityManager, ICurrentUserService currentUserService, IConfiguration configuration)
        {
            _logger = logger;
            _dataAccess = dataAccess;
            _identityManager = identityManager;
            _currentUserService = currentUserService;
            _configuration = configuration;
        }

        public async Task<UpdateActionPermissionEndPointVm> Handle(UpdateActionPermissionEndPointRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("UpdateActionPermissionEndPointRequest.Handle - In process");

            var modules = await _dataAccess.GetAuthReferenceLookupList(ModuleLookupType);
            var moduleId = modules.FirstOrDefault(x => x.Name == ModuleLookupName)?.Id;
            if (string.IsNullOrEmpty(moduleId))
            {
                throw new ApplicationException($"AuthReferenceLookup with Name='{ModuleLookupName}' and Type='{ModuleLookupType}' was not found.");
            }

            var permissionSets = await _dataAccess.GetAuthReferenceLookupList(PermissionSetLookupType);
            var permissionSetId = permissionSets.FirstOrDefault(x => x.Name == PermissionSetLookupName)?.Id;
            if (string.IsNullOrEmpty(permissionSetId))
            {
                throw new ApplicationException($"AuthReferenceLookup with Name='{PermissionSetLookupName}' and Type='{PermissionSetLookupType}' was not found.");
            }

            var existingPermissions = await _identityManager.GetPermissionsAsync();
            var existingValues = existingPermissions.Select(x => x.PermissionValue).ToHashSet(StringComparer.OrdinalIgnoreCase);
            // PermissionDisplayName has a UNIQUE constraint, so a colliding display name must be rejected up front -
            // otherwise the insert throws and aborts every remaining item in the same batch.
            var seenDisplayNames = existingPermissions.Select(x => x.PermissionDisplayName).ToHashSet(StringComparer.OrdinalIgnoreCase);

            var distinctItems = request.Items
                .Where(x => !string.IsNullOrEmpty(x.PermissionValue))
                .GroupBy(x => x.PermissionValue, StringComparer.OrdinalIgnoreCase)
                .Select(g => g.First())
                .ToList();

            var toAdd = new List<UpdateActionPermissionEndPointDto>();
            var toSkip = new List<UpdateActionPermissionEndPointDto>();
            var toConflict = new List<UpdateActionPermissionEndPointDto>();
            var toUpdateEndpointCandidates = new List<UpdateActionPermissionEndPointDto>();

            foreach (var item in distinctItems)
            {
                if (existingValues.Contains(item.PermissionValue))
                {
                    // Permission already exists - if we have a route for it, try to backfill ActionPermissionEndPoint
                    // instead of skipping outright. The DAL only updates rows where that column is still empty.
                    if (!string.IsNullOrEmpty(item.ActionPermissionEndPoint))
                    {
                        toUpdateEndpointCandidates.Add(item);
                    }
                    else
                    {
                        toSkip.Add(item);
                    }
                }
                else if (!seenDisplayNames.Add(item.PermissionDisplayName))
                {
                    toConflict.Add(item);
                }
                else
                {
                    toAdd.Add(item);
                }
            }

            if (toAdd.Count > 0)
            {
                var apiName = _configuration["Api:api_name"];
                await _dataAccess.AddActionPermissionEndPoint(toAdd, moduleId, permissionSetId, PermissionType, apiName, _currentUserService.UserName);
            }

            var updatedValues = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (toUpdateEndpointCandidates.Count > 0)
            {
                var updated = await _dataAccess.UpdateCodePermissionActionEndpoints(toUpdateEndpointCandidates, _currentUserService.UserName);
                updatedValues = updated.ToHashSet(StringComparer.OrdinalIgnoreCase);
            }

            var toUpdate = toUpdateEndpointCandidates.Where(x => updatedValues.Contains(x.PermissionValue)).ToList();
            // Candidates whose ActionPermissionEndPoint was already set are true no-ops - fold them back into skipped.
            toSkip.AddRange(toUpdateEndpointCandidates.Where(x => !updatedValues.Contains(x.PermissionValue)));

            _logger.LogInformation("UpdateActionPermissionEndPointRequest.Handle - Completed");

            return new UpdateActionPermissionEndPointVm
            {
                AddedCount = toAdd.Count,
                SkippedCount = toSkip.Count,
                ConflictedCount = toConflict.Count,
                UpdatedActionEndpointCount = toUpdate.Count,
                AddedPermissionValues = toAdd.Select(x => x.PermissionValue).ToList(),
                SkippedPermissionValues = toSkip.Select(x => x.PermissionValue).ToList(),
                ConflictedPermissionValues = toConflict.Select(x => $"{x.PermissionValue} (display name '{x.PermissionDisplayName}' collides)").ToList(),
                UpdatedActionEndpointPermissionValues = toUpdate.Select(x => x.PermissionValue).ToList()
            };
        }
    }
}
