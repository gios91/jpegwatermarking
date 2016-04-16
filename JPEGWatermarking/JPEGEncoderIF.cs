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
        Tuple<byte[,], byte[,], byte[,]> getRGBMatrix(string pathFile);

        Tuple<byte[,], byte[,], byte[,]> getRGBMatrix(Bitmap b);

        void setRGBMatrix(byte[,] RMatrix, byte[,] GMatrix, byte[,] BMatrix, ref Bitmap b);

        Tuple<float[,], float[,], float[,]> getYCbCrMatrix(byte[,] RMatrix, byte[,] GMatrix, byte[,] BMatrix);

        Tuple<byte[,], byte[,], byte[,]> getRGBFromYCbCr(float[,] YMatrix, float[,] CbMatrix, float[,] CrMatrix);

        Tuple<float[,], float[,]> get420Subsampling(float[,] Cb, float[,] Cr, int paddingType);

        Tuple<float[,], float[,]> get422Subsampling(float[,] Cb, float[,] Cr, int paddingType);

        Tuple<double[,], double[,], double[,]> getDCTMatrices(double[,] Y, double[,] Cb, double[,] Cr, int subsamplingType, int paddingType);

        Tuple<double[,], double[,], double[,]> getQuantizedMatrices(double[,] Ydct, double[,] Cbdct, double[,] Crdct);

        Tuple<Int16[,], Int16[,], Int16[,]> getRoundToIntMatrices(double[,] YQ, double[,] CbQ, double[,] CrQ);

        Tuple<ArrayList, ArrayList, ArrayList> getACEncoding(Int16[,] YMatrixQ, Int16[,] CbMatrixQ, Int16[,] CrMatrixQ);

        Tuple<Int16[], Int16[], Int16[]> getDCEncoding(Int16[,] YMatrixQ, Int16[,] CbMatrixQ, Int16[,] CrMatrixQ);

        void printMatriciRGB(byte[,] RMatrix, byte[,] GMatrix, byte[,] BMatrix, int width, int height);

        void printMatriciYCbCr(float[,] YMatrix, float[,] CbMatrix, float[,] CrMatrix, int width, int height);

        int[] getImageDimensions();

        void printMatrice(float[,] M, int row, int column);

        void printMatrice(int[,] M, int row, int column);

        void printMatrice(double[,] M, int row, int column);

        void printMatrice(Int16[,] M);
    }
}
