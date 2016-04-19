using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPEGWatermarking
{
    public interface ChannelEncoderIF
    {
        BitArray RipetizioneEncoding(byte[] stream, int numR);  //codifica a ripetizione per trasmissione su canale non ideale
    }
}
