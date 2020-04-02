using KongoCache.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;

namespace KongoCache.Worker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<ICacheManager<string, string>, CacheManager<string, string>>();
                    services.AddSingleton<ICacheManager<string, Dictionary<string, string>>, CacheManager<string, Dictionary<string, string>>>();
                    services.AddSingleton<ICacheManager<string, HashSet<string>>, CacheManager<string, HashSet<string>>>();

                    services.AddHostedService<Worker>();
                });
    }
}
