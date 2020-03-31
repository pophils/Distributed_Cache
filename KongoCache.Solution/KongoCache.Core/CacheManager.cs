using KongoCache.Core.DTOs;
using KongoCache.Core.Gateway;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace KongoCache.Core
{
    public class CacheManager<K, V> : ICacheManager<K, V>
    {
        ConcurrentQueue<CacheOpMetaData<K, V>> _opsQueue;
        LRUCache<K, V> _lruRamGateway;
        Queue<CacheOpMetaData<K, V>> _completedOpsQueue;

        public void EnqueueOps(CacheOpMetaData<K, V> op)
        {
            if (_opsQueue is null)
                _opsQueue = new ConcurrentQueue<CacheOpMetaData<K, V>>();

            _opsQueue.Enqueue(op);
        }

        public CacheOpMetaData<K, V> DequeueOps()
        {
            if (_opsQueue != null && !_opsQueue.IsEmpty)
            {
                if (_opsQueue.TryDequeue(out CacheOpMetaData<K, V> op))
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

        public void EnqueueCompletedOps(CacheOpMetaData<K, V> op)
        {
            if (_completedOpsQueue is null)
                _completedOpsQueue = new Queue<CacheOpMetaData<K, V>>();

            _completedOpsQueue.Enqueue(op);
        }

        public CacheOpMetaData<K, V> DequeueCompletedOps()
        {
            if (_completedOpsQueue != null && _completedOpsQueue.Count > 0)
            {
                if (_completedOpsQueue.TryDequeue(out CacheOpMetaData<K, V> op))
                {
                    return op;
                }
            }

            return default;
        }                     
    }
}
