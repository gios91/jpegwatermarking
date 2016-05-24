using FreeImageAPI;
using JPEGEncoding;
using JPEGWatermarking;
using LZ78Encoding;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace JPEGWatermarkingWeb.Controller
{
    public static class PageController
    {
        //riferimento alle immagini di input e watermarked
        public static FIBITMAP inputImage = new FIBITMAP();
        public static FIBITMAP inputImageToDecode = new FIBITMAP();
        public static Bitmap jpegToSerialize;
        public static Bitmap decodedJpeg;

        public static float mse;
        public static float psnr; 

        public static string getLZ78EncodingString(string textToEncode)
        {
            Tuple<byte[], int, int, string> dictEncodingResult = Workflow.encodingWatermarkingText(textToEncode);
            string result = dictEncodingResult.Item4;
            return result;
        }

        public static void initializeModules()
        {
            Workflow.initModules();
        }

        internal static string[] getJpegQualityParamsItems()
        {
            string[] jpegQualityFlags = JPEGUtility.getJpegQualityParamsStringItems();
            return jpegQualityFlags;
        }

        internal static string[] getJpegChromaSubItems()
        {
            string[] jpegChromaSubFlags = JPEGUtility.getJpegChromaSubParamsStringItems();
            return jpegChromaSubFlags;
        }

        internal static void codificaToJpeg(string pathInputImage, string pathOutputImage, string selectedJpegQualityTag, string selectedChromaSubTag)
        {
            //Workflow.setInputImagePath(pathInputImage);
            readInputImage(pathInputImage);
            //Workflow.setOutputJpegImagePath(pathOutputImage);
            //Workflow.setJpegQualityTag(JPEGUtility.stringToQualityParam(selectedJpegQualityTag));
            //Workflow.setChromaSubTag(JPEGUtility.stringToChromaSubParam(selectedChromaSubTag));
            jpegEncoding(pathOutputImage, JPEGUtility.stringToQualityParam(selectedJpegQualityTag), JPEGUtility.stringToChromaSubParam(selectedChromaSubTag));
        }

        private static void jpegEncoding(string outputImagePath, FREE_IMAGE_SAVE_FLAGS jpegQuality, FREE_IMAGE_SAVE_FLAGS chromaSubsamplingType)
        {
            //void encodeToJpeg(FIBITMAP dib, string pathOutput, FREE_IMAGE_SAVE_FLAGS jpegQuality, FREE_IMAGE_SAVE_FLAGS jpegSubsampling);
            FreeImage.Save(FREE_IMAGE_FORMAT.FIF_JPEG, inputImage, outputImagePath, jpegQuality | chromaSubsamplingType);
            /*
            if (!inputImage.IsNull)
                FreeImage.Unload(inputImage);       //disallocare la memoria occupata dal BMP
            */

            //carico il jpeg scritto con la classe Image per semplificare la serializzazione
            //jpegFromInputImage = FreeImage.Load(FREE_IMAGE_FORMAT.FIF_JPEG, outputImagePath, FREE_IMAGE_LOAD_FLAGS.JPEG_ACCURATE);

            using (Stream jpegStream = System.IO.File.Open(outputImagePath, System.IO.FileMode.Open))
            {
                System.Drawing.Image read = System.Drawing.Image.FromStream(jpegStream);
                jpegToSerialize = new Bitmap(read);
            }
            //inserire eventuale stampa delle matrici e della codifica DC-AC del jpeg
        }

        public static bool checkLuminanceRGBWatermarkingChosen(string selectedWatermarkingMethod)
        {
            if (JPEGUtility.stringToWaterMethod(selectedWatermarkingMethod) == Workflow.LUMINANCE_RGB_WATERMARKING)
                return true;
            return false;
        }
        
        internal static bool doWatermarkingDecoding()
        {
            bool decodificabile = Workflow.decodeWatermarking();
            return decodificabile;
        }

        public static bool checkAdvRGBWatermarkingChosen(string selectedWatermarkingMethod)
        {
            if (JPEGUtility.stringToWaterMethod(selectedWatermarkingMethod) == Workflow.ADVANCED_RGB_WATERMARKING)
                return true;
            return false;
        }

        internal static string getDecodedWatermark()
        {
            string decodedWatermark = Workflow.getDecodedWatermarking();
            return decodedWatermark;
        }

        internal static string getImageQualityStats()
        {
            getMSEAndPSNR();
            StringBuilder sb = new StringBuilder();
            sb.Append(">> *** Info qualità dell'immagine watermarked  *** \n");
            sb.Append(String.Format(" Val. MSE =  {0} \n", mse));
            sb.Append(String.Format(" Val. PSNR = {0} \n", psnr));
            return sb.ToString();
        }

        internal static void singleError(double alpha)
        {
            Workflow.singleError(alpha);
        }

        internal static void burstError(double p, double r)
        {
            Workflow.burstError(p, r);
        }

        internal static bool decodeTrasmittedImage()
        {
            Workflow.ripetizioneDecoding();
            decodedJpeg = Workflow.deserializeJpegImage();
            if (decodedJpeg == null)
                return false;
            return true;
        }

        /*
        public static int stringToWaterMethod(string watermarkingMethodItem)
        {
            if (watermarkingMethodItem == "Advanced RGB watermarking")
                return Workflow.ADVANCED_RGB_WATERMARKING;
            else if (watermarkingMethodItem == "Luminance RGB watermarking")
                return Workflow.LUMINANCE_RGB_WATERMARKING;
            return -1;
        }
        */

        public static bool doLuminanceRGBWatermarking(string pathOutputWatermarkedImage, int numLSBSelectedBlock, int numLSBNonSelectedBlock)
        {
            Tuple<byte[,], byte[,], byte[,]> watermarkingResult = Workflow.doLuminanceRGBWatermarking(jpegToSerialize, numLSBSelectedBlock, numLSBNonSelectedBlock);
            if (watermarkingResult == null)
                return false;
            byte[,] RMatrixWater = watermarkingResult.Item1;
            byte[,] GMatrixWater = watermarkingResult.Item2;
            byte[,] BMatrixWater = watermarkingResult.Item3;
            Workflow.setWatermarkedRGBToJpeg(RMatrixWater, GMatrixWater, BMatrixWater, ref jpegToSerialize);
            writeWatermarkedJpeg(pathOutputWatermarkedImage);
            printSelectedBlockImage();
            return true;
        }

        private static void printSelectedBlockImage()
        {
            Bitmap selectedBlockImage = new Bitmap(jpegToSerialize);
            List<int[]> selBlock = Workflow.getSelectedBlocks(jpegToSerialize);
            int dimX = selectedBlockImage.Width;
            int dimY = selectedBlockImage.Height;
            for (int i=0; i< dimX; i+=8) 
                for (int j=0; j<dimY; j+=8)
                {
                    int[] row = new int[2];
                    row[0] = i; row[1] = j;
                    if(isContained(selBlock,row))
                    {
                        setWhiteBlock(ref selectedBlockImage, j, i);
                    }
                    else
                    {
                        setBlackBlock(ref selectedBlockImage, j, i);
                    }
                }
            selectedBlockImage.Save(HttpContext.Current.Server.MapPath("img/luminanceSelBlockImage.jpg"), ImageFormat.Jpeg);
        }

        private static void setWhiteBlock(ref Bitmap image, int x, int y)
        {
            for (int i = x; i < x+8; i++)
                for (int j = y; j < y+8; j++)
                {
                    image.SetPixel(i, j, Color.White);
                }
        }

        private static void setBlackBlock(ref Bitmap image, int x, int y)
        {
            for (int i = x; i < x + 8; i++)
                for (int j = y; j < y + 8; j++)
                {
                    image.SetPixel(i, j, Color.Black);
                }
        }

        private static bool isContained(List<int[]> blockSequence, int[] colRow)
        {
            foreach (int[] idx in blockSequence)
                if (idx[0] == colRow[0] && idx[1] == colRow[1])
                    return true;
            return false;
        }

        public static bool doRGBAdvancedWatermarking(string pathOutputWatermarkedImage)
        {
            Tuple<byte[,], byte[,], byte[,]> watermarkingResult = Workflow.doAdvancedRGBWatermarking(jpegToSerialize);
            if (watermarkingResult == null)
                return false;
            byte[,] RMatrixWater = watermarkingResult.Item1;
            byte[,] GMatrixWater = watermarkingResult.Item2;
            byte[,] BMatrixWater = watermarkingResult.Item3;
            Workflow.setWatermarkedRGBToJpeg(RMatrixWater, GMatrixWater, BMatrixWater, ref jpegToSerialize);
            writeWatermarkedJpeg(pathOutputWatermarkedImage);
            return true;
        }

        internal static void doWatermarking(string selectedWatermarkingMethod, string watermarkedJpegOutputPath)
        {
            int selectedWaterMethod = JPEGUtility.stringToWaterMethod(selectedWatermarkingMethod);
            Tuple<byte[,], byte[,], byte[,]> watermarkingResult = Workflow.writeWatermarkingOnJpegRgb(jpegToSerialize, selectedWaterMethod);
            byte[,] RMatrixWater = watermarkingResult.Item1;
            byte[,] GMatrixWater = watermarkingResult.Item2;
            byte[,] BMatrixWater = watermarkingResult.Item3;
            Workflow.setWatermarkedRGBToJpeg(RMatrixWater, GMatrixWater, BMatrixWater, ref jpegToSerialize);
            writeWatermarkedJpeg(watermarkedJpegOutputPath);
        }

        internal static void doRipetizioneEncoding(int numRipetizioni)
        {
            byte[] imageStream = Workflow.serializeJpegImage(jpegToSerialize);
            Workflow.ripetizioneEncoding(imageStream, numRipetizioni);
        }

        internal static string[] getWatermarkingMethodsItems()
        {
            string[] watermarkingMethods = JPEGUtility.getWatermarkingMethodsItems();
            return watermarkingMethods;
        }

        public static void getMSEAndPSNR()
        {
            //calcolo indici di qualità dell'img watermarked
            Tuple<byte[,], byte[,], byte[,]> rgbOriginalImage = Workflow.getRGBMatrixFI(inputImage);
            byte[,] RMatrix = rgbOriginalImage.Item1;
            byte[,] GMatrix = rgbOriginalImage.Item2;
            byte[,] BMatrix = rgbOriginalImage.Item3;

            Tuple<byte[,], byte[,], byte[,]> rgbWatermarkedImage = Workflow.getRGBMatrix(jpegToSerialize);
            byte[,] RMatrixWater = rgbWatermarkedImage.Item1;
            byte[,] GMatrixWater = rgbWatermarkedImage.Item2;
            byte[,] BMatrixWater = rgbWatermarkedImage.Item3;

            Tuple<float[,], float[,], float[,]> yCbCrOriginalImage = Workflow.getYCbCrMatrix(RMatrix, GMatrix, BMatrix);
            float[,] YMatrix = yCbCrOriginalImage.Item1;

            Tuple<float[,], float[,], float[,]> yCbCrWatermarkedImage = Workflow.getYCbCrMatrix(RMatrixWater, GMatrixWater, BMatrixWater);
            float[,] YMatrixWater = yCbCrWatermarkedImage.Item1;

            mse = WatermarkingTestUtility.getMSE(YMatrix, YMatrixWater);
            psnr = WatermarkingTestUtility.getPSNR(mse);
        }

        public static void readInputImage(string inputImagePath)
        {
            if (!File.Exists(inputImagePath))
            {
                Console.WriteLine(inputImagePath + " non può essere caricato. Errore.");
                return;
            }
            inputImage = FreeImage.Load(FREE_IMAGE_FORMAT.FIF_BMP, inputImagePath, FREE_IMAGE_LOAD_FLAGS.DEFAULT);
        }

        public static void readInputImageToDecode(string inputImagePath)
        {
            if (!File.Exists(inputImagePath))
            {
                Console.WriteLine(inputImagePath + " non può essere caricato. Errore.");
                return;
            }
            inputImageToDecode = FreeImage.Load(FREE_IMAGE_FORMAT.FIF_JPEG, inputImagePath, FREE_IMAGE_LOAD_FLAGS.DEFAULT);
        }


        public static void writeWatermarkedJpeg(string outputWatermarkedImagePath)
        {
            jpegToSerialize.Save(outputWatermarkedImagePath, ImageFormat.Jpeg);
        }

        public static void writeDecodedJpeg(string outputDecodedImagePath)
        {
            decodedJpeg.Save(outputDecodedImagePath, ImageFormat.Jpeg);
        }

        public static int getNumBitImage()
        {
            return (Int32) Workflow.numBitImage;
        }

        public static int getNumBitWatermark()
        {
            return (Int32) Workflow.numBitWatermarking;
        }

        public static int getNumBitAvailableForWatermarking()
        {
            return (Int32) Workflow.numAvailableBitImage;
        }

        public static double getNumBitWatermarkOnBitAvailable()
        {
            return Workflow.numBitWatermarking / Workflow.numAvailableBitImage;
        }

        public static double getNumBitWatermarkOnBitImage()
        {
            return Workflow.numBitWatermarking / Workflow.numBitImage;
        }

        public static int getMSE()
        {
            return (Int32)WatermarkingTestUtility.mseValue;
        }

        public static int getPSNR()
        {
            return (Int32)WatermarkingTestUtility.psnrValue;
        }

        public static string readFromFile(string pathFile)
        {
            string s = string.Empty;
            using (var reader = new StreamReader(pathFile))
            {
                string line = string.Empty;
                while ((line = reader.ReadLine()) != null)
                {
                    s += line;
                }
            }
            return Encoding.UTF8.GetString(Encoding.Default.GetBytes(s));
        }

        public static string getWatermarkingNotPossibleStats()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(">> +++ Watermarking Info +++ \n");
            sb.Append(">> numero bit watermarking = " + Workflow.numBitWatermarking + " bit \n");
            sb.Append(">> numero bit immagine = " + Workflow.numBitImage + " bit \n");
            if (Workflow.chosenWatermarkingType == Workflow.ADVANCED_RGB_WATERMARKING)
            {
                sb.Append(">> numero bit livello 4 RGB = " + Workflow.numBitLevel4 + " bit \n");
                sb.Append(String.Format(">> max bit disponibili per watermarking [Lvl 4] / bit watermarking (perc.) =  {0:0.00%} \n", (Workflow.numBitLevel4 / Workflow.numBitWatermarking)));
            }
            else if (Workflow.chosenWatermarkingType == Workflow.LUMINANCE_RGB_WATERMARKING)
            {
                sb.Append(">> numero blocchi Y selezionati = " + Workflow.numSelectedBlock + " \n");
                sb.Append(">> numero blocchi Y non selezionati = " + Workflow.numNonSelectedBlock + " \n");
                sb.Append(String.Format(">> numero blocchi Y selezionati / numero blocchi Y Non selezionati (perc.) =  {0:0.00%} \n", (Workflow.numSelectedBlock / Workflow.numNonSelectedBlock)));
                sb.Append(String.Format(">> max bit disponibili per watermarking [Selected Block LSB = " + Workflow.numLSBSelectedBlock + ", Not Selected Block LSB = " + Workflow.numLSBNonSelectedBlock + "] = {0} \n", Workflow.numAvailableBitImage));
                sb.Append(String.Format(">> max bit disponibili per watermarking [Selected Block LSB = " + Workflow.numLSBSelectedBlock + ", Not Selected Block LSB = " + Workflow.numLSBNonSelectedBlock + "] / bit watermarking (perc.) =  {0:0.00%} \n", (Workflow.numAvailableBitImage / Workflow.numBitWatermarking)));
            }
            sb.Append(">> * Tecnica watermarking = " + Workflow.getWatermarkingMethod() + "\n");
            return sb.ToString();
        }

        public static string getWatermarkingStats()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(">> +++ Watermarking Info +++ \n");
            sb.Append(">> * Tecnica watermarking = " + Workflow.getWatermarkingMethod() + "\n");
            sb.Append(">> numero bit watermarking = "+ Workflow.numBitWatermarking + " bit \n");
            sb.Append(">> numero bit disponibili per watermarking = " + Workflow.numAvailableBitImage + " bit \n");
            sb.Append(">> numero bit immagine = " + Workflow.numBitImage + " bit \n");
            sb.Append(String.Format(">> bit watermarking / bit disponibili per watermarking (perc.) =  {0:0.00%} \n", (Workflow.numBitWatermarking / Workflow.numAvailableBitImage)));
            sb.Append(String.Format(">> bit watermarking / bit totali immagine (perc.) =  {0:0.00%} \n", (Workflow.numBitWatermarking / Workflow.numBitImage)));
            if (Workflow.chosenWatermarkingType == Workflow.LUMINANCE_RGB_WATERMARKING)
            {
                sb.Append(String.Format(">> max bit disponibili per watermarking [Selected Block LSB = " + Workflow.numLSBSelectedBlock + ", Not Selected Block LSB = " + Workflow.numLSBNonSelectedBlock + "] = {0} \n", Workflow.numAvailableBitImage));
                sb.Append(String.Format(">> max bit disponibili per watermarking [Selected Block LSB = " + Workflow.numLSBSelectedBlock + ", Not Selected Block LSB = " + Workflow.numLSBNonSelectedBlock + "] / bit watermarking (perc.) =  {0:0.00%} \n", (Workflow.numAvailableBitImage / Workflow.numBitWatermarking)));
                sb.Append(">> numero blocchi Y selezionati = " + Workflow.numSelectedBlock + " \n");
                sb.Append(">> numero blocchi Y non selezionati = " + Workflow.numNonSelectedBlock + " \n");
                sb.Append(String.Format(">> numero blocchi Y selezionati / numero blocchi Y Non selezionati (perc.) =  {0:0.00%} \n", (Workflow.numSelectedBlock / Workflow.numNonSelectedBlock)));
                sb.Append(">> numero bit LSB per blocchi Y selezionati = " + Workflow.numLSBSelectedBlock + " \n");
                sb.Append(">> numero bit LSB per blocchi Y non selezionati = " + Workflow.numLSBNonSelectedBlock + " \n");
            }
            else if (Workflow.chosenWatermarkingType == Workflow.ADVANCED_RGB_WATERMARKING)
            {
                sb.Append(">> numero livello LSB necessario per watermarking = " + Workflow.numRGBWatermarkingLevel + " Lvl \n");
            }
            return sb.ToString();
        }

        public static string getChannelErrorStats()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(">> +++ Channel Error Info +++ \n");
            sb.Append(String.Format(">> codifica di canale = R({0},1) \n", Workflow.numRipetizioni));
            if (Workflow.chosenChannelErrorModel == Workflow.SINGLE_ERROR_CHANNEL)
            {
                sb.Append(">> tipo di errore di canale = SINGLE ERROR \n");
                sb.Append(">> alpha = " + Workflow.alphaChError + " \n");
                double numBitStream = Workflow.channelCodedStream.Count;
                double numBitError = Workflow.channelError.getNumBitSingleError();
                sb.Append(String.Format(">> num bit alterati = {0} \n", numBitError));
                sb.Append(String.Format(">> bit alterati / bit stream (perc.) =  {0:0.00%} \n", (numBitError / numBitStream)));
            }
            else if (Workflow.chosenChannelErrorModel == Workflow.GILBERT_ELLIOT_BURST_CHANNEL)
            {
                sb.Append(">> tipo di errore di canale = GILBERT ELLIOT BURST ERROR \n");
                sb.Append(">> p = " + Workflow.pValue + " \n");
                sb.Append(">> r = " + Workflow.rValue + " \n");
                double numBitStream = Workflow.channelCodedStream.Count;
                double numBitError = Workflow.getNumBitErrorBurst();    //numero di bit ad 1 del burst noise vector
                sb.Append(String.Format(">> bit alterati / bit stream (perc.) =  {0:0.00%} \n", (numBitError / numBitStream)));
                sb.Append(">> Forma del vettore burst (primi 100 bit): \n " + Workflow.getGilbertBurstNoiseVectorAsString() + " \n");
            }
            return sb.ToString();
        }
    }
}