using Microsoft.Extensions.DependencyInjection;

namespace PostgresRepository
{
    public static class DIRepositoryConfigurator
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IConnectionRepository, ConnectionRepository>();
            services.AddSingleton<IDataRepository, PostgresRepository>();
        }
    }
}
