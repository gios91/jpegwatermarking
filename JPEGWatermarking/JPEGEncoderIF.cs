using FreeImageAPI;
using System;
using System.Collections;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPEGEncoding
{
    public interface JPEGEncoderIF
    {
        Tuple<byte[,], byte[,], byte[,]> getRGBMatrixFI(FIBITMAP dib);
        
        void setRGBMatrix(byte[,] RMatrix, byte[,] GMatrix, byte[,] BMatrix, ref FIBITMAP dib);

        void setRGBMatrix(byte[,] RMatrix, byte[,] GMatrix, byte[,] BMatrix, ref Bitmap dib);

        //

        void encodeToJpeg(FIBITMAP dib, string pathOutput, FREE_IMAGE_SAVE_FLAGS jpegQuality, FREE_IMAGE_SAVE_FLAGS jpegSubsampling);

        byte[] serializeJpegImage(Bitmap b);

        //

        Tuple<float[,], float[,], float[,]> getYCbCrMatrix(byte[,] RMatrix, byte[,] GMatrix, byte[,] BMatrix);

        Tuple<byte[,], byte[,], byte[,]> getRGBFromYCbCr(float[,] YMatrix, float[,] CbMatrix, float[,] CrMatrix);

        Tuple<float[,], float[,]> get420Subsampling(float[,] Cb, float[,] Cr, int paddingType);

        Tuple<float[,], float[,]> get422Subsampling(float[,] Cb, float[,] Cr, int paddingType);

        Tuple<double[,], double[,], double[,]> getDCTMatrices(double[,] Y, double[,] Cb, double[,] Cr, int subsamplingType, int paddingType);

        Tuple<double[,], double[,], double[,]> getIDCTMatrices(double[,] Y, double[,] Cb, double[,] Cr);

        Tuple<double[,], double[,], double[,]> getQuantizedMatrices(double[,] Ydct, double[,] Cbdct, double[,] Crdct);

        Tuple<Int16[,], Int16[,], Int16[,]> getRoundToIntMatrices(double[,] YQ, double[,] CbQ, double[,] CrQ);

        //

        Tuple<ArrayList, ArrayList, ArrayList> getACEncoding(Int16[,] YMatrixQ, Int16[,] CbMatrixQ, Int16[,] CrMatrixQ);

        Tuple<Int16[], Int16[], Int16[]> getDCEncoding(Int16[,] YMatrixQ, Int16[,] CbMatrixQ, Int16[,] CrMatrixQ);
        
    }
}
