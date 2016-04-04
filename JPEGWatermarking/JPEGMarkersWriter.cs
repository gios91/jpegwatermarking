using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPEGEncoding
{
    using System;
    using System.IO;

   public class JPEGMarkersWriter
   {
        private int imageWidthPx;
        private int imageHeightPx;
        private int SUBSAMPLING_TYPE;
        
        public JPEGMarkersWriter(int imageWidthPx, int imageHeightPx, int SUBSAMPLING_TYPE)
        {
           this.imageWidthPx = imageWidthPx;
           this.imageHeightPx = imageHeightPx;
           this.SUBSAMPLING_TYPE = SUBSAMPLING_TYPE;
        }

        public void writeSOI(BinaryWriter bw)
        {
            UInt16 marker_0 = 0xD8FF; 
            bw.Write(marker_0);
        }

        public void writeAPP0(BinaryWriter bw)
        {
            UInt16 marker = 0xFFE0;
            UInt16 length = 16; // Length of segment excluding APP0 marker		
            byte versionH = 1; // 
            byte versionL = 1; // 1.1
            byte xyUnits = 0;   // No units; width:height pixel aspect ratio = Xdensity:Ydensity
            UInt16 xDensity = 1;  // Horizontal pixel density.
            UInt16 yDensity = 1;  // Vertical pixel density.
            byte xThumbn = 0; // No thumb
            byte yThumbn = 0; // No thumb
            //Marker (2 B)
            writeHex(bw, marker);
            //Length (2 B)
            writeHex(bw, length);
            //Identifier (5 B)
            bw.Write('J');
            bw.Write('F');
            bw.Write('I');
            bw.Write('F');
            bw.Write((byte)0x0);
            //Version (2 B)
            bw.Write(versionH);
            bw.Write(versionL);
            //Density Units (1 B)
            bw.Write(xyUnits);
            //X Density (1 B)
            writeHex(bw, xDensity);
            //Y Density (1 B)
            writeHex(bw, yDensity);
            //X Thumbnail (1 B) 
            bw.Write(xThumbn);
            //Y Thumbnail (1 B) 
            bw.Write(yThumbn);
        }

        public void writeCOM(BinaryWriter bw)
        {
            UInt16 marker = 0xFFFE;
            bw.Write(marker);
            string s = "A";
            bw.Write(s);
            // si assume non sia utilizzato nell'immagine
        }

        public void writeDQT(BinaryWriter bw)
        {
            UInt16 marker = 0xFFDB;
            UInt16 length = 132;  // = Length of segment excluding DQT marker	
            byte QTinfoY = 0;     // bit 0..3: Table identifier (0 for Y), bit 4..7: Quantization value size (0 for 8 bit)
            byte QTinfoCbCr = 1;  // bit 0..3: Table identifier (1 for CbCr), bit 4..7: ...            
            //Marker (2 B)
            writeHex(bw, marker);
            //Length (2 B)
            writeHex(bw, length);
            //DQT matrix info (1 B)
            bw.Write(QTinfoY);
            //DQT matrix Y 
            writeByteArray(bw, matrixToByteArray(JPEGUtility.QuantizationYMatrix));
            //DQT matrix info (1 B)
            bw.Write(QTinfoCbCr);
            //DQT matrix CbCr
            writeByteArray(bw, matrixToByteArray(JPEGUtility.QuantizationCMatrix));
        }

        public void writeSOF(BinaryWriter bw)
        {
            UInt16 marker = 0xFFC0;
            UInt16 length = 17;     // Standard value for truecolor JPEG
            byte precision = 8;     // Range of value for a color component           
            byte numComponents = 3; // Y, Cb, Cr
            byte YID = 1;  // Component ID
            byte YSubsamplingFactor = 0x22; // 4:4:4 sampling factor for Y (bit 0-3 vert., 4-7 hor.)
            byte QTForY = 0;  // Quantization Table number for Y = 0
            byte CbID = 2; // Component ID
            byte CbSubsamplingFactor = getSubsamplingFactor();
            byte QTForCb = 1; // Quantization Table number for Cb = 1
            byte CrID = 3; // Component ID
            byte CrSubsamplingFactor = getSubsamplingFactor();
            byte QTForCr = 1; // Quantization Table number for Cr = 1
            writeHex(bw, marker);
            writeHex(bw, length);
            bw.Write(precision);
            writeHex(bw, imageHeightPx);
            writeHex(bw, imageWidthPx);
            bw.Write(numComponents);
            bw.Write(YID);
            bw.Write(YSubsamplingFactor);
            bw.Write(QTForY);
            bw.Write(CbID);
            bw.Write(CbSubsamplingFactor);
            bw.Write(QTForCb);
            bw.Write(CrID);
            bw.Write(CrSubsamplingFactor);
            bw.Write(QTForCr);
        }

        public void writeDHT(BinaryWriter bw)
        {
            UInt16 marker = 0xFFC4;
            UInt16 length = 0x01A2;
            byte HTYDCinfo = 0x00; // bit 0..3: number of HT (0..3), for Y =0
                                   // bit 4  :type of HT, 0 = DC table, 1 = AC table
                                   // bit 5..7: not used, must be 0
            byte[] YDCDHTwordsCount = intToByteArray(JPEGUtility.DHTLuminanceDC.categoryWordCounts); 
            byte[] YDCDHTWords = intToByteArray(JPEGUtility.DHTLuminanceDC.categoryWordCounts);
            byte HTYACinfo = 0x10; // = 0x10
            byte[] YACDHTwordsCount = intToByteArray(JPEGUtility.DHTLuminanceAC.categoryWordCounts); 
            byte[] YACDHTWords = intToByteArray(JPEGUtility.DHTLuminanceAC.categoryWordCounts);
            byte HTCbDCinfo = 0x01; // = 1
            byte[] CDCDHTwordsCount = intToByteArray(JPEGUtility.DHTChrominanceDC.categoryWordCounts);
            byte[] CDCDHTWords = intToByteArray(JPEGUtility.DHTChrominanceDC.categoryWordCounts);
            byte HTCbACinfo = 0x11; //  = 0x11
            byte[] CACDHTwordsCount = intToByteArray(JPEGUtility.DHTChrominanceAC.categoryWordCounts);
            byte[] CACDHTWords = intToByteArray(JPEGUtility.DHTChrominanceAC.categoryWordCounts);
            writeHex(bw, marker);
            writeHex(bw, length);
            bw.Write(HTYDCinfo);
            writeByteArray(bw, YDCDHTwordsCount);
            writeByteArray(bw, YDCDHTWords);
            bw.Write(HTYACinfo);
            writeByteArray(bw, YACDHTwordsCount);
            writeByteArray(bw, YACDHTWords);
            bw.Write(HTCbDCinfo);
            writeByteArray(bw, CDCDHTwordsCount);
            writeByteArray(bw, CDCDHTWords);
            bw.Write(HTCbACinfo);
            writeByteArray(bw, CACDHTwordsCount);
            writeByteArray(bw, CACDHTWords);
        }

        public void writeSOS(BinaryWriter bw)
        {
            UInt16 marker = 0xFFDA;
            UInt16 length = 12;
            byte numComponents = 3; // Components: YCbCr
            byte YID = 1;
            byte DHTTableY = 0; // bits 0..3: AC table , bits 4..7: DC table 
            byte CbID = 2;
            byte DHTTableCb = 0x11;
            byte CrID = 3;
            byte DHTTableCr = 0x11;
            byte spectralSelectionStar = 0, 
            spectralSelectionEnd = 0x3F, 
            successiveApproximation = 0; // 0,63,0 for JPEG image
            writeHex(bw, marker);
            writeHex(bw, length);
            bw.Write(numComponents);
            bw.Write(YID);
            bw.Write(DHTTableY);
            bw.Write(CbID);
            bw.Write(DHTTableCb);
            bw.Write(CrID);
            bw.Write(DHTTableCr);
            bw.Write(spectralSelectionStar);
            bw.Write(spectralSelectionEnd);
            bw.Write(successiveApproximation);
        }

        public void writeEOI(BinaryWriter bw)
        {
            UInt16 marker = 0xD9FF;
            bw.Write(marker);
        }

        public void WriteJPEGHeader(BinaryWriter bw)
        {
            writeSOI(bw);
            /*
            writeAPP0(bw);
            writeDQT(bw);
            writeSOF(bw);
            writeDHT(bw);
            writeSOS(bw);
            */
            //writeCOM(bw);
            writeEOI(bw);
            bw.Close();
        }

        private byte getSubsamplingFactor()
        {
            if (SUBSAMPLING_TYPE == JPEGUtility.NO_SUBSAMPLING)
                return 0x22;
            else if (SUBSAMPLING_TYPE == JPEGUtility.SUBSAMPLING_420)
                return 0x11;
            else
                return 0x12;
        }

        public static void writeHex(BinaryWriter bwX, Int32 data)
        {
            bwX.Write((byte)(data / 256));
            bwX.Write((byte)(data % 256));
        }

        public static void writeByteArray(BinaryWriter bwX, Byte[] data)
        {
            int len = data.Length;
            for (int i = 0; i < len; i++)
            {
                bwX.Write((byte)data[i]);
            }
        }

        private byte[] matrixToByteArray(byte[,] Matrix)
        {
            int rows = Matrix.GetLength(0);
            int columns = Matrix.GetLength(1);
            int dim = rows * columns;
            byte[] v = new byte[dim];
            int cnt = 0;
            for (int i=0; i<rows; i++)
                for (int j=0; j<columns; j++)
                {
                    v[cnt] = Matrix[i, j];
                    cnt++;
                }
            return v;
        }

        public byte[] intToByteArray(int[] Vector)
        {
            byte[] byteV = new byte[Vector.Length];
            for (int i = 0; i < Vector.Length; i++)
                byteV[i] = Convert.ToByte(Vector[i]);
            return byteV;
        }
    }
}
