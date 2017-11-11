using System;
using System.Linq;
using LapsWebApi.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace LapsWebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            var vcapServices = Environment.GetEnvironmentVariable("VCAP_SERVICES");

            if (vcapServices != null)
            {
                dynamic json = JsonConvert.DeserializeObject(vcapServices);

                if (json["compose-for-mysql"] != null)
                {
                    try
                    {
                        Configuration["compose-for-mysql:0:credentials:uri"] = json["compose-for-mysql"][0].credentials.uri;
                    }
                    catch
                    {
                        Console.WriteLine("Failed to read Compose for MySQL uri, ignore this and continue without a database.");
                    }
                }
            }
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var databaseUri = Configuration["compose-for-mysql:0:credentials:uri"];

            if (!string.IsNullOrEmpty(databaseUri))
            {
                services.AddDbContext<LapsDbContext>(options => options.UseMySql(
                    GetConnectionString(databaseUri)));

                services.AddDataProtection()
                    .SetDefaultKeyLifetime(TimeSpan.FromDays(365));
            }

            services.AddMvc();
        }

        static string GetConnectionString(string databaseUri)
        {
            string connectionString;

            try
            {
                var username = databaseUri.Split('/')[2].Split(':')[0];
                var password = databaseUri.Split(':')[2].Split('@')[0];
                var portSplit = databaseUri.Split(':');
                var port = portSplit.Length == 4 ? portSplit[3].Split('/')[0] : null;
                var hostSplit = databaseUri.Split('@')[1];
                var hostname = port == null ? hostSplit.Split('/')[0] : hostSplit.Split(':')[0];
                var databaseSplit = databaseUri.Split('/');
                var database = databaseSplit.Length == 4 ? databaseSplit[3] : null;
                var optionsSplit = database?.Split('?');
                database = optionsSplit.First();
                port = port ?? "3306"; // if port is null, use 3306
                connectionString = $"Server={hostname};uid={username};pwd={password};Port={port};Database={database};SSL Mode=Required;";
            }
            catch (IndexOutOfRangeException ex)
            {
                throw new FormatException("Invalid database uri format", ex);
            }

            return connectionString;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/static-files
            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseMvc();
        }
    }

    public class LapsDbContext : DbContext
    {
        public DbSet<Track> Tracks { get; set; }

        // http://www.mithunvp.com/aspnet-core-web-api-entity-framework-core/

        // https://docs.microsoft.com/en-us/aspnet/core/data/ef-mvc/migrations

        // dotnet ef migrations remove
        // dotnet ef migrations add LapsDb
        // dotnet ef database update

        public LapsDbContext(DbContextOptions options) : base(options) { }
        public LapsDbContext() { }
    }
}
