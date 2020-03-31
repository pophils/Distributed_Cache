using KongoCache.Core.DTOs;
using KongoCache.Core.Gateway;
using System;
using System.Collections.Generic;
using System.Text;

namespace KongoCache.Core
{
    public interface ICacheManager<K, V>
    {
        void EnqueueOps(CacheOpMetaData<K, V> op);
        CacheOpMetaData<K, V> DequeueOps();
        void EnqueueCompletedOps(CacheOpMetaData<K, V> op);
        CacheOpMetaData<K, V> DequeueCompletedOps();
    }
}
