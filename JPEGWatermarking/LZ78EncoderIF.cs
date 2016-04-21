using System;
using System.Collections;
using System.Collections.Generic;

namespace LZ78Encoding
{
    public interface LZ78EncoderIF
    {
        Tuple<Dictionary<string, int[]>, Dictionary<int, string>> getEncoding(string stringToEncode);

        Tuple<Dictionary<string, Int16[]>, string[]> getCompactEncoding(string stringToEncode);

        /*
        //TODO
        byte[] getByteArrayEncoding(Dictionary<int, string> dictChars, List<int[]> dict);

        //test
        byte[] getByteEncoding(Dictionary<int, string> dictChars, List<int[]> dict);
        */

    }
}
