# Distributed Cache

A fault tolerant distributed caching system with LRU evictions. Being built with .net core. and currently support string, HashTable, Set, ArrayList, LinkedList, SortedSet, MaxHeap, and MinHeap caching.

Currently 2 ** 64 keys and each internal database would be able to store 2 ** 64 elements. Basically the memory footprint would be limited by the machine.
