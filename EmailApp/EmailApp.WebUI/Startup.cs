using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using EmailApp.Business;
using EmailApp.Business.TypiCode;
using EmailApp.DataAccess;
using EmailApp.DataAccess.EfModel;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace EmailApp.WebUI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            string connectionStringName = Configuration["ConnectionString"];
            string connectionString = Configuration.GetConnectionString(connectionStringName);
            services.AddDbContext<EmailContext>(options =>
            {
                if (connectionStringName == "SqliteEmailDb")
                {
                    options.UseSqlite(connectionString);
                }
                else
                {
                    options.UseNpgsql(connectionString);
                }
            });

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<IAccountRepository, AccountRepository>();

            services.AddScoped<IInboxCleaner, InboxCleaner>();

            services.AddHttpContextAccessor();

            services.AddHttpClient<TypiCodeService>();

            services.AddHealthChecks()
                .AddDbContextCheck<EmailContext>();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "EmailApp.WebUI", Version = "v1" });
            });

            var origins = Configuration.GetSection("CorsOrigins")
                .GetChildren()
                .Select(x => x.Value)
                .ToArray();

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAngular", policy =>
                    policy.WithOrigins(origins)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "EmailApp.WebUI v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            // applies CORS policy to all action methods
            app.UseCors("AllowAngular");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health");
                endpoints.MapControllers();
            });
        }
    }
}
