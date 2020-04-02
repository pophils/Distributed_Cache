using System;

namespace KongoCache.Core.DTOs
{
    public class CacheOpMetaData
    {
        public string key;
        public string Value;
        public CacheContentType cacheContentType;
        public OpType opType;
        public Guid sessionId;

    }
}