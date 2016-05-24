<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="JPEGWatermarkingWeb.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <meta name="description" content="" />
    <meta name="author" content="" />
    <title> JPEG Watermarking </title>
    <!-- Bootstrap Css -->
    <link href="css/bootstrap.min.css" type="text/css" rel="Stylesheet" />
    <link rel="stylesheet" type="text/css" href="css/animate.css" />
    <link rel="stylesheet" type="text/css" href="css/style.css" />
    <link href="css/default.css" rel="stylesheet" type="text/css" />
    <link href="font-awesome/css/font-awesome.min.css" rel="stylesheet" type="text/css" />
     <link href="http://maxcdn.bootstrapcdn.com/font-awesome/4.2.0/css/font-awesome.min.css"
        rel="stylesheet" type="text/css" />
</head>
<body id="page-top" data-spy="scroll" data-target=".navbar-custom">
    <form id="form1" runat="server">
    <div id="preloader">
        <div id="load">
        </div>
    </div>
    <nav class="navbar navbar-custom navbar-fixed-top" role="navigation">
        <div class="container">
            <div class="navbar-header page-scroll">
                <button type="button" class="navbar-toggle" data-toggle="collapse" data-target=".navbar-main-collapse">
                    <i class="fa fa-bars"></i>
                </button>
                <a class="navbar-brand" href="index.aspx">
                    <h1>JPEGWatermarking</h1>
                </a>
            </div>
      <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="true"/>
      <!-- Collect the nav links, forms, and other content for toggling -->
      <div class="collapse navbar-collapse navbar-right navbar-main-collapse">
      <ul class="nav navbar-nav">
        <li class="active"><a href="#lz78">LZ78</a></li>
        <li><a href="#watermarking">Watermarking</a></li>
		<li><a href="#canale">Canale</a></li>
		<li><a href="#decodifica">Decodifica</a></li>
    	<li><a href="#info">Info</a></li>
        <li><a href="#charts">Grafici</a></li>
        <li><a href="#utility">Utility</a></li>
      </ul>
            </div>
        </div>
    </nav>
    <!-- Section: intro -->
    <section id="intro" class="intro">
	
		<div class="slogan">
			<h2><span class="text_color">JPEGWatermarking</span> </h2>
			<h4>Dal testo all'immagine, dall'immagine al testo: 
            <br />
             la tecnica di watermarking, illustrata passo dopo passo.
            </h4>
		</div>
		<div class="page-scroll">
			<a href="#lz78" class="btn btn-circle">
				<i class="fa fa-angle-double-down animated"></i>
			</a>
		</div>
    </section>
    <!-- /Section: intro -->
    <section id="feature">
        <div class="container">
           <div class="center wow fadeInDown">
                <h2>Cosa posso fare?</h2>
                <p class="lead">Codifica il un testo con LZ78, scegli l'immagine e la codifica JPEG desiderata, la tecnica di Watermarking più adatta 
                    per inserire il testo nell'immagine. Potrai simulare la trasmissione su un canale non ideale, ed ottenere informazioni sulla qualità
                    dell'immagine contenente il watermark. Sarà possibile decodificare il testo all'interno di un immagine precedentemente elaborata.
                    <br />
                    Tutto questo, in una sola app. 
                </p>
            </div>
        </div><!--/.container-->
    </section>
    <!--/#feature-->

    <!-- Section: lz78 -->
    <section id="lz78" class="home-section text-center">
		<div class="heading-about">
			<div class="container">
			<div class="row">
				<div class="col-lg-8 col-lg-offset-2">
					<div class="wow fadeInUp" data-wow-delay="0.4s">
					<div class="section-heading">
					<h2>LZ78</h2>
					<br />
					</div>
					</div>
				</div>
			</div>
			</div>
		</div>
		<div class="container">
		<asp:UpdateProgress runat="server" id="UpdateProgress1" AssociatedUpdatePanelID="UpdatePanel1"> 
	        <ProgressTemplate> 
            Loading... 
	        </ProgressTemplate> 
	    </asp:UpdateProgress> 
       <div class="row">
            <div class="col-xs-6">
                <div class="wow bounceInUp" data-wow-delay="0.2s">
                <div class="team boxed-grey">
                <h3> Testo in ingresso </h3>
				    <asp:TextBox id="lz78InputText" TextMode="MultiLine" Height="200px" Width="300px" runat="server" 
                        ToolTip="Scrivi qui il testo da codificare, o clicca su 'Scegli File' per inserire il testo da file." />
                    <br />
                    <asp:FileUpload id="inputTextUpload" runat="server" ToolTip="Clicca per inserire il testo da file." />
                    <asp:Label Text="1) " runat="server"/><asp:Button Id="uploadInputText" OnClick="UploadInputText" Text="Upload Testo" runat="server" ToolTip="Clicca per inserire il testo del file nella casella di input." />
                    <br />
                </div>
				</div>
            </div>
			<div class="col-xs-6">
        		<div class="wow bounceInUp" data-wow-delay="0.5s">
                <div class="team boxed-grey">
                <asp:UpdatePanel ID="UpdatePanel1" runat="server"> 
	            <ContentTemplate> 
                    <h3> Codifica a dizionario </h3>
				    <asp:TextBox id="lz78OutputText" TextMode="MultiLine" Height="200px" Width="300px" runat="server" />
                    <br /> <br />
                    <asp:Label Text="2) " runat="server"/><asp:Button Id="lz78Button" OnClick="DoLZ78Encoding" Text="Codifica" runat="server" ToolTip="Clicca per effettuare la codifica del testo di input." />
                </ContentTemplate>
                </asp:UpdatePanel>
                </div>
		        </div>
            </div>
			</div>
        </div>		
	</section>
    <!-- /Section: lz78 -->
    
    <!-- Section: watermarking -->
    <section id="watermarking" class="home-section text-center">
		<div class="heading-about">
			<div class="container">
			<div class="row">
				<div class="col-lg-8 col-lg-offset-2">
					<div class="wow fadeInUp" data-wow-delay="0.4s">
					<div class="section-heading">
					<h2>Watermarking</h2>
					<br />
					</div>
					</div>
				</div>
			</div>
			</div>
		</div>
		<div class="container">
		<asp:UpdateProgress runat="server" id="UpdateProgress2" AssociatedUpdatePanelID="UpdatePanel2"> 
	        <ProgressTemplate> 
            Loading... 
	        </ProgressTemplate> 
	    </asp:UpdateProgress> 
        <div class="row">
            <div class="col-xs-6">
				<div class="wow bounceInUp" data-wow-delay="0.2s">
                <div class="team boxed-grey">
                <h3> Immagine in ingresso </h3>
		            <asp:UpdatePanel ID="UpdatePanel9" runat="server"> 
	                <ContentTemplate> 
           	        <asp:Label Text="1) [scegliere una sola opzione]" runat="server"/>
                    <br /> 
                    <asp:RadioButton ID="uploadImageRadioButton" Text="Upload Immagine" runat="server" GroupName="group1"/>
                    <asp:RadioButton ID="sampleImageRadioButton" Text="Usa Sample Image (Twitter Logo)" runat="server" GroupName="group1" Checked="true"/>
                    </ContentTemplate> 
           	        </asp:UpdatePanel>
                    <asp:FileUpload id="FileUploadControl" runat="server" ToolTip="Clicca per scegliere l'immagine da utilizzare." />
                    <asp:Label Text="2) " runat="server"/><asp:Button runat="server" id="UploadButton" text="Upload" onclick="UploadButton_Click" ToolTip="Clicca per effettuare l'upload dell'immagine o utilizzare l'immagine di esempio Twitter Logo."/>
                    <asp:Image ID="uploadDoneImage" ImageUrl="img/redball.png" Width="16px" Height="16px" runat="server"/>
                    <br /><br />
                    <asp:Image ID="inputImage" ImageUrl="img/null_input_image_320x320.png" Width="320px" Height="320px" runat="server"/>
				</div>
                </div>
            </div>
            <div class="col-xs-6">
		        <div class="wow bounceInUp" data-wow-delay="0.5s">
                <div class="team boxed-grey">
                    <asp:UpdatePanel ID="UpdatePanel2" runat="server"> 
	                <ContentTemplate> 
           	        <h3> Codifica JPEG </h3>
		                <asp:ListBox ID="jpegQualityParams" SelectionMode="Single" runat="server" />
                        <asp:ListBox ID="jpegChromaSubParams" SelectionMode="Single" runat="server" />
                        <br />
                        <asp:Label Text="3) " runat="server"/><asp:Button runat="server" id="Button1" text="Codifica" onclick="CodificaToJpeg" ToolTip="Clicca per effettuare la codifica JPEG dell'immagine di input scelta." />
                        <br />
                        <asp:Label id="jpegMessage" Text="In attesa di codifica..." runat="server" />
                        <br />
                        <asp:Image ID="outputJpegImageASP" ImageUrl="img/null_input_image_320x320.png" Width="320px" Height="320px" runat="server"/>
                        </ContentTemplate>
                        </asp:UpdatePanel>
                </div>
				</div>
            </div>
            </div>
            <br /> <br /> 
            <!-- scelta metodo watermarking -->
            <div class="row">
            <asp:UpdatePanel ID="UpdatePanel4" runat="server"> 
	        <ContentTemplate>
            <div class="col-xs-6">
				<div class="wow bounceInUp" data-wow-delay="0.2s">
                <div class="team boxed-grey">
                <h3> Tecnica Watermarking </h3>
                    <br /> 
		            <asp:Label Text="4) [scegliere una sola opzione] " runat="server"/> <br />
                    <p><asp:RadioButtonList ID="watermarkingMethodRadioButton" runat="server" /></p>
                    <br /> <br /> 
                    <p> num LSB Blocchi Selezionati 
                        <asp:TextBox id="numLsbYBlockText" TextMode="Number" min="1" max="4" step="1" value="1" runat="server"/>
                    </p>
                    <p> num LSB Blocchi Non Selezionati 
                        <asp:TextBox id="numLsbOtherBlockText" TextMode="Number" min="1" max="4" step="1" value="1" runat="server"/>
                    </p>
                    <asp:Label Text="5) " runat="server"/><asp:Button runat="server" id="Button2" text="Inietta" onclick="DoWatermarking" ToolTip="Clicca per effettuare il watermarking." />
                    <br /> <br /> <br /> <br /> <br /> <br />
                </div>
                </div>
            </div>
		    <div class="col-xs-6">
                <div class="wow bounceInUp" data-wow-delay="0.5s">
                <div class="team boxed-grey">
                    <h3> Immagine con Watermark </h3>
		                <asp:Label id="esitoWatermarkingLabel" Text="In attesa di codifica..." runat="server" />
                        <br />
                        <asp:Image ID="watermarkedImageASP" ImageUrl="img/null_input_image_320x320.png" Width="320px" Height="320px" runat="server"/>
                        <br /> <br />
                        <asp:Button id="downloadWatermarkedImageButton" text="Salva su Desktop" onclick="DownloadWatermarkedImage" runat="server" ToolTip="Clicca per salvare l'immagine watermarked sul desktop." />
                        <br /> 
                        <asp:Label id="downloadDesktopImageLabel" Text="In attesa di download..." runat="server" />
                        <br />
			    </div>
				</div>
            </div>  
            </ContentTemplate>
            </asp:UpdatePanel>
            </div>
	</section>
    <!-- /Section:  -->
    <!-- Section: canale -->
    <section id="canale" class="home-section text-center">
		<div class="heading-about">
			<div class="container">
			<div class="row">
				<div class="col-lg-8 col-lg-offset-2">
					<div class="wow fadeInUp" data-wow-delay="0.4s">
					<div class="section-heading">
					<h2>Canale</h2>
					<br />
					</div>
					</div>
				</div>
			</div>
			</div>
		</div>
		<div class="container">
		<asp:UpdateProgress runat="server" id="UpdateProgress3" AssociatedUpdatePanelID="UpdatePanel5"> 
	        <ProgressTemplate> 
            Loading... 
	        </ProgressTemplate> 
	    </asp:UpdateProgress> 
        <div class="row">
            <asp:UpdatePanel ID="UpdatePanel5" runat="server"> 
	        <ContentTemplate>
            <div class="col-xs-6">
            	<div class="wow bounceInUp" data-wow-delay="0.2s">
                <div class="team boxed-grey">
                <h3> Codifica di Canale </h3>
                    <br /> <br />
		            num Ripetizioni <asp:TextBox id="numRipetizioniTextBox" TextMode="Number" min="1" max="30" step="1" value="1" runat="server"/>
                    <asp:Label Text="1) " runat="server"/><asp:Button id="Button3" text="Codifica" onclick="DoChannelEncoding" runat="server" ToolTip="Clicca per effettuare la codifica di canale (codifica a ripetizione)." />
                    <asp:Image ID="channelEncodingDoneImage" ImageUrl="img/redball.png" Width="16px" Height="16px" runat="server"/>
                    <br /> <br /> 
                    <asp:Label Text="2) [scegliere una sola opzione]" runat="server"/>
                    <p> 
                        <asp:RadioButton ID="singleErrorRadioButton" Text="Single Error" runat="server" GroupName="group2" Checked="true"/>
                        <br /> 
                        alpha <asp:TextBox id="alphaTextBox" value="0,001" runat="server"/>
                    </p>
                    <p> 
                        <asp:RadioButton ID="burstErrorRadioButton" Text="Gilbert-Elliot Burst Error" runat="server" GroupName="group2" />
                        <br />  
                        p <asp:TextBox id="pTextBox" value="0,5" runat="server"/> (p = prob. di transizione da stato G a stato B)
                        <br />
                        r <asp:TextBox id="rTextBox" value="0,5" runat="server"/> (r = prob. di transizione da stato B a stato G)
                    </p>
                    <asp:Label Text="3) " runat="server"/><asp:Button id="Button6" text="Trasmetti" onclick="DoChannelError" runat="server" ToolTip="Clicca per simulare la trasmissione del flusso binario sul canale."/>
                    <asp:Image ID="channelErrorDoneImage" ImageUrl="img/redball.png" Width="16px" Height="16px" runat="server"/>
                    <br /> <br /> <br /> <br /> 
                </div>
                </div>
            </div>
            <div class="col-xs-6">
		        <div class="wow bounceInUp" data-wow-delay="0.5s">
                <div class="team boxed-grey">
                    <h3> Immagine Decodificata </h3>
		                <asp:Label id="decodificaLabel" Text="In attesa di decodifica..." runat="server" />
                        <br />
                        <asp:Image ID="decodedImageASP" ImageUrl="img/null_input_image_320x320.png" Width="320px" Height="320px" runat="server"/>
			    </div>
				</div>
            </div>
            </ContentTemplate>
            </asp:UpdatePanel>
            </div>
            </div>
	</section>
    <!-- /Section:  -->
    <!-- Section: decodifica -->
    <section id="decodifica" class="home-section text-center">
		<div class="heading-about">
			<div class="container">
			<div class="row">
				<div class="col-lg-8 col-lg-offset-2">
					<div class="wow fadeInUp" data-wow-delay="0.4s">
					<div class="section-heading">
					<h2>Decodifica Watermarking</h2>
					<br />
					</div>
					</div>
				</div>
			</div>
			</div>
		</div>
		<div class="container">
		<asp:UpdateProgress runat="server" id="UpdateProgress4" AssociatedUpdatePanelID="UpdatePanel5"> 
	        <ProgressTemplate> 
            Loading... 
	        </ProgressTemplate> 
	    </asp:UpdateProgress> 
        <div class="row">
            <asp:UpdatePanel ID="UpdatePanel6" runat="server"> 
	        <ContentTemplate>
            <div class="col-xs-6">
            	<div class="wow bounceInUp" data-wow-delay="0.2s">
                <div class="team boxed-grey">
                <h3> Immagine con watermark </h3>
                    <asp:Image ID="decodedImageASP2" ImageUrl="img/null_input_image_320x320.png" Width="320px" Height="320px" runat="server"/>
                    <br /> <br /> 
                    <asp:Label Text="1) " runat="server"/><asp:Button id="Button4" text="Decodifica" onclick="DoWatermarkingDecoding" runat="server" ToolTip="Clicca per estrarre il watermark dall'immagine e decodificarlo." />
		        </div>
                </div>
            </div>
            <div class="col-xs-6">
		        <div class="wow bounceInUp" data-wow-delay="0.5s">
                <div class="team boxed-grey">
                    <h3> Testo decodificato </h3>
		                <asp:Label id="esitoDecodificaLabel" Text="In attesa di decodifica..." runat="server" />
                        <asp:Image ID="decodingWatermarkDone" ImageUrl="img/redball.png" Width="16px" Height="16px" runat="server"/>
                        <br /> <br />
                        <asp:TextBox id="decodedWatermarkTextBox" TextMode="MultiLine" Height="200px" Width="300px" runat="server" />
                    <br /> <br /> <br /> <br /> <br /> <br /> <br />
                </div>
				</div>
            </div>
            </ContentTemplate>
            </asp:UpdatePanel>
            </div>
            </div>
	</section>
    <!-- /Section:  -->
    <!-- Section: info -->
    <section id="info" class="home-section text-center">
		<div class="heading-about">
			<div class="container">
			<div class="row">
				<div class="col-lg-8 col-lg-offset-2">
					<div class="wow fadeInUp" data-wow-delay="0.4s">
					<div class="section-heading">
					<h2>INFO</h2>
					<br />
					</div>
					</div>
				</div>
			</div>
			</div>
		</div>
		<div class="container">
		<asp:UpdatePanel ID="UpdatePanel3" runat="server"> 
	    <ContentTemplate> 
        <div class="row">
                <div class="wow bounceInUp" data-wow-delay="0.2s">
                <div class="team boxed-grey">
                <h3> Informazioni utili </h3>
				    <asp:TextBox id="logTextBox" TextMode="MultiLine" Height="200px" Width="1000px" runat="server" />
                    <br />
                    <asp:Button id="saveInfoButton" text="Download info su Desktop" onclick="DownloadLogTextInfo" runat="server" ToolTip="Clicca per salvare il log sul Desktop." />
                    <asp:Label id="downloadLogFileLabel" Text="In attesa di download..." runat="server" />
                    <br />
                </div>
				</div>
        </div>
        </ContentTemplate>
        </asp:UpdatePanel>	
        </div>	
	</section>
    <!-- /Section: info -->


    <!-- Section: charts -->
    <section id="charts" class="home-section text-center">
		<div class="heading-about">
			<div class="container">
			<div class="row">
				<div class="col-lg-8 col-lg-offset-2">
					<div class="wow fadeInUp" data-wow-delay="0.4s">
					<div class="section-heading">
					<h2>GRAFICI</h2>
					<br />
					</div>
					</div>
				</div>
			</div>
			</div>
		</div>
		<div class="container">
		<div class="row">
                <div class="wow bounceInUp" data-wow-delay="0.2s">
                <div class="team boxed-grey">
                <asp:UpdatePanel ID="UpdatePanel8" runat="server"> 
	            <ContentTemplate>
                    <asp:Chart ID="WatermarkingChart" runat="server">
                    <Titles>
                        <asp:Title Text="Confronto tra bit watermark e bit immagine"/>
                    </Titles>
                    <series>
                        <asp:Series Legend="bitLegend" Name="BitWatermark" LegendText="bit watermark" YValueType="Int32" ChartType="Column" IsValueShownAsLabel="True" ChartArea="AreaWatermark"/>
                        <asp:Series Legend="bitLegend" Name="BitAvailableForWatermarking" LegendText="bit img per watermark" YValueType="Int32" ChartType="Column" IsValueShownAsLabel="True" ChartArea="AreaWatermark"/>
                    </series>
                    <chartareas>    
                        <asp:ChartArea Name="AreaWatermark"/>
                    </chartareas>
                    <Legends>
                        <asp:Legend Name="bitLegend" Title="Tipo di bit">
                    </asp:Legend>
                    </Legends>
                    </asp:Chart>
                    <asp:Chart ID="PercentageChart" runat="server">
                    <Titles>
                        <asp:Title Text="Percentuali di utilizzo dei bit"/>
                    </Titles>
                    <series>
                        <asp:Series Legend="bitLegend1" Name="BitWatermarkOnBitAvailable" LabelFormat="P0"  LegendText="bit waterm \ bit disp (%)" YValueType="Int32" ChartType="Column" IsValueShownAsLabel="True" ChartArea="AreaWatermark1"/>
                        <asp:Series Legend="bitLegend1" Name="BitWatermarkOnBitImage" LabelFormat="P0" LegendText="bit waterm \ bit img (%)" YValueType="Int32" ChartType="Column" IsValueShownAsLabel="True" ChartArea="AreaWatermark1"/>
                    </series>
                    <chartareas>    
                        <asp:ChartArea Name="AreaWatermark1">
                        </asp:ChartArea>
                    </chartareas>
                    <Legends>
                        <asp:Legend Name="bitLegend1" Title="Percentuali di utilizzo">
                    </asp:Legend>
                    </Legends>
                    </asp:Chart>
                    <asp:Chart ID="MSEChart" runat="server">
                    <Titles>
                        <asp:Title Text="Qualità immagine watermarked"/>
                    </Titles>
                    <series>
                        <asp:Series Legend="bitLegend3" Name="MSE" LegendText="MSE" YValueType="Int32" ChartType="Column" IsValueShownAsLabel="True" ChartArea="AreaWatermark3"/>
                        <asp:Series Legend="bitLegend3" Name="PSNR" LegendText="PSNR" YValueType="Int32" ChartType="Column" IsValueShownAsLabel="True" ChartArea="AreaWatermark3"/>
                    </series>
                    <chartareas>    
                        <asp:ChartArea Name="AreaWatermark3"/>
                    </chartareas>
                    <Legends>
                        <asp:Legend Name="bitLegend3" Title="Indicatori qualità">
                    </asp:Legend>
                    </Legends>
                    </asp:Chart>
                </ContentTemplate>
                </asp:UpdatePanel>
                </div>
                </div>
        </div>
        </div>	
	</section>
    <!-- /Section: charts -->
    <!-- Section: utility -->
    <section id="utility" class="home-section text-center">
		<div class="heading-about">
			<div class="container">
			<div class="row">
				<div class="col-lg-8 col-lg-offset-2">
					<div class="wow fadeInUp" data-wow-delay="0.4s">
					<div class="section-heading">
					<h2>Utility </h2> 
                     <h3> Da immagine watermarked a testo in chiaro </h3>
					<br />
					</div>
					</div>
				</div>
			</div>
			</div>
		</div>
		<div class="container">
		<asp:UpdateProgress runat="server" id="UpdateProgress5" AssociatedUpdatePanelID="UpdatePanel5"> 
	        <ProgressTemplate> 
            Loading... 
	        </ProgressTemplate> 
	    </asp:UpdateProgress> 
        <div class="row">
           <div class="col-xs-6">
            	<div class="wow bounceInUp" data-wow-delay="0.2s">
                <div class="team boxed-grey">
                <h3> Upload Immagine con watermark </h3>
                    <asp:FileUpload id="imageToDecodeUpload" runat="server" />
                    <asp:Button runat="server" id="Button7" text="Upload" onclick="UploadWatermarkedImage" />
                    <br/> <br/>
                    <asp:Image ID="uploadedImageForDecode" ImageUrl="img/null_input_image_320x320.png" Width="320px" Height="320px" runat="server"/>
                    <br /> <br /> 
                    <p>
                       <asp:Label id="Label2" Text="Metodo di watermarking utilizzato" runat="server" />
                       <asp:RadioButtonList ID="watermarkingMethodForDecodingRadioButton" runat="server" />
                    </p>
		        </div>
                </div>
            </div>
            <div class="col-xs-6">
		        <div class="wow bounceInUp" data-wow-delay="0.5s">
                <div class="team boxed-grey">
                    <asp:UpdatePanel ID="UpdatePanel7" runat="server"> 
	                <ContentTemplate>
                    <h3> Testo decodificato </h3>
		                <asp:Label id="Label1" Text="In attesa di decodifica..." runat="server" />
                        <asp:Image ID="Image2" ImageUrl="img/redball.png" Width="16px" Height="16px" runat="server"/>
                        <br /> <br />
                        <asp:TextBox id="decodificaUploadWaterImage" TextMode="MultiLine" Height="200px" Width="300px" runat="server" />
                        <br /> <br /> <br /> 
                        <asp:Button id="Button5" text="Decodifica" onclick="DoWatermarkingDecodingUploadedImage" runat="server" />
                        <br /> <br /> <br /> <br />
                  </ContentTemplate>
                 </asp:UpdatePanel>
                 </div>
				</div>
            </div>
            </div>
            </div>
	</section>
    <!-- /Section:  -->
    <footer>
		<div class="container">
			<div class="row">
				<div class="col-md-12 col-lg-12">
					<div class="wow shake" data-wow-delay="0.4s">
					<div class="page-scroll marginbot-30">
						<a href="#intro" id="totop" class="btn btn-circle">
							<i class="fa fa-angle-double-up animated"></i>
						</a>
					</div>
					</div>
					<p>&copy;Copyright 2016 - Giuseppe Silvestri | Francesco Giuseppe Pirillo. </p>
				</div>
			</div>	
		</div>
	</footer>
    <!-- Core JavaScript Files -->
    <script src="js/jquery.min.js" type="text/javascript"></script>
    <script src="js/bootstrap.min.js" type="text/javascript"></script>
    <script src="js/jquery.easing.min.js" type="text/javascript"></script>
    <script src="js/jquery.scrollTo.js" type="text/javascript"></script>
    <script src="js/wow.min.js"></script>
    <!-- Custom Theme JavaScript -->
    <script src="js/custom.js" type="text/javascript"></script>
    </form>
</body>
</html>
