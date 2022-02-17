using api.Mapping;
using api.Models;
using api.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Identity;
using System.Reflection;
using Microsoft.Extensions.FileProviders;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using SignalR.Providers;
using System;
using api.Extensions;

namespace api
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }

        public IConfiguration Configuration { get; }

        public IWebHostEnvironment _env { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            // var connectionString = Configuration.GetConnectionString("cs");
            // var MigrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            services.AddControllers()
                    .AddNewtonsoftJson(options =>
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            var server = Configuration["DbServer"] ?? "127.0.0.1";
            var port = Configuration["DbPort"] ?? "1433"; // Default SQL Server port
            var user = Configuration["DbUser"] ?? "SA"; // Warning do not use the SA account
            var password = Configuration["Password"] ?? "DockerTest123";
            var database = Configuration["Database"] ?? "kanban";

            var connectionString = $"Server={server},{port};Initial Catalog={database};User ID={user};Password={password}";

            Console.WriteLine(connectionString);

            // Add Db context as a service to our application
            services.AddDbContext<AppDbContext>(options => 
                options.UseSqlServer(connectionString));
            
            services.AddControllersWithViews();

            services.AddIdentity<User, UserRole>(options =>
                    {
                        if (_env.IsDevelopment())
                        {
                            options.SignIn.RequireConfirmedAccount = false;
                        }
                        else 
                        {
                            options.SignIn.RequireConfirmedAccount = true;
                        }
                    })
                    .AddEntityFrameworkStores<AppDbContext>()
                    .AddDefaultTokenProviders();

            // Simple example with dependency injection for a data provider.
            services.AddScoped<IProjectsRepository, ProjectsRepository>();
            services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            services.AddScoped<IDepartmentMemberRepository, DepartmentMemberRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IClaimsRepository, ClaimsRepository>();
            services.AddSingleton<IUserIdProvider, EmailBasedUserIdProvider>();

            // Add mapping for DTO
            services.AddAutoMapper(typeof(MappingProfile));

            services.AddCors(o => o.AddPolicy("CorsPolicy", builder =>
            {
                builder
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
                .WithOrigins("https://localhost:8080");
            }));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            DatabaseManagementService.MigrationInitialisation(app);

            if (_env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // app.UseHttpsRedirection();

            app.ConfigureCustomExceptionMiddleware();

            app.UseRouting();

            app.UseCors("CorsPolicy");

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
