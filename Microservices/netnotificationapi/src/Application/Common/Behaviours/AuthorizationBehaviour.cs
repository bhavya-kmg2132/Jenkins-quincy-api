using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Application.Common.Behaviours
{
    public class AuthorizationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthorizationBehaviour(ICurrentUserService currentUserService, IHttpContextAccessor httpContextAccessor)
        {
            _currentUserService = currentUserService;
            _httpContextAccessor = httpContextAccessor;
        }


        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            //  var authorizeRequestAttributes = request.GetType().GetCustomAttributes<AuthorizeRequestAttribute>();


            // User is authorized or authorization not required
            return await next();
        }


        //public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        //{
        //    var authorizeRequestAttributes = request.GetType().GetCustomAttributes<AuthorizeRequestAttribute>();

        //    if (authorizeRequestAttributes.Any())
        //    {
        //        // Must be authenticated user
        //        if (_currentUserService.UserId == null)
        //        {
        //            throw new UnauthorizedAccessException();
        //        }

        //        //// Role-based authorization
        //        var authorizeRequestAttributesWithRoles = authorizeAttributes.Where(a => !string.IsNullOrWhiteSpace(a.Roles));

        //        if (authorizeAttributesWithRoles.Any())
        //        {
        //            foreach (var roles in authorizeRequestAttributesWithRoles.Select(a => a.Roles.Split(',')))
        //            {
        //                var authorized = false;
        //                foreach (var role in roles)
        //                {
        //                    var isInRole = await _identityService.IsInRoleAsync(_currentUserService.UserId, role.Trim());
        //                    if (isInRole)
        //                    {
        //                        authorized = true;
        //                        break;
        //                    }
        //                }

        //                // Must be a member of at least one role in roles
        //                if (!authorized)
        //                {
        //                    throw new ForbiddenAccessException();
        //                }
        //            }
        //        }

        //        //// Policy-based authorization
        //        //var authorizeRequestAttributesWithPolicies = authorizeRequestAttributes.Where(a => !string.IsNullOrWhiteSpace(a.Policy));
        //        //if (authorizeRequestAttributesWithPolicies.Any())
        //        //{
        //        //    foreach (var policy in authorizeRequestAttributesWithPolicies.Select(a => a.Policy))
        //        //    {
        //        //        var authorized = await _identityService.AuthorizeAsync(_currentUserService.UserId, policy);

        //        //        if (!authorized)
        //        //        {
        //        //            throw new ForbiddenAccessException();
        //        //        }
        //        //    }
        //        //}
        //    }

        //    // User is authorized / authorization not required
        //    return await next();
        //}

    }
}
