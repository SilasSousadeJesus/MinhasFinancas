
using Hangfire;
using Hangfire.MySql;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Minhas_Financas_Hangfire.Interfaces;
using MinhasFinancas.Domain.Entities;
using MinhasFinancas.Infra;
using MinhasFinancas.Infra.Data.config.configMigrate;

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

            // Add services to the container.
            builder.Services.AddHttpClient();

            // HangFire
            //Client
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
            //Server
            builder.Services.AddHangfireServer();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            app.MigrateDatabase();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.UseHangfireDashboard("/backgroundJobs");

            //0 0 1 1 * significa que a tarefa será executada à meia-noite, no dia 1º de janeiro, independentemente do dia da semana.
            RecurringJob.AddOrUpdate<IBemPatrimonialJobs>(x => x.FilaJobs(), "0 0 1 1 *");

            app.Run();
        }
    }
}


