using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FreeImageAPI;

namespace JPEGWatermarking
{
    class JPEGDecoder : JPEGDecoderIF
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

        public Bitmap deserializeJpegImage(byte[] imageStream)
        {
            ImageConverter ic = new ImageConverter();
            Image img = (Image)ic.ConvertFrom(imageStream);
            return new Bitmap(img);
        }
        
    }
}
