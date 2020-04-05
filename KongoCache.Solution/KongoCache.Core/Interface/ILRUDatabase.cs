namespace KongoCache.Core.Interface
{
    public interface ILRUDatabase<K, V> 
    {
        public void Insert(K kongokey, V value);

        public void Remove(K kongokey);

        public V Get(K kongokey); 

    }
}
