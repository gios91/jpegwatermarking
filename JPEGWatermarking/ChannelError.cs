using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPEGWatermarking
{
    class ChannelError : ChannerErrorIF
    {
        public BitArray singleError(BitArray stream, double alpha)
        {
            ArrayList posError = new ArrayList();   //contiene le posizioni correntemente alterate dal canale
            BitArray res = new BitArray(stream.Length);
            for (int k = 0; k < stream.Length; k++)
                res[k] = stream[k];
            //in base al valore di alpha, altera i bit all'interno di stream
            int numError = Convert.ToInt32(alpha * stream.Length);
            for (int i=0; i<numError; i++)
            {
                int randomPos = new Random().Next(0, (stream.Length - 1));
                while (posError.Contains(randomPos))
                {
                    randomPos = new Random().Next(0, (stream.Length - 1));
                }
                if (res[randomPos]) res[randomPos] = false;
                else res[randomPos] = true;
                posError.Add(randomPos);
            }
            return res;
        }

        /*
        public static void Main(string[] args)
        {
            ChannelEncoderIF enc = new ChannelEncoder();
            ChannerErrorIF err = new ChannelError();
            ChannelDecoderIF dec = new ChannelDecoder();

            byte[] test = { 17, 255 };
            //BitArray testArray = new BitArray(test);
            BitArray coded = enc.RipetizioneEncoding(test, 5);
            BitArray errorArray = err.singleError(coded, 0.01);
            int numError = getNumError(coded, errorArray);
            BitArray decoded = dec.RipetizioneDecoding(errorArray, 5);
            BitArray testArray = new BitArray(test);
            bool equals = equalArray(testArray, decoded);
            Console.Write("coded e decoded sono uguali? {0}",equals);
        }
        */
        private static int getNumError(BitArray v1, BitArray v2)
        {
            int cnt = 0;
            for (int i = 0; i < v1.Length; i++)
                if (v1[i] != v2[i]) cnt++;
            return cnt;
        }

        private static bool equalArray(BitArray v1, BitArray v2)
        {
            for (int i = 0; i < v1.Length; i++)
                if (v1[i] != v2[i]) return false;
            return true;
        }
    }
}
