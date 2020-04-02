using KongoCache.Core.DTOs;
using KongoCache.Core.Gateway;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace KongoCache.Core
{
    public class CacheManager<K, V> : ICacheManager<K, V>
    {
        ConcurrentQueue<CacheOpMetaData> _opsQueue;
        LRUCache<K, V> _lruRamGateway;
        Queue<CacheOpMetaData> _completedOpsQueue;
        IDictionary<Guid, string> resultMap;


        public void PutResult(string resultContent, Guid sessionId)
        {
            if (resultMap is null)
                resultMap = new Dictionary<Guid, string>();

            resultMap[sessionId] = resultContent; 
        }

        public bool TryGetResult(Guid sessionId, out string result)
        {
            result = default;

            if (resultMap is null)
                return false;

            if (resultMap.TryGetValue(sessionId, out result))
            {
                resultMap.Remove(sessionId);
                return true;
            }

            return false;
        }
        
        public void EnqueueOps(CacheOpMetaData op)
        {
            if (_opsQueue is null)
                _opsQueue = new ConcurrentQueue<CacheOpMetaData>();

            _opsQueue.Enqueue(op);
        }

        public CacheOpMetaData DequeueOps()
        {
            if (_opsQueue != null && !_opsQueue.IsEmpty)
            {
                if (_opsQueue.TryDequeue(out CacheOpMetaData op))
                {
                    return op;
                }
            }

            return default;
        }

        public LRUCache<K, V> LRURamGateway
        {
            get 
            {
                if (_lruRamGateway is null)
                    _lruRamGateway = new LRUCache<K, V>();

                return _lruRamGateway;            
            }
        }

        public void EnqueueCompletedOps(CacheOpMetaData op)
        {
            if (_completedOpsQueue is null)
                _completedOpsQueue = new Queue<CacheOpMetaData>();

            _completedOpsQueue.Enqueue(op);
        }

        public CacheOpMetaData DequeueCompletedOps()
        {
            if (_completedOpsQueue != null && _completedOpsQueue.Count > 0)
            {
                if (_completedOpsQueue.TryDequeue(out CacheOpMetaData op))
                {
                    return op;
                }
            }

            return default;
        }                     
    }
}
