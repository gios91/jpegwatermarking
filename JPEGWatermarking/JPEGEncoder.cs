using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using System.Drawing;

namespace JPEGEncoding
{
    class JPEGEncoder
    {
        //INSERIRE UN'INTERFACCIA JPEGEncoderIF

        public Tuple<byte[,], byte[,], byte[,]> getRGBMatrix(string pathFile)
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

        public Tuple<float[,], float[,], float[,]> getYCbCrMatrix(byte[,] RMatrix, byte[,] GMatrix, byte[,] BMatrix)
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

        public Tuple<float[,], float[,]> get420Subsampling(float[,] Cb, float[,] Cr)
        {
            //SI ASSUME PER ORA CHE LE MATRICI YCC ABBIANO DIMENSIONE MULTIPLA DI 18 px
            int rows = Cb.GetLength(0);
            int columns = Cb.GetLength(1);
            float[,] CbSub = new float[rows / 2, columns / 2];
            float[,] CrSub = new float[rows / 2, columns / 2];
            for (int i = 0; i < CbSub.Length; i += 8)
                for (int j = 0; j < CrSub.Length; j += 8)
                {
                    Tuple<float[,], float[,]> result = get420SubsamplingBlock(Cb, Cr, 2*i, 2*j);
                    float[,] CbBlock = result.Item1;
                    float[,] CrBlock = result.Item2;
                    for (int k = i; k<8; k++)
                        for (int w = j; w<8; j++)
                        {
                            CbSub[i,j] = CbBlock[i,j];
                            CrSub[i,j] = CrBlock[i,j];
                        }
                }
            return Tuple.Create(CbSub, CrSub);
        }
        
        public Tuple<float[,], float[,]> get420SubsamplingBlock(float[,] Cb, float[,] Cr, int k, int w)
        {
            //k = indice di riga da cui parte il blocco, w=indice di colonna
            float[,] CbSub = new float[8, 8];
            float[,] CrSub = new float[8, 8];
            for (int i = 0; i<8; i++) 
                for(int j = 0; j<8; j++)
                {
                    CbSub[i, j] = (Cb[k + 2*i,w + 2*j] + Cb[k + 2*i, w + 2*j + 1] + Cb[k + 2*i + 1, w + 2*j] + Cb[k + 2*i + 1, w + 2*j + 1]) / 4;
                    CrSub[i, j] = (Cr[k + 2*i, w + 2*j] + Cr[k + 2*i, w + 2*j + 1] + Cr[k + 2*i + 1, w + 2*j] + Cr[k + 2*i + 1, w + 2*j + 1]) / 4;
                }
            return Tuple.Create(CbSub,CrSub);
        }
        
        private YCbCr RGBToYCbCr(RGB rgb)
        {
            float fr = (float)rgb.R / 255;
            float fg = (float)rgb.G / 255;
            float fb = (float)rgb.B / 255;

            float Y = (float)(0.2989 * fr + 0.5866 * fg + 0.1145 * fb);
            float Cb = (float)(-0.1687 * fr - 0.3313 * fg + 0.5000 * fb);
            float Cr = (float)(0.5000 * fr - 0.4184 * fg - 0.0816 * fb);

            return new YCbCr(Y, Cb, Cr);
        }

        public void printMatriciRGB(byte[,] RMatrix, byte[,] GMatrix, byte[,] BMatrix, int width, int height)
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

        public void printMatriciYCbCr(float[,] YMatrix, float[,] CbMatrix, float[,] CrMatrix, int width, int height)
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

        public void printMatrici(float[,] CbSub, float[,] CrSub)
        {
            int x = CbSub.GetLength(0);
            int y = CbSub.GetLength(1);
            Console.WriteLine("Chroma Cb Sub Matrix");
            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    Console.Write(CbSub[i, j] + " ");
                }
                Console.WriteLine();
            }
            Console.WriteLine("Chroma Cr Sub Matrix");
            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    Console.Write(CrSub[i, j] + " ");
                }
                Console.WriteLine();
            }
        }
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
