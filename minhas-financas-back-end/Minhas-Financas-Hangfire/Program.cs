
using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MinhasFinancas.Domain.Entities;
using MinhasFinancas.Infra;

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
                options.UseSqlServer(connectionString);
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
                              .UseSqlServerStorage(connectionString));
            //Server
            builder.Services.AddHangfireServer();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

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

            //RecurringJob.AddOrUpdate<IBackgroundJobsGestaoAlimentarAppService>(x => x.FilaJobs(), "0 0 * * *");

            app.Run();
        }
    }
}
