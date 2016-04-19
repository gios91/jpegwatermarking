using FreeImageAPI;
using JPEGEncoding;
using LZ78Encoding;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace JPEGWatermarking
{
    class TestWatermarking
    {
        const string path = "C:\\Users\\Giuseppe\\OneDrive\\Documenti\\Progetto_Teoria_Informazione\\jpegtest\\Google3.bmp";
        const string outFileName = "C:\\Users\\Giuseppe\\OneDrive\\Documenti\\Progetto_Teoria_Informazione\\jpegtest\\Google3_nosub.jpg";

        static FIBITMAP dib = new FIBITMAP();
        string message = null;
        static JPEGEncoderIF jpg = new JPEGEncoder(dib);

        /*

        public static void Main(string[] args)
        {
            byte EOB = 77;            
            if (!dib.IsNull)
                FreeImage.Unload(dib);
            dib = FreeImage.Load(FREE_IMAGE_FORMAT.FIF_BMP, path, FREE_IMAGE_LOAD_FLAGS.DEFAULT);
            
            //int[] dimXY = jpg.getImageDimensions();
            Tuple<byte[,], byte[,], byte[,]> rgbResult = jpg.getRGBMatrixFI(dib);
            byte[,] RMatrix = rgbResult.Item1;
            byte[,] GMatrix = rgbResult.Item2;
            byte[,] BMatrix = rgbResult.Item3;
            Console.WriteLine("******** RGB INIZIALI ********");
            //jpg.printMatriciRGB(RMatrix, GMatrix, BMatrix, RMatrix.GetLength(0), RMatrix.GetLength(1));

            //YCBCR
            int rows = RMatrix.GetLength(0);
            int columns = RMatrix.GetLength(1);
            Tuple<float[,], float[,], float[,]> yCbCrResult = jpg.getYCbCrMatrix(RMatrix, GMatrix, BMatrix);
            float[,] YMatrix = yCbCrResult.Item1;
            float[,] CbMatrix = yCbCrResult.Item2;
            float[,] CrMatrix = yCbCrResult.Item3;
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
            Tuple<double[,], double[,], double[,]> DCTResult = jpg.getDCTMatrices(YDMatrix, CbDMatrix, CrDMatrix,
                JPEGUtility.NO_SUBSAMPLING, JPEGUtility.ZERO_BLOCK_PADDING);
            double[,] YDCTMatrix = DCTResult.Item1;
            double[,] CbDCTMatrix = DCTResult.Item2;
            double[,] CrDCTMatrix = DCTResult.Item3;

            Tuple<double[,], double[,], double[,]> test = jpg.modificaDCT(YDCTMatrix, CbDCTMatrix, CrDCTMatrix);
            Console.WriteLine("Y DCT modificata");
            jpg.printMatrice(test.Item1);
            
            //Da DCT a IDCT
            Tuple<double[,], double[,], double[,]> IDCTResult = jpg.getIDCTMatrices(test.Item1, test.Item2, test.Item3);
            double[,] YIDCTMatrix = IDCTResult.Item1;
            double[,] CbIDCTMatrix = IDCTResult.Item2;
            double[,] CrIDCTMatrix = IDCTResult.Item3;

            //Da IDCT (YCbCR double) ad YCbCr float
            float[,] YMatrixF = new float[rows, columns];
            float[,] CbMatrixF = new float[rows, columns];
            float[,] CrMatrixF = new float[rows, columns];
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < columns; j++)
                {
                    YMatrixF[i, j] = (float)YIDCTMatrix[i, j];
                    CbMatrixF[i, j] = (float)CbIDCTMatrix[i, j];
                    CrMatrixF[i, j] = (float)CrIDCTMatrix[i, j];
                }

            //Da YCbCr float ad RGB da scrivere sul JPEG
            Tuple<byte[,], byte[,], byte[,]> ycbcrtorgb = jpg.getRGBFromYCbCr(YMatrixF, CbMatrixF, CrMatrixF);
            byte[,] RMatrixToWr = ycbcrtorgb.Item1;
            byte[,] GMatrixToWr = ycbcrtorgb.Item2;
            byte[,] BMatrixToWr = ycbcrtorgb.Item3;

            Console.WriteLine("R");
            jpg.printMatrice(RMatrix);

            Console.WriteLine("R MODIFICATO");
            jpg.printMatrice(RMatrixToWr);
            
            //scrivere il JPEG da RGB modificate
            jpg.setRGBMatrix(RMatrixToWr, GMatrixToWr, BMatrixToWr, ref dib);

            FreeImage.Save(FREE_IMAGE_FORMAT.FIF_JPEG, dib, outFileName, FREE_IMAGE_SAVE_FLAGS.JPEG_SUBSAMPLING_444 | FREE_IMAGE_SAVE_FLAGS.JPEG_QUALITYSUPERB);

            //lettura JPEG watermarked
            FIBITMAP watermarkedJpeg = FreeImage.Load(FREE_IMAGE_FORMAT.FIF_JPEG, outFileName, FREE_IMAGE_LOAD_FLAGS.JPEG_ACCURATE);

            Tuple<byte[,], byte[,], byte[,]> rgbwatermarked = jpg.getRGBMatrixFI(watermarkedJpeg);
            byte[,] RMatrixWater = rgbwatermarked.Item1;
            byte[,] GMatrixWater = rgbwatermarked.Item2;
            byte[,] BMatrixWater = rgbwatermarked.Item3;


            Console.WriteLine("R WATERM");
            jpg.printMatrice(RMatrixWater);


            //Da RGB water a Ycbcr water
            Tuple<float[,], float[,], float[,]> yCbCrWater = jpg.getYCbCrMatrix(RMatrixWater, GMatrixWater, BMatrixWater);
            float[,] YMatrixWater = yCbCrWater.Item1;
            float[,] CbMatrixWater = yCbCrWater.Item2;
            float[,] CrMatrixWater = yCbCrWater.Item3;
            double[,] YDMatrixWater = new double[rows, columns];
            double[,] CbDMatrixWater = new double[rows, columns];
            double[,] CrDMatrixWater = new double[rows, columns];
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < columns; j++)
                {
                    YDMatrixWater[i, j] = (double)YMatrixWater[i, j];
                    CbDMatrixWater[i, j] = (double)CbMatrixWater[i, j];
                    CrDMatrixWater[i, j] = (double)CrMatrixWater[i, j];
                }
            Tuple<double[,], double[,], double[,]> DCTResultWater = jpg.getDCTMatrices(YDMatrixWater, CbDMatrixWater, CrDMatrixWater,
                JPEGUtility.NO_SUBSAMPLING, JPEGUtility.ZERO_BLOCK_PADDING);
            double[,] YDCTMatrixRESULT = DCTResultWater.Item1;
            double[,] CbDCTMatrixRESULT = DCTResultWater.Item2;
            double[,] CrDCTMatrixRESULT = DCTResultWater.Item3;

            Console.WriteLine("Y DCT modificata");
            jpg.printMatrice(test.Item1);

            Console.WriteLine("Y DCT LETTA DAL JPEG");
            jpg.printMatrice(YDCTMatrixRESULT);
            
            //codifica LZ78 di un testo da file
            /*
            string pathTesto = "C:\\Users\\Giuseppe\\OneDrive\\Documenti\\Progetto_Teoria_Informazione\\stringhelz78\\riga.txt";
            string s = leggiDaFile(pathTesto);
            Console.WriteLine(s);
            LZ78EncoderIF enc = new LZ78Encoder();
            Tuple<Dictionary<string, int[]>, Dictionary<int, string>> result = enc.getEncoding(s);
            Dictionary<string, int[]> dict = result.Item1;
            Dictionary<int, string> dictNewChars = result.Item2;
            //test serializzazione
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream dictStream = new MemoryStream();
            MemoryStream dictNewCharsStream = new MemoryStream();
            bf.Serialize(dictStream, dict);
            bf.Serialize(dictNewCharsStream, dictNewChars);

            byte[] dictArray = dictStream.ToArray();
            byte[] dictNewCharsArray = dictNewCharsStream.ToArray();

            Console.WriteLine("dict dim = {0}", dictArray.Length);
            Console.WriteLine("dictNewChar dim = {0}", dictNewCharsArray.Length);

            byte[] finalStream = wm.createWatermarkingString(dictArray, dictNewCharsArray);
            Console.WriteLine("dict dim = {0}", finalStream.Length);

            //inserimento della codifica nell'immagine
            Tuple<byte[,], byte[,], byte[,]> rgbResultW = wm.doRGBWatermarking(RMatrix, GMatrix, BMatrix, finalStream);
            Console.WriteLine("******** RGB WATERMARKED ********");
            byte[,] RMatrixW = rgbResultW.Item1;
            byte[,] GMatrixW = rgbResultW.Item2;
            byte[,] BMatrixW = rgbResultW.Item3;
            jpg.printMatriciRGB(RMatrixW, GMatrixW, BMatrixW, RMatrixW.GetLength(0), RMatrixW.GetLength(1));

            //scrittura jpeg watermarked
            //DA RGB A JPEG: CODIFICA

            jpg.setRGBMatrix(RMatrixW, GMatrixW, BMatrixW, ref dib);
            */
            //scrittura JPEG
            /*
            FreeImage.Save(FREE_IMAGE_FORMAT.FIF_JPEG, dib, outFileName, FREE_IMAGE_SAVE_FLAGS.JPEG_SUBSAMPLING_444 | FREE_IMAGE_SAVE_FLAGS.JPEG_QUALITYSUPERB);
            
            //lettura JPEG watermarked
            FIBITMAP waterJpeg = FreeImage.Load(FREE_IMAGE_FORMAT.FIF_JPEG, outFileName, FREE_IMAGE_LOAD_FLAGS.JPEG_ACCURATE);

            Tuple<byte[,], byte[,], byte[,]> rgbResult2 = jpg.getRGBMatrixFI(waterJpeg);
            byte[,] RMatrix2 = rgbResult2.Item1;
            byte[,] GMatrix2 = rgbResult2.Item2;
            byte[,] BMatrix2 = rgbResult2.Item3;
            //jpg.printMatriciRGB(RMatrix2, GMatrix2, BMatrix2, RMatrix2.GetLength(0), RMatrix2.GetLength(1));

            jpg.printMatrice(RMatrix2);

            int EOD = 5;        //deve essere noto al decodificatore
            int EOS = 6;        //deve essere noto al decodificatore

            //Int32 [,] MatrixDiff = printDifferenza(RMatrix, RMatrix2);
            //jpg.printMatrice(MatrixDiff);

            /*
            byte[] bytestreamFromJPEG = wm.getRGBWatermarking(RMatrix2, GMatrix2, BMatrix2, EOS);
            bool equalString = checkEquals(finalStream, bytestreamFromJPEG);

            Console.WriteLine("array originario uguale ad array dopo jpeg? {0}", equalString);
            
        }
        
        public static Int32[,] printDifferenza(byte[,] M, byte[,] N)
        {
            Int32[,] res = new Int32[M.GetLength(0), M.GetLength(1)];
            for (int i=0; i<M.GetLength(0); i++)
                for(int j=0; j<M.GetLength(1); j++)
                {
                    res[i, j] = (Int32) M[i, j] - N[i, j];
                }
            return res;
        }
             
        public static bool checkEquals(byte[]v1, byte[]v2)
        {
            for (int i = 0; i < v1.Length; i++)
                if (v1[i] != v2[i])
                    return false;
            return true;
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

        public static string leggiDaFile(string path)
        {
            int numline = 0;
            string s = string.Empty;
            using (var reader = new StreamReader(path))
            {
                string line = string.Empty;
                while ((line = reader.ReadLine()) != null)
                {
                    s += line;
                    numline++;
                    int x = 0;
                    if (numline == 9)
                    {
                        x = 0;
                    }
                    if (numline == 10)
                        break;
                }
            }
            return s;
        }
        */
    }
}
