using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPEGWatermarking
{
    static class WatermarkingTestUtility
    {
        /*
         *   MSE = (1 / N * M) * ( sum i ( sum j ( X[i,j] - Y[i,j] )^2 ) ) , X = Y original image, Y = Y watermarked image
         */
         
        public static float getMSE(float[,] YMatrixOriginal, float[,] YMatrixWater)
        {
            int rows = YMatrixOriginal.GetLength(0);
            int colums = YMatrixOriginal.GetLength(1);
            float mse = 0;
            for (int i=0; i<rows; i++)
                for (int j=0; j<colums; j++)
                {
                    float diff = YMatrixOriginal[i,j] - YMatrixWater[i, j];
                    mse += (float) Math.Pow(diff, 2);
                }
            return mse / (rows * colums);
        }

        /*
         *   PSNR = 10 * log10 ( MAX_VALUE / MSE ) , MAX_VALUE = 2^L - 1, L = #bit per word  
         */

        public static float getPSNR(float MSE, int bitPrecision = 8)    //c# optional parameter
        {
            float pixelMaxValue = (float) (Math.Pow(2, bitPrecision) - 1);
            float psnr = (float) (10 * Math.Log10( Math.Pow(pixelMaxValue,2) / MSE));
            return psnr;
        }

    }
}
