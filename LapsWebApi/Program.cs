using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace LapsWebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
               .ConfigureAppConfiguration((builderContext, config) =>
               {
                   // delete all default configuration providers
                   config.Sources.Clear();
                   config.AddJsonFile("vcap-local.json", optional: true);
               })
                .UseStartup<Startup>()
                .Build();
    }
}
