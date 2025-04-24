using Microsoft.Extensions.DependencyInjection;

namespace Drugs.Library.Repository
{
    internal static class RepositoryExtensions
    {
        internal static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            // Register each repository as a scoped service
            services.AddScoped<DrugSqliteRepository>();
            services.AddScoped<SideEffectSqliteRepository>();
            services.AddScoped<CategorySqliteRepository>();
            services.AddScoped<DrugSideEffectLinkSqliteRepository>();
            services.AddScoped<DrugCategoryLinkSqliteRepository>();

            return services;
        }
    }
}