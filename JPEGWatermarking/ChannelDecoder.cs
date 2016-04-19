using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPEGWatermarking
{
    class ChannelDecoder : ChannelDecoderIF
    {
        public BitArray RipetizioneDecoding(BitArray received, int numR)
        {
            int resultLength = received.Length / numR;
            BitArray result = new BitArray(resultLength);
            int resIndex = 0;
            for (int i=0; i<received.Length; i+=numR)
            {
                int cntZero = 0;
                int cntUno = 0;
                for (int j=i; j<i+numR; j++)
                {
                    if (received[j]) cntUno++;
                    else cntZero++;
                }
                if (cntUno > cntZero) result[resIndex] = true;
                else result[resIndex] = false;
                resIndex++;
            }
            return result;
        }
    }
}
