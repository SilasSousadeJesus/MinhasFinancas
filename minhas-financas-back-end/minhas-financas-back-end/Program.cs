
using AspNetCore.Scalar;
using Microsoft.EntityFrameworkCore;
using MinhasFinancas.Application.Interfaces;
using MinhasFinancas.Application.Services;
using MinhasFinancas.Infra;
using Scalar.AspNetCore;

namespace minhas_financas_back_end
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

            // Add services to the container.


            //builder.Services.AddScoped<IAutenticacaoAppService, AutenticacaoAppService>();
            //builder.Services.AddTransient<CustomJwtBearerHandler>();


            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                /// SCALAR
                app.MapScalarApiReference();

                app.UseScalar(options =>
                {
                    options.UseTheme(Theme.Default);
                    options.RoutePrefix = "api-docs";
                });
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
