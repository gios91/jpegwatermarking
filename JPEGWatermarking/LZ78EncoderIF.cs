﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace LZ78Encoding
{
    public interface LZ78EncoderIF
    {
        Tuple<Dictionary<string, int[]>, Dictionary<int, string>> getEncoding(string stringToEncode);
    }
}