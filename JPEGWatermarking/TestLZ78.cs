﻿using LZ78Encoding;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace JPEGWatermarking
{
    class TestLZ78
    {
        /*
        public static void Main(string[] args)
        {
            string path = "C:\\Users\\Giuseppe\\OneDrive\\Documenti\\Progetto_Teoria_Informazione\\stringhelz78\\riga.txt";
            string s = leggiDaFile(path);
            Console.WriteLine(s);
            LZ78EncoderIF enc = new LZ78Encoder();
            Tuple<Dictionary<string, int[]>, Dictionary<int, string>> result = enc.getEncoding(s);
            JPEGWatermarkerIF wm = new JPEGWatermarker();

            Dictionary<string, int[]> dict = result.Item1;
            Dictionary<int, string> dictNewChars = result.Item2;

            //test serializzazione
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream dictStream = new MemoryStream();
            MemoryStream dictNewCharsStream = new MemoryStream();
            bf.Serialize(dictStream, dict);
            bf.Serialize(dictNewCharsStream, dictNewChars);

            byte[] dictArray = dictStream.ToArray();
            byte[] dictNewCharsArray = dictNewCharsStream.ToArray();

            Console.WriteLine("dict dim = {0}", dictArray.Length);
            Console.WriteLine("dictNewChar dim = {0}", dictNewCharsArray.Length);

            byte[] finalStream = wm.getDictFinalStream(dictArray, dictNewCharsArray);
            Console.WriteLine("dict dim = {0}", finalStream.Length);

            /*
            //test deserializzazione 
            MemoryStream dictStreamDes = new MemoryStream();
            MemoryStream dictNewCharsStreamDes = new MemoryStream();
            dictStreamDes.Write(dictArray, 0, dictArray.Length);
            dictStreamDes.Position = 0;
            dictNewCharsStreamDes.Write(dictNewWordsArray, 0, dictNewWordsArray.Length);
            dictNewCharsStreamDes.Position = 0;

            Dictionary<string, int[]> dictFromByte = bf.Deserialize(dictStreamDes) as Dictionary<string, int[]>;
            Dictionary<int, string> dictNewCharsFromByte = bf.Deserialize(dictNewCharsStreamDes) as Dictionary<int, string>;

            //stampa dei dizionari deserializzati
            Console.WriteLine("STAMPA DEI DIZIONARI DESERIALIZZATI");
            Console.WriteLine(dictFromByte);
            Dictionary<string, int[]>.KeyCollection keys = dictFromByte.Keys;
            Dictionary<int, string>.KeyCollection keys1 = dictNewCharsFromByte.Keys;
            foreach (string k1 in keys)
                Console.WriteLine(k1 + ", [" + dictFromByte[k1][0] + "," + dictFromByte[k1][1] + "," + dictFromByte[k1][2] + "]");
            foreach (int k2 in keys1)
                Console.WriteLine(k2 + " , " + dictNewCharsFromByte[k2]);
            List<int[]> lista = new List<int[]>();
            Dictionary<string, int[]>.ValueCollection valori = dictFromByte.Values;
            foreach (int[] v in valori)
            {
                int[] array = new int[2];
                array[0] = v[1];
                array[1] = v[2];
                lista.Add(array);
            }
            /*
            Dictionary<string, int[]>.KeyCollection keys1 = dictFromByte.Keys;
            Dictionary<int, string>.KeyCollection keys2 = dictNewCharsFromByte.Keys;
            foreach (string k1 in keys1)
                Console.WriteLine(k1 + ", [" + dict[k1][0] + "," + dict[k1][1] + "," + dict[k1][2] + "]");
            foreach (int k2 in keys2)
                Console.WriteLine(k2 + " , " + dictNewChars[k2]);
            List<int[]> lista = new List<int[]>();
            Dictionary<string, int[]>.ValueCollection valori = dict.Values;
            foreach (int[] v in valori)
            {
                int[] array = new int[2];
                array[0] = v[1];
                array[1] = v[2];
                lista.Add(array);
            }
            

            /*

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
            //enc.getByteEncoding(dictNewChars, lista);
            //enc.getByteArrayEncoding(dictNewChars, lista);
            LZ78DecoderIF dec = new LZ78Decoder();
            string decoded = dec.getDecoding(dictNewChars, lista);
            Console.WriteLine("stringa decodificata :"+decoded);
            Console.WriteLine("stringa decodificata uguale all' originale? "+decoded.Equals(s));
            Console.ReadLine();
        }
        */


        public static string leggiDaFile(string path)
        {
            int numline = 0;
            string s = string.Empty;
            using (var reader = new StreamReader(path))
            {
                string line = string.Empty;
                while ((line = reader.ReadLine()) != null)
                {
                    s += line;
                    numline++;
                    int x=0;
                    if (numline == 9) {
                         x = 0;
                    }
                    if (numline == 10)
                        break;
                }
            }
            return s;
        }
    }
}
