using KongoCache.Core.DTOs;
using KongoCache.Core.Gateway;
using KongoCache.Core.Interface;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace KongoCache.Core
{
    public class CacheManager<K, V> : ICacheManager<K, V>
    {
        ConcurrentQueue<CacheOpMetaData> _opsQueue;
        ILRUDatabase<K, V> _database;
        Queue<CacheOpMetaData> _completedOpsQueue;
        IDictionary<Guid, LinkedList<string>> replyMap;

        public void AddReply(string replyContent, Guid clientSessionId)
        {
            if (replyMap is null)
                replyMap = new Dictionary<Guid, LinkedList<string>>();

            if (replyMap[clientSessionId] is null)
                replyMap[clientSessionId] = new LinkedList<string>();

            replyMap[clientSessionId].AddLast(replyContent); 
        }

        public bool TryGetReply(Guid clientSessionId, out string reply)
        {
            reply = default;

            if (replyMap is null)
                return false;

            if (replyMap.TryGetValue(clientSessionId, out LinkedList<string> replies))
            {
                reply = replies.First.Value;
                replies.RemoveFirst(); 
                return true;
            }

            return false;
        }

        public bool RepliesEmpty(Guid clientSessionId)
        { 
            if (replyMap is null)
                return false;

            if (replyMap.TryGetValue(clientSessionId, out LinkedList<string> replies))
            {
                return replies.Count == 0;
            }

            return true;
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

        public ILRUDatabase<K, V> LRUDatabase()
        {
            if (_database is null)
                _database = new LRUDatabase<K, V>();

            return _database;
        }
    }
}