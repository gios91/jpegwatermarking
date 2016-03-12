using LZ78Encoding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPEGWatermarking
{
    class TestLZ78
    {
        static void Main(string[] args)
        {
            string s = "acbbacbccaabbccbccbaaccbaaccbaarccbrccarcc";
            //string s = "acbbc";
            LZ78EncoderIF enc = new LZ78Encoder();
            Tuple<Dictionary<string, int[]>, Dictionary<int, string>> result = enc.getEncoding(s); 
            Dictionary<string, int[]> dict = result.Item1;
            Dictionary<int, string> dictNewChars = result.Item2;
            Dictionary<string, int[]>.KeyCollection keys1 = dict.Keys;
            Dictionary<int, string>.KeyCollection keys2 = dictNewChars.Keys;
            foreach (string k1 in keys1)
                Console.WriteLine(k1 + ", [" + dict[k1][0] + "," + dict[k1][1] + "," + dict[k1][2] + "]");
            foreach (int k2 in keys2)
                Console.WriteLine(k2 + " , " + dictNewChars[k2]);
            List<int[]> lista = new List<int[]>();
            Dictionary<string, int[]>.ValueCollection valori = dict.Values;
            foreach (int[] v in valori) {
                int[] array = new int[2];
                array[0] = v[1];
                array[1] = v[2];
                lista.Add(array);
            }
            LZ78DecoderIF dec = new LZ78Decoder();
            string decoded = dec.getDecoding(dictNewChars, lista);
            Console.WriteLine("stringa decodificata :"+decoded);
            Console.WriteLine("stringa decodificata uguale all' originale? "+decoded.Equals(s));
            Console.ReadLine();
        }
    }
}
