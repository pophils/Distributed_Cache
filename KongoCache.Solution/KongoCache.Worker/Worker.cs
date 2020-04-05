using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using KongoCache.Core;
using KongoCache.Core.DTOs;
using KongoCache.Core.RequestProcessor;
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

        HashTableRequestProcessor _hashMapRequestProcessor;
        TextRequestProcessor _textRequestProcessor;

        readonly ICacheManager<string, string> _textCacheManager;
        readonly ICacheManager<string, Dictionary<string, string>> _hashMapCacheManager;
        readonly ICacheManager<string, HashSet<string>> _setCacheManager;
        readonly ICacheManager<string, List<string>> _arrayListCacheManager;
        readonly ICacheManager<string, SortedSet<string>> _sortedSetCacheManager;
        readonly ICacheManager<string, LinkedList<string>> _linkedListCacheManager;
        readonly ICacheManager<string, BinaryHeap<(int score, string value)>> _minHeapCacheManager;
        readonly ICacheManager<string, BinaryHeap<(int score, string value)>> _maxHeapCacheManager;



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

                InitRequestProcessors();

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
                DisposeRequestProcessors();
            } 
        }

        void StopKongoServer()
        {
            try
            {
                _logger.LogDebug("Kongo server stopping...");
                _kongoServer.Stop();
                _logger.LogDebug("Kongo server stopped...");

                DisposeRequestProcessors();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Kongo server could not be stopped due to {ex.Message}");
            }
        }

        void InitRequestProcessors()
        {
            if (_textRequestProcessor is null)
                _textRequestProcessor = new TextRequestProcessor();

            _textRequestProcessor.InitRequestProcessor(_textCacheManager);


            if (_hashMapRequestProcessor is null)
                _hashMapRequestProcessor = new HashTableRequestProcessor();

            _hashMapRequestProcessor.InitRequestProcessor(_hashMapCacheManager);
        }


        void DisposeRequestProcessors()
        {
            _textRequestProcessor?.Dispose();
            _hashMapRequestProcessor?.Dispose();

            _textRequestProcessor = default;
            _hashMapRequestProcessor = default;
        }


    }
}
