using JPEGEncoding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPEGEncoding
{
    class TestJPEG
    {
        public static void Main(string[] args)
        {
            string path = "C:\\Users\\Giuseppe\\OneDrive\\Documenti\\Progetto_Teoria_Informazione\\google_plus_32.bmp";
            JPEGEncoder jpg = new JPEGEncoder();
            Tuple<byte[,],byte[,], byte[,]> rgbResult = jpg.getRGBMatrix(path);
            byte[,] RMatrix = rgbResult.Item1;
            byte[,] GMatrix = rgbResult.Item2;
            byte[,] BMatrix = rgbResult.Item3;
            int xPx = RMatrix.GetLength(0);
            int yPx = RMatrix.GetLength(1);
            //jpg.printMatriciRGB(RMatrix, GMatrix, BMatrix, xPx, yPx);
            Tuple<float[,], float[,], float[,]> yCbCrResult = jpg.getYCbCrMatrix(RMatrix, GMatrix, BMatrix);
            float[,] YMatrix = yCbCrResult.Item1;
            float[,] CbMatrix = yCbCrResult.Item2;
            float[,] CrMatrix = yCbCrResult.Item3;
            //jpg.printMatriciYCbCr(YMatrix, CbMatrix, CrMatrix,xPx,yPx);
            Tuple<float[,], float[,]> chromaResult = jpg.get420Subsampling(CbMatrix, CrMatrix);
            float[,] CbMatrixSub = chromaResult.Item1;
            float[,] CrMatrixSub = chromaResult.Item2;
            jpg.printMatrici(CbMatrixSub, CrMatrixSub);
        }
    }
}
