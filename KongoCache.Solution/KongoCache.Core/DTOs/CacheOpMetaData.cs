using System;

namespace KongoCache.Core.DTOs
{
    public class CacheOpMetaData
    {
        public string KongoKey;
        public string HashKey;
        public string Value;
        public CacheContentType KongoContentType;
        public OpType KongoOpType;
        public Guid ClientSessionId;
        public TimeSpan KeepAliveSpan;
    }
}