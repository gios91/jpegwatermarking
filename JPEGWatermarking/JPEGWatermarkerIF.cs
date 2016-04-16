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

        byte[] decodeRGBWatermarking(byte[,] R, byte[,] G, byte[,] B);     //EOS è il numero di bit da estrarre
        
        Tuple<Dictionary<int, string>, List<int[]>> decodeLZ8Dict(byte[] byteString);

        byte[] decodeRGBWatermarkingString(byte[] byteString);

        byte[] getDictFinalStream(byte[] dict, byte[] dictNewChars);
    }
}
