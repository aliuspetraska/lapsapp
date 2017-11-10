using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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
                services.AddDbContext<ParkingSharingContext>(options => options.UseMySql(
                    GetConnectionString(databaseUri)));

                services.AddDataProtection()
                    .SetDefaultKeyLifetime(TimeSpan.FromDays(365));
            }

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseMvc();
        }
    }
}
