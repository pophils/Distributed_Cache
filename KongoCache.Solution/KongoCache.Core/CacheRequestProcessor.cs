using KongoCache.Core.DTOs;
using System;
using System.Collections.Generic; 
using System.Threading.Tasks;

namespace KongoCache.Core
{
    public class CacheRequestProcessor
    {
        CacheOpMetaData hashTableOpMetadata;

        readonly ICacheManager<string, HashSet<string>> _setCacheManager;
        readonly ICacheManager<string, List<string>> _arrayListCacheManager;
        readonly ICacheManager<string, SortedSet<string>> _sortedSetCacheManager;
        readonly ICacheManager<string, LinkedList<string>> _linkedListCacheManager;
        readonly ICacheManager<string, BinaryHeap<(int score, string value)>> _minHeapCacheManager;
        readonly ICacheManager<string, BinaryHeap<(int score, string value)>> _maxHeapCacheManager;

        Task hashTableProcessorTask;


        public void InitTextRequestProcessor(ICacheManager<string, string> _textCacheManager)
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
                                    _textCacheManager.AddReply(OpsResponseCode.SUCCESSFUL_ADDITION, textOpMetadata.ClientSessionId);
                                    _textCacheManager.EnqueueCompletedOps(textOpMetadata);
                                }
                                catch (OutOfMemoryException)
                                {
                                    _textCacheManager.AddReply(OpsResponseCode.MEMORYOVERFLOW, textOpMetadata.ClientSessionId);
                                }

                                break;

                            case OpType.GET:
                                _textCacheManager.AddReply(OpsResponseCode.SUCCESS + Appconstants.RESPONSE_SEPERATOR + _textCacheManager.LRUDatabase().Get(textOpMetadata.KongoKey),
                                    textOpMetadata.ClientSessionId);
                                break;

                            case OpType.DELETE:
                                _textCacheManager.LRUDatabase().Remove(textOpMetadata.KongoKey);
                                _textCacheManager.AddReply(OpsResponseCode.SUCCESSFUL_DELETION, textOpMetadata.ClientSessionId);
                                _textCacheManager.EnqueueCompletedOps(textOpMetadata); 
                                break;
                        }
                    }
                }
            });
        }

        public void InitashTableRequestProcessor(ICacheManager<string, Dictionary<string, string>> _hashMapCacheManager)
        {
            hashTableProcessorTask = Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    hashTableOpMetadata = _hashMapCacheManager.DequeueOps();

                    if (textOpMetadata != null)
                    {
                        switch (textOpMetadata.KongoOpType)
                        {
                            case OpType.ADD:
                                try
                                {
                                    _hashMapCacheManager.LRUDatabase().Insert(textOpMetadata.KongoKey, new Dictionary<string, string>() { 

                                    } );
                                    _hashMapCacheManager.AddReply(OpsResponseCode.SUCCESSFUL_ADDITION, textOpMetadata.ClientSessionId);
                                    _hashMapCacheManager.EnqueueCompletedOps(textOpMetadata);
                                }
                                catch (OutOfMemoryException)
                                {
                                    _hashMapCacheManager.AddReply(OpsResponseCode.MEMORYOVERFLOW, textOpMetadata.ClientSessionId);
                                }

                                break;

                            case OpType.GET:
                                _hashMapCacheManager.AddReply(OpsResponseCode.SUCCESS + Appconstants.RESPONSE_SEPERATOR + _hashMapCacheManager.LRUDatabase().Get(textOpMetadata.KongoKey),
                                    textOpMetadata.ClientSessionId);
                                break;

                            case OpType.DELETE:
                                _hashMapCacheManager.LRUDatabase().Remove(textOpMetadata.KongoKey);
                                _hashMapCacheManager.AddReply(OpsResponseCode.SUCCESSFUL_DELETION, textOpMetadata.ClientSessionId);
                                _hashMapCacheManager.EnqueueCompletedOps(textOpMetadata);
                                break;
                        }
                    }
                }
            });
        }
    }
}
