using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SchemaDriftDetector.Storage.Context;

namespace SchemaDriftDetector
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSchemaDriftDetection(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<SchemaDriftDetectorDbContext>(options =>
                options.UseSqlite(connectionString));

            return services;
        }
    }
}