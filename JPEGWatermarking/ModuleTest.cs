using FreeImageAPI;
using JPEGEncoding;
using LZ78Encoding;
using System;
using System.Collections;
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
    class ModuleTest
    {
        private static string inputImagePath;
        private static string outputImagePath;
        private static string outputWatermarkedImagePath;
        private static string inputTextPath;
        private static int numRipetizioni;
        private static double alphaChError;

        private static FREE_IMAGE_SAVE_FLAGS jpegQuality;
        private static FREE_IMAGE_SAVE_FLAGS chromaSubsamplingType;

        private static int EOD;
        private static int EOS;

        private static FIBITMAP inputImage = new FIBITMAP();
        private static Bitmap jpegToSerialize;

        private static LZ78EncoderIF dictEncoder;
        private static LZ78DecoderIF dictDecoder;
        private static JPEGWatermarkerIF waterEncoder;
        private static JPEGEncoderIF jpegEncoder;
        private static JPEGDecoderIF jpegDecoder;
        private static ChannelEncoderIF channelEncoder;
        private static ChannelDecoderIF channelDecoder;
        private static ChannerErrorIF channelError;

        //parametri per testing
        private static double numBitWatermarking;
        private static double numBitImage;
        private static int numAvailableBitImage;
        private static int numRGBWatermarkingLevel;

        //parametri per Luminance RGB watermarking
        private static int numLSBSelectedBlock;
        private static int numLSBNonSelectedBlock;
        private static List<int[]> blockSequence;
        private static float delta;

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
        
        
        public static void Main(string[] args)
        {
            //insertInputImagePath();
            //insertOutputImagePath();
            //insertInputTextPath();
            insertJpegQuality();
            insertChromaSubsamplingType();

            inputImagePath = "C:\\Users\\Giuseppe\\OneDrive\\Documenti\\Progetto_Teoria_Informazione\\jpegtest\\twitter320x320\\twitter320x320.bmp";
            outputImagePath = "C:\\Users\\Giuseppe\\OneDrive\\Documenti\\Progetto_Teoria_Informazione\\jpegtest\\twitter320x320\\twitter320x320_jpg.jpg";
            outputWatermarkedImagePath = "C:\\Users\\Giuseppe\\OneDrive\\Documenti\\Progetto_Teoria_Informazione\\jpegtest\\twitter320x320\\twitter320x320_watermarked.jpg";
            inputTextPath = "C:\\Users\\Giuseppe\\OneDrive\\Documenti\\Progetto_Teoria_Informazione\\stringhelz78\\primo_canto_ok_12.txt";

            initModules();
            // 1) codifica LZ del testo 
            Tuple<byte[], int, int> dictEncodingResult = encodingWatermarkingText();
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
            //insertNumRipetizioni();

            numRipetizioni = 5;
            alphaChError = 0.005;

            Console.WriteLine("Codifico con R({0},1) per trasmissione su canale...", numRipetizioni);
            BitArray channelCodedStream = channelEncoder.RipetizioneEncoding(imageStream, numRipetizioni);
            // 7) simulazione errore di canale
            //insertAlphaChannelError();

            Console.WriteLine("Applico errore di canale...");
            BitArray receivedNoiseStream = channelError.singleError(channelCodedStream, alphaChError);
            // 8) decodifica bitarray stream sul canale

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

        private static bool checkEqualsWatermarking(byte[] watermarking, byte[] decodedWatermarking)
        {
            for (int i=0; i<watermarking.Length; i++)
                if (watermarking[i] != decodedWatermarking[i])
                    return false;
            return true;
            
        }

        private static void qualityTesting()
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

        private static void printResume(float mse, float psnr)
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

        private static void writeWatermarkedJpeg()
        {
            jpegToSerialize.Save(outputWatermarkedImagePath, ImageFormat.Jpeg);
        }

        private static void printDecodedText(string text)
        {
            Console.WriteLine(text);
        }

        private static byte[] bitArrayToByteArray(BitArray decodedStream)
        {
            byte[] decArray = new byte[decodedStream.Length / 8];
            decodedStream.CopyTo(decArray, 0);
            return decArray;
        }

        private static void setWatermarkedRGBToJpeg(byte[,] RMatrixWater, byte[,] GMatrixWater, byte[,] BMatrixWater)
        {
            //sostituisce i valori originari rgb del jpeg con quelli modificati dopo il watermarking
            jpegEncoder.setRGBMatrix(RMatrixWater, GMatrixWater, BMatrixWater, ref jpegToSerialize);
        }

        private static Tuple<byte[,], byte[,], byte[,]> writeWatermarkingOnJpegRgb(byte[] watermarking)
        {
            Tuple<byte[,], byte[,], byte[,]> jpegRGB = jpegDecoder.getRGBMatrix(jpegToSerialize);
            byte[,] RMatrixJPG = jpegRGB.Item1;
            byte[,] GMatrixJPG = jpegRGB.Item2;
            byte[,] BMatrixJPG = jpegRGB.Item3;
            Tuple<float[,], float[,], float[,]> jpegYcbcr = jpegDecoder.getYCbCrMatrix(RMatrixJPG, GMatrixJPG, BMatrixJPG);
            delta = 1;
            numLSBSelectedBlock = 3;
            numLSBNonSelectedBlock = 1;
            blockSequence = waterEncoder.getBlocksForYWatermarking(jpegYcbcr.Item1, delta);
            // scrittura watermarking su RGB del JPEG
            //return waterEncoder.doRGBWatermarking(RMatrixJPG, GMatrixJPG, BMatrixJPG, watermarking);
            Tuple<byte[,], byte[,], byte[,]> advWater = waterEncoder.doLuminanceRGBWatermarking(RMatrixJPG, GMatrixJPG, BMatrixJPG, watermarking, blockSequence, numLSBSelectedBlock, numLSBNonSelectedBlock);
            return advWater;
        }

        /*
        // VA BENE PER TECNICA ADVANCED RGB
        private static Tuple<byte[,], byte[,], byte[,], int> writeWatermarkingOnJpegRgb(byte[] watermarking)
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

        private static void jpegEncoding()
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

        private static void readInputImage()
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

        private static void insertChromaSubsamplingType()
        {
            Console.WriteLine("Inserire il tipo di Chroma Subsampling JPEG desiderato");
            Console.Write("> ");
            int sub = (int)Console.Read();
            //aggiungere le altre tipologie di qualità del jpg
            if (sub == 0)
                chromaSubsamplingType = FREE_IMAGE_SAVE_FLAGS.JPEG_SUBSAMPLING_444;
        }

        private static void insertJpegQuality()
        {
            Console.WriteLine("Inserire qualità della codifica JPEG desiderata");
            Console.Write("> ");
            int jpegQ = (int)Console.Read();
            //aggiungere le altre tipologie di qualità del jpg
            if (jpegQ == 0)
                jpegQuality = FREE_IMAGE_SAVE_FLAGS.JPEG_QUALITYSUPERB;
        }

        private static void insertInputImagePath()
        {
            //inserire controlli sulla validità del path

            Console.WriteLine("Inserire path immagine da codificare in formato JPEG");
            Console.Write(" > ");
            string imagePath = Console.ReadLine();
            inputImagePath = imagePath;
        }

        private static void insertOutputImagePath()
        {
            //inserire controlli sulla validità del path

            Console.WriteLine("Inserire path si salvataggio del JPEG");
            Console.Write(" > ");
            string imagePath = Console.ReadLine();
            outputImagePath = imagePath;
        }

        private static void insertInputTextPath()
        {
            //inserire controlli sulla validità del path

            Console.WriteLine("Inserire path testo da codificare");
            Console.Write(" > ");
            string textPath = Console.ReadLine();
            inputTextPath = textPath;
        }

        private static void insertNumRipetizioni()
        {
            Console.WriteLine("Inserire il numero di ripetizioni per la codifica di canale R(?,1)");
            Console.Write("> ");
            int nr = Console.Read();
            numRipetizioni = nr;
        }

        private static void insertAlphaChannelError()
        {
            Console.WriteLine("Inserire l'errore di canale alpha");
            Console.Write("> ");
            double alpha = (double)Console.Read();
            alphaChError = alpha;
        }

        private static Tuple<byte[], int, int> encodingWatermarkingText()
        {
            string toEncode = readFromFile(inputTextPath);
            /*
            Tuple<Dictionary<string, int[]>, Dictionary<int, string>> encodeResult = dictEncoder.getEncoding(toEncode);
            Dictionary<string, int[]> dict = encodeResult.Item1;
            Dictionary<int, string> dictNewChars = encodeResult.Item2;
            */
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
            /*
            Console.WriteLine("dict dim = {0}", dictArray.Length);
            Console.WriteLine("dictNewChar dim = {0}", dictNewCharsArray.Length);
            */
            Tuple<byte[], int, int> waterResult = waterEncoder.createWatermarkingString(dictArray, dictNewCharsArray);
            //Console.WriteLine("dict dim = {0}", finalStream.Length);
            byte[] finalStream = waterResult.Item1;
            int EOD = waterResult.Item2;
            int EOS = waterResult.Item3;
            return Tuple.Create(finalStream, EOD, EOS);
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

        private static void initModules()
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
