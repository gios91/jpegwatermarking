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

        Tuple<byte[,], byte[,], byte[,], int> doAdvancedRGBWatermarking(byte[,] R, byte[,] G, byte[,] B, byte[] byteString);      //level è in max num di LSB utilizzati per px

        Tuple<byte[,], byte[,], byte[,]> doLuminanceRGBWatermarking(byte[,] R, byte[,] G, byte[,] B, byte[] byteString, List<int[]> blockSequence, int numLSBSelectedBlock, int numLSBNonSelectedBlock);      //level è in max num di LSB utilizzati per px

        //YWatermarking
        List<int[]> getBlocksForYWatermarking(float[,] Y, float delta);

        //decodifica watermarking

        byte[] getRGBWatermarking(byte[,] R, byte[,] G, byte[,] B, int EOS);     //EOS è il marcatore di fine watermarking

        byte[] getAdvancedRGBWatermarking(byte[,] R, byte[,] G, byte[,] B, int EOS, int level);     //EOS è il marcatore di fine watermarking; level è in max num di LSB utilizzati per px

        byte[] getLuminanceRGBWatermarking(byte[,] R, byte[,] G, byte[,] B, int EOS, List<int[]> blockSequence, int numLSBSelectedBlock, int numLSBNonSelectedBlock);

        Tuple<byte[],byte[]> decodeWatermarkingString(byte[] byteString, int EOD, int EOS);

        Tuple<byte[],int,int> createWatermarkingString(byte[] dict, byte[] dictNewChars);

        byte[] getDictByteEncoding(List<int[]> dict);

        byte[] getDictNewCharsByteEncoding(Dictionary<int, string> dictNewChars);

        //dict decoding

        List<int[]> getDictByteDecoding(byte[] dict);

        Dictionary<int, string> getDictNewCharsByteDecoding(byte[] dictNewChars);

        List<Int16[]> getDictByteCompactDecoding(byte[] dict);

        string[] getDictNewCharsByteCompactDecoding(byte[] dictNewChars);

    }
}
