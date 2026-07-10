using Hangfire;
using Hangfire.MySql;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Minhas_Financas_Hangfire.Interfaces;
using Minhas_Financas_Hangfire.Services;
using MinhasFinancas.Application.Interfaces;
using MinhasFinancas.Application.Services;
using MinhasFinancas.Domain.Entities;
using MinhasFinancas.Domain.Services.AnaliseFinanceira;
using MinhasFinancas.Domain.Services.AnaliseFinanceira.Indicadores;
using MinhasFinancas.Infra;
using MinhasFinancas.Infra.Data.config.configMigrate;
using MinhasFinancas.Infra.Data.Interfaces;
using MinhasFinancas.Infra.Data.Repositories;

namespace Minhas_Financas_Hangfire
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("ConnectionMinhasFinancas");

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            });

            builder.Services.AddIdentity<Usuario, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.AddHttpClient();

            builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            builder.Services.AddScoped<ILancamentoRepository, LancamentoRepository>();
            builder.Services.AddScoped<IBemMaterialRepository, BemMaterialRepository>();
            builder.Services.AddScoped<IPassivoRepository, PassivoRepository>();
            builder.Services.AddScoped<IPerfilFinanceiroRepository, PerfilFinanceiroRepository>();
            builder.Services.AddScoped<IHistoricoMfScoreRepository, HistoricoMfScoreRepository>();

            builder.Services.AddScoped<ICalculadorIndicadorFinanceiro, EconomiaMensalIndicador>();
            builder.Services.AddScoped<ICalculadorIndicadorFinanceiro, PercentualEconomiaIndicador>();
            builder.Services.AddScoped<ICalculadorIndicadorFinanceiro, ReservaEmergenciaAtualIndicador>();
            builder.Services.AddScoped<ICalculadorIndicadorFinanceiro, ReservaEmergenciaIdealIndicador>();
            builder.Services.AddScoped<ICalculadorIndicadorFinanceiro, ComprometimentoRendaIndicador>();
            builder.Services.AddScoped<ICalculadorIndicadorFinanceiro, ComprometimentoFinanceiroFuturoIndicador>();
            builder.Services.AddScoped<ICalculadorIndicadorFinanceiro, ComprometimentoFinanceiroFuturo90DiasIndicador>();
            builder.Services.AddScoped<ICalculadorIndicadorFinanceiro, ComprometimentoFinanceiroFuturo180DiasIndicador>();
            builder.Services.AddScoped<ICalculadorIndicadorFinanceiro, ComprometimentoFinanceiroFuturo365DiasIndicador>();
            builder.Services.AddScoped<ICalculadorIndicadorFinanceiro, EndividamentoIndicador>();
            builder.Services.AddScoped<ICalculadorIndicadorFinanceiro, PatrimonioLiquidoIndicador>();
            builder.Services.AddScoped<ICalculadorIndicadorFinanceiro, PercentualPatrimonioAlvoIndicador>();
            builder.Services.AddScoped<IIndicadoresFinanceirosService, IndicadoresFinanceirosService>();
            builder.Services.AddScoped<ISaudeFinanceiraService, SaudeFinanceiraService>();
            builder.Services.AddScoped<IAnaliseFinanceiraAppService, AnaliseFinanceiraAppService>();
            builder.Services.AddScoped<IMfScoreCalculoAppService, MfScoreCalculoAppService>();

            builder.Services.AddScoped<IBemPatrimonialJobs, BemPatrimonialJobs>();
            builder.Services.AddScoped<IMfScoreJobs, MfScoreJobs>();

            builder.Services.AddHangfire(configuration =>
                configuration.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UseStorage(new MySqlStorage(
                        connectionString,
                        new MySqlStorageOptions
                        {
                            PrepareSchemaIfNecessary = true
                        })));

            builder.Services.AddHangfireServer();
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            app.MigrateDatabase();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.UseHangfireDashboard("/backgroundJobs");

            RecurringJob.AddOrUpdate<IBemPatrimonialJobs>(
                "atualizacao-anual-bens-patrimoniais",
                x => x.FilaJobs(),
                "0 0 1 1 *");

            RecurringJob.AddOrUpdate<IMfScoreJobs>(
                "historico-mensal-mf-score",
                x => x.GerarHistoricoMensalAsync(),
                "0 2 1 * *");

            app.Run();
        }
    }
}
