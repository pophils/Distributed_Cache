using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using KongoCache.Core;
using Microsoft.Extensions.DependencyInjection; 
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace KongoCache.Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger; 
        private readonly IServiceScopeFactory _serviceScopeFactory;
        KongoServer _kongoServer;
        private bool serverRunning;

        readonly ICacheManager<string, string> _textCacheManager;
        readonly ICacheManager<string, Dictionary<string, string>> _hashMapCacheManager;
        readonly ICacheManager<string, HashSet<string>> _setCacheManager;

        public Worker(ILogger<Worker> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;

            using var scope = _serviceScopeFactory.CreateScope();

            _textCacheManager = scope.ServiceProvider.GetRequiredService<ICacheManager<string, string>>();
            _hashMapCacheManager = scope.ServiceProvider.GetRequiredService<ICacheManager<string, Dictionary<string, string>>>();
            _setCacheManager = scope.ServiceProvider.GetRequiredService<ICacheManager<string, HashSet<string>>>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                if (!serverRunning)
                    StartKongoServer();

                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }

            if (stoppingToken.IsCancellationRequested)
            {
                StopKongoServer();
            }
        }


        void StartKongoServer()
        {
            try
            {                 
                serverRunning = true;
                int port = 65332;

                _logger.LogDebug($"Init Kongo server with port: {port}"); 
                _kongoServer = new KongoServer(IPAddress.Any, port, _logger,
                    _textCacheManager, _hashMapCacheManager, _setCacheManager);
                 
                _logger.LogDebug("Kongo server starting...");
                _kongoServer.Start();
                _logger.LogDebug("Kongo server started...");
            }
            catch(Exception ex)
            {
                _logger.LogError($"Kongo server could not be started due to {ex.Message}");
                serverRunning = false;
            } 
        }

        void StopKongoServer()
        {
            try
            {
                _logger.LogDebug("Kongo server stopping...");
                _kongoServer.Stop();
                _logger.LogDebug("Kongo server stopped...");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Kongo server could not be stopped due to {ex.Message}");
            }
        }

       
    }
}
