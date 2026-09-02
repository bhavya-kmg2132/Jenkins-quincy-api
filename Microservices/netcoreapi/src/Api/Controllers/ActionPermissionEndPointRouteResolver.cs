using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Api.Controllers
{
    /// <summary>
    /// Maps a MediatR request type (stored as a Permission's PermissionValue) to the API route that
    /// triggers it, so ActionPermissionEndPoint can be derived from routing instead of hand-maintained.
    /// Shared by SystemManagerController's bulk sync and NetAuthController's single AddPermission.
    /// </summary>
    internal static class ActionPermissionEndPointRouteResolver
    {
        /// <summary>
        /// Finds the route for the action whose MediatR request type matches permissionValue by name.
        /// Returns null if no match is found.
        /// </summary>
        public static string ResolveActionPermissionEndPoint(IActionDescriptorCollectionProvider actionDescriptorCollectionProvider, string permissionValue)
        {
            if (string.IsNullOrWhiteSpace(permissionValue))
            {
                return null;
            }

            foreach (var descriptor in actionDescriptorCollectionProvider.ActionDescriptors.Items.OfType<ControllerActionDescriptor>())
            {
                if (descriptor.AttributeRouteInfo?.Template == null)
                {
                    continue;
                }

                var requestType = ResolveRequestType(descriptor);
                if (requestType != null && requestType.Name.Equals(permissionValue, StringComparison.OrdinalIgnoreCase))
                {
                    return ToActionPermissionEndPoint(descriptor.AttributeRouteInfo.Template);
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the MediatR request type an action sends, whether it's bound as a parameter or
        /// (for parameter-less actions) inferred from the {ActionName}Query/Command/Request convention.
        /// </summary>
        public static Type ResolveRequestType(ControllerActionDescriptor descriptor)
        {
            var requestParameter = descriptor.Parameters.FirstOrDefault(p => typeof(MediatR.IBaseRequest).IsAssignableFrom(p.ParameterType));
            return requestParameter?.ParameterType
                ?? ResolveRequestTypeByConvention(descriptor.ActionName, descriptor.ControllerName);
        }

        /// <summary>
        /// Finds the MediatR request type for an action that doesn't bind it as a parameter (e.g. it's
        /// constructed inline as `Mediator.Send(new GetAcmeProductListQuery())`), by matching the action
        /// name against the {ActionName}Query / {ActionName}Command / {ActionName}Request convention.
        /// Falls back to null (skip) if the match is missing or ambiguous.
        /// </summary>
        public static Type ResolveRequestTypeByConvention(string actionName, string controllerName)
        {
            var candidateNames = new[] { actionName + "Query", actionName + "Command", actionName + "Request" };

            var candidates = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(GetLoadableTypes)
                .Where(t => candidateNames.Contains(t.Name) && typeof(MediatR.IBaseRequest).IsAssignableFrom(t))
                .ToList();

            if (candidates.Count <= 1)
            {
                return candidates.FirstOrDefault();
            }

            // Ambiguous by name alone (e.g. the same action name used in multiple features) -
            // narrow down to the type living under the controller's own feature namespace.
            var narrowed = candidates.Where(t => t.Namespace?.Contains(controllerName, StringComparison.OrdinalIgnoreCase) == true).ToList();
            return narrowed.Count == 1 ? narrowed[0] : null;
        }

        private static IEnumerable<Type> GetLoadableTypes(System.Reflection.Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (System.Reflection.ReflectionTypeLoadException ex)
            {
                return ex.Types.Where(t => t != null);
            }
        }

        public static string ToActionPermissionEndPoint(string routeTemplate)
        {
            var segments = routeTemplate
                .Split('/', StringSplitOptions.RemoveEmptyEntries)
                .Where(segment => !segment.Equals("api", StringComparison.OrdinalIgnoreCase)
                                   && !Regex.IsMatch(segment, "^v(\\{version:apiVersion\\}|\\d+)$", RegexOptions.IgnoreCase));

            return string.Join("/", segments);
        }
    }
}
