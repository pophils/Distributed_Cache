using KongoCache.Core.DTOs;
using System;
using System.Threading.Tasks;

namespace KongoCache.Core.RequestProcessor
{
    public class TextRequestProcessor : IDisposable
    {
        Task textProcessorTask;
        CacheOpMetaData textOpMetadata;

        public void InitRequestProcessor(ICacheManager<string, string> _textCacheManager)
        {
            textProcessorTask = Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    textOpMetadata = _textCacheManager.DequeueOps();

                    if (textOpMetadata != null)
                    {
                        switch (textOpMetadata.KongoOpType)
                        {
                            case OpType.ADD:
                                try
                                {
                                    _textCacheManager.LRUDatabase().Insert(textOpMetadata.KongoKey, textOpMetadata.Value);
                                    _textCacheManager.AddReply(OpsResponseCode.SUCCESS + Appconstants.RESPONSE_SEPERATOR + OpType.ADD, textOpMetadata.ClientSessionId);
                                    _textCacheManager.EnqueueCompletedOps(textOpMetadata);
                                }
                                catch (OutOfMemoryException)
                                {
                                    _textCacheManager.AddReply(OpsResponseCode.MEMORYOVERFLOW + Appconstants.RESPONSE_SEPERATOR + OpType.ADD, textOpMetadata.ClientSessionId);
                                }

                                break;

                            case OpType.GET:
                                _textCacheManager.AddReply(OpsResponseCode.SUCCESS + Appconstants.RESPONSE_SEPERATOR + OpType.GET
                                    + _textCacheManager.LRUDatabase().Get(textOpMetadata.KongoKey),
                                    textOpMetadata.ClientSessionId);
                                break;

                            case OpType.DELETE:
                                _textCacheManager.LRUDatabase().Remove(textOpMetadata.KongoKey);
                                _textCacheManager.AddReply(OpsResponseCode.SUCCESS + OpType.DELETE, textOpMetadata.ClientSessionId);
                                _textCacheManager.EnqueueCompletedOps(textOpMetadata);
                                break;
                        }
                    }
                }
            });
        }


        public void Dispose()
        {
            textProcessorTask?.Dispose();
        }
    }
}
