using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPEGEncoding
{
    public interface JPEGEncoderIF
    {
        Tuple<byte[,], byte[,], byte[,]> getRGBMatrix(string pathFile);

        Tuple<float[,], float[,], float[,]> getYCbCrMatrix(byte[,] RMatrix, byte[,] GMatrix, byte[,] BMatrix);

        Tuple<byte[,], byte[,], byte[,]> getRGBFromYCbCr(float[,] YMatrix, float[,] CbMatrix, float[,] CrMatrix);

        Tuple<float[,], float[,]> get420Subsampling(float[,] Cb, float[,] Cr, int paddingType);

        Tuple<float[,], float[,]> get422Subsampling(float[,] Cb, float[,] Cr, int paddingType);

        Tuple<double[,], double[,], double[,]> getDCTMatrices(double[,] Y, double[,] Cb, double[,] Cr, int subsamplingType, int paddingType);

        Tuple<double[,], double[,], double[,]> getQuantizedMatrices(double[,] Ydct, double[,] Cbdct, double[,] Crdct);

        Tuple<double[,],double[,],double[,]> getACEncoding(double[,] YMatrixQ, double[,] CbMatrixQ, double[,] CrMatrixQ);

        Tuple<double[], double[], double[]> getDCEncoding(double[,] YMatrixQ, double[,] CbMatrixQ, double[,] CrMatrixQ);

        void printMatriciRGB(byte[,] RMatrix, byte[,] GMatrix, byte[,] BMatrix, int width, int height);

        void printMatriciYCbCr(float[,] YMatrix, float[,] CbMatrix, float[,] CrMatrix, int width, int height);

        void printMatrice(float[,] M, int row, int column);

        void printMatrice(double[,] M, int row, int column);


    }
}
