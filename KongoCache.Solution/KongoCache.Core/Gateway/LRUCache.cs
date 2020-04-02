using KongoCache.Core.Interface;
using System.Collections.Generic;

namespace KongoCache.Core.Gateway
{
    public class LRUCache<K, V> : ILRUCache<K, V>
    {
        IDictionary<K, LinkedListNode<(K, V)>> _hashTable;
        LinkedList<(K, V)> _keyList;

        long _capacity = long.MaxValue; // just for testing
        long _size = 0;

        public LRUCache(){} 

        public void Insert(K key, V value)
        {
            EnsureInternalStoreIsReadyForStorage();

            if (_hashTable.ContainsKey(key))
                _keyList.Remove(_hashTable[key]);

            if (_size < _capacity)
            {
                _hashTable[key] = _keyList.AddFirst((key, value));
                _size++;
                return;
            }

            _hashTable.Remove(_keyList.Last.Value.Item1);
            _keyList.RemoveLast();

            _hashTable[key] = _keyList.AddFirst((key, value));
        }
        
        public void Remove(K key)
        {
            if (_hashTable is null || _keyList is null)
                return;
            
            if (_hashTable.ContainsKey(key))
            {
                _keyList.Remove(_hashTable[key]);
                _hashTable.Remove(key);
            }             
        }

        public V Get(K key)
        {
            if (_hashTable is null || _keyList is null)
                return default;

            if (_hashTable.ContainsKey(key))
            {
                LinkedListNode<(K, V)> node = _hashTable[key];
                _keyList.Remove(node);
                _keyList.AddFirst(node);

                return node.Value.Item2;
            }

            return default;
        }
               
        void EnsureInternalStoreIsReadyForStorage()
        {
            if (_hashTable is null)
                _hashTable = new Dictionary<K, LinkedListNode<(K, V)>>();

            if (_keyList is null)
                _keyList = new LinkedList<(K, V)>();
        }
                  
    }
}
