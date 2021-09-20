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
            string connectionString = Configuration.GetConnectionString("SqliteEmailDb");
            services.AddDbContext<EmailContext>(options =>
            {
                options.UseSqlite(connectionString);
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

            services.AddCors(options =>
            {
                options.AddPolicy("AllowNgServe", policy =>
                    policy.WithOrigins("http://localhost:4200",
                                       "https://2107-escalona-email-app-ui.azurewebsites.net")
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
            app.UseCors("AllowNgServe");

            app.UseAuthentication();
            app.UseAuthorization();

            // if this is a new user, add him
            app.Use(async (context, next) =>
            {
                // this could be a separate middleware class
                // (more unit testable, could use constructor injection)
                if (context.User.Identity.IsAuthenticated)
                {

                    var userAddress = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
                    var uow = context.RequestServices.GetRequiredService<IUnitOfWork>();
                    if (await uow.AccountRepository.AddIfNotExistsAsync(userAddress))
                    {
                        await uow.SaveAsync();
                    }
                }
                await next();
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health");
                endpoints.MapControllers();
            });
        }
    }
}
