using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ78Encoding
{
    class LZ78Decoder : LZ78DecoderIF
    {
        public string getDecoding(Dictionary<int, string> dictChars, List<int[]> dict)
        {
            Dictionary<int,string> dictDec = new Dictionary<int,string>();
            string decoded = string.Empty;
            int countTuple = 1;
            foreach (int[] row in dict) {
                if (row[0] == 0)
                {
                    dictDec.Add(countTuple, dictChars[row[1]]);
                    decoded = decoded + dictChars[row[1]];
                    countTuple++;
                }
                else
                {
                    string point1 = dictDec[row[0]];
                    string point2 = string.Empty;
                    if (dictDec.ContainsKey(row[1]))
                    {
                         point2 = dictDec[row[1]];
                    }
                    else
                    {
                        point2 = dictChars[row[1]];
                    }
                    string conc = point1 + point2;
                    decoded = decoded + conc;
                    dictDec.Add(countTuple, conc);
                    countTuple++;
                }
            }
            return decoded;
        }
    }
}
