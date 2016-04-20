using System;
using System.Collections;
using System.Collections.Generic;

namespace LZ78Encoding
{
    public interface LZ78DecoderIF
    {
        string getDecoding(List<int[]> dict, Dictionary<int, string> dictChars);

        /*
        //TODO
        Tuple<Dictionary<int, string>, List<int[]>> getEncodingFromByteArray(byte[] encoding);
        */
    }
}