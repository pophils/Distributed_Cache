using KongoCache.Core;
using KongoCache.Core.DTOs;
using Microsoft.Extensions.Logging;
using NetCoreServer;
using Newtonsoft.Json; 
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace KongoCache.Worker
{
    public class KongoSession : TcpSession
    {
        readonly ILogger<Worker> _logger; 
        readonly ICacheManager<string, string> _textCacheManager;
        readonly ICacheManager<string, Dictionary<string, string>> _hashMapCacheManager;
        readonly ICacheManager<string, HashSet<string>> _setCacheManager;

        public KongoSession(TcpServer server, ILogger<Worker> logger,
             ICacheManager<string, string> textCacheManager,
            ICacheManager<string, Dictionary<string, string>> hashMapCacheManager,
            ICacheManager<string, HashSet<string>> setCacheManager) : base(server) {
            _logger = logger;
            _textCacheManager = textCacheManager;
            _hashMapCacheManager = hashMapCacheManager;
            _setCacheManager = setCacheManager;
        }

        protected override void OnConnected()
        {
            _logger.LogDebug($"Kongo session with Id {Id} connected!");             
            SendAsync($"You are connected to Kongo Server with Session Id {Id}");
        }

        protected override void OnDisconnected()
        { 
            _logger.LogDebug($"Kongo session with Id {Id} disconnected!");
        }

        protected override void OnReceived(byte[] buffer, long offset, long size)
        {
            string message = Encoding.UTF8.GetString(buffer, (int)offset, (int)size); 
            _logger.LogDebug($"Kongo session with Id {Id} received a message {message}");

            CacheOpMetaData ops = JsonConvert.DeserializeObject<CacheOpMetaData>(message);
            string result;
            switch (ops.cacheContentType)
            {
                case CacheContentType.Text:
                    _textCacheManager.EnqueueOps(ops);

                    while (!_textCacheManager.TryGetResult(Id, out result))
                    {}
                    SendAsync(result);
                    
                    break;
                case CacheContentType.HashTable:
                    _hashMapCacheManager.EnqueueOps(ops);

                    while (!_hashMapCacheManager.TryGetResult(Id, out result))
                    { }
                    SendAsync(result);

                    break;
                case CacheContentType.Set:
                    _setCacheManager.EnqueueOps(ops);

                    while (!_setCacheManager.TryGetResult(Id, out result))
                    { }
                    SendAsync(result);

                    break;
            }

            Disconnect(); 
        }

        protected override void OnError(SocketError error)
        { 
            _logger.LogError($"Kongo session with Id {Id} caught an error with code {error}");

        } 
    }
}
