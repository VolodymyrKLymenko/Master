using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GeneticlAlgorithmCalculation
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            var logger = host.Services.GetRequiredService<ILogger<Program>>();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<ApplicationContext>();

                DataGenerator.Initialize(services);
            }

            try
            {
                logger.LogInformation("Stating hosting identity service");

                host.Run();

                logger.LogInformation("Finish hosting identity service");
                return 0;
            }
            catch (Exception e)
            {
                logger.LogError($"Cannot start hosting identity service, {e.Message}");
                return -1;
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
