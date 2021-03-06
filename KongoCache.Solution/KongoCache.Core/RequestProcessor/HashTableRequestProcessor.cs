﻿using KongoCache.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace KongoCache.Core.RequestProcessor
{ 
    public class HashTableRequestProcessor
    {  

        public void InitRequestProcessor(ICacheManager<string, Dictionary<string, string>> cacheManager)
        { 
                Dictionary<string, string> _hashTable;

                cacheManager.RequestProcessorBlock = new ActionBlock<CacheOpMetaData>(opMetadata =>
                {
                    if (opMetadata != null)
                    {
                        switch (opMetadata.KongoOpType)
                        {
                            case OpType.HADD:
                                try
                                {
                                    _hashTable = cacheManager.LRUDatabase.Get(opMetadata.KongoKey);

                                    if (_hashTable is null)
                                        _hashTable = new Dictionary<string, string>();

                                    _hashTable[opMetadata.HashKey] = opMetadata.Value;
                                    cacheManager.LRUDatabase.Insert(opMetadata.KongoKey, _hashTable);

                                    cacheManager.AddReply(OpsResponseCode.SUCCESS + Appconstants.RESPONSE_SEPERATOR + OpType.HADD + Appconstants.RESPONSE_SEPERATOR +
                                        _hashTable.Keys.Count, opMetadata.ClientSessionId);

                                   // cacheManager.EnqueueCompletedOps(opMetadata);
                                }
                                catch (OutOfMemoryException)
                                {
                                    cacheManager.AddReply(OpsResponseCode.MEMORYOVERFLOW + Appconstants.RESPONSE_SEPERATOR + OpType.HADD, opMetadata.ClientSessionId);
                                }

                                break;

                            case OpType.HGET:
                                _hashTable = cacheManager.LRUDatabase.Get(opMetadata.KongoKey); 

                                if (_hashTable != null && _hashTable.ContainsKey(opMetadata.HashKey))
                                {                                  

                                    cacheManager.AddReply(OpsResponseCode.SUCCESS + Appconstants.RESPONSE_SEPERATOR + OpType.HGET +
                                        _hashTable[opMetadata.HashKey], opMetadata.ClientSessionId);
                                }
                                else
                                { 
                                    cacheManager.AddReply(OpsResponseCode.SUCCESS + Appconstants.RESPONSE_SEPERATOR + OpType.HGET, opMetadata.ClientSessionId);
                                }

                                break;

                            case OpType.HGETALL:
                                _hashTable = cacheManager.LRUDatabase.Get(opMetadata.KongoKey);

                                if (_hashTable != null && _hashTable.Count > 0)
                                {
                                    //((_hashTable.Count * 2) - 1) accounts for new line btw each reply entry
                                    StringBuilder replyBuilder = new StringBuilder(_hashTable.Count * 2 + ((_hashTable.Count * 2) - 1));

                                    foreach (string key in _hashTable.Keys)
                                    {
                                        replyBuilder.Append(key);
                                        replyBuilder.Append("\n");
                                        replyBuilder.Append(_hashTable[key]);
                                        replyBuilder.Append("\n");
                                    }

                                    replyBuilder.Remove(replyBuilder.Length - 1, 1); // remove the last new line

                                    cacheManager.AddReply(OpsResponseCode.SUCCESS + Appconstants.RESPONSE_SEPERATOR + OpType.HGETALL + replyBuilder.ToString(),
                                        opMetadata.ClientSessionId);
                                }
                                else
                                    cacheManager.AddReply(OpsResponseCode.SUCCESS + Appconstants.RESPONSE_SEPERATOR + OpType.HGETALL,
                                   opMetadata.ClientSessionId);

                                break;

                            case OpType.HREMOVE:
                                cacheManager.LRUDatabase.Remove(opMetadata.KongoKey);
                                cacheManager.AddReply(OpsResponseCode.SUCCESS + Appconstants.RESPONSE_SEPERATOR + OpType.HREMOVE,
                                       opMetadata.ClientSessionId);
                              //  cacheManager.EnqueueCompletedOps(opMetadata);
                                break;

                            case OpType.HREMOVEKEY:

                                _hashTable = cacheManager.LRUDatabase.Get(opMetadata.KongoKey);

                                if (_hashTable != null && _hashTable.ContainsKey(opMetadata.HashKey))
                                {
                                    _hashTable.Remove(opMetadata.HashKey);
                                    cacheManager.AddReply(OpsResponseCode.SUCCESS + Appconstants.RESPONSE_SEPERATOR + OpType.HREMOVEKEY + _hashTable.Count,
                                  opMetadata.ClientSessionId);
                                    cacheManager.EnqueueCompletedOps(opMetadata);
                                }
                                else
                                {
                                    cacheManager.AddReply(OpsResponseCode.SUCCESS + Appconstants.RESPONSE_SEPERATOR + OpType.HREMOVEKEY,
                                     opMetadata.ClientSessionId);
                                }

                                break;
                        }
                    }
                },
                new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism =  Environment.ProcessorCount, SingleProducerConstrained = true });
                 
        }
           
         
    }
}
