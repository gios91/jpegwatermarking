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
            
            Console.WriteLine("num errori = {0}", numError);

            for (int i=0; i<numError; i++)
            {
                int randomPos = new Random((stream.Length - 1)).Next();
                while (posError.Contains(randomPos))
                {
                    randomPos = new Random(stream.Length - 1).Next();
                }
                Console.WriteLine("random pos = {0}", randomPos);

                if (res[randomPos]) res[randomPos] = false;
                else res[randomPos] = true;
                posError.Add(randomPos);
            }
            return res;
        }

        public static void Main(string[] args)
        {
            ChannelEncoderIF enc = new ChannelEncoder();
            ChannerErrorIF err = new ChannelError();
            ChannelDecoderIF dec = new ChannelDecoder();

            byte[] test = { 17, 255 };
            //BitArray testArray = new BitArray(test);
            BitArray coded = enc.RipetizioneEncoding(test, 3);
            BitArray errorArray = err.singleError(coded, 0.1);
            int numError = getNumError(coded, errorArray);
            BitArray decoded = dec.RipetizioneDecoding(errorArray, 3);

        }

        private static int getNumError(BitArray v1, BitArray v2)
        {
            int cnt = 0;
            for (int i = 0; i < v1.Length; i++)
                if (v1[1] != v2[i]) cnt++;
            return cnt;
        }

    }
}
