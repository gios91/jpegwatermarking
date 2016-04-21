using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ78Encoding
{
    class LZ78Decoder : LZ78DecoderIF
    {
        public string getDecoding(List<int[]> dict, Dictionary<int, string> dictChars)
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
                    if (!point2.Equals("EOF"))
                    {
                        decoded = decoded + conc;
                        dictDec.Add(countTuple, conc);
                        countTuple++;
                    }
                    else
                    {
                        decoded = decoded + point1;
                        dictDec.Add(countTuple, conc);
                        countTuple++;
                    }
                }
            }
            return decoded;
        }

        public string getCompactDecoding(List<Int16[]> dict, string[] dictChars)
        {
            Dictionary<int, string> dictDec = new Dictionary<int, string>();
            string decoded = string.Empty;
            int countTuple = 1;
            foreach (Int16[] row in dict)
            {
                if (row[0] == 0)
                {
                    int negateIndex = row[1] * -1;
                    dictDec.Add(countTuple, dictChars[negateIndex]);
                    decoded = decoded + dictChars[negateIndex];
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
                        int negateIndex = row[1] * -1;
                        point2 = dictChars[negateIndex];
                    }
                    string conc = point1 + point2;
                    if (!point2.Equals("EOF"))
                    {
                        decoded = decoded + conc;
                        dictDec.Add(countTuple, conc);
                        countTuple++;
                    }
                    else
                    {
                        decoded = decoded + point1;
                        dictDec.Add(countTuple, conc);
                        countTuple++;
                    }
                }
            }
            return decoded;
        }
        
    }
}
