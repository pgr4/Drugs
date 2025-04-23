using Drugs.Library.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace Drugs.Library
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddDrugLibrary(this IServiceCollection services)
        {
            // Register the Repositories
            RepositoryExtensions.AddRepositories(services);

            // Register the Service
            services.AddScoped<Service>();

            return services;
        }
    }
}