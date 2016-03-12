﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ78Encoding
{
    class LZ78Encoder : LZ78EncoderIF
    {
        public Tuple<Dictionary<string, int[]>, Dictionary<int, string>> getEncoding(string stringToEncode)
        {
            Dictionary<string, int[]> dict = new Dictionary<string, int[]>();
            Dictionary<int, string> dictNewChars = new Dictionary<int, string>();   //dizionario per i nuovi caratteri incontrati non nel dizionario
            string s = string.Empty;
            int index0 = -1, index1 = -1;   //index0 è primo indice del pointer, index1 è il secondo (prossimo elemento)
            int step = 0;                   //step0 indica la fase di ricerca di index0 
            int i = 0;                      //indice iterazione caratteri della stringa
            int dictIndex = 1;              //indice di entry nel dizionario
            string sc1 = string.Empty;
            int dictIndexNewChars = stringToEncode.Length+1;              //indice di entry nel dizionario dei nuovi caratteri
            while (i < stringToEncode.Length)
            {
                char c = stringToEncode[i]; 
                if (step == 0)
                {
                     sc1 = s + c;
                    if (dict.ContainsKey(sc1))
                    {
                        index0 = dict[sc1][0];
                        s = sc1;
                        i++;
                    }
                    else
                    {
                        if (index0 == -1)
                        {
                            //è la prima volta che analizzo una nuova lettera non presente nel dizionario
                            int[] row = new int[3]; //tupla con [ entry , index0, index1 ]
                            row[0] = dictIndex; row[1] = 0; row[2] = dictIndexNewChars;
                            dictNewChars.Add(dictIndexNewChars, sc1);
                            dict.Add(sc1, row);
                            sc1 = string.Empty; 
                            index0 = -1; index1 = -1;
                            dictIndex++;
                            dictIndexNewChars++;
                            i++;
                            s = string.Empty;
                            continue;
                        }
                        step = 1;
                        s = string.Empty;
                        sc1 = sc1.Substring(0, sc1.Length - 1);
                    }
                }
                if (step == 1)
                {
                    string sc2 = s + c;
                    if (dict.ContainsKey(sc2))
                    {
                        index1 = dict[sc2][0];
                        s = sc2;
                        
                        i++;
                        if (i == stringToEncode.Length) {
                            int[] row = new int[3]; //tupla con [ entry , index0, index1 ]
                            row[0] = dictIndex; row[1] = index0; row[2] = index1;
                            string conc = string.Concat(sc1, sc2);
                            dict.Add(conc, row);
                        }
                    }
                    else
                    {
                        if (index1 == -1)
                        {
                            //è la prima volta che analizzo una nuova lettera non nel dizionario per index1
                            int[] row1 = new int[3]; //tupla con [ entry , index0, index1 ]
                            row1[0] = dictIndex; row1[1] = 0; row1[2] = dictIndexNewChars;
                            dictNewChars.Add(dictIndexNewChars, sc2);
                            dict.Add(string.Concat(sc1, sc2), row1);
                            sc1 = string.Empty;
                            index0 = -1; index1 = -1;
                            dictIndex++;
                            dictIndexNewChars++;
                            i++;
                            s = string.Empty;
                        }
                        else
                        {
                            //add to dict    
                            sc2 = sc2.Substring(0, sc2.Length - 1);

                            int[] row = new int[3]; //tupla con [ entry , index0, index1 ]
                            row[0] = dictIndex; row[1] = index0; row[2] = index1;
                            string conc = string.Concat(sc1, sc2);
                            dict.Add(conc, row);
                            sc1 = string.Empty; sc2 = string.Empty;
                            index0 = -1; index1 = -1;   //resettare index0 ed index1 dopo ogni aggiunta nel dict
                            step = 0;
                            dictIndex++;
                            s = string.Empty;
                                //arretra l'indice di scorrimento per valutare il carattere scartato in precedenza
                        }
                    }
                }
            }
            return Tuple.Create(dict,dictNewChars);
        }

        static void Main(string[] args)
        {
            string s = "acbbacbccaabbccb";
            //string s = "acbbc";
            LZ78EncoderIF enc = new LZ78Encoder();
            Tuple<Dictionary<string, int[]>, Dictionary<int, string>> result = enc.getEncoding(s);
            Dictionary<string, int[]> dict = result.Item1;
            Dictionary<int, string> dictNewChars = result.Item2;
            Dictionary<string, int[]> .KeyCollection keys1 = dict.Keys;
            Dictionary<int, string>.KeyCollection keys2 = dictNewChars.Keys;
            foreach (string k1 in keys1) 
                Console.WriteLine(k1 +", ["+ dict[k1][0] + "," + dict[k1][1] + "," + dict[k1][2] +"]");
            foreach (int k2 in keys2)
                Console.WriteLine(k2 +" , "+ dictNewChars[k2]);
            Console.ReadLine();
        }
    }
}