using JPEGWatermarkingWeb.Controller;
using System;
using System.Collections.Generic;
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
                uploadDoneImage.ImageUrl = pathGreenBall;
                inputImage.ImageUrl = pathInputImage;
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
                    downloadWatermarkedImageButton.Enabled = false;
                    downloadDesktopImageLabel.Text = "Download non possibile";
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
        }


        protected void DoWatermarkingDecodingUploadedImage(object sender, EventArgs e)
        {
            //TODO
        }


        protected void UploadWatermarkedImage(object sender, EventArgs e)
        {
            //TODO

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
                uploadDoneImage.ImageUrl = pathGreenBall;
                inputImage.ImageUrl = pathInputImage;
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
            if (singleErrorRadioButton.Checked && !burstErrorRadioButton.Checked)
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
        
        private static void initTextBox()
        {   
            
        }
    }
}