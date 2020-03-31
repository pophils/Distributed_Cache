namespace KongoCache.Core.DTOs
{
    public class CacheOpMetaData<K, V>
    {
        public K key;
        public V Value;
        public OpType OpType;
    }
}