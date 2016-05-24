using JPEGWatermarkingWeb.Controller;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace JPEGWatermarkingWeb
{
    public partial class Default : Page
    {
        private static string pathGreenBall = "img/greenball.png";
        private static string pathRedCross = "img/redcross.png";

        string testPath = "img/null_input_image_320x320.png";

        private static string pathInputImage = "img/SampleInputImage/twitter320x320.bmp";
        private static string pathOutputImage = "img/SampleInputImage/twitter320x320_jpeg.jpg";
        private static string pathOutputWatermarkedImage = "img/SampleInputImage/twitter320x320_jpeg_watermarked.jpg";
        private static string outputWatermarkedImageName = "twitter320x320_jpeg_watermarked.jpg";
        private static string pathOutputDecodedImage = "img/twitter320x320_jpeg_decoded.jpg";

        private static bool uploadedInputImage = false;

        private static string pathInputUploadImage;
        private static string pathOutputUploadImage;
        private static string pathOutputUploadWatermarkedImage;
        private static string outputUploadWatermarkedImageName;
        private static string pathOutputUploadDecodedImage; 
                
        protected void Page_Load(object sender, EventArgs e)
        {
            PageController.initializeModules();
            initJpegParamsList();
            initWatermarkingMethodsList();
            initWatermarkingDecodingMethodsList();
            lz78OutputText.ReadOnly = true;
            logTextBox.ReadOnly = true;
            decodedWatermarkTextBox.ReadOnly = true;
            //initTextBox();
        }
        
        protected void DoLZ78Encoding(object sender, EventArgs e)
        {
            string textToEncode = lz78InputText.Text;
            lz78OutputText.Text = PageController.getLZ78EncodingString(textToEncode);
        }

        protected void UploadButton_Click(object sender, EventArgs e)
        {
            if (uploadImageRadioButton.Checked && !sampleImageRadioButton.Checked)
            {
                if (FileUploadControl.HasFile)
                {
                    uploadedInputImage = true;
                    string filename = null;
                    try
                    {
                        filename = Path.GetFileName(FileUploadControl.FileName);
                        FileUploadControl.SaveAs(Server.MapPath("~/img/upload/") + filename);
                        uploadDoneImage.ImageUrl = pathGreenBall;
                        pathInputUploadImage = "~/img/upload/" + filename;
                        string newFilename = filename.Replace(".bmp", "");
                        pathOutputUploadImage = "~/img/upload/" + newFilename + "_jpeg.jpg";
                        pathOutputUploadWatermarkedImage = "~/img/upload/" + newFilename + "_jpeg_watermarked.jpg";
                        outputUploadWatermarkedImageName = newFilename + "_jpeg_watermarked.jpg";
                        pathOutputUploadDecodedImage = "~/img/upload/" + newFilename + "_jpeg_decoded.jpg";
                    }
                    catch (Exception ex) { }
                    inputImage.ImageUrl = "img/upload/" + filename;
                }
            }
            else if (!uploadImageRadioButton.Checked && sampleImageRadioButton.Checked)
            {
                uploadedInputImage = false;
                uploadDoneImage.ImageUrl = pathGreenBall;
                inputImage.ImageUrl = pathInputImage;
            }
        }

        protected void UploadInputText(object sender, EventArgs e)
        {
            if (inputTextUpload.HasFile)
            {
                string filename = null;
                try
                {
                    filename = Path.GetFileName(inputTextUpload.FileName);
                    string pathFile = HttpContext.Current.Server.MapPath("~/intext/")+ filename;
                    inputTextUpload.SaveAs(pathFile);
                    string inputText = PageController.readFromFile(pathFile);
                    lz78InputText.Text = inputText;
                }
                catch (Exception ex) { }
            }
        }

        /*
        protected void UploadImage(object sender, EventArgs e)
        {
            if (inputImageUpload.HasFile)
            {
                string fileName = Path.GetFileName(inputImageUpload.PostedFile.FileName);
                inputImageUpload.PostedFile.SaveAs(HttpContext.Current.Server.MapPath("~/img/upload/"+fileName));
                inputImage.ImageUrl = "img/upload/" + fileName;
                Response.Redirect(Request.Url.AbsoluteUri);
            }
        }
        */

        protected void DownloadWatermarkedImage(object sender, EventArgs e)
        {

            string imageName = null;
            if (uploadedInputImage)
                imageName = outputUploadWatermarkedImageName;
            else
               imageName = outputWatermarkedImageName;
            string outputPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), imageName);
            PageController.writeWatermarkedJpeg(outputPath);
            downloadDesktopImageLabel.Text = "Salvata su: " + outputPath;
        }
        
        protected void DownloadLogTextInfo(object sender, EventArgs e)
        {
            string time = DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss");
            string filename = "watermarking_log_"+ time +".txt";
            string outputPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), filename);
            StreamWriter logFile = null;
            try
            {
                logFile = File.CreateText(outputPath); // creating file
                logFile.Write(logTextBox.Text);
                logFile.Close();
                downloadLogFileLabel.Text = "Log salvato su: " + outputPath;
            }
            catch (Exception ex) { }
        }

        protected void CodificaToJpeg(object sender, EventArgs e)
        {
            string resolvedInputPath = null;
            string resolvedOutputPath = null;
            if (uploadedInputImage)
            {
                resolvedInputPath = HttpContext.Current.Server.MapPath(pathInputUploadImage);
                resolvedOutputPath = HttpContext.Current.Server.MapPath(pathOutputUploadImage);
            }
            else
            {
                resolvedInputPath = HttpContext.Current.Server.MapPath(pathInputImage);
                resolvedOutputPath = HttpContext.Current.Server.MapPath(pathOutputImage);
            }
            string resolvedTestPath = HttpContext.Current.Server.MapPath(testPath);
            string selectedJpegQualityTag = jpegQualityParams.SelectedItem.Text;
            string selectedChromaSubTag = jpegChromaSubParams.SelectedItem.Text;
            PageController.codificaToJpeg(resolvedInputPath, resolvedOutputPath, selectedJpegQualityTag, selectedChromaSubTag);
            jpegMessage.Text = "Codificato con " + jpegQualityParams.SelectedItem.Text + " , " + jpegChromaSubParams.SelectedItem.Text;
            if (uploadedInputImage)
            {
                outputJpegImageASP.ImageUrl = pathOutputUploadImage;
            }
            else
            {
                outputJpegImageASP.ImageUrl = pathOutputImage;
            }
        }

        protected void DoWatermarking(object sender, EventArgs e)
        {
            string resolvedOutputWaterPath = null;
            if (uploadedInputImage)
            {
                resolvedOutputWaterPath = HttpContext.Current.Server.MapPath(pathOutputUploadWatermarkedImage);
            }
            else
            {
                resolvedOutputWaterPath = HttpContext.Current.Server.MapPath(pathOutputWatermarkedImage);
            }
            string selectedWatermarkingMethod = watermarkingMethodRadioButton.SelectedItem.Text;
            if (PageController.checkAdvRGBWatermarkingChosen(selectedWatermarkingMethod))
            {
                bool canDoWatermarking = PageController.doRGBAdvancedWatermarking(resolvedOutputWaterPath);
                if (!canDoWatermarking)
                {
                    string errorText = "Numero di bit disponibili inferiore a quello di watermarking.";
                    esitoWatermarkingLabel.Text = errorText;
                    watermarkedImageASP.ImageUrl = pathRedCross;
                    decodedImageASP.ImageUrl = pathRedCross;
                    decodedImageASP2.ImageUrl = pathRedCross;
                    downloadWatermarkedImageButton.Enabled = false;
                    downloadDesktopImageLabel.Text = "Download non possibile";
                    logTextBox.Text += PageController.getWatermarkingNotPossibleStats();
                    return;
                }
            }
            else if (PageController.checkLuminanceRGBWatermarkingChosen(selectedWatermarkingMethod))
            {
                int numLSBSelectedBlock = Int32.Parse(numLsbYBlockText.Text);
                int numLSBNonSelectedBlock = Int32.Parse(numLsbOtherBlockText.Text);
                bool canDoWatermarking = PageController.doLuminanceRGBWatermarking(resolvedOutputWaterPath, numLSBSelectedBlock, numLSBNonSelectedBlock);
                if (!canDoWatermarking)
                {
                    string errorText = "Numero di bit disponibili inferiore a quello di watermarking.";
                    esitoWatermarkingLabel.Text = errorText;
                    watermarkedImageASP.ImageUrl = pathRedCross;
                    decodedImageASP.ImageUrl = pathRedCross;
                    decodedImageASP2.ImageUrl = pathRedCross;
                    downloadWatermarkedImageButton.Enabled = false;
                    downloadDesktopImageLabel.Text = "Download non possibile";
                    logTextBox.Text += PageController.getWatermarkingNotPossibleStats();
                    return;
                }
            }
            if (uploadedInputImage)
            {
                watermarkedImageASP.ImageUrl = pathOutputUploadWatermarkedImage;
            }
            else
            {
                watermarkedImageASP.ImageUrl = pathOutputWatermarkedImage;
            }
            string watermarkingStats = PageController.getWatermarkingStats();
            string watermarkingOk = "Watermarking avvenuto con successo.";
            esitoWatermarkingLabel.Text = watermarkingOk;
            logTextBox.Text += watermarkingStats;
        }
        
        protected void DoWatermarkingDecoding(object sender, EventArgs e)
        {
            try {
                bool decodificabile = PageController.doWatermarkingDecoding();
                if (!decodificabile)
                {
                    esitoDecodificaLabel.Text = "Watermark non decodificabile";
                }
                else
                {
                    esitoDecodificaLabel.Text = "Watermark decodificato con successo!";
                    decodedWatermarkTextBox.Text = PageController.getDecodedWatermark();
                    decodingWatermarkDone.ImageUrl = pathGreenBall;
                }
                string imageQualityStats = PageController.getImageQualityStats();
                logTextBox.Text += imageQualityStats;
                plotWatermarkingChart();
                plotWatermarkingPercentageChart();
                plotImageQualityChart();
            }
            catch (Exception ex)
            {
                esitoDecodificaLabel.Text = "Watermark non decodificabile";
            }
        }

        protected void UploadWatermarkedImage(object sender, EventArgs e)
        {
            string pathImageToDecodeWatermark = null;
            if (imageToDecodeUpload.HasFile)
            {
                string filename = null;
                try
                {
                    filename = Path.GetFileName(imageToDecodeUpload.FileName);
                    string realPath = HttpContext.Current.Server.MapPath("~/img/decodeUpload/") + filename;
                    imageToDecodeUpload.SaveAs(realPath);
                    pathImageToDecodeWatermark = "~/img/decodeUpload/" + filename;
                }
                catch (Exception ex) { }
                uploadedImageForDecode.ImageUrl = pathImageToDecodeWatermark;
                PageController.readInputImageToDecode(pathImageToDecodeWatermark);
            }
        }

        protected void DoWatermarkingDecodingUploadedImage(object sender, EventArgs e)
        {
            string selectedWatermarkingMethodForDecoding = watermarkingMethodForDecodingRadioButton.SelectedItem.Text;
            if (PageController.checkAdvRGBWatermarkingChosen(selectedWatermarkingMethodForDecoding))
            {

            }
            else if (PageController.checkLuminanceRGBWatermarkingChosen(selectedWatermarkingMethodForDecoding))
            {

            }
        }

        protected void DoChannelEncoding(object sender, EventArgs e)
        {
            int numRipetizioni = Int32.Parse(numRipetizioniTextBox.Text);
            PageController.doRipetizioneEncoding(numRipetizioni);
            channelEncodingDoneImage.ImageUrl = pathGreenBall;
        }

        protected void DoChannelError(object sender, EventArgs e)
        {
            string resolvedOutputDecodedImagePath = null;
            if (uploadedInputImage)
            {
                watermarkedImageASP.ImageUrl = pathOutputUploadDecodedImage;
                resolvedOutputDecodedImagePath = HttpContext.Current.Server.MapPath(pathOutputUploadDecodedImage);
            }
            else
            {
                watermarkedImageASP.ImageUrl = pathOutputDecodedImage;
                resolvedOutputDecodedImagePath = HttpContext.Current.Server.MapPath(pathOutputDecodedImage);
            }
            if (singleErrorRadioButton.Checked && burstErrorRadioButton.Checked)
            {
                singleErrorRadioButton.Checked = false;
                burstErrorRadioButton.Checked = false;
                return;
            }
            else if (singleErrorRadioButton.Checked && !burstErrorRadioButton.Checked)
            {
                string alphaString = alphaTextBox.Text;
                double alpha = Double.Parse(alphaString);
                PageController.singleError(alpha);
            }
            else if (!singleErrorRadioButton.Checked && burstErrorRadioButton.Checked)
            {
                double p = Double.Parse(pTextBox.Text);
                double r = Double.Parse(rTextBox.Text);
                PageController.burstError(p, r);
            }
            channelErrorDoneImage.ImageUrl = pathGreenBall;
            //decodifica immagine ricevuta dal canale
            bool imageDecoded = PageController.decodeTrasmittedImage();
            if (!imageDecoded)
            {
                string decodeError = "Non è possibile decodificare l'immagine.";
                decodificaLabel.Text = decodeError;
                decodedImageASP.ImageUrl = pathRedCross;
                decodedImageASP2.ImageUrl = pathRedCross;
                logTextBox.Text += PageController.getChannelErrorStats();
                return;
            }
            PageController.writeDecodedJpeg(resolvedOutputDecodedImagePath);
            if (uploadedInputImage)
            {
                decodedImageASP.ImageUrl = pathOutputUploadDecodedImage;
                decodedImageASP2.ImageUrl = pathOutputUploadDecodedImage;
            }
            else
            {
                decodedImageASP.ImageUrl = pathOutputDecodedImage;
                decodedImageASP2.ImageUrl = pathOutputDecodedImage;
            }
            string decodeOk = "Decodifica immagine avvenuta correttamente.";
            logTextBox.Text += PageController.getChannelErrorStats();
            decodificaLabel.Text = decodeOk;
        }

        /*
        protected void Invia(object sender, EventArgs e)
        {
            messaggio.Text = jpegQualityParams.SelectedItem.Text + " , " + jpegChromaSubParams.SelectedItem.Text;
        }
        */

        private void initJpegParamsList()
        {
            if (jpegQualityParams.Items.Count == 0 && jpegChromaSubParams.Items.Count == 0)
            {
                string[] jpegQualityParamsItems = PageController.getJpegQualityParamsItems();
                string[] jpegChromaSubItems = PageController.getJpegChromaSubItems();
                foreach (string item in jpegQualityParamsItems)
                    jpegQualityParams.Items.Add(item);
                foreach (string item in jpegChromaSubItems)
                    jpegChromaSubParams.Items.Add(item);
            }
            //jpegQualityParams.Items[0].Selected = true;
            //jpegChromaSubParams.Items[0].Selected = true;
        }
        
        protected void UncheckSampleImageRadioButton(object sender, EventArgs e)
        {
           sampleImageRadioButton.Checked = false;
        }

        protected void UncheckUploadImageRadioButton(object sender, EventArgs e)
        {
           uploadImageRadioButton.Checked = false;
        }

        private void initWatermarkingMethodsList()
        {
            if (watermarkingMethodRadioButton.Items.Count == 0)
            {
                string[] watermarkingMethodsItems = PageController.getWatermarkingMethodsItems();
                foreach (string item in watermarkingMethodsItems)
                    watermarkingMethodRadioButton.Items.Add(item);
            }
            //watermarkingMethodRadioButton.Items[0].Selected = true;
        }

        private void initWatermarkingDecodingMethodsList()
        {
            if (watermarkingMethodForDecodingRadioButton.Items.Count == 0)
            {
                string[] watermarkingMethodsItems = PageController.getWatermarkingMethodsItems();
                foreach (string item in watermarkingMethodsItems)
                    watermarkingMethodForDecodingRadioButton.Items.Add(item);
            }
            //watermarkingMethodRadioButton.Items[0].Selected = true;
        }

        private void plotWatermarkingChart()
        {
            System.Web.UI.DataVisualization.Charting.Series bitWatermarkingSerie = WatermarkingChart.Series["BitWatermark"];
            int numBitWatermark = PageController.getNumBitWatermark();
            bitWatermarkingSerie.Points.AddXY("# bit watermark", numBitWatermark);
            System.Web.UI.DataVisualization.Charting.Series bitAvailableSerie = WatermarkingChart.Series["BitAvailableForWatermarking"];
            int numAvailableBit = PageController.getNumBitAvailableForWatermarking();
            bitAvailableSerie.Points.AddXY("# bit per watermarking", numAvailableBit);
        }

        private void plotWatermarkingPercentageChart()
        {
            System.Web.UI.DataVisualization.Charting.Series bitWatermarkingSerie = PercentageChart.Series["BitWatermarkOnBitAvailable"];
            double numBitWatermarkOnBitAvailable = PageController.getNumBitWatermarkOnBitAvailable();
            bitWatermarkingSerie.Points.AddXY("# bit water / # bit disp.", numBitWatermarkOnBitAvailable);
            System.Web.UI.DataVisualization.Charting.Series bitAvailableSerie = PercentageChart.Series["BitWatermarkOnBitImage"];
            double numBitWatermarkOnBitImage = PageController.getNumBitWatermarkOnBitImage();
            bitAvailableSerie.Points.AddXY("# bit water / # bit img", numBitWatermarkOnBitImage);
        }

        private void plotImageQualityChart()
        {
            System.Web.UI.DataVisualization.Charting.Series mseSerie = MSEChart.Series["MSE"];
            double mse = PageController.getMSE();
            mseSerie.Points.AddXY("MSE", mse);
            System.Web.UI.DataVisualization.Charting.Series psnrSerie = MSEChart.Series["PSNR"];
            double psnr = PageController.getPSNR();
            psnrSerie.Points.AddXY("PSNR", psnr);
        }

        private static void initTextBox()
        {   
            
        }
    }
}