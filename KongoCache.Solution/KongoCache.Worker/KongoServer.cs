using KongoCache.Core;
using Microsoft.Extensions.Logging;
using NetCoreServer;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace KongoCache.Worker
{
    public class KongoServer : TcpServer
    {
        readonly ILogger<Worker> _logger;
        readonly ICacheManager<string, string> _textCacheManager;
        readonly ICacheManager<string, Dictionary<string, string>> _hashMapCacheManager;

        public KongoServer(IPAddress address, int port, ILogger<Worker> logger,
            ICacheManager<string, string> textCacheManager,
            ICacheManager<string, Dictionary<string, string>> hashMapCacheManager) : base(address, port)
        {
            _logger = logger;
            _textCacheManager = textCacheManager;
            _hashMapCacheManager = hashMapCacheManager;
        }

        protected override TcpSession CreateSession() { return new KongoSession(this, _logger, _textCacheManager, _hashMapCacheManager); }

        protected override void OnError(SocketError error)
        {
            _logger.LogError($"Kongo server caught an error with code {error}");
        }
    }

}
