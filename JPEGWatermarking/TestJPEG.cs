﻿using DCTLib;
using JPEGEncoding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace JPEGEncoding
{
    class TestJPEG
    {
        
        /*
        public static void Main(string[] args)
        {
            
            string path = "C:\\Users\\Giuseppe\\OneDrive\\Documenti\\Progetto_Teoria_Informazione\\jpegtest\\Google3.bmp";
            string jpegSampleFile = "C:\\Users\\Giuseppe\\OneDrive\\Documenti\\Progetto_Teoria_Informazione\\jpegtest\\pitfallball.jpg";
            string pathOutFile = "C:\\Users\\Giuseppe\\OneDrive\\Documenti\\Progetto_Teoria_Informazione\\jpegtest\\output_TEST_JPEGENCODER.jpg";
            //tipo di subsampling applicato
            
            JPEGEncoderIF jpg = new JPEGEncoder(path);
            int[] dimXY = jpg.getImageDimensions();

            Tuple<byte[,], byte[,], byte[,]> rgbResult = jpg.getRGBMatrix(path);
            byte[,] RMatrix = rgbResult.Item1;
            byte[,] GMatrix = rgbResult.Item2;
            byte[,] BMatrix = rgbResult.Item3;
            Tuple<float[,], float[,], float[,]> yCbCrResult = jpg.getYCbCrMatrix(RMatrix, GMatrix, BMatrix);
            float[,] YMatrix = yCbCrResult.Item1;
            float[,] CbMatrix = yCbCrResult.Item2;
            float[,] CrMatrix = yCbCrResult.Item3;
            int rows = YMatrix.GetLength(0);
            int columns = YMatrix.GetLength(1);
            //Subsampling
            /*
            Tuple<float[,], float[,]> chromaResult = jpg.get420Subsampling(CbMatrix, CrMatrix, JPEGUtility.NO_SUBSAMPLING);
            float[,] CbMatrixSub = chromaResult.Item1;
            float[,] CrMatrixSub = chromaResult.Item2;
            Console.WriteLine("SUBSAMPLING");
            */
            /*
            //jpg.printMatriciYCbCr(YMatrix, CbMatrixSub, CrMatrixSub, rows, columns);
            //DCT TEST
            double[,] YDMatrix = new double[rows, columns];
            double[,] CbDMatrix = new double[rows, columns];
            double[,] CrDMatrix = new double[rows, columns];
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < columns; j++)
                {
                    YDMatrix[i, j] = (double)YMatrix[i, j];
                    CbDMatrix[i, j] = (double)CbMatrix[i, j];
                    CrDMatrix[i, j] = (double)CrMatrix[i, j];
                }
            Tuple<double[,], double[,], double[,]> DCTResult = jpg.getDCTMatrices(YDMatrix, CbDMatrix, CrDMatrix, JPEGUtility.NO_SUBSAMPLING, JPEGUtility.NO_SUBSAMPLING);
            double[,] YDCTMatrix = DCTResult.Item1;
            double[,] CbDCTMatrix = DCTResult.Item2;
            double[,] CrDCTMatrix = DCTResult.Item3;
            Tuple<double[,], double[,], double[,]> QuantizationResult = jpg.getQuantizedMatrices(YDCTMatrix, CbDCTMatrix, CrDCTMatrix);
            double[,] YQMatrix = QuantizationResult.Item1;
            double[,] CbQMatrix = QuantizationResult.Item2;
            double[,] CrQMatrix = QuantizationResult.Item3;
            //ROUNDING YCbCR Quantized
            Tuple<Int16[,], Int16[,], Int16[,]> RoundingResult = jpg.getRoundToIntMatrices(YQMatrix, CbQMatrix, CrQMatrix);
            Int16[,] YQIntMatrix = RoundingResult.Item1;
            Int16[,] CbQIntMatrix = RoundingResult.Item2;
            Int16[,] CrQIntMatrix = RoundingResult.Item3;


            //DA RGB A JPEG: CODIFICA

            Bitmap bmp1 = new Bitmap(path);
            ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);

            // Create an Encoder object based on the GUID
            // for the Quality parameter category.
            System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;

            // Create an EncoderParameters object.
            // An EncoderParameters object has an array of EncoderParameter
            // objects. In this case, there is only one
            // EncoderParameter object in the array.
            EncoderParameters myEncoderParameters = new EncoderParameters(1);

            EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 50L);
            myEncoderParameters.Param[0] = myEncoderParameter;
            bmp1.Save(pathOutFile+"_test50.jpg", jpgEncoder, myEncoderParameters);

            myEncoderParameter = new EncoderParameter(myEncoder, 100L);
            myEncoderParameters.Param[0] = myEncoderParameter;
            bmp1.Save(pathOutFile + "_test100.jpg", jpgEncoder, myEncoderParameters);

            // Save the bitmap as a JPG file with zero quality level compression.
            myEncoderParameter = new EncoderParameter(myEncoder, 0L);
            myEncoderParameters.Param[0] = myEncoderParameter;
            bmp1.Save(pathOutFile + "_test0.jpg", jpgEncoder, myEncoderParameters);
            */
            //DA IMMAGINE JPEG A RGB: DECODIFICA

            /*
            Console.WriteLine("TEST DECODIFICA");
            Bitmap input = ConvertToBitmap(pathOutFile + "_test100.jpg");
            Tuple<byte[,], byte[,], byte[,]> rgbResult2 = jpg.getRGBMatrix(input);
            byte[,] RMatrix2 = rgbResult2.Item1;
            byte[,] GMatrix2 = rgbResult2.Item2;
            byte[,] BMatrix2 = rgbResult2.Item3;
            jpg.printMatriciRGB(RMatrix, GMatrix, BMatrix, RMatrix.GetLength(0), RMatrix.GetLength(1));
            */
                
            /*
            Console.WriteLine("ARRAY RGB");

            Bitmap bit = new Bitmap(path);
            byte[] bmp = imageToArray(bit);
            for (int i = 0; i < bmp.Length; i++)
                Console.Write(bmp[i] + " ");
            */

            //**************************************************************
            /*
            JPEGMarkersWriter markerwr = new JPEGMarkersWriter((byte)dimXY[0], (byte)dimXY[1], JPEGUtility.NO_SUBSAMPLING);
            BinaryWriter bw = new BinaryWriter(File.Open(pathOutFile, FileMode.Create));
            markerwr.WriteJPEGFile(bw, null, null, null, null, null, null, toSByteArray(YMatrix), toSByteArray(CbMatrix), toSByteArray(CrMatrix));
            //markerwr.EncodeImageBufferToJpg(toIntArray(YQIntMatrix), toIntArray(CbQIntMatrix), toIntArray(CrQIntMatrix), bw);
            */

            /*

            Tuple<ArrayList, ArrayList, ArrayList> resultAC = jpg.getACEncoding(YQIntMatrix, CbQIntMatrix, CrQIntMatrix);
            Tuple<Int16[], Int16[], Int16[]> resultDC = jpg.getDCEncoding(YQIntMatrix, CbQIntMatrix, CrQIntMatrix);
            JPEGMarkersWriter markerwr = new JPEGMarkersWriter((byte)dimXY[0], (byte)dimXY[1], JPEGUtility.NO_SUBSAMPLING);
            BinaryWriter bw = new BinaryWriter(File.Open(pathOutFile, FileMode.Create));
            Console.WriteLine("************ Y MATRIX *************");
            jpg.printMatrice(YQIntMatrix);
            Int16[] YDC = resultDC.Item1;
            ArrayList YAC = resultAC.Item1;
            printDC(YDC, "************ Y DC *************");
            Console.WriteLine();
            printAC(YAC, "************ Y AC *************");
            Console.WriteLine();
            Console.WriteLine("************ Cb MATRIX *************");
            jpg.printMatrice(CbQIntMatrix);
            Int16[] CbDC = resultDC.Item2;
            ArrayList CbAC = resultAC.Item2;
            printDC(CbDC, "************ Cb DC *************");
            Console.WriteLine();
            printAC(CbAC, "************ Cb AC *************");
            Console.WriteLine("************ Cr MATRIX *************");
            jpg.printMatrice(CrQIntMatrix);
            Int16[] CrDC = resultDC.Item3;
            ArrayList CrAC = resultAC.Item3;
            printDC(CrDC, "************ Cr DC *************");
            Console.WriteLine();
            printAC(CrAC, "************ Cr AC *************");
            //markerwr.WriteJPEGFile(bw, YDC, resultDC.Item2, resultDC.Item3, resultAC.Item1, resultAC.Item2, resultAC.Item3);

            Console.WriteLine("*********** HUFFMAN Y DC ********");
            JPEGUtility.BitString[] v1 = JPEGUtility.getYDCHCode();
            foreach (JPEGUtility.BitString s in v1)
                Console.WriteLine(s.print());
            
            Console.WriteLine("*********** HUFFMAN Y AC ********");
            JPEGUtility.BitString[] v2 = JPEGUtility.getYACHCode();
            for (int i=0; i< v2.Length; i++)
                Console.WriteLine(v2[i].print());
            /*
            Tuple<ArrayList, ArrayList, ArrayList> resultAC = jpg.getACEncoding(YQIntMatrix, CbQIntMatrix, CrQIntMatrix);
            Tuple<int[], int[], int[]> resultDC = jpg.getDCEncoding(YQIntMatrix, CbQIntMatrix, CrQIntMatrix);
            JPEGMarkersWriter markerwr = new JPEGMarkersWriter(dimXY[0], dimXY[1], JPEGUtility.NO_SUBSAMPLING);
            BinaryWriter bw = new BinaryWriter(File.Open(pathOutFile, FileMode.Create));
            int[] YDC = resultDC.Item1;
            ArrayList YAC = resultAC.Item1;
            printDC(YDC,"************ Y DC *************");
            Console.WriteLine();
            printAC(YAC, "************ Y AC *************");
            markerwr.WriteJPEGFile(bw, YDC, resultDC.Item2, resultDC.Item3, resultAC.Item1, resultAC.Item2, resultAC.Item3);
            readFile(pathOutFile);
            */

            /*
            Tuple<byte[,], byte[,], byte[,]> rgbResult = jpg.getRGBMatrix(path);
            byte[,] RMatrix = rgbResult.Item1;
            byte[,] GMatrix = rgbResult.Item2;
            byte[,] BMatrix = rgbResult.Item3;
            //jpg.printMatriciRGB(RMatrix, GMatrix, BMatrix, RMatrix.GetLength(0), RMatrix.GetLength(1));
            Tuple<float[,], float[,], float[,]> yCbCrResult = jpg.getYCbCrMatrix(RMatrix, GMatrix, BMatrix);
            float[,] YMatrix = yCbCrResult.Item1;
            float[,] CbMatrix = yCbCrResult.Item2;
            float[,] CrMatrix = yCbCrResult.Item3;
            int rows = YMatrix.GetLength(0);
            int columns = YMatrix.GetLength(1);
            //jpg.printMatriciYCbCr(YMatrix, CbMatrix, CrMatrix,rows,columns);
            //Subsampling
            Tuple<float[,], float[,]> chromaResult = jpg.get420Subsampling(CbMatrix, CrMatrix, JPEGEncoder.COPY_BLOCK_PADDING);
            float[,] CbMatrixSub = chromaResult.Item1;
            float[,] CrMatrixSub = chromaResult.Item2;
            Console.WriteLine("SUBSAMPLING");
            //jpg.printMatriciYCbCr(YMatrix, CbMatrixSub, CrMatrixSub, rows, columns);
            //DCT TEST
            double[,] YDMatrix = new double[rows, columns]; 
            double[,] CbDMatrix = new double[rows, columns];
            double[,] CrDMatrix = new double[rows, columns];
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < columns; j++)
                {
                    YDMatrix [i,j] = (double) YMatrix[i,j];
                    CbDMatrix[i, j] = (double) CbMatrix[i, j];
                    CrDMatrix[i, j] = (double) CrMatrix[i, j];
                }
            JPEGDebugger debug = new JPEGDebugger();
            debug.DCTDebbugging(YMatrix, CbMatrixSub, CrMatrixSub, JPEGEncoder.SUBSAMPLING_420, JPEGEncoder.COPY_BLOCK_PADDING);

            //sunsampling 420 o 422 
            /*
            Tuple<float[,], float[,]> chromaResult = jpg.get422Subsampling(CbMatrix, CrMatrix, subsamplingType);
            float[,] CbMatrixSub = chromaResult.Item1;
            float[,] CrMatrixSub = chromaResult.Item2;
            /*
            Console.WriteLine("SUBSAMPLING Cb");
            jpg.printMatrice(CbMatrixSub, CbMatrixSub.GetLength(0), CbMatrixSub.GetLength(1));
            Console.WriteLine("SUBSAMPLING Cr");
            jpg.printMatrice(CrMatrixSub, CrMatrixSub.GetLength(0), CrMatrixSub.GetLength(1));
            //classe per fare debug 
            JPEGDebugger debug = new JPEGDebugger();
            Boolean subsamplingOk = debug.Subsampling422Debugger(CbMatrixSub, CrMatrixSub, CbMatrix, CrMatrix, subsamplingType);
            Console.WriteLine("chroma sub tipo " + subsamplingType + " ok? " + subsamplingOk);
            */

            /*
            //Test DCT2D on single block
            double[,] CbMatrixTest8x8 = new double[8,8];
            for (int i=0; i<8; i++)
                for (int j=0; j<8; j++)
                {
                    CbMatrixTest8x8[i,j] = (double) CbMatrixSub[i,j];
                }
            Console.WriteLine("+++ Blocco 0,0 test matrice Cb +++");
            jpg.printMatrice(CbMatrixTest8x8, 8, 8);
            DCT dct = new DCT(8, 8);
            double[,] CbMatrixTest8x8Result = dct.DCT2D(CbMatrixTest8x8);
            Console.WriteLine("+++ Blocco 0,0 test DCT +++");
            jpg.printMatrice(CbMatrixTest8x8Result, 8, 8);
            */

            /*
            //Test DCT2D on single block
            double[,] CbMatrixTest8x8 = new double[8, 8];
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                {
                    CbMatrixTest8x8[i, j] = (double)CbMatrixSub[i, j];
                }
            Console.WriteLine("+++ Blocco 0,0 test matrice Cb +++");
            jpg.printMatrice(CbMatrixTest8x8, 8, 8);
            DCT dct = new DCT(8, 8);
            double[,] CbMatrixTest2x2Result = dct.DCT2D(CbMatrixTest8x8);
            Console.WriteLine("+++ Blocco 0,0 test DCT +++");
            jpg.printMatrice(CbMatrixTest2x2Result, 8, 8);
            double[,] CbMatrixTestInverse = dct.IDCT2D(CbMatrixTest2x2Result);
            Console.WriteLine("+++ Blocco 0,0 test inverse DCT +++");
            jpg.printMatrice(CbMatrixTestInverse, 8, 8);
            */

            /*
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
            */

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

            
        }

        public static Bitmap ConvertToBitmap(string fileName)
        {
            Bitmap bitmap;
            using (Stream bmpStream = System.IO.File.Open(fileName, System.IO.FileMode.Open))
            {
                Image image = Image.FromStream(bmpStream);
                bitmap = new Bitmap(image);
            }
            return bitmap;
        }

        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {

            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }


        private static byte[] imageToArray(Bitmap b)
        {
            using(MemoryStream mem = new MemoryStream())
            {
                b.Save(mem, ImageFormat.Bmp);
                return mem.ToArray();
            }
        }




        private static byte[] toByteArray(byte[,] M)
        {
            int rows = M.GetLength(0);
            int columns = M.GetLength(1);
            int dim = rows * columns;
            byte[] v = new byte[rows * columns];
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < columns; j++)
                    v[(i * columns) + j] = M[i, j];
            return v;
        }


        private static byte ConvertToByte(bool[] bools)
        {
            byte b = 0;

            byte bitIndex = 0;
            for (int i = 0; i < 8; i++)
            {
                if (bools[i])
                {
                    b |= (byte)(((byte)1) << 7 - bitIndex);
                }
                bitIndex++;
            }

            return b;
        }

        private static void readFile(string inputFilename)
        {
            byte[] fileBytes = File.ReadAllBytes(inputFilename);
            
            StringBuilder sb = new StringBuilder();
            foreach (UInt16 b in fileBytes)
            {
                string hex = string.Format("{0:X2}", Convert.ToString(b, 2).PadLeft(8, '0'));
                sb.Append(hex);
            }
            
            string outputFilename = "C:\\Users\\Giuseppe\\OneDrive\\Documenti\\Progetto_Teoria_Informazione\\jpegtest\\TEST_JPEG_HEX.txt";
            //File.WriteAllText(outputFilename, BitConverter.ToString(fileBytes).Replace("-", ""));
            string s = "";
            for (int i = 0; i < fileBytes.Length; i++)
                s += Convert.ToString(fileBytes[i],2).PadLeft(8,'0'); 
            File.WriteAllText(outputFilename, s);
            /*
            
            FileStream inFile = File.Open(pathOut, FileMode.Open);
            using (BinaryReader rd = new BinaryReader(inFile))
            {
                UInt16 hexIn = rd.ReadUInt16();
                string hex = string.Format("{0:X2}", hexIn);
                Console.Write(hex + " ");
            }

            
        }

        private static Int16[] toIntArray(int [,] Matrix)
        {
            int rows = Matrix.GetLength(0);
            int columns = Matrix.GetLength(1);
            int dim = rows * columns;
            Int16[] v = new Int16[rows * columns];
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < columns; j++)
                    v[(i * columns) + j] = (Int16) Matrix[i, j];
            return v;
        }

        private static SByte[] toSByteArray(int[,] Matrix)
        {
            int rows = Matrix.GetLength(0);
            int columns = Matrix.GetLength(1);
            int dim = rows * columns;
            SByte[] v = new SByte[rows * columns];
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < columns; j++)
                    v[(i * columns) + j] = (SByte)Matrix[i, j];
            return v;
        }

        private static SByte[] toSByteArray(double[,] Matrix)
        {
            int rows = Matrix.GetLength(0);
            int columns = Matrix.GetLength(1);
            int dim = rows * columns;
            SByte[] v = new SByte[rows * columns];
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < columns; j++)
                    v[(i * columns) + j] = (SByte)Matrix[i, j];
            return v;
        }

        private static SByte[] toSByteArray(float[,] Matrix)
        {
            int rows = Matrix.GetLength(0);
            int columns = Matrix.GetLength(1);
            int dim = rows * columns;
            SByte[] v = new SByte[rows * columns];
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < columns; j++)
                    v[(i * columns) + j] = (SByte)Matrix[i, j];
            return v;
        }


        private static void printDC(Int16[] dc, string message)
        {
            Console.WriteLine(message);
            for (int i=0; i<dc.Length; i++)
            {
                Console.Write(dc[i] + " ");
            }
        }

        private static void printAC(ArrayList ac, string message)
        {
            Console.WriteLine(message);
            for (int i = 0; i < ac.Count; i++)
            {
                int[] acBlock = (int[])ac[i];
                for (int j = 0; j < acBlock.Length; j++)
                {
                    Console.Write(acBlock[j] + " ");
                }
                Console.WriteLine("\n");
            }
        }
        */
    }
}
