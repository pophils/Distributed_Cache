using KongoCache.Core.Interface;
using System;
using System.Collections.Generic;

namespace KongoCache.Core.Gateway
{

    public class LRUDatabase<K, V> : ILRUDatabase<K, V>
    {
        IDictionary<K, LinkedListNode<(K, V)>> _hashTable;
        LinkedList<(K, V)> _keyList;

        ulong _capacity = Environment.Is64BitOperatingSystem ? ulong.MaxValue: uint.MaxValue; // (long)Math.Pow(2, 64); // just for testing
        ulong _size = 0;

        public LRUDatabase(){} 

        public void Insert(K kongokey, V value)
        {
            EnsureInternalStoreIsReadyForStorage();

            if (_hashTable.ContainsKey(kongokey))
                _keyList.Remove(_hashTable[kongokey]);

            if (_size < _capacity)
            {
                _hashTable[kongokey] = _keyList.AddFirst((kongokey, value));
                _size++;
                return;
            }

            _hashTable.Remove(_keyList.Last.Value.Item1);
            _keyList.RemoveLast();

            _hashTable[kongokey] = _keyList.AddFirst((kongokey, value));
        }
        
        public void Remove(K kongokey)
        {
            if (_hashTable is null || _keyList is null)
                return;
            
            if (_hashTable.ContainsKey(kongokey))
            { 
                _keyList.Remove(_hashTable[kongokey]);
                _hashTable.Remove(kongokey);
            }             
        }

        public V Get(K kongokey)
        {
            if (_hashTable is null || _keyList is null)
                return default;

            if (_hashTable.ContainsKey(kongokey))
            {
                LinkedListNode<(K, V)> node = _hashTable[kongokey];
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
