using FreeImageAPI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPEGWatermarking
{
    public interface JPEGDecoderIF
    {
        Tuple<byte[,], byte[,], byte[,]> getRGBMatrixFI(FIBITMAP jpeg); //restituisce le matrici RGB dal jpeg prodotto

        Bitmap deserializeJpegImage(byte[] imageStream);

        Tuple<byte[,], byte[,], byte[,]> getRGBMatrix(Bitmap b);

        Tuple<float[,], float[,], float[,]> getYCbCrMatrix(byte[,] RMatrix, byte[,] GMatrix, byte[,] BMatrix);
        
    }
}
