using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
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
            int dimIndex0 = 0; //dimensione dell'ultimo match trovato su index0
            int dimIndex1 = 0;
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
                        dimIndex0 = sc1.Length;
                        i++;
                        if (i == stringToEncode.Length)
                        {   
                            //caso EOF dopo sc1
                            string eof = "EOF";
                            dictNewChars.Add(dictIndexNewChars, eof);
                            int[] row = new int[3]; //tupla con [ entry , index0, index1 ]
                            row[0] = dictIndex; row[1] = index0; row[2] = dictIndexNewChars;
                            dict.Add(sc1+" "+eof, row);
                        }
                    }
                    else
                    {
                        if (index0 == -1)
                        {
                            //è la prima volta che analizzo una nuova lettera non presente nel dizionario
                            int[] row = new int[3]; //tupla con [ entry , index0, index1 ]
                            row[0] = dictIndex; row[1] = 0;
                            if (!dictNewChars.ContainsValue(sc1))
                            {
                                row[2] = dictIndexNewChars;
                                dictNewChars.Add(dictIndexNewChars, sc1);
                                dictIndexNewChars++;
                            }
                            else
                            {
                                Dictionary<int, string>.KeyCollection keys = dictNewChars.Keys;
                                foreach (int k in keys)
                                    if (dictNewChars[k].Equals(sc1))
                                    {
                                        row[2] = k; break;
                                    }
                            }
                            dict.Add(sc1, row);
                            sc1 = string.Empty;
                            index0 = -1; index1 = -1;
                            dictIndex++;
                            i++;
                            s = string.Empty;
                            continue;
                        }
                        else if (sottoStringa(sc1, dict.Keys))
                        {
                            i++;
                            s = sc1;
                            continue;
                        }
                        else {
                            step = 1;
                            s = string.Empty;
                            i = i - ((sc1.Length - dimIndex0) - 1);
                            sc1 = sc1.Substring(0, sc1.Length -(sc1.Length-dimIndex0));
                            dimIndex0 = 0;
                            s = string.Empty;
                            continue;
                         
                        }
                    }
                }
                if (step == 1)
                {
                    string sc2 = s + c;
                    if (dict.ContainsKey(sc2))
                    {
                        index1 = dict[sc2][0];
                        s = sc2;
                        dimIndex1 = sc2.Length;
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
                            row1[0] = dictIndex; row1[1] = index0;
                            if (!dictNewChars.ContainsValue(sc2))
                            {
                                row1[2] = dictIndexNewChars;
                                dictNewChars.Add(dictIndexNewChars, sc2);
                                dictIndexNewChars++;
                            }
                            else
                            {
                                Dictionary<int, string>.KeyCollection keys = dictNewChars.Keys;
                                foreach (int k in keys)
                                    if (dictNewChars[k].Equals(sc2))
                                    {
                                        row1[2] = k; break;
                                    }
                            }
                            dict.Add(string.Concat(sc1, sc2), row1);
                            sc1 = string.Empty;
                            index0 = -1; index1 = -1;
                            dictIndex++;
                            i++;
                            step = 0;
                            s = string.Empty;
                        }
                        else if (sottoStringa(sc2, dict.Keys))
                        {
                            i++;
                            s = sc2;
                            continue;
                        }
                        else
                        {
                            //add to dict    
                            i = i - ((sc2.Length - dimIndex1)-1);
                            sc2 = sc2.Substring(0, sc2.Length - (sc2.Length - dimIndex1));
                            dimIndex1 = 0;
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

        public byte[] getByteArrayEncoding(Dictionary<int, string> dictChars, List<int[]> dict)
        {
            var binFormatter = new BinaryFormatter();
            var mStream = new MemoryStream();
            binFormatter.Serialize(mStream, dict);
            //This gives you the byte array.
            byte[] byteDict = mStream.ToArray();
            Console.WriteLine(byteDict.Length);
            return byteDict;
        }

        public byte[] getByteEncoding(Dictionary<int, string> dictChars, List<int[]> dict)
        {
            int dataDim = 0;
            foreach (int[] v in dict)
            {
                var binFormatter = new BinaryFormatter();
                var mStream = new MemoryStream();
                binFormatter.Serialize(mStream, v);
                //This gives you the byte array.
                dataDim += mStream.ToArray().Length;
            }
            Console.WriteLine("dim in byte dei dati:" + dataDim); 
            return null;
        }

        private bool sottoStringa(string sc1, Dictionary<string, int[]>.KeyCollection keys)
        {
            bool verificato=false;
            foreach (String k in keys) {
                if (k.Contains(sc1))
                {
                    int index = k.IndexOf(sc1);
                    if (index == 0)
                    {
                        verificato = true;
                        break;
                    }
                }
            }
            return verificato;
        }
    }     
 }