
using AspNetCore.Scalar;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MinhasFinancas.API.Extensions;
using MinhasFinancas.Application.Configurations;
using MinhasFinancas.Application.Interfaces;
using MinhasFinancas.Application.Services;
using MinhasFinancas.CrossCutting.Reports;
using MinhasFinancas.Domain.Entities;
using MinhasFinancas.Domain.Services.AnaliseFinanceira;
using MinhasFinancas.Domain.Services.AnaliseFinanceira.Indicadores;
using MinhasFinancas.Infra;
using MinhasFinancas.Infra.Data.config.configMigrate;
using MinhasFinancas.Infra.Data.Interfaces;
using MinhasFinancas.Infra.Data.Repositories;
using MinhasFinancas.Infra.IA;
using MinhasFinancas.Infra.IA.Avaliadores;
using MinhasFinancas.Infra.IA.Construtores;
using MinhasFinancas.Infra.IA.Especialistas;
using MinhasFinancas.Infra.IA.Especialistas.Compromissos;
using MinhasFinancas.Infra.IA.Especialistas.Dividas;
using MinhasFinancas.Infra.IA.Especialistas.FluxoCaixa;
using MinhasFinancas.Infra.IA.Especialistas.Interfaces;
using MinhasFinancas.Infra.IA.Especialistas.Patrimonio;
using MinhasFinancas.Infra.IA.Especialistas.PlanoEstrategico;
using MinhasFinancas.Infra.IA.Especialistas.ReservaEmergencia;
using MinhasFinancas.Infra.IA.Interpretadores;
using MinhasFinancas.Infra.IA.Modelos;
using MinhasFinancas.Infra.IA.Provedores;
using MinhasFinancas.Infra.Reports.Excel;
using Scalar.AspNetCore;
using System.Text.Json.Serialization;

namespace minhas_financas_back_end
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // criar logicar para desativar os lançamentos fixos e parcelados caso o cara tente realizar o lancamento
            // criar logica para ter registros de saque e deposito (movimentação)
            // criar sistema de metas/sonhos/objetivos estilo caixinha do nubank;
            // continua criando logica para passivo e relacionar com ativos no relatorios

            var builder = WebApplication.CreateBuilder(args);

            // Configuração do AutoMapper
            builder.Services.AddAutoMapper(typeof(MappingProfile));

            var connectionString = builder.Configuration.GetConnectionString("ConnectionMinhasFinancas");
            builder.Services.Configure<ConfiguracaoOpenAI>(builder.Configuration.GetSection("OpenAI"));
            builder.Services.AddHttpClient<IProvedorIA, OpenAIProvider>((serviceProvider, client) =>
            {
                var configuracao = serviceProvider
                    .GetRequiredService<Microsoft.Extensions.Options.IOptions<ConfiguracaoOpenAI>>()
                    .Value;

                var baseUrl = string.IsNullOrWhiteSpace(configuracao.BaseUrl)
                    ? "https://api.openai.com/v1/"
                    : configuracao.BaseUrl;

                client.BaseAddress = new Uri(baseUrl);
                client.Timeout = Timeout.InfiniteTimeSpan;
            });

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            });

            // Add services to the container.

            builder.Services.AddIdentity<Usuario, IdentityRole>()
                                .AddEntityFrameworkStores<ApplicationDbContext>()
                                .AddDefaultTokenProviders();

            builder.Services.AddScoped<IAutenticacaoAppService, AutenticacaoAppService>();
            builder.Services.AddScoped<IUsuarioAppService, UsuarioAppService>();
            builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();

            builder.Services.AddScoped<IContaAppService, ContaAppService>();
            builder.Services.AddScoped<IContaRepository, ContaRepository>();


            builder.Services.AddScoped<ICartaoAppService, CartaoAppService>();
            builder.Services.AddScoped<ICartaoRepository, CartaoRepository>();

            builder.Services.AddScoped<ICategoriaAppService, CategoriaAppService>();
            builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();

            builder.Services.AddScoped<ILancamentoAppService, LancamentoAppService>();
            builder.Services.AddScoped<ILancamentoRepository, LancamentoRepository>();
            builder.Services.AddScoped<ICalculadorIndicadorFinanceiro, EconomiaMensalIndicador>();
            builder.Services.AddScoped<ICalculadorIndicadorFinanceiro, PercentualEconomiaIndicador>();
            builder.Services.AddScoped<ICalculadorIndicadorFinanceiro, ReservaEmergenciaAtualIndicador>();
            builder.Services.AddScoped<ICalculadorIndicadorFinanceiro, ReservaEmergenciaIdealIndicador>();
            builder.Services.AddScoped<ICalculadorIndicadorFinanceiro, ComprometimentoRendaIndicador>();
            builder.Services.AddScoped<ICalculadorIndicadorFinanceiro, EndividamentoIndicador>();
            builder.Services.AddScoped<ICalculadorIndicadorFinanceiro, PatrimonioLiquidoIndicador>();
            builder.Services.AddScoped<ICalculadorIndicadorFinanceiro, PercentualPatrimonioAlvoIndicador>();
            builder.Services.AddScoped<IIndicadoresFinanceirosService, IndicadoresFinanceirosService>();
            builder.Services.AddScoped<ISaudeFinanceiraService, SaudeFinanceiraService>();
            builder.Services.AddScoped<IInsightsFinanceirosService, InsightsFinanceirosService>();
            builder.Services.AddScoped<IResumoFinanceiroIAService, ResumoFinanceiroIAService>();
            builder.Services.AddScoped<IAnaliseFinanceiraAppService, AnaliseFinanceiraAppService>();
            builder.Services.AddScoped<IAssistenteFinanceiroAppService, AssistenteFinanceiroAppService>();
            builder.Services.AddScoped<IAnaliseFinanceiraHistoricaAppService, AnaliseFinanceiraHistoricaAppService>();
            builder.Services.AddScoped<ISaudeFinanceiraAppService, SaudeFinanceiraAppService>();
            builder.Services.AddScoped<IInteligenciaFinanceiraAppService, InteligenciaFinanceiraAppService>();
            builder.Services.AddScoped<InterpretadorMemoriaFinanceira>();
            builder.Services.AddScoped<InterpretadorDecisaoFinanceira>();
            builder.Services.AddScoped<InterpretadorEstrategico>();
            builder.Services.AddScoped<AvaliadorConsistenciaEstrategica>();
            builder.Services.AddScoped<IEspecialistaFinanceiro, EspecialistaDividas>();
            builder.Services.AddScoped<IEspecialistaFinanceiro, EspecialistaReservaEmergencia>();
            builder.Services.AddScoped<IEspecialistaFinanceiro, EspecialistaFluxoCaixa>();
            builder.Services.AddScoped<IEspecialistaFinanceiro, EspecialistaPatrimonio>();
            builder.Services.AddScoped<IEspecialistaFinanceiro, EspecialistaPlanoEstrategico>();
            builder.Services.AddScoped<IEspecialistaFinanceiro, EspecialistaCompromissos>();
            builder.Services.AddScoped<IEspecialistasFinanceirosService, EspecialistasFinanceirosService>();
            builder.Services.AddScoped<ConstrutorContextoIA>();
            builder.Services.AddScoped<ConstrutorPromptIA>();
            builder.Services.AddScoped<AssistenteFinanceiroService>();
            builder.Services.AddScoped<IAnaliseFinanceiraHistoricaRepository, AnaliseFinanceiraHistoricaRepository>();
            builder.Services.AddScoped<ExcelWorkbookFactory>();
            builder.Services.AddScoped<ExcelStyleHelper>();
            builder.Services.AddScoped<IExcelReport<LancamentosExcelReportData>, LancamentosExcelReport>();
            builder.Services.AddScoped<IExcelReport<FluxoCaixaSimplesExcelReportData>, FluxoCaixaSimplesExcelReport>();

            builder.Services.AddScoped<IDashboardAppService, DashboardAppService>();
            builder.Services.AddScoped<IRelatoriosAppService, RelatoriosAppService>();

            builder.Services.AddScoped<IBemPatrimonialAppService, BemPatrimonialAppService>();
            builder.Services.AddScoped<IBemMaterialRepository, BemMaterialRepository>();

            builder.Services.AddScoped<IPassivoAppService, PassivoAppService>();
            builder.Services.AddScoped<IPassivoRepository, PassivoRepository>();
            builder.Services.AddScoped<IPatrimonioAppService, PatrimonioAppService>();
            builder.Services.AddScoped<IPatrimonioRepository, PatrimonioRepository>();

            builder.Services.AddScoped<IMetaAppService, MetaAppService>();
            builder.Services.AddScoped<IMetaRepository, MetaRepository>();
            builder.Services.AddScoped<IPerfilFinanceiroAppService, PerfilFinanceiroAppService>();
            builder.Services.AddScoped<IPerfilFinanceiroRepository, PerfilFinanceiroRepository>();
            builder.Services.AddScoped<IPlanoEstrategicoFinanceiroAppService, PlanoEstrategicoFinanceiroAppService>();
            builder.Services.AddScoped<IPlanoEstrategicoFinanceiroRepository, PlanoEstrategicoFinanceiroRepository>();
            builder.Services.AddScoped<ICompromissoFinanceiroAppService, CompromissoFinanceiroAppService>();
            builder.Services.AddScoped<ICompromissoFinanceiroRepository, CompromissoFinanceiroRepository>();

            builder.Services.AddScoped<ISorteiosAppService, SorteiosAppService>();

            builder.Services.AddScoped<IPotencialCompraImovelAppService, PotencialCompraImovelAppService>();
            builder.Services.AddScoped<IProjecaoAppService, ProjecaoAppService>();
            builder.Services.AddScoped<IProjecaoRepository, ProjecaoRepository>();
            builder.Services.AddScoped<ISimulacaoFinanceiraAppService, SimulacaoFinanceiraAppService>();
            builder.Services.AddScoped<ISimulacaoFinanceiraRepository, SimulacaoFinanceiraRepository>();
            builder.Services.AddScoped<SimulacaoFinanceiraEngine>();

            // Add authentication services
            AuthenticationSetup.AddAuthentication(builder.Services, builder.Configuration);

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("Frontend", policy =>
                {
                    policy
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials()
                        .WithOrigins(
                            "http://localhost:3000",
                            "http://127.0.0.1:3000"
                        );
                });
            });

            builder.Services
                .AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "API - Minhas Finanças", Version = "v1" });

                // Adicione as configurações do JWT aqui
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Insira o token JWT desta maneira: Bearer {seu_token}",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] { }
                }
            });
            });

            var app = builder.Build();

            app.MigrateDatabase();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                //swagger
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Minhas Finanças - V1");
                    c.RoutePrefix = "swagger";
                });

                /// SCALAR
                app.MapScalarApiReference();

                app.UseScalar(options =>
                {
                    options.UseTheme(Theme.Default);
                    options.RoutePrefix = "api-docs";
                });
            }

            if (!app.Environment.IsDevelopment())
            {
                app.UseHttpsRedirection();
            }

            app.UseCors("Frontend");
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}

