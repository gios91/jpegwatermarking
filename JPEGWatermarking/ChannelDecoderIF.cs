using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPEGWatermarking
{
    public interface ChannelDecoderIF
    {

        BitArray RipetizioneDecoding(BitArray received, int numR);  //codifica a ripetizione per trasmissione su canale non ideale

    }
}
