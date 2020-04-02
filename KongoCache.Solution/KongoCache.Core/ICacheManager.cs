using KongoCache.Core.DTOs;
using KongoCache.Core.Gateway;
using System;
using System.Collections.Generic;
using System.Text;

namespace KongoCache.Core
{
    public interface ICacheManager<K, V>
    {
        void EnqueueOps(CacheOpMetaData op);
        CacheOpMetaData DequeueOps();
        void EnqueueCompletedOps(CacheOpMetaData op);
        CacheOpMetaData DequeueCompletedOps();
        void PutResult(string resultContent, Guid sessionId);
        bool TryGetResult(Guid sessionId, out string result);
    }
}
