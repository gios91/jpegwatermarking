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
        
        public static void Main(string[] args)
        {
            byte EOB = 77;
            string path = "C:\\Users\\Giuseppe\\OneDrive\\Documenti\\Progetto_Teoria_Informazione\\jpegtest\\bluesky.bmp";
            string pathOutFile = "C:\\Users\\Giuseppe\\OneDrive\\Documenti\\Progetto_Teoria_Informazione\\jpegtest\\output_TEST_JPEGENCODER.jpg";
            JPEGWatermarkerIF wm = new JPEGWatermarker();
            
            Bitmap b = new Bitmap(path);

            JPEGEncoderIF jpg = new JPEGEncoder(path);
            int[] dimXY = jpg.getImageDimensions();
            Tuple<byte[,], byte[,], byte[,]> rgbResult = jpg.getRGBMatrix(path);
            byte[,] RMatrix = rgbResult.Item1;
            byte[,] GMatrix = rgbResult.Item2;
            byte[,] BMatrix = rgbResult.Item3;
            Console.WriteLine("******** RGB INIZIALI ********");
            //jpg.printMatriciRGB(RMatrix, GMatrix, BMatrix, RMatrix.GetLength(0), RMatrix.GetLength(1));
            //codifica LZ78 di un testo da file

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

            byte[] finalStream = wm.getDictFinalStream(dictArray, dictNewCharsArray);
            Console.WriteLine("dict dim = {0}", finalStream.Length);

            //inserimento della codifica nell'immagine
            Tuple<byte[,], byte[,], byte[,]> rgbResultW = wm.doRGBWatermarking(RMatrix, GMatrix, BMatrix, finalStream);
            Console.WriteLine("******** RGB WATERMARKED ********");
            byte[,] RMatrixW = rgbResultW.Item1;
            byte[,] GMatrixW = rgbResultW.Item2;
            byte[,] BMatrixW = rgbResultW.Item3;
            //jpg.printMatriciRGB(RMatrixW, GMatrixW, BMatrixW, RMatrixW.GetLength(0), RMatrixW.GetLength(1));

            /*
            Console.WriteLine("************* DECODIFICA **************");
            byte[] bres = wm.decodeRGBWatermarking(RMatrixW, GMatrixW, BMatrixW);
            for (int i = 0; i < bres.Length; i++)
                Console.Write(bres[i] + " ");
            */

            //scrittura jpeg watermarked
            //DA RGB A JPEG: CODIFICA

            Bitmap newb = new Bitmap(path);
            jpg.setRGBMatrix(RMatrixW, GMatrixW, BMatrixW, ref newb);
            ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);

            // Create an Encoder object based on the GUID
            // for the Quality parameter category.
            System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;

            // Create an EncoderParameters object.
            // An EncoderParameters object has an array of EncoderParameter
            // objects. In this case, there is only one
            // EncoderParameter object in the array.
            EncoderParameters myEncoderParameters = new EncoderParameters(1);
            Int64 quality = 100L;
            EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, quality);
            myEncoderParameters.Param[0] = myEncoderParameter;
            newb.Save(pathOutFile + "_test_RGB_watermarking.jpg", jpgEncoder, myEncoderParameters);

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
    }
}
