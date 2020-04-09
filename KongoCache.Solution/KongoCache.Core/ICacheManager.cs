using KongoCache.Core.DTOs;
using KongoCache.Core.Interface;
using System;
using System.Threading.Tasks.Dataflow;

namespace KongoCache.Core
{
    public interface ICacheManager<K, V>
    {
        //void EnqueueOps(CacheOpMetaData op);
        //CacheOpMetaData DequeueOps();
        void EnqueueCompletedOps(CacheOpMetaData op);
        CacheOpMetaData DequeueCompletedOps();
        void AddReply(string replyContent, Guid clientSessionId);
        bool TryGetReply(Guid clientSessionId, out string replyContent);
        bool RepliesEmpty(Guid clientSessionId);


        ILRUDatabase<K, V> LRUDatabase {
            get;      
        }

        ActionBlock<CacheOpMetaData> RequestProcessorBlock
        {

            get; set;
        }
    }
}
