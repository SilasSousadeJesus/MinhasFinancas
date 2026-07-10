using Hangfire;
using Hangfire.MySql;
using MinhasFinancas.API.Jobs;

namespace MinhasFinancas.API.Extensions
{
    public static class HangfireExtensions
    {
        public static IServiceCollection AddHangfireMinhasFinancas(this IServiceCollection services, string connectionString)
        {
            services.AddHangfire(configuration =>
                configuration
                    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UseStorage(new MySqlStorage(
                        connectionString,
                        new MySqlStorageOptions
                        {
                            PrepareSchemaIfNecessary = true
                        })));

            services.AddHangfireServer();
            services.AddScoped<IBemPatrimonialJobs, BemPatrimonialJobs>();
            services.AddScoped<IMfScoreJobs, MfScoreJobs>();

            return services;
        }

        public static WebApplication UseHangfireMinhasFinancas(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseHangfireDashboard("/hangfire");
            }

            RecurringJob.AddOrUpdate<IBemPatrimonialJobs>(
                "atualizacao-anual-bens-patrimoniais",
                x => x.FilaJobs(),
                "0 0 1 1 *");

            RecurringJob.AddOrUpdate<IMfScoreJobs>(
                "historico-mensal-mf-score",
                x => x.GerarHistoricoMensalAsync(),
                "0 2 1 * *");

            return app;
        }
    }
}
