using KongoCache.Core;
using KongoCache.Core.DTOs;
using Microsoft.Extensions.Logging;
using NetCoreServer;
using System;
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

        static readonly IDictionary<string, (CacheContentType, OpType)> opTypeContentTypeMap = new Dictionary<string, (CacheContentType, OpType)>()
        {
            { "ADD", (CacheContentType.Text, OpType.ADD) },
            { "GET", (CacheContentType.Text, OpType.GET) },
            { "REM", (CacheContentType.Text, OpType.REMOVE) },
            { "HGET", (CacheContentType.HashTable, OpType.HGET) },
            { "HADD", (CacheContentType.HashTable, OpType.HADD) },
            { "HREM", (CacheContentType.HashTable, OpType.HREMOVE) },
            { "HGETALL", (CacheContentType.HashTable, OpType.HGETALL) },
            { "HREMKEY", (CacheContentType.HashTable, OpType.HREMOVEKEY) }
        };

        public KongoSession(TcpServer server, ILogger<Worker> logger,
             ICacheManager<string, string> textCacheManager,
            ICacheManager<string, Dictionary<string, string>> hashMapCacheManager) : base(server)
        {
            _logger = logger;
            _textCacheManager = textCacheManager;
            _hashMapCacheManager = hashMapCacheManager;

            _logger.LogInformation($"In Kongo session with Id");

        }

        protected override void OnConnected()
        {
            _logger.LogInformation($"Kongo session with Id {Id} connected!");
            SendAsync($"You are connected to Kongo Server with Session Id {Id}");
        }

        protected override void OnDisconnected()
        {
            _logger.LogInformation($"Kongo session with Id {Id} disconnected!");
        }

        protected override void OnReceived(byte[] buffer, long offset, long size)
        {
            string message = Encoding.UTF8.GetString(buffer, (int)offset, (int)size);

            if (string.IsNullOrEmpty(message))
            {
                SendAsync("Invalid request");
                Disconnect();
            }

            _logger.LogInformation($"Kongo session with Id {Id} received a message {message}");

            try
            {
                string[] requestContent = message.Split(" ", StringSplitOptions.RemoveEmptyEntries); // // HADD KONGOKEY HashKey HashValue

                if (requestContent.Length < 2) // at least optype kongo key
                {
                    SendAsync("Invalid operation");
                    Disconnect();
                }

                string opType = requestContent[0].ToUpper();

                if (!opTypeContentTypeMap.TryGetValue(opType, out (CacheContentType contentType, OpType opType) operation))
                {
                    SendAsync("Invalid operation");
                    Disconnect();
                }

                CacheOpMetaData opMetaData = default;

                _logger.LogInformation("Enter switch for opMetaData");

                switch (operation.contentType)
                {
                    case CacheContentType.Text:

                        opMetaData = RequestParser.ParseTextRequest(requestContent, operation.opType);
                        opMetaData.ClientSessionId = Id; 

                        _textCacheManager.EnqueueOps(opMetaData); 

                        // keep checking for replies
                        while (true)
                        {
                            if (_textCacheManager.TryGetReply(Id, out string reply))
                            {
                                SendAsync(reply);
                                break;
                            }
                        }

                        break;

                    case CacheContentType.HashTable:
                        opMetaData = RequestParser.ParseHashMapRequest(requestContent, operation.opType);
                        _logger.LogInformation("RequestParser.ParseTextRequest for opMetaData");

                        opMetaData.ClientSessionId = Id;
                        _logger.LogInformation("opMetaData.ClientSessionId  for opMetaData");


                        if (_hashMapCacheManager is null)
                            _logger.LogInformation("_hashMapCacheManager is null for opMetaData");
                        else
                            _logger.LogInformation("_hashMapCacheManager is not null for opMetaData");


                        _hashMapCacheManager.EnqueueOps(opMetaData);

                        _logger.LogInformation("_textCacheManager opMetaData HASH SET ENQUED SUCCESSFULLY for opMetaData");

                        while (true)
                        {
                            if (_hashMapCacheManager.TryGetReply(Id, out string reply))
                            {
                                SendAsync(reply);
                                _logger.LogInformation("SendAsync(reply) HASH SET; for opMetaData");

                                break;

                            }
                        }

                        break;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Kongo session with Id {Id} caught an error parsing request {ex.Message}");
                SendAsync("Invalid operation");
                Disconnect();
            }

        }

        protected override void OnError(SocketError error)
        {
            _logger.LogError($"Kongo session with Id {Id} caught an error with code {error}");
        }
    }
}
