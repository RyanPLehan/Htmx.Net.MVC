using ContosoUniversity.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;

namespace ContosoUniversity
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // By default, already addes the following logging providers
            // builder.Logging.AddConsole();
            // builder.Logging.AddDebug();

            // Add services to the container.
            ConfigureServices(builder.Services, builder.Configuration);

            // Build and configure Webapplication
            var app = builder.Build();
            ConfigureApplication(app);

            // Create Database
            //CreateDbIfNotExists(app);     // Moved to ConfigureServices

            app.Run();
        }


        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // Configure data repository / infrastructure
            services.AddInfrastructure(configuration);

            services.AddDatabaseDeveloperPageExceptionFilter();

            // See the following for differences between AddMvc, AddControllers, AddControllersWithViews, and AddRazorPages
            // https://dotnettutorials.net/lesson/difference-between-addmvc-and-addmvccore-method/
            services.AddControllersWithViews();
        }

        private static void ConfigureApplication(WebApplication app)
        {
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}