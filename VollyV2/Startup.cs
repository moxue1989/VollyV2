using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VollyV2.Chat;
using VollyV2.Data;
using VollyV2.Models;
using VollyV2.Services;

namespace VollyV2
{
    public class Startup
    {
        private static readonly string ConnectionString = Environment.GetEnvironmentVariable("connection_string");
        private static readonly string AutoMigrate = Environment.GetEnvironmentVariable("auto_migrate");
        private IHostingEnvironment CurrentEnvironment { get; }
        public Startup(IHostingEnvironment env, IConfiguration configuration)
        {
            Configuration = configuration;
            CurrentEnvironment = env;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(GetConnectionString());
            });

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
                    {
                        options.Password.RequireDigit = false;
                        options.Password.RequiredLength = 8;
                        options.Password.RequireNonAlphanumeric = false;
                        options.Password.RequireUppercase = false;
                        options.Password.RequireLowercase = false;
                        options.User.RequireUniqueEmail = true;
                    })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();

            services.AddSingleton<IImageManager, ImageManager>();

            services.AddMemoryCache();

            services.AddMvc();

            services.AddSignalR();

            services.AddScoped<IAuthorizationHandler, OpportunityAuthorizationHandler>();

            services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
            {
                builder.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod();
            }));
        }

        private string GetConnectionString()
        {
            if (CurrentEnvironment.EnvironmentName == "Development")
            {
                return Configuration.GetConnectionString("Develop");
            }
            return ConnectionString;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (AutoMigrate == "true")
            {
                using (var serviceScope =
                    app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
                    context.Database.Migrate();
                }
            }

            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");                
            }

            var options = new RewriteOptions()
                .AddRedirectToHttpsPermanent();

            app.UseRewriter(options);

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseCors("MyPolicy");

            app.UseSignalR(routes =>
            {
                routes.MapHub<ChatHub>("/chathub");
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
