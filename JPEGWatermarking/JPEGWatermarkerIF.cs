using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPEGWatermarking
{
    public interface JPEGWatermarkerIF
    {
        /* RGB watermarking su Least Significative Bit; byteString rappresenta una stringa generica di bit, che nel 
           caso specifico della codifica LZ78 adottata verrà convertita nella coppia < dict, dictNewChars >
        */
        Tuple<byte[,], byte[,], byte[,]> doRGBWatermarking(byte[,] R, byte[,] G, byte[,] B, byte[] byteString);

        //decodifica watermarking
        byte[] getRGBWatermarking(byte[,] R, byte[,] G, byte[,] B, int EOS);     //EOS è il numero di bit da estrarre
        
        Tuple<byte[],byte[]> decodeWatermarkingString(byte[] byteString, int EOD, int EOS);

        Tuple<byte[],int,int> createWatermarkingString(byte[] dict, byte[] dictNewChars);

        byte[] getDictByteEncoding(List<int[]> dict);

        byte[] getDictNewCharsByteEncoding(Dictionary<int, string> dictNewChars);

        //dict decoding

        List<int[]> getDictByteDecoding(byte[] dict);

        Dictionary<int, string> getDictNewCharsByteDecoding(byte[] dictNewChars);
        
    }
}
