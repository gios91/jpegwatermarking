using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FreeImageAPI;
using JPEGEncoding;

namespace JPEGWatermarking
{
    public class JPEGDecoder : JPEGDecoderIF
    {
        public Tuple<byte[,], byte[,], byte[,]> getRGBMatrixFI(FIBITMAP jpeg)
        {
            int yPx = (int)FreeImage.GetWidth(jpeg);
            int xPx = (int)FreeImage.GetHeight(jpeg);
            byte[,] RMatrix = new byte[xPx, yPx];
            byte[,] GMatrix = new byte[xPx, yPx];
            byte[,] BMatrix = new byte[xPx, yPx];
            for (int i = 0; i < xPx; i++)
            {
                //legge una singola riga del bitmap
                Scanline<RGBTRIPLE> bitmapRow = new Scanline<RGBTRIPLE>(jpeg, xPx - 1 - i);
                RGBTRIPLE[] rgb = bitmapRow.Data;
                for (int j = 0; j < yPx; j++)
                {
                    RMatrix[i, j] = rgb[j].rgbtRed;
                    GMatrix[i, j] = rgb[j].rgbtGreen;
                    BMatrix[i, j] = rgb[j].rgbtBlue;
                }
            }
            return Tuple.Create(RMatrix, GMatrix, BMatrix);
        }

        public Tuple<byte[,], byte[,], byte[,]> getRGBMatrix(Bitmap b)
        {
            int yPx = b.Width;
            int xPx = b.Height;
            byte[,] RMatrix = new byte[xPx, yPx];
            byte[,] GMatrix = new byte[xPx, yPx];
            byte[,] BMatrix = new byte[xPx, yPx];
            for (int i = 0; i < xPx; i++)
                for (int j = 0; j < yPx; j++)
                {
                    RMatrix[i, j] = b.GetPixel(j, i).R;
                    GMatrix[i, j] = b.GetPixel(j, i).G;
                    BMatrix[i, j] = b.GetPixel(j, i).B;
                }
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

        public Bitmap deserializeJpegImage(byte[] imageStream)
        {
            ImageConverter ic = new ImageConverter();
            Image img = null;
            try {
                img = (Image)ic.ConvertFrom(imageStream);
            } catch (Exception e)
            {
                //non è possibile decodificare correttamente l'immagine per la presenza 
                //di un numero eccessivo di errori burst
                return null;
            }
            return new Bitmap(img);
        }

        private YCbCr RGBToYCbCr(RGB rgb)
        {
            float fr = (float)rgb.R;
            float fg = (float)rgb.G;
            float fb = (float)rgb.B;

            float Y = (float)(0.299 * fr + 0.587 * fg + 0.114 * fb);
            float Cb = (float)(-0.1687 * fr - 0.3313 * fg + 0.5 * fb) + 128;
            float Cr = (float)(0.5 * fr - 0.4187 * fg - 0.0813 * fb) + 128;

            return new YCbCr(Y, Cb, Cr);
        }

        private RGB YCbCrToRGB(YCbCr ycbcr)
        {

            byte r = (byte)(ycbcr.Y + 0.0000 * (ycbcr.Cb - 128) + 1.402 * (ycbcr.Cr - 128));
            byte g = (byte)(ycbcr.Y - 0.34414 * (ycbcr.Cb - 128) - 0.71414 * (ycbcr.Cr - 128));
            byte b = (byte)(ycbcr.Y + 1.772 * (ycbcr.Cb - 128) + 0.0000 * (ycbcr.Cr - 128));

            return new RGB(r, g, b);
        }

    }
}
