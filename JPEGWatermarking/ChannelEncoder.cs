using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPEGWatermarking
{
    public class ChannelEncoder : ChannelEncoderIF
    {
        public BitArray RipetizioneEncoding(byte[] stream, int numR)
        {
            JPEGWatermarker.BitBinaryWriter bbw = new JPEGWatermarker.BitBinaryWriter();
            BitArray result = bbw.getREncoding(stream, numR);
            return result;
            //controllare se sono al contrario o il verso è esatto
        }
    }

    /*
    class Test
    {
        public static void Main(string [] args)
        {
            ChannelEncoderIF ch = new ChannelEncoder();
            byte[] v = { 170,85 };
            BitArray result = ch.RipetizioneEncoding(v, 3);
            ChannelDecoderIF chd = new ChannelDecoder();
            BitArray dec = chd.RipetizioneDecoding(result, 3);
            JPEGWatermarker.BitBinaryWriter bbw = new JPEGWatermarker.BitBinaryWriter();
            byte[] decoded = bbw.getByteArray(dec);
            
        }
    }
    */
}
