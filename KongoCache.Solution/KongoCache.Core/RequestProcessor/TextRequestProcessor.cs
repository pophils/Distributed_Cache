using KongoCache.Core.DTOs;
using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace KongoCache.Core.RequestProcessor
{
    public class TextRequestProcessor
    { 

        public void InitRequestProcessor(ICacheManager<string, string> _textCacheManager)
        {
            _textCacheManager.RequestProcessorBlock = new ActionBlock<CacheOpMetaData>(textOpMetadata =>
            {
                if (textOpMetadata != null)
                {
                    switch (textOpMetadata.KongoOpType)
                    {
                        case OpType.ADD:
                            try
                            {
                                _textCacheManager.LRUDatabase.Insert(textOpMetadata.KongoKey, textOpMetadata.Value);
                                _textCacheManager.AddReply(OpsResponseCode.SUCCESS + Appconstants.RESPONSE_SEPERATOR + OpType.ADD, textOpMetadata.ClientSessionId);
                                _textCacheManager.EnqueueCompletedOps(textOpMetadata);
                            }
                            catch (OutOfMemoryException)
                            {
                                _textCacheManager.AddReply(OpsResponseCode.MEMORYOVERFLOW + Appconstants.RESPONSE_SEPERATOR + OpType.ADD, textOpMetadata.ClientSessionId);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("AddReply exc: " + ex.Message);
                            }

                            break;

                        case OpType.GET:
                            _textCacheManager.AddReply(OpsResponseCode.SUCCESS + Appconstants.RESPONSE_SEPERATOR + OpType.GET
                                + Appconstants.RESPONSE_SEPERATOR + _textCacheManager.LRUDatabase.Get(textOpMetadata.KongoKey),
                                textOpMetadata.ClientSessionId);
                            break;

                        case OpType.REMOVE:
                            _textCacheManager.LRUDatabase.Remove(textOpMetadata.KongoKey);
                            _textCacheManager.AddReply(OpsResponseCode.SUCCESS + OpType.REMOVE, textOpMetadata.ClientSessionId);
                            _textCacheManager.EnqueueCompletedOps(textOpMetadata);
                            break;
                    }
                }
            }, new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = Environment.ProcessorCount });





            //textProcessorTask = Task.Factory.StartNew(() =>
            //{
            //    while (true)
            //    {
            //        textOpMetadata = _textCacheManager.DequeueOps();

            //        if (textOpMetadata != null)
            //        {
            //            switch (textOpMetadata.KongoOpType)
            //            {
            //                case OpType.ADD:
            //                    try
            //                    {
            //                        _textCacheManager.LRUDatabase().Insert(textOpMetadata.KongoKey, textOpMetadata.Value);
            //                        _textCacheManager.AddReply(OpsResponseCode.SUCCESS + Appconstants.RESPONSE_SEPERATOR + OpType.ADD, textOpMetadata.ClientSessionId);
            //                        _textCacheManager.EnqueueCompletedOps(textOpMetadata);
            //                    }
            //                    catch (OutOfMemoryException)
            //                    {
            //                        _textCacheManager.AddReply(OpsResponseCode.MEMORYOVERFLOW + Appconstants.RESPONSE_SEPERATOR + OpType.ADD, textOpMetadata.ClientSessionId);
            //                    }
            //                    catch(Exception ex)
            //                    {
            //                        Console.WriteLine("AddReply exc: " + ex.Message);
            //                    }

            //                    break;

            //                case OpType.GET:
            //                    _textCacheManager.AddReply(OpsResponseCode.SUCCESS + Appconstants.RESPONSE_SEPERATOR + OpType.GET
            //                        + Appconstants.RESPONSE_SEPERATOR + _textCacheManager.LRUDatabase().Get(textOpMetadata.KongoKey),
            //                        textOpMetadata.ClientSessionId);
            //                    break;

            //                case OpType.REMOVE:
            //                    _textCacheManager.LRUDatabase().Remove(textOpMetadata.KongoKey);
            //                    _textCacheManager.AddReply(OpsResponseCode.SUCCESS + OpType.REMOVE, textOpMetadata.ClientSessionId);
            //                    _textCacheManager.EnqueueCompletedOps(textOpMetadata);
            //                    break;
            //            }
            //        }
            //    }
            //});
        }
 
    }
}
