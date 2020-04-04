﻿using System;
using System.Collections.Generic;
using System.Text;

namespace KongoCache.Core.DTOs
{
    public sealed class OpsResponseCode
    {
        OpsResponseCode() { }

        public const string SUCCESS = "00";
        public const string MEMORYOVERFLOW = "9X1";
        public const string SUCCESSFUL_ADDITION = "0A";
        public const string SUCCESSFUL_DELETION = "0D";

    }
}
