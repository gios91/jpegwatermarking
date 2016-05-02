using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPEGWatermarking
{
    public interface ChannerErrorIF
    {
        BitArray singleError(BitArray stream, double alpha);

        BitArray gilberElliotBurstError(BitArray stream, double p, double r);
    }
}
