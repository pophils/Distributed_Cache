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
                    services.AddSingleton<ICacheManager<string, List<string>>, CacheManager<string, List<string>>>();
                    services.AddSingleton<ICacheManager<string, SortedSet<string>>, CacheManager<string, SortedSet<string>>>();
                    services.AddSingleton<ICacheManager<string, LinkedList<string>>, CacheManager<string, LinkedList<string>>>();

                    // Transient caause we want to maintain min and max heap
                    services.AddTransient<ICacheManager<string, BinaryHeap<(int score, string value)>>, CacheManager<string, BinaryHeap<(int score, string value)>>>();


                    services.AddHostedService<Worker>();
                });
    }
}
