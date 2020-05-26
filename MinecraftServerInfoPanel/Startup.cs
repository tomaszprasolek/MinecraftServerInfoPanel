using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MinecraftServerInfoPanel.BL;
using MinecraftServerInfoPanel.BL.EmailSender;
using MinecraftServerInfoPanel.BL.RecentActivityChecker;
using MinecraftServerInfoPanel.BL.RecentActivityEmailSender;
using MinecraftServerInfoPanel.Database;
using Serilog;

namespace MinecraftServerInfoPanel
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
            services.AddRazorPages();

            // Enable cookie authentication
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie();

            services.AddDbContextPool<MinecraftDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("MinecraftDb"));
            });

            services.AddSingleton<IConsoleDataDowloader, ConsoleDataDowloader>();
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddScoped<IRecentActivityEmailSender, RecentActivityEmailSender>();
            services.AddScoped<IRecentActivityChecker, RecentActivityChecker>();

            //services.AddHostedService<CheckConsoleBackgroundService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseSerilogRequestLogging();

            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
