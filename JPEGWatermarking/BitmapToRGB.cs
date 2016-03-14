using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using System.Drawing;

namespace BitmapToRGB
{
    class RGBTest
    {
        public static Tuple<byte[,], byte[,], byte[,]> rgbTest(string pathFile)
        {
            Bitmap b = new Bitmap(pathFile);
            int yPx = b.Height;
            int xPx = b.Width;
            byte[,] RMatrix = new byte[xPx, yPx];
            byte[,] GMatrix = new byte[xPx, yPx];
            byte[,] BMatrix = new byte[xPx, yPx];
            for (int i = 0; i < xPx; i++)
                for (int j = 0; j < yPx; j++)
                {
                    RMatrix[i, j] = b.GetPixel(i, j).R;
                    GMatrix[i, j] = b.GetPixel(i, j).G;
                    BMatrix[i, j] = b.GetPixel(i, j).B;
                }

            //printMatriciRGB(RMatrix, GMatrix, BMatrix, xPx, yPx);
            return Tuple.Create(RMatrix, GMatrix, BMatrix);
        }

        public static Tuple<float[,], float[,], float[,]> YCbCrTest(byte[,] RMatrix, byte[,] GMatrix, byte[,] BMatrix)
        {
            int xPx = RMatrix.GetLength(0);
            int yPx = RMatrix.GetLength(1);
            float[,] YMatrix = new float[xPx, yPx];
            float[,] CbMatrix = new float[xPx, yPx];
            float[,] CrMatrix = new float[xPx, yPx];
            for (int i = 0; i < xPx; i++)
                for (int j = 0; j < yPx; j++)
                {
                    RGB rgb = new RGB(RMatrix[i, j], GMatrix[i, j], BMatrix[i, j]);
                    YCbCr ycbcr = RGBToYCbCr(rgb);
                    YMatrix[i, j] = ycbcr.Y;
                    CbMatrix[i, j] = ycbcr.Cb;
                    CrMatrix[i, j] = ycbcr.Cr;
                }
            return Tuple.Create(YMatrix, CbMatrix, CrMatrix);
        }
        
        public static YCbCr RGBToYCbCr(RGB rgb)
        {
            float fr = (float)rgb.R / 255;
            float fg = (float)rgb.G / 255;
            float fb = (float)rgb.B / 255;

            float Y = (float)(0.2989 * fr + 0.5866 * fg + 0.1145 * fb);
            float Cb = (float)(-0.1687 * fr - 0.3313 * fg + 0.5000 * fb);
            float Cr = (float)(0.5000 * fr - 0.4184 * fg - 0.0816 * fb);

            return new YCbCr(Y, Cb, Cr);
        }

        private static void printMatriciRGB(byte[,] RMatrix, byte[,] GMatrix, byte[,] BMatrix, int width, int height)
        {
            Console.WriteLine("R Matrix");
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Console.Write(RMatrix[i, j] + " ");
                }
                Console.WriteLine();
            }
            Console.WriteLine("G Matrix");
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Console.Write(GMatrix[i, j] + " ");
                }
                Console.WriteLine();
            }
            Console.WriteLine("B Matrix");
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Console.Write(BMatrix[i, j] + " ");
                }
                Console.WriteLine();
            }
        }

        private static void printMatriciYCbCr(float[,] YMatrix, float[,] CbMatrix, float[,] CrMatrix, int width, int height)
        {
            Console.WriteLine("Y Matrix");
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Console.Write(YMatrix[i, j] + " ");
                }
                Console.WriteLine();
            }
            Console.WriteLine("Cb Matrix");
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Console.Write(CbMatrix[i, j] + " ");
                }
                Console.WriteLine();
            }
            Console.WriteLine("Cr Matrix");
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Console.Write(CrMatrix[i, j] + " ");
                }
                Console.WriteLine();
            }
        }
        
        /*
        public static void Main(string[] args)
        {
            string path = "C:\\Users\\Giuseppe\\OneDrive\\Documenti\\Progetto_Teoria_Informazione\\feed.jpg";
            Tuple<byte[,],byte[,], byte[,]> rgbResult = rgbTest(path);
            byte[,] RMatrix = rgbResult.Item1;
            byte[,] GMatrix = rgbResult.Item2;
            byte[,] BMatrix = rgbResult.Item3;
            int xPx = RMatrix.GetLength(0);
            int yPx = RMatrix.GetLength(1);
            printMatriciRGB(RMatrix, GMatrix, BMatrix, xPx, yPx);
            Tuple<float[,], float[,], float[,]> yCbCrResult = YCbCrTest(RMatrix, GMatrix, BMatrix);
            float[,] YMatrix = yCbCrResult.Item1;
            float[,] CbMatrix = yCbCrResult.Item2;
            float[,] CrMatrix = yCbCrResult.Item3;
            printMatriciYCbCr(YMatrix, CbMatrix, CrMatrix,xPx,yPx);
        }
        */

    }//RGBTest


        public struct RGB
        {
            private byte _r;
            private byte _g;
            private byte _b;

            public RGB(byte r, byte g, byte b)
            {
                this._r = r;
                this._g = g;
                this._b = b;
            }
            public byte R
            {
                get { return this._r; }
                set { this._r = value; }
            }
            public byte G
            {
                get { return this._g; }
                set { this._g = value; }
            }
            public byte B
            {
                get { return this._b; }
                set { this._b = value; }
            }
            public bool Equals(RGB rgb)
            {
                return (this.R == rgb.R) && (this.G == rgb.G) && (this.B == rgb.B);
            }
        }

        public struct YCbCr
        {
            private float _y;
            private float _cb;
            private float _cr;

            public YCbCr(float y, float cb, float cr)
            {
                this._y = y;
                this._cb = cb;
                this._cr = cr;
            }

            public float Y
            {
                get { return this._y; }
                set { this._y = value; }
            }

            public float Cb
            {
                get { return this._cb; }
                set { this._cb = value; }
            }

            public float Cr
            {
                get { return this._cr; }
                set { this._cr = value; }
            }

            public bool Equals(YCbCr ycbcr)
            {
                return (this.Y == ycbcr.Y) && (this.Cb == ycbcr.Cb) && (this.Cr == ycbcr.Cr);
            }
        }
}
