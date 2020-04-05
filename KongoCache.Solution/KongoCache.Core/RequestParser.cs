using KongoCache.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace KongoCache.Core
{
    public class RequestParser
    { 
        public static CacheOpMetaData ParseTextRequest(string[] requestContent, OpType opType)
        {
            switch (opType)
            {
                case OpType.ADD: // ADD KONGOKEY VALUE
                    if (requestContent.Length != 3)
                        throw new InvalidOperationException();
                    return new CacheOpMetaData()
                    {
                        KongoKey = requestContent[1],
                        Value = requestContent[2],
                        KongoContentType = CacheContentType.Text,
                        KongoOpType = opType
                    };

                case OpType.GET: // GET KONGOKEY
                    if (requestContent.Length != 2)
                        throw new InvalidOperationException();
                    return new CacheOpMetaData()
                    {
                        KongoKey = requestContent[1], 
                        KongoContentType = CacheContentType.Text,
                        KongoOpType = opType
                    };

                case OpType.REMOVE: // REM KONGOKEY
                    if (requestContent.Length != 2)
                        throw new InvalidOperationException();
                    return new CacheOpMetaData()
                    {
                        KongoKey = requestContent[1],
                        KongoContentType = CacheContentType.Text,
                        KongoOpType = opType
                    };
            }

            throw new InvalidOperationException(); 
        }

        public static CacheOpMetaData ParseHashMapRequest(string[] requestContent, OpType opType)
        {
            switch (opType)
            {
                case OpType.HADD:
                    if (requestContent.Length != 4)  // HADD KONGOKEY HashKey HashValue
                        throw new InvalidOperationException();
                    return new CacheOpMetaData()
                    {
                        KongoKey = requestContent[1],
                        HashKey = requestContent[2],
                        Value = requestContent[3],
                        KongoContentType = CacheContentType.HashTable,
                        KongoOpType = opType
                    };

                case OpType.HGET:  // HGET KONGOKEY HashKey
                    if (requestContent.Length != 3)
                        throw new InvalidOperationException();
                    return new CacheOpMetaData()
                    {
                        KongoKey = requestContent[1],
                        HashKey = requestContent[2],
                        KongoContentType = CacheContentType.HashTable,
                        KongoOpType = opType
                    };

                case OpType.HGETALL: // HGETALL KONGOKEY
                    if (requestContent.Length != 2)
                        throw new InvalidOperationException();
                    return new CacheOpMetaData()
                    {
                        KongoKey = requestContent[1],
                        KongoContentType = CacheContentType.HashTable,
                        KongoOpType = opType
                    };

                case OpType.HREMOVE: // HREM KONGOKEY
                    if (requestContent.Length != 2)
                        throw new InvalidOperationException();
                    return new CacheOpMetaData()
                    {
                        KongoKey = requestContent[1],
                        KongoContentType = CacheContentType.HashTable,
                        KongoOpType = opType
                    };

                case OpType.HREMOVEKEY:  // HREMKEY KONGOKEY HashKey 
                    if (requestContent.Length != 3)
                        throw new InvalidOperationException();
                    return new CacheOpMetaData()
                    {
                        KongoKey = requestContent[1],
                        HashKey = requestContent[2],
                        KongoContentType = CacheContentType.HashTable,
                        KongoOpType = opType
                    };
            }

            throw new InvalidOperationException();
        }
    }
}