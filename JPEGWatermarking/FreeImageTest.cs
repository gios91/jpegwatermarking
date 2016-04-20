using FreeImageAPI;
using JPEGEncoding;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPEGWatermarking
{
    class FreeImageTest
    {
        const string path = "C:\\Users\\Giuseppe\\OneDrive\\Documenti\\Progetto_Teoria_Informazione\\jpegtest\\blueskyok.bmp";
        const string outFileName = "C:\\Users\\Giuseppe\\OneDrive\\Documenti\\Progetto_Teoria_Informazione\\jpegtest\\bluesky_jpeg_nosubsampling444.jpg";
        static FIBITMAP dib = new FIBITMAP();
        string message = null;

        static JPEGEncoderIF jpg = new JPEGEncoder(dib);        

        /*
        public static void Main(string[] args)
        {
            if (!File.Exists(path))
            {
                Console.WriteLine(path + " does not exist. Aborting.");
                return;
            }

            // Try to unload the bitmap handle (in case it is not null).
            // Always assert that a handle (like dib) is unloaded before it is reused, because
            // on unmanaged side there is no garbage collector that will clean up unreferenced
            // objects.
            // The following code will produce a memory leak (in case the bitmap is loaded
            // successfully) because the handle to the first bitmap is lost:
            //   dib = FreeImage.Load(FREE_IMAGE_FORMAT.FIF_JPEG, fileName, FREE_IMAGE_LOAD_FLAGS.JPEG_ACCURATE);
            //   dib = FreeImage.Load(FREE_IMAGE_FORMAT.FIF_JPEG, fileName, FREE_IMAGE_LOAD_FLAGS.JPEG_ACCURATE);
            if (!dib.IsNull)
                FreeImage.Unload(dib);

            // Loading a sample bitmap. In this case it's a .jpg file. 'Load' requires the file
            // format or the loading process will fail. An additional flag (the default value is
            // 'DEFAULT') can be set to enable special loading options.
            dib = FreeImage.Load(FREE_IMAGE_FORMAT.FIF_BMP, path, FREE_IMAGE_LOAD_FLAGS.DEFAULT);

            // Check if the handle is null which means the bitmap could not be loaded.
            if (dib.IsNull)
            {
                Console.WriteLine("Loading bitmap failed. Aborting.");
                // Check whether there was an error message.
                return;
            }
            //prendere le matrici RGB dal file Bitmap
            Tuple<byte[,], byte[,], byte[,]> rgbResult = jpg.getRGBMatrixFI(dib);
            byte[,] RMatrix = rgbResult.Item1;
            byte[,] GMatrix = rgbResult.Item2;
            byte[,] BMatrix = rgbResult.Item3;
            Console.WriteLine("******** RGB INIZIALI ********");
            JPEGUtility.printMatriciRGB(RMatrix, GMatrix, BMatrix, BMatrix.GetLength(0), BMatrix.GetLength(1));


            /*

            // Try flipping the bitmap.
            if (!FreeImage.FlipHorizontal(dib))
            {
                Console.WriteLine("Unable to flip bitmap.");
                // Check whether there was an error message.
            }

            // Store the bitmap back to disk. Again the desired format is needed. In this case the format is 'TIFF'.
            // An output filename has to be chosen (which will be overwritten without a warning).
            // A flag can be provided to enable pluginfunctions (compression is this case).
            
            FreeImage.Save(FREE_IMAGE_FORMAT.FIF_JPEG, dib, outFileName, FREE_IMAGE_SAVE_FLAGS.JPEG_SUBSAMPLING_444 );
            
            // The bitmap was saved to disk but is still allocated in memory, so the handle has to be freed.
            if (!dib.IsNull)
                FreeImage.Unload(dib);

            // Make sure to set the handle to null so that it is clear that the handle is not pointing to a bitmap.
            dib.SetNull();
          
        }
        */
    }
}
