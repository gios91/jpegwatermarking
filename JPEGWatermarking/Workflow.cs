using FreeImageAPI;
using JPEGEncoding;
using LZ78Encoding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace JPEGWatermarking
{
    public static class Workflow
    {
        public static LZ78EncoderIF dictEncoder;
        public static LZ78DecoderIF dictDecoder;
        public static JPEGWatermarkerIF waterEncoder;
        public static JPEGEncoderIF jpegEncoder;
        public static JPEGDecoderIF jpegDecoder;
        public static ChannelEncoderIF channelEncoder;
        public static ChannelDecoderIF channelDecoder;
        public static ChannerErrorIF channelError;

        //step watermarking
        public static byte[] watermarking;
        //(parametri di scrittura blocchi del watermarking)
        public static int EOD;
        public static int EOS;

        //parametri per caricamento e salvataggio img
        public static string inputImagePath;
        public static string outputImagePath;
        public static string outputWatermarkedImagePath;

        //parametri per testo da codificare
        public static string inputTextPath;

        //parametri di qualità del jpeg da input img
        public static FREE_IMAGE_SAVE_FLAGS jpegQuality;
        public static FREE_IMAGE_SAVE_FLAGS chromaSubsamplingType;

        //parametri scelta della tecnica di watermarking 
        public static readonly int ADVANCED_RGB_WATERMARKING = 0;
        public static readonly int LUMINANCE_RGB_WATERMARKING = 1;
        public static int chosenWatermarkingType { get; set; }

        //parametri scelta del modello di errore di canale
        public static readonly int SINGLE_ERROR_CHANNEL = 0;
        public static readonly int GILBERT_ELLIOT_BURST_CHANNEL = 1;
        public static int chosenChannelErrorModel { get; set; }

        //parametri per codifica di canale e trasmissione info
        public static int numRipetizioni { get; set; }
        public static double alphaChError { get; set; }
        public static double pValue { get; set; }
        public static double rValue { get; set; }

        //riferimento alle immagini di input e watermarked
        public static FIBITMAP inputImage = new FIBITMAP();
        public static Bitmap jpegToSerialize;
        public static Bitmap decodedJpeg;

        //parametri di trasmissione immagine ed errore
        public static BitArray channelCodedStream;
        public static BitArray receivedNoiseStream { get; set; }
        public static BitArray gilbertElliotNoiseVector { get; set; }
        public static BitArray decodedStream;
        public static byte[] decodedStreamArray;

        //parametri per statistiche
        public static double numBitWatermarking { get; set; }
        public static double numBitImage { get; set; }
        public static double numAvailableBitImage { get; set; }

        //paramtri per advanced rgb watermarking
        public static int numRGBWatermarkingLevel { get; set; }

        //parametri per Luminance RGB watermarking
        public static int numLSBSelectedBlock { get; set; }
        public static int numLSBNonSelectedBlock { get; set;}
        public static double numSelectedBlock { get; set; }
        public static double numNonSelectedBlock { get; set; }
        public static List<int[]> blockSequence { get; set; }
        public static float delta = 1;

        //testo decodificato
        public static string decodedText;

        static Stopwatch stopwatch = new Stopwatch();  //utile per test sui tempi di trasmissione sul canale

        /*
        public static void Main(string[] args)
        {
            inputImagePath = "C:\\Users\\Giuseppe\\OneDrive\\Documenti\\Progetto_Teoria_Informazione\\jpegtest\\Google3.bmp";
            initModules();
            readInputImage();
            //test
            Tuple<byte[,], byte[,], byte[,]> rgb = jpegDecoder.getRGBMatrixFI(inputImage);
            Tuple<float[,], float[,], float[,]> ycbcr = jpegDecoder.getYCbCrMatrix(rgb.Item1, rgb.Item2, rgb.Item3);
            float delta = 1;
            List<int[]> blockSeq = waterEncoder.getBlocksForYWatermarking(ycbcr.Item1, delta);
        }
        */

        public static void setInputImagePath(string pathInputImage)
        {
            inputImagePath = pathInputImage;
        }

        public static void setOutputJpegImagePath(string pathOutputJpegImage)
        {
            outputImagePath = pathOutputJpegImage;
        }

        public static void setJpegQualityTag(FREE_IMAGE_SAVE_FLAGS qualityTag)
        {
            jpegQuality = qualityTag;
        }

        public static void setChromaSubTag(FREE_IMAGE_SAVE_FLAGS chromaSubTag)
        {
            chromaSubsamplingType = chromaSubTag;
        }


        public static void ripetizioneDecoding()
        {
            decodedStream = channelDecoder.RipetizioneDecoding(receivedNoiseStream, numRipetizioni);
            // 9) decodifica watermarking da RGB
            decodedStreamArray = bitArrayToByteArray(decodedStream);
        }
        
        public static Bitmap deserializeJpegImage()
        {
            decodedJpeg = jpegDecoder.deserializeJpegImage(decodedStreamArray);  //se null, l'immagine non può essere decodificata
            return decodedJpeg;
        }
        
        public static void Main(string[] args)
        {

            //insertInputImagePath();
            //insertOutputImagePath();
            //insertInputTextPath();
            //insertWatermarkingMode();
            //insertChannelErrorMode();
            //insertJpegQuality();
            //insertChromaSubsamplingType();
            //insertNumRipetizioni();
            //insertAlphaChannelError();
            //insertNumLSBSelectedLuminanceBlock();
            //insertNumLSBNonSelectedLuminanceBlock();

            chosenChannelErrorModel = SINGLE_ERROR_CHANNEL;
            chosenWatermarkingType = LUMINANCE_RGB_WATERMARKING;
            jpegQuality = FREE_IMAGE_SAVE_FLAGS.JPEG_QUALITYSUPERB;
            chromaSubsamplingType = FREE_IMAGE_SAVE_FLAGS.JPEG_SUBSAMPLING_444;
            numLSBSelectedBlock = 3;
            numLSBNonSelectedBlock = 1;
            numRipetizioni = 5;
            alphaChError = 0.001;

            inputImagePath = "C:\\Users\\Giuseppe\\OneDrive\\Documenti\\Progetto_Teoria_Informazione\\jpegtest\\twitter320x320\\twitter320x320.bmp";
            outputImagePath = "C:\\Users\\Giuseppe\\OneDrive\\Documenti\\Progetto_Teoria_Informazione\\jpegtest\\twitter320x320\\twitter302x320_jpg.jpg";
            outputWatermarkedImagePath = "C:\\Users\\Giuseppe\\OneDrive\\Documenti\\Progetto_Teoria_Informazione\\jpegtest\\twitter320x320\\twitter320x320_watermarked.jpg";
            inputTextPath = "C:\\Users\\Giuseppe\\OneDrive\\Documenti\\Progetto_Teoria_Informazione\\stringhelz78\\primo_canto_ok_12.txt";

            initModules();

            string toEncode = readFromFile(inputTextPath);
            
            // 1) codifica LZ del testo 
            Tuple<byte[], int, int, string> dictEncodingResult = encodingWatermarkingText(toEncode);
            byte[] watermarking = dictEncodingResult.Item1;
            numBitWatermarking = watermarking.Length * 8;

            Console.WriteLine("[Info] dim watermarking = {0} bit", watermarking.Length * 8);

            EOD = dictEncodingResult.Item2;
            EOS = dictEncodingResult.Item3;
            //+++ messaggio di testo codificato correttamente +++
            // 2) lettura file immagine di input 
            Console.WriteLine("Leggo img da input...");
            readInputImage();
            //test
            Tuple<byte[,], byte[,], byte[,]> res = jpegDecoder.getRGBMatrixFI(inputImage);

            // 3) codifica JPEG dell'immagine di input 
            //jpegQuality = FREE_IMAGE_SAVE_FLAGS.JPEG_QUALITYGOOD;
            //chromaSubsamplingType = FREE_IMAGE_SAVE_FLAGS.JPEG_SUBSAMPLING_420;

            // scrittura JPEG
            Console.WriteLine("Scrivo immagine jpeg ...");
            jpegEncoding();
            // 4) scrittura watermarking sull rgb del jpeg

            Tuple<byte[,], byte[,], byte[,]> res1 = jpegDecoder.getRGBMatrix(jpegToSerialize);

            Console.WriteLine("Scrivo watermarking sul JPEG...");

            //Tuple<byte[,], byte[,], byte[,], int> rgbWater = writeWatermarkingOnJpegRgb(watermarking);
            Tuple<byte[,], byte[,], byte[,]> rgbWater = writeWatermarkingOnJpegRgb(watermarking);
            byte[,] RMatrixWater = rgbWater.Item1;
            byte[,] GMatrixWater = rgbWater.Item2;
            byte[,] BMatrixWater = rgbWater.Item3;
            //int advRGBWatermarkingLevel = rgbWater.Item4;

            //Console.WriteLine("[ADVWater] Livello watermarking necessario = Lv{0}", advRGBWatermarkingLevel);

            //numRGBWatermarkingLevel = advRGBWatermarkingLevel;
            numAvailableBitImage = RMatrixWater.GetLength(0) * RMatrixWater.GetLength(1) * numRGBWatermarkingLevel * 3;
            numBitImage = RMatrixWater.GetLength(0) * RMatrixWater.GetLength(1) * 8 * 3;
            Console.WriteLine("[Info] bit totali immagine = {0} bit", numBitImage);
            Console.WriteLine("[Info] bit disponibili per watermarking = {0} bit", numAvailableBitImage);

            Console.WriteLine("Leggo img da input...");
            setWatermarkedRGBToJpeg(RMatrixWater, GMatrixWater, BMatrixWater);

            Console.WriteLine("Scrivo JPEG watermarked su file...");
            writeWatermarkedJpeg();
            //eventualmente scrivere come bitmap il jpeg con watermarking
            // 5) serializzazione del jpeg modificato

            Console.WriteLine("Serializzo il JPEG da trasmettere...");
            byte[] imageStream = jpegEncoder.serializeJpegImage(jpegToSerialize);
            // 6) codifica di canale sullo stream

            Console.WriteLine("Codifico con R({0},1) per trasmissione su canale con prob di errore alpha = {1} ...", numRipetizioni, alphaChError);
            BitArray channelCodedStream = channelEncoder.RipetizioneEncoding(imageStream, numRipetizioni);
            // 7) simulazione errore di canale
            //insertAlphaChannelError();

            Console.WriteLine("Applico errore di canale...");

            stopwatch.Start();

            BitArray receivedNoiseStream = channelError.singleError(channelCodedStream, alphaChError);
            // 8) decodifica bitarray stream sul canale

            stopwatch.Stop();
            Console.WriteLine(">> Tempo di trasmissione sul canale: {0} ms.", stopwatch.ElapsedMilliseconds);

            Console.WriteLine("Decodifico lo stream trasmesso sul canale...");
            BitArray decodedStream = channelDecoder.RipetizioneDecoding(receivedNoiseStream, numRipetizioni);
            // 9) decodifica watermarking da RGB
            byte[] decodedStreamArray = bitArrayToByteArray(decodedStream);

            Console.WriteLine("Deserializzo il JPEG trasmesso sul canale...");
            Bitmap decodedJpeg = jpegDecoder.deserializeJpegImage(decodedStreamArray);
            Tuple<byte[,], byte[,], byte[,]> rgbDecodedImage = jpegDecoder.getRGBMatrix(decodedJpeg);
            byte[,] RMatrixDecoded = rgbDecodedImage.Item1;
            byte[,] GMatrixDecoded = rgbDecodedImage.Item2;
            byte[,] BMatrixDecoded = rgbDecodedImage.Item3;

            //Console.WriteLine("Prelevo watermarking da RGB del JPEG trasmesso...");
            //byte[] decodedWatermarking = waterEncoder.getRGBWatermarking(RMatrixDecoded, GMatrixDecoded, BMatrixDecoded, EOS);
            Console.WriteLine("[ADVWater] Prelevo watermarking da RGB del JPEG trasmesso...");

            byte[] decodedWatermarking = waterEncoder.getLuminanceRGBWatermarking(RMatrixDecoded, GMatrixDecoded, BMatrixDecoded, EOS, blockSequence, numLSBSelectedBlock, numLSBNonSelectedBlock);

            bool equalsWater = checkEqualsWatermarking(watermarking, decodedWatermarking);
            Console.WriteLine("[WaterCheck] Decoded Watermarking uguale al Coded Watermarking? = {0}",equalsWater);
            
            //USATO PER ADVANCED RGB WATER
            //byte[] decodedWatermarking = waterEncoder.getAdvancedRGBWatermarking(RMatrixDecoded, GMatrixDecoded, BMatrixDecoded, EOS, advRGBWatermarkingLevel);

            // 10) passaggio dal watermarking al testo

            Console.WriteLine("Decodifico il watermarking...");
            Tuple<byte[], byte[]> dictStream = waterEncoder.decodeWatermarkingString(decodedWatermarking, EOD, EOS);

            byte[] dictByte = dictStream.Item1;
            byte[] dictNewCharsByte = dictStream.Item2;

            //List<int[]> dict = waterEncoder.getDictByteCompactDecoding(dictByte);
            //Dictionary<int, string> dictNewChars = waterEncoder.getDictNewCharsByteCompactDecoding(dictNewCharsByte);
            //string decodedText = dictDecoder.getDecoding(dict, dictNewChars);


            List<Int16[]> dict = waterEncoder.getDictByteCompactDecoding(dictByte);
            string[] dictNewChars = waterEncoder.getDictNewCharsByteCompactDecoding(dictNewCharsByte);

            string decodedText = dictDecoder.getCompactDecoding(dict, dictNewChars);

            Console.WriteLine("Risultato: ");
            printDecodedText(decodedText);

            //Testing sulla qualità dell'immagine watermarked ottenuta
            qualityTesting();

        }

        public static void getGilbertBurstNoiseVector(double p, double r)
        {
            gilbertElliotNoiseVector = new BitArray(channelCodedStream.Length);
            channelError.gilberElliotBurstError(gilbertElliotNoiseVector, p, r);
        }

        public static void ripetizioneEncoding(byte[] imageStream, int numR)
        {
            numRipetizioni = numR;
            channelCodedStream = channelEncoder.RipetizioneEncoding(imageStream, numRipetizioni);
        }

        public static void singleError(double alphaChannelError)
        {
            chosenChannelErrorModel = SINGLE_ERROR_CHANNEL;
            alphaChError = alphaChannelError;
            receivedNoiseStream = channelError.singleError(channelCodedStream, alphaChError);
        }

        public static void burstError(double p, double r)
        {
            chosenChannelErrorModel = GILBERT_ELLIOT_BURST_CHANNEL;
            pValue = p;
            rValue = r;
            Tuple<BitArray,BitArray> channelErrorRes = channelError.gilberElliotBurstError(channelCodedStream, p, r);
            receivedNoiseStream = channelErrorRes.Item1;
            gilbertElliotNoiseVector = channelErrorRes.Item2;
        }

        public static byte[] serializeJpegImage(Bitmap jpegToSerialize)
        {
            return jpegEncoder.serializeJpegImage(jpegToSerialize);
        }

        public static bool decodeWatermarking()
        {
            byte[] decodedWatermarking = null;
            bool equalsWater = false;
            Tuple<byte[], byte[]> dictStream = null;
            byte[] dictByte = null;
            byte[] dictNewCharsByte = null;
            List<Int16[]> dict = null;
            string[] dictNewChars = null;

            Tuple<byte[,], byte[,], byte[,]> rgbDecodedImage = jpegDecoder.getRGBMatrix(decodedJpeg);
            byte[,] RMatrixDecoded = rgbDecodedImage.Item1;
            byte[,] GMatrixDecoded = rgbDecodedImage.Item2;
            byte[,] BMatrixDecoded = rgbDecodedImage.Item3;
            
            if (chosenWatermarkingType == ADVANCED_RGB_WATERMARKING)
            {
                decodedWatermarking = waterEncoder.getAdvancedRGBWatermarking(RMatrixDecoded, GMatrixDecoded, BMatrixDecoded, EOS, numRGBWatermarkingLevel);
            }
            else if (chosenWatermarkingType == LUMINANCE_RGB_WATERMARKING)
            {
                decodedWatermarking = waterEncoder.getLuminanceRGBWatermarking(RMatrixDecoded, GMatrixDecoded, BMatrixDecoded, EOS, blockSequence, numLSBSelectedBlock, numLSBNonSelectedBlock);
            }
            equalsWater = checkEqualsWatermarking(watermarking, decodedWatermarking);
            if (!equalsWater) {
                return false;
            }
            else
            {
                dictStream = waterEncoder.decodeWatermarkingString(decodedWatermarking, EOD, EOS);
                dictByte = dictStream.Item1;
                dictNewCharsByte = dictStream.Item2;
                dict = waterEncoder.getDictByteCompactDecoding(dictByte);
                dictNewChars = waterEncoder.getDictNewCharsByteCompactDecoding(dictNewCharsByte);
                decodedText = dictDecoder.getCompactDecoding(dict, dictNewChars);
            }
            return true;
        }

        public static string getDecodedWatermarking()
        {
            return decodedText;
        }

        public static void insertWatermarkingMode()
        {
            Console.WriteLine("+ Inserire il tipo di errore di canale desiderato");
            Console.WriteLine("+ [ 0 : Single Error Channel , 1 : Burst Error Channel ]");
            Console.Write("> ");
            int c = (int)Console.Read();
            //aggiungere le altre tipologie di qualità del jpg
            if (c == SINGLE_ERROR_CHANNEL)
                chosenChannelErrorModel = SINGLE_ERROR_CHANNEL;
            else if (c == GILBERT_ELLIOT_BURST_CHANNEL)
                chosenChannelErrorModel = GILBERT_ELLIOT_BURST_CHANNEL;
            else
                throw new Exception("[Errore!] Modello di errore di canale indicato non valido! ");
        }

        public static void insertChannelErrorMode()
        {
            Console.WriteLine("+ Inserire il tipo di Watermarking desiderato");
            Console.WriteLine("+ [ 0 : Advanced RGB , 1 : LuminanceRGB ]");
            Console.Write("> ");
            int w = (int)Console.Read();
            //aggiungere le altre tipologie di qualità del jpg
            if (w == ADVANCED_RGB_WATERMARKING)
                chosenWatermarkingType = ADVANCED_RGB_WATERMARKING;
            else if (w == LUMINANCE_RGB_WATERMARKING)
                chosenWatermarkingType = LUMINANCE_RGB_WATERMARKING;
            else
                throw new Exception("[Errore!] Tecnica di watermarking indicata non valida! ");
        }
        
        public static bool checkEqualsWatermarking(byte[] watermarking, byte[] decodedWatermarking)
        {
            for (int i=0; i<watermarking.Length; i++)
                if (watermarking[i] != decodedWatermarking[i])
                    return false;
            return true;
        }

        public static void qualityTesting()
        {
            //calcolo indici di qualità dell'img watermarked
            Tuple<byte[,], byte[,], byte[,]> rgbOriginalImage = jpegDecoder.getRGBMatrixFI(inputImage);
            byte[,] RMatrix = rgbOriginalImage.Item1;
            byte[,] GMatrix = rgbOriginalImage.Item2;
            byte[,] BMatrix = rgbOriginalImage.Item3;

            Tuple<byte[,], byte[,], byte[,]> rgbWatermarkedImage = jpegDecoder.getRGBMatrix(jpegToSerialize);
            byte[,] RMatrixWater = rgbWatermarkedImage.Item1;
            byte[,] GMatrixWater = rgbWatermarkedImage.Item2;
            byte[,] BMatrixWater = rgbWatermarkedImage.Item3;

            Tuple<float[,], float[,], float[,]> yCbCrOriginalImage = jpegDecoder.getYCbCrMatrix(RMatrix, GMatrix, BMatrix);
            float[,] YMatrix = yCbCrOriginalImage.Item1;

            Tuple<float[,], float[,], float[,]> yCbCrWatermarkedImage = jpegDecoder.getYCbCrMatrix(RMatrixWater, GMatrixWater, BMatrixWater);
            float[,] YMatrixWater = yCbCrWatermarkedImage.Item1;

            float mse = WatermarkingTestUtility.getMSE(YMatrix, YMatrixWater);
            float psnr = WatermarkingTestUtility.getPSNR(mse);

            printResume(mse, psnr);

        }

        public static double getNumBitErrorBurst()
        {
            double cntError = 1.0;
            for (int i = 0; i < gilbertElliotNoiseVector.Count; i++)
                if (gilbertElliotNoiseVector[i])
                    cntError++;
            return cntError;
        }

        public static string getGilbertBurstNoiseVectorAsString()
        {
            int stringSize = 100;
            StringBuilder sb = new StringBuilder(100);
            for (int i = 0; i < stringSize; i++)
                if (gilbertElliotNoiseVector[i])
                    sb.Append("1");
                else
                    sb.Append("0");
            return sb.ToString();
        }

        public static Tuple<byte[,], byte[,], byte[,]> getRGBMatrixFI(FIBITMAP jpeg)
        {
            return jpegDecoder.getRGBMatrixFI(jpeg);
        }

        public static Tuple<byte[,], byte[,], byte[,]> getRGBMatrix(Bitmap jpeg)
        {
            return jpegDecoder.getRGBMatrix(jpeg);
        }

        public static Tuple<float[,], float[,], float[,]> getYCbCrMatrix(byte[,] RMatrix, byte[,] GMatrix, byte[,] BMatrix)
        {
            return jpegDecoder.getYCbCrMatrix(RMatrix, GMatrix, BMatrix);
        }

        public static void printResume(float mse, float psnr)
        {
            Console.WriteLine(" *** Parametri di compressione JPEG *** ");
            Console.WriteLine(" Qualità JPEG = {0} ", jpegQuality);
            Console.WriteLine(" Chroma Subsampling JPEG = {0} ", chromaSubsamplingType);
            Console.WriteLine(" *** Dati sulla codifica watermarking  *** ");
            Console.WriteLine(" RGB Watermarking ");
            Console.WriteLine(" Bit disponibili per watermarking injection = {0} ", numAvailableBitImage);
            Console.WriteLine(" Bit disponibili di watermarking inseriti = {0} ", numBitWatermarking);
            Console.WriteLine(" Livello LSB utilizzato = Liv. {0} ", numRGBWatermarkingLevel);
            Console.WriteLine(" Bit watermarking / Bit disponibili per watermarking (perc.) =  {0:0.00%}", (numBitWatermarking / numAvailableBitImage));
            Console.WriteLine(" Bit watermarking / Bit immagine (perc.) =  {0:0.00%}", (numBitWatermarking / numBitImage));
            Console.WriteLine(" *** Dati sulla qualità dell'immagine watermarked  *** ");
            Console.WriteLine(" Val. MSE =  {0} ", mse);
            Console.WriteLine(" Val. PSNR = {0} ", psnr);
        }

        public static void writeWatermarkedJpeg()
        {
            jpegToSerialize.Save(outputWatermarkedImagePath, ImageFormat.Jpeg);
        }

        public static void printDecodedText(string text)
        {
            Console.WriteLine(text);
        }

        public static byte[] bitArrayToByteArray(BitArray decodedStream)
        {
            byte[] decArray = new byte[decodedStream.Length / 8];
            decodedStream.CopyTo(decArray, 0);
            return decArray;
        }

        public static void setWatermarkedRGBToJpeg(byte[,] RMatrixWater, byte[,] GMatrixWater, byte[,] BMatrixWater)
        {
            //sostituisce i valori originari rgb del jpeg con quelli modificati dopo il watermarking
            jpegEncoder.setRGBMatrix(RMatrixWater, GMatrixWater, BMatrixWater, ref jpegToSerialize);
        }

        //web app version
        public static void setWatermarkedRGBToJpeg(byte[,] RMatrixWater, byte[,] GMatrixWater, byte[,] BMatrixWater, ref Bitmap jpegImage)
        {
            //sostituisce i valori originari rgb del jpeg con quelli modificati dopo il watermarking
            jpegEncoder.setRGBMatrix(RMatrixWater, GMatrixWater, BMatrixWater, ref jpegImage);
        }

        //web app version
        public static Tuple<byte[,], byte[,], byte[,]> doAdvancedRGBWatermarking(Bitmap jpegToSerialize)
        {
            chosenWatermarkingType = ADVANCED_RGB_WATERMARKING;
            Tuple<byte[,], byte[,], byte[,]> jpegRGB = jpegDecoder.getRGBMatrix(jpegToSerialize);
            byte[,] RMatrixJPG = jpegRGB.Item1;
            byte[,] GMatrixJPG = jpegRGB.Item2;
            byte[,] BMatrixJPG = jpegRGB.Item3;
            numBitImage = RMatrixJPG.GetLength(0) * RMatrixJPG.GetLength(1) * 8 * 3;
            // scrittura watermarking su RGB del JPEG
            Tuple<byte[,], byte[,], byte[,], int> advWater = waterEncoder.doAdvancedRGBWatermarking(RMatrixJPG, GMatrixJPG, BMatrixJPG, watermarking);
            byte[,] RMatrixWater = advWater.Item1;
            byte[,] GMatrixWater = advWater.Item2;
            byte[,] BMatrixWater = advWater.Item3;
            numRGBWatermarkingLevel = advWater.Item4;
            numAvailableBitImage = RMatrixWater.GetLength(0) * RMatrixWater.GetLength(1) * numRGBWatermarkingLevel * 3;
            return Tuple.Create(RMatrixWater, GMatrixWater, BMatrixWater);
        }

        public static Tuple<byte[,], byte[,], byte[,]> doLuminanceRGBWatermarking(Bitmap jpegToSerialize, int numLSBSelectedB, int numLSBNonSelectedB)
        {
            chosenWatermarkingType = LUMINANCE_RGB_WATERMARKING;
            numLSBSelectedBlock = numLSBSelectedB;
            numLSBNonSelectedBlock = numLSBNonSelectedB;
            Tuple<byte[,], byte[,], byte[,]> jpegRGB = jpegDecoder.getRGBMatrix(jpegToSerialize);
            byte[,] RMatrixJPG = jpegRGB.Item1;
            byte[,] GMatrixJPG = jpegRGB.Item2;
            byte[,] BMatrixJPG = jpegRGB.Item3;
            numBitImage = RMatrixJPG.GetLength(0) * RMatrixJPG.GetLength(1) * 8 * 3;
            Tuple<float[,], float[,], float[,]> jpegYcbcr = jpegDecoder.getYCbCrMatrix(RMatrixJPG, GMatrixJPG, BMatrixJPG);
            blockSequence = waterEncoder.getBlocksForYWatermarking(jpegYcbcr.Item1, delta);
            // scrittura watermarking su RGB del JPEG
            Tuple<byte[,], byte[,], byte[,]> lumWater = waterEncoder.doLuminanceRGBWatermarking(RMatrixJPG, GMatrixJPG, BMatrixJPG, watermarking, blockSequence, numLSBSelectedBlock, numLSBNonSelectedBlock);
            int numBlock = (RMatrixJPG.GetLength(0) * RMatrixJPG.GetLength(1)) / 64;
            numSelectedBlock = blockSequence.Count;
            numNonSelectedBlock = numBlock - numSelectedBlock;
            numAvailableBitImage = (64 * (numLSBSelectedBlock * 3) * numSelectedBlock) + (64 * (numLSBNonSelectedBlock * 3) * numNonSelectedBlock);
            return lumWater;
        }

        public static Tuple<byte[,], byte[,], byte[,]> writeWatermarkingOnJpegRgb(byte[] watermarking)
        {
            Tuple<byte[,], byte[,], byte[,]> jpegRGB = jpegDecoder.getRGBMatrix(jpegToSerialize);
            byte[,] RMatrixJPG = jpegRGB.Item1;
            byte[,] GMatrixJPG = jpegRGB.Item2;
            byte[,] BMatrixJPG = jpegRGB.Item3;
            if (chosenWatermarkingType == LUMINANCE_RGB_WATERMARKING)
            {
                Tuple<float[,], float[,], float[,]> jpegYcbcr = jpegDecoder.getYCbCrMatrix(RMatrixJPG, GMatrixJPG, BMatrixJPG);
                blockSequence = waterEncoder.getBlocksForYWatermarking(jpegYcbcr.Item1, delta);
                // scrittura watermarking su RGB del JPEG
                //return waterEncoder.doRGBWatermarking(RMatrixJPG, GMatrixJPG, BMatrixJPG, watermarking);
                Tuple<byte[,], byte[,], byte[,]> lumWater = waterEncoder.doLuminanceRGBWatermarking(RMatrixJPG, GMatrixJPG, BMatrixJPG, watermarking, blockSequence, numLSBSelectedBlock, numLSBNonSelectedBlock);
                return lumWater;
            }
            else if (chosenWatermarkingType == ADVANCED_RGB_WATERMARKING)
            {
                Tuple<byte[,], byte[,], byte[,], int> advWater = waterEncoder.doAdvancedRGBWatermarking(RMatrixJPG, GMatrixJPG, BMatrixJPG, watermarking);
                byte[,] RMatrixWater = advWater.Item1;
                byte[,] GMatrixWater = advWater.Item2;
                byte[,] BMatrixWater = advWater.Item3;
                numRGBWatermarkingLevel = advWater.Item4;
                return Tuple.Create(RMatrixWater, GMatrixWater, BMatrixWater);
            }
            return null;  //placeholder
        }

        //metodo per web app
        public static Tuple<byte[,], byte[,], byte[,]> writeWatermarkingOnJpegRgb(Bitmap jpegImage, int watermarkingMethod)
        {
            Tuple<byte[,], byte[,], byte[,]> jpegRGB = jpegDecoder.getRGBMatrix(jpegImage);
            byte[,] RMatrixJPG = jpegRGB.Item1;
            byte[,] GMatrixJPG = jpegRGB.Item2;
            byte[,] BMatrixJPG = jpegRGB.Item3;
            if (watermarkingMethod == LUMINANCE_RGB_WATERMARKING)
            {
                Tuple<float[,], float[,], float[,]> jpegYcbcr = jpegDecoder.getYCbCrMatrix(RMatrixJPG, GMatrixJPG, BMatrixJPG);
                blockSequence = waterEncoder.getBlocksForYWatermarking(jpegYcbcr.Item1, delta);
                // scrittura watermarking su RGB del JPEG
                //return waterEncoder.doRGBWatermarking(RMatrixJPG, GMatrixJPG, BMatrixJPG, watermarking);
                Tuple<byte[,], byte[,], byte[,]> lumWater = waterEncoder.doLuminanceRGBWatermarking(RMatrixJPG, GMatrixJPG, BMatrixJPG, watermarking, blockSequence, numLSBSelectedBlock, numLSBNonSelectedBlock);
                int numBlock = (RMatrixJPG.GetLength(0) * RMatrixJPG.GetLength(1)) / 64;
                int numSelectedBlock = blockSequence.Count; 
                int numNonSelectedBlock = numBlock - numSelectedBlock;
                numAvailableBitImage = (64 * (numLSBSelectedBlock * 3) * numSelectedBlock) + (64 * (numLSBNonSelectedBlock * 3) * numNonSelectedBlock);
                return lumWater;
            }
            else if (watermarkingMethod == ADVANCED_RGB_WATERMARKING)
            {
                Tuple<byte[,], byte[,], byte[,], int> advWater = waterEncoder.doAdvancedRGBWatermarking(RMatrixJPG, GMatrixJPG, BMatrixJPG, watermarking);
                byte[,] RMatrixWater = advWater.Item1;
                byte[,] GMatrixWater = advWater.Item2;
                byte[,] BMatrixWater = advWater.Item3;
                numRGBWatermarkingLevel = advWater.Item4;
                return Tuple.Create(RMatrixWater, GMatrixWater, BMatrixWater);
            }
            return null;  //placeholder
        }
        

        /*
        // VA BENE PER TECNICA ADVANCED RGB
        public static Tuple<byte[,], byte[,], byte[,], int> writeWatermarkingOnJpegRgb(byte[] watermarking)
        {
            Tuple<byte[,], byte[,], byte[,]> jpegRGB = jpegDecoder.getRGBMatrix(jpegToSerialize);
            byte[,] RMatrixJPG = jpegRGB.Item1;
            byte[,] GMatrixJPG = jpegRGB.Item2;
            byte[,] BMatrixJPG = jpegRGB.Item3;
            // scrittura watermarking su RGB del JPEG
            //return waterEncoder.doRGBWatermarking(RMatrixJPG, GMatrixJPG, BMatrixJPG, watermarking);
            Tuple<byte[,], byte[,], byte[,], int> advWater = waterEncoder.doAdvancedRGBWatermarking(RMatrixJPG, GMatrixJPG, BMatrixJPG, watermarking);
            return advWater;
        }
        */

        public static void jpegEncoding()
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
                Image read = Image.FromStream(jpegStream);
                jpegToSerialize = new Bitmap(read);
            }
            //inserire eventuale stampa delle matrici e della codifica DC-AC del jpeg
        }

        
        public static void readInputImage()
        {
            if (!File.Exists(inputImagePath))
            {
                Console.WriteLine(inputImagePath + " non può essere caricato. Errore.");
                return;
            }
            /*
            if (!inputImage.IsNull)
                FreeImage.Unload(inputImage);
            */
            inputImage = FreeImage.Load(FREE_IMAGE_FORMAT.FIF_BMP, inputImagePath, FREE_IMAGE_LOAD_FLAGS.DEFAULT);
        }
        
        public static void insertChromaSubsamplingType()
        {
            Console.WriteLine("Inserire il tipo di Chroma Subsampling JPEG desiderato");
            Console.Write("> ");
            int sub = (int)Console.Read();
            //aggiungere le altre tipologie di qualità del jpg
            if (sub == 0)
                chromaSubsamplingType = FREE_IMAGE_SAVE_FLAGS.JPEG_SUBSAMPLING_444;
        }

        public static void insertJpegQuality()
        {
            Console.WriteLine("Inserire qualità della codifica JPEG desiderata");
            Console.Write("> ");
            int jpegQ = (int)Console.Read();
            //aggiungere le altre tipologie di qualità del jpg
            if (jpegQ == 0)
                jpegQuality = FREE_IMAGE_SAVE_FLAGS.JPEG_QUALITYSUPERB;
        }

        public static void insertInputImagePath()
        {
            //inserire controlli sulla validità del path

            Console.WriteLine("Inserire path immagine da codificare in formato JPEG");
            Console.Write(" > ");
            string imagePath = Console.ReadLine();
            inputImagePath = imagePath;
        }

        public static void insertOutputImagePath()
        {
            //inserire controlli sulla validità del path

            Console.WriteLine("Inserire path si salvataggio del JPEG");
            Console.Write(" > ");
            string imagePath = Console.ReadLine();
            outputImagePath = imagePath;
        }

        public static void insertInputTextPath()
        {
            //inserire controlli sulla validità del path

            Console.WriteLine("Inserire path testo da codificare");
            Console.Write(" > ");
            string textPath = Console.ReadLine();
            inputTextPath = textPath;
        }

        public static void insertNumRipetizioni()
        {
            Console.WriteLine("Inserire il numero di ripetizioni per la codifica di canale R(?,1)");
            Console.Write("> ");
            int nr = Console.Read();
            numRipetizioni = nr;
        }

        public static void insertAlphaChannelError()
        {
            Console.WriteLine("Inserire l'errore di canale alpha");
            Console.Write("> ");
            double alpha = (double)Console.Read();
            alphaChError = alpha;
        }

        public static void insertNumLSBSelectedLuminanceBlock()
        {
            Console.WriteLine("Inserire il num di bit LSB da utilizzare nei blocchi Y selezionati ");
            Console.Write("> ");
            int numLSB = (int)Console.Read();
            //aggiungere le altre tipologie di qualità del jpg
            if (numLSB <= 0 || numLSB > 8)
                throw new Exception("[Errore!] Numero di bit LSB superiore a quello massimo utilizzabile.");
            numLSBSelectedBlock = numLSB;
        }

        public static void insertNumLSBNonSelectedLuminanceBlock()
        {
            Console.WriteLine("Inserire il num di bit LSB da utilizzare nei blocchi non Y ");
            Console.Write("> ");
            int numLSB = (int)Console.Read();
            //aggiungere le altre tipologie di qualità del jpg
            if (numLSB <= 0 || numLSB > 8)
                throw new Exception("[Errore!] Numero di bit LSB superiore a quello massimo utilizzabile.");
            numLSBNonSelectedBlock = numLSB;
        }

        public static Tuple<byte[], int, int, string> encodingWatermarkingText(string toEncode)
        {
            Tuple<Dictionary<string, Int16[]>, string[]> encodeResult = dictEncoder.getCompactEncoding(toEncode);
            Dictionary<string, Int16[]> dict = encodeResult.Item1;
            string[] dictNewChars = encodeResult.Item2;
            //dict come List<int[]> per decodifica
            List<Int16[]> dictList = new List<Int16[]>();
            Dictionary<string, Int16[]>.ValueCollection values = dict.Values;
            foreach (Int16[] v in values)
            {
                Int16[] entry = new Int16[2];
                entry[0] = v[1];
                entry[1] = v[2];
                dictList.Add(entry);
            }
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream dictStream = new MemoryStream();
            MemoryStream dictNewCharsStream = new MemoryStream();
            bf.Serialize(dictStream, dictList);
            bf.Serialize(dictNewCharsStream, dictNewChars);
            byte[] dictArray = dictStream.ToArray();
            byte[] dictNewCharsArray = dictNewCharsStream.ToArray();
            Tuple<byte[], int, int> waterResult = waterEncoder.createWatermarkingString(dictArray, dictNewCharsArray);
            watermarking = waterResult.Item1;
            numBitWatermarking = watermarking.Length * 8;
            EOD = waterResult.Item2;
            EOS = waterResult.Item3;
            //stringa di decodifica
            string codedString = printLZ78Encoding(dict,dictNewChars);
            return Tuple.Create(watermarking, EOD, EOS, codedString);
        }

        private static string printLZ78Encoding(Dictionary<string, Int16[]> dict, string[] dictNewChars)
        {
            StringBuilder sb = new StringBuilder();
            List<Int16[]> entriesDict = new List<Int16[]>();
            Dictionary<string, Int16[]>.KeyCollection keys = dict.Keys;
            sb.Append(" + Tuple dizionario + \n");
            foreach (string k1 in keys)
            {
                Int16[] row = new Int16[2];
                row[0] = dict[k1][1];
                row[1] = dict[k1][2];
                sb.Append(k1 + ", [" + dict[k1][0] + "," + dict[k1][1] + "," + dict[k1][2] + "] \n");
            }
            sb.Append(" + Tuple nuovi caratteri + \n");
            for (int i = 0; i < dictNewChars.Length; i++)
            {
                sb.Append("[" + i + "," + dictNewChars[i] + "] \n");
            }
            return sb.ToString();
        }
        
        public static string readFromFile(string path)
        {
            string s = string.Empty;
            using (var reader = new StreamReader(path))
            {
                string line = string.Empty;
                while ((line = reader.ReadLine()) != null)
                {
                    s += line;
                }
            }
            return Encoding.UTF8.GetString(Encoding.Default.GetBytes(s));
        }
       
        public static string getWatermarkingMethod()
        {
            if (chosenWatermarkingType == ADVANCED_RGB_WATERMARKING)
                return "ADVANCED RGB WATERMARKING";
            else if (chosenWatermarkingType == LUMINANCE_RGB_WATERMARKING)
                return "LUMINANCE RGB WATERMARKING";
            return "";
        }

        public static void initModules()
        {
            dictEncoder = new LZ78Encoder();
            dictDecoder = new LZ78Decoder();
            waterEncoder = new JPEGWatermarker();
            jpegEncoder = new JPEGEncoder();
            jpegDecoder = new JPEGDecoder();
            channelEncoder = new ChannelEncoder();
            channelDecoder = new ChannelDecoder();
            channelError = new ChannelError();
        }

    }
}
