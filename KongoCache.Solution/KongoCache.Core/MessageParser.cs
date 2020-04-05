using KongoCache.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace KongoCache.Core
{
    public class MessageParser
    {
        public static OpType GetOpType(ReadOnlySpan<char> requestSpan)
        {
            try
            {
                ReadOnlySpan<char> optype = requestSpan.Slice(0, requestSpan.IndexOf(" "));

                if (optype.Equals("ADD", StringComparison.OrdinalIgnoreCase))
                    return OpType.ADD;

                if (optype.Equals("GET", StringComparison.OrdinalIgnoreCase))
                    return OpType.GET;

                if (optype.Equals("REM", StringComparison.OrdinalIgnoreCase))
                    return OpType.REMOVE;

                if (optype.Equals("HADD", StringComparison.OrdinalIgnoreCase))
                    return OpType.HADD;

                if (optype.Equals("HGET", StringComparison.OrdinalIgnoreCase))
                    return OpType.HGET;

                if (optype.Equals("HGETALL", StringComparison.OrdinalIgnoreCase))
                    return OpType.HGETALL;

                if (optype.Equals("HREM", StringComparison.OrdinalIgnoreCase))
                    return OpType.HREMOVE;

                if (optype.Equals("HREMKEY", StringComparison.OrdinalIgnoreCase))
                    return OpType.HREMOVEKEY;
            }
            catch
            {
               
            }

            return OpType.INVALID;
        }           
    }
}