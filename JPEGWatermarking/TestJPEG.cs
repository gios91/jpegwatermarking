using JPEGEncoding;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPEGEncoding
{
    class TestJPEG
    {
        public static void Main(string[] args)
        {
            string path = "C:\\Users\\Giuseppe\\OneDrive\\Documenti\\Progetto_Teoria_Informazione\\google_maps_64.bmp";
            JPEGEncoder jpg = new JPEGEncoder();

            Tuple<byte[,], byte[,], byte[,]> rgbResult = jpg.getRGBMatrix(path);
            byte[,] RMatrix = rgbResult.Item1;
            byte[,] GMatrix = rgbResult.Item2;
            byte[,] BMatrix = rgbResult.Item3;
            
            Tuple<float[,], float[,], float[,]> yCbCrResult = jpg.getYCbCrMatrix(RMatrix, GMatrix, BMatrix);
            float[,] YMatrix = yCbCrResult.Item1;
            float[,] CbMatrix = yCbCrResult.Item2;
            float[,] CrMatrix = yCbCrResult.Item3;
            //jpg.printMatriciYCbCr(YMatrix, CbMatrix, CrMatrix,YMatrix.GetLength(0), YMatrix.GetLength(1));
            Tuple<float[,], float[,]> chromaResult = jpg.get420Subsampling(CbMatrix, CrMatrix);
            float[,] CbMatrixSub = chromaResult.Item1;
            float[,] CrMatrixSub = chromaResult.Item2;
            Console.WriteLine("Matrici Chroma Subsampling");
            //jpg.printMatrici(CbMatrixSub, CrMatrixSub);
            Tuple<float[,], float[,]> expResult = jpg.espandiCbCr(CbMatrixSub, CrMatrixSub);
            float[,] CbMatrixExp = expResult.Item1;
            float[,] CrMatrixExp = expResult.Item2;
            Console.WriteLine("Matrici Exp");
            //jpg.printMatrici(CbMatrixExp, CrMatrixExp);
            Tuple<byte[,],byte[,], byte[,]> rgbFromYCCResult = jpg.getRGBFromYCbCr(YMatrix,CbMatrixExp,CrMatrixExp);
            byte[,] RMatrixExp = rgbFromYCCResult.Item1;
            byte[,] GMatrixExp = rgbFromYCCResult.Item2;
            byte[,] BMatrixExp = rgbFromYCCResult.Item3;

            //jpg.printMatriciRGB(RMatrixExp, GMatrixExp, BMatrixExp, RMatrixExp.GetLength(0), RMatrixExp.GetLength(1));

            int xPx = RMatrix.GetLength(0);
            int yPx = RMatrix.GetLength(1);
            Bitmap bm = new Bitmap(path);
            for (int i=0; i< xPx; i++)
                for (int j=0; j<yPx; j++)
                {
                    bm.SetPixel(i, j, Color.FromArgb((byte)RMatrixExp[i, j], (byte)GMatrixExp[i, j], (byte)BMatrixExp[i, j]));
                }
            string pathRGB = "C:\\Users\\Giuseppe\\OneDrive\\Documenti\\Progetto_Teoria_Informazione\\google_maps_64_Chroma.bmp";
            bm.Save(pathRGB);

            /*
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

            */

        }
    }
}
