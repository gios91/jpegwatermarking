using System;
using System.Collections;
using System.Collections.Generic;

namespace LZ78Encoding
{
    public interface LZ78DecoderIF
    {
        string getDecoding(Dictionary<int, string> dictChars, List<int[]> dict);

        //TODO
        Tuple<Dictionary<int, string>, List<int[]>> getEncodingFromByteArray(byte[] encoding);
    }
}