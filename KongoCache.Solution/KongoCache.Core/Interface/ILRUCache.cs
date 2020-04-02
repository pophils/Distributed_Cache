namespace KongoCache.Core.Interface
{
    public interface ILRUCache<K, V> 
    {
        public void Insert(K key, V value);

        public void Remove(K key);

        public V Get(K key);
    }
}
