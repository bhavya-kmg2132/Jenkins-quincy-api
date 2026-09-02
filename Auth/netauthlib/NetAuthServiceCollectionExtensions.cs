using Microsoft.Extensions.DependencyInjection;

namespace netauthlib
{
    public static class NetAuthServiceCollectionExtensions
    {
        public static IServiceCollection AddNetAuthLib(this IServiceCollection services)
        {
            services.AddTransient<INetAuthProvider, NetAuthProvider>();
            return services;
        }
    }
}
