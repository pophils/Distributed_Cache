using KongoCache.Core;
using KongoCache.Core.DTOs;
using Microsoft.Extensions.Logging;
using NetCoreServer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KongoCache.Worker
{
    public class KongoSession : TcpSession
    {
        readonly ILogger<Worker> _logger;
        readonly ICacheManager<string, string> _textCacheManager;
        readonly ICacheManager<string, Dictionary<string, string>> _hashMapCacheManager;
        Task textReplySenderTask;
        Task hashReplySenderTask;
        CancellationTokenSource repliesSenderCancellationTokenSource;
        CancellationToken repliesSenderCancellationToken;
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

            InitRepliesSenderTasks();
        }     


        protected override void OnConnected()
        {
            _logger.LogInformation($"Kongo session with Id {Id} connected!");
            Send($"You are connected to Kongo Server with Session Id {Id}");

            s = new Stopwatch();
        }

        protected override void OnDisconnected()
        {
            _logger.LogInformation($"Kongo session with Id {Id} disconnected!");
            repliesSenderCancellationTokenSource.Cancel();
            DisposeRepliesSenderTasks();
        }

        Stopwatch s;

        protected override void OnReceived(byte[] buffer, long offset, long size)
        {
            s.Restart();

            string message = Encoding.UTF8.GetString(buffer, (int)offset, (int)size);

            if (string.IsNullOrEmpty(message))
            {
                Send(OpsResponseCode.INVALID_OPS);
                return;
            }

           // _logger.LogInformation($"Kongo session with Id {Id} received a message {message}");

            try
            {
                string[] requestContent = message.Split(" ", StringSplitOptions.None); // // HADD KONGOKEY HashKey HashValue

                if (requestContent.Length < 2) // at least optype kongo key
                {
                    Send(OpsResponseCode.INVALID_OPS);
                    s.Stop();
                    _logger.LogInformation($"{s.ElapsedMilliseconds} milli sec");
                    return;
                }

                string opType = requestContent[0].ToUpper();

                if (!opTypeContentTypeMap.TryGetValue(opType, out (CacheContentType contentType, OpType opType) operation))
                {
                    Send(OpsResponseCode.INVALID_OPS); 
                }

                CacheOpMetaData opMetaData = default;                 

                switch (operation.contentType)
                {
                    case CacheContentType.Text:

                        opMetaData = RequestParser.ParseTextRequest(requestContent, operation.opType);
                        opMetaData.ClientSessionId = Id; 
                        _textCacheManager.RequestProcessorBlock.Post(opMetaData);

                        break;

                    case CacheContentType.HashTable:
                        opMetaData = RequestParser.ParseHashMapRequest(requestContent, operation.opType);
                        opMetaData.ClientSessionId = Id;
                        _hashMapCacheManager.RequestProcessorBlock.Post(opMetaData); 

                        break;
                }

            }
            catch (Exception ex)
            {
               // _logger.LogError($"Kongo session with Id {Id} caught an error parsing request {ex.Message}");
                Send(OpsResponseCode.INVALID_OPS);
                s.Stop();
                _logger.LogInformation($"{s.ElapsedMilliseconds} milli sec");

            }

        }

        protected override void OnError(SocketError error)
        {
            _logger.LogError($"Kongo session with Id {Id} caught an error with code {error}");
        }

        void InitRepliesSenderTasks()
        {
            repliesSenderCancellationTokenSource = new CancellationTokenSource();
            repliesSenderCancellationToken = repliesSenderCancellationTokenSource.Token;

            textReplySenderTask = Task.Factory.StartNew(() =>
            {
                InitTextRequestResponseSender();

            }, repliesSenderCancellationToken);

            hashReplySenderTask = Task.Factory.StartNew(() =>
            {
                InitHashRequestResponseSender();
            }, repliesSenderCancellationToken);

        }
        void InitTextRequestResponseSender()
        {
            // keep polling for replies
            while (true)
            {
                if (repliesSenderCancellationToken.IsCancellationRequested)
                    break;

                if (_textCacheManager.TryGetReply(Id, out string reply))
                {
                    Send(reply);
                    s.Stop();
                    _logger.LogInformation($"{s.ElapsedMilliseconds} milli sec");

                   // _logger.LogInformation($"Text Reply sent");
                }
            }
        }
        void InitHashRequestResponseSender()
        {
            // keep polling for replies
            while (true)
            {
                if (repliesSenderCancellationToken.IsCancellationRequested)
                    break;

                if (_hashMapCacheManager.TryGetReply(Id, out string reply))
                {
                    Send(reply);
                    s.Stop();
                    _logger.LogInformation($"{s.ElapsedMilliseconds} milli sec");

                  //  _logger.LogInformation($"Hash Reply sent");

                }
            }
        }
        void DisposeRepliesSenderTasks()
        {
            while (true)
            {
                if (textReplySenderTask is null)
                    break;

                if (textReplySenderTask.IsCompleted)
                {
                    textReplySenderTask.Dispose();
                    break;
                }
            }

            while (true)
            {
                if (hashReplySenderTask is null)
                    break;

                if (hashReplySenderTask.IsCompleted)
                {
                    hashReplySenderTask.Dispose();
                    break;
                }
            }

            repliesSenderCancellationTokenSource.Dispose();

            _logger.LogInformation($"All Response sender task disposed!");
        }
    }
}