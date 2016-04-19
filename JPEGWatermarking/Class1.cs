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
        public void singleError(ref BitArray stream, double alpha);
    }
}
