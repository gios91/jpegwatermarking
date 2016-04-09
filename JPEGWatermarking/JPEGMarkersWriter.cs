using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPEGEncoding
{
    using System;
    using System.Collections;
    using System.IO;

    public class JPEGMarkersWriter
   {
        private int imageWidthPx;
        private int imageHeightPx;
        private int SUBSAMPLING_TYPE;

        private static bool I = true;
        private static bool O = false;

        UInt16[] binaryDimToHex = { 0x00, 0x01, 0x03, 0x07, 0x0f, 0x1f, 0x3f, 0x7f, 0xff, 0x1ff, 0x3ff, 0xff00};
        UInt16[] categoryToHex = { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b };

        private BitBinaryWriter bbw;

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
            UInt16 marker = 0xE0FF;
            UInt16 length = 16; // Length of segment excluding APP0 marker		
            byte versionH = 1; // 
            byte versionL = 1; // 1.1
            byte xyUnits = 0;   // No units; width:height pixel aspect ratio = Xdensity:Ydensity
            UInt16 xDensity = 1;  // Horizontal pixel density.
            UInt16 yDensity = 1;  // Vertical pixel density.
            byte xThumbn = 0; // No thumb
            byte yThumbn = 0; // No thumb
            //Marker (2 B)
            bw.Write(marker);
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
            bw.Write(xDensity);
            //Y Density (1 B)
            bw.Write(yDensity);
            //X Thumbnail (1 B) 
            bw.Write(xThumbn);
            //Y Thumbnail (1 B) 
            bw.Write(yThumbn);
        }

        public void writeCOM(BinaryWriter bw)
        {
            UInt16 marker = 0xFEFF;
            bw.Write(marker);
            string s = "A";
            bw.Write(s);
            // si assume non sia utilizzato nell'immagine
        }

        public void writeDQT(BinaryWriter bw)
        {
            UInt16 marker = 0xDBFF;
            UInt16 length = 132;  // = Length of segment excluding DQT marker	
            byte QTinfoY = 0;     // bit 0..3: Table identifier (0 for Y), bit 4..7: Quantization value size (0 for 8 bit)
            byte QTinfoCbCr = 1;  // bit 0..3: Table identifier (1 for CbCr), bit 4..7: ...            
            //Marker (2 B)
            bw.Write(marker);
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
            UInt16 marker = 0xC0FF;
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
            bw.Write(marker);
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
            UInt16 marker = 0xC4FF;
            UInt16 length = 0x01A2;
            byte HTYDCinfo = 0x00; // bit 0..3: number of HT (0..3), for Y =0
                                   // bit 4  :type of HT, 0 = DC table, 1 = AC table
                                   // bit 5..7: not used, must be 0
            byte[] YDCDHTwordsCount = intToByteArray(JPEGUtility.DHTLuminanceDC.categoryWordCounts); 
            byte[] YDCDHTWords = intToByteArray(JPEGUtility.DHTLuminanceDC.categoryWords);
            byte HTYACinfo = 0x10; // = 0x10
            byte[] YACDHTwordsCount = intToByteArray(JPEGUtility.DHTLuminanceAC.categoryWordCounts); 
            byte[] YACDHTWords = intToByteArray(JPEGUtility.DHTLuminanceAC.categoryWords);
            byte HTCbDCinfo = 0x01; // = 1
            byte[] CDCDHTwordsCount = intToByteArray(JPEGUtility.DHTChrominanceDC.categoryWordCounts);
            byte[] CDCDHTWords = intToByteArray(JPEGUtility.DHTChrominanceDC.categoryWords);
            byte HTCbACinfo = 0x11; //  = 0x11
            byte[] CACDHTwordsCount = intToByteArray(JPEGUtility.DHTChrominanceAC.categoryWordCounts);
            byte[] CACDHTWords = intToByteArray(JPEGUtility.DHTChrominanceAC.categoryWords);
            bw.Write(marker);
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
            UInt16 marker = 0xDAFF;
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
            bw.Write(marker);
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

        public void writeSOSY(BinaryWriter bw)
        {
            UInt16 marker = 0xDAFF;
            UInt16 length = 8;
            byte numComponents = 1; // # components: 1 {Y}
            byte YID = 1;
            byte DHTTableY = 0; // bits 0..3: AC table , bits 4..7: DC table 
            byte spectralSelectionStar = 0,
            spectralSelectionEnd = 0x3F,
            successiveApproximation = 0; // 0,63,0 for JPEG image
            bw.Write(marker);
            writeHex(bw, length);
            bw.Write(numComponents);
            bw.Write(YID);
            bw.Write(DHTTableY);
            bw.Write(spectralSelectionStar);
            bw.Write(spectralSelectionEnd);
            bw.Write(successiveApproximation);
        }

        public void writeSOSCb(BinaryWriter bw)
        {
            UInt16 marker = 0xDAFF;
            UInt16 length = 8;
            byte numComponents = 1; // # components: 1 {Y}
            byte CbID = 2;
            byte DHTTableCb = 0x11;  // bits 0..3: AC table , bits 4..7: DC table 
            byte spectralSelectionStar = 0,
            spectralSelectionEnd = 0x3F,
            successiveApproximation = 0; // 0,63,0 for JPEG image
            bw.Write(marker);
            writeHex(bw, length);
            bw.Write(numComponents);
            bw.Write(CbID);
            bw.Write(DHTTableCb);
            bw.Write(spectralSelectionStar);
            bw.Write(spectralSelectionEnd);
            bw.Write(successiveApproximation);
        }

        public void writeSOSCr(BinaryWriter bw)
        {
            UInt16 marker = 0xDAFF;
            UInt16 length = 8;
            byte numComponents = 1; // # components: 1 {Y}
            byte CbID = 2;
            byte DHTTableCr = 0x11;  // bits 0..3: AC table , bits 4..7: DC table 
            byte spectralSelectionStar = 0,
            spectralSelectionEnd = 0x3F,
            successiveApproximation = 0; // 0,63,0 for JPEG image
            bw.Write(marker);
            writeHex(bw, length);
            bw.Write(numComponents);
            bw.Write(CbID);
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

        public void WriteJPEGFile(BinaryWriter bw, int[] YDC, int[] CbDC, int[] CrDC, ArrayList YAC, ArrayList CbAC, ArrayList CrAC)
        {
            //si assume che sia presente uno SOS per ogni componente (es. SOS|Y|SOS|Cb|SOS|Cr )
            bbw = new BitBinaryWriter(bw.BaseStream);   //si associa al BitBinaryWriter lo stesso stream del BinaryWriter
            //DEBUG
            
            writeSOI(bw);
            writeAPP0(bw);
            writeDQT(bw);
            writeSOF(bw);
            writeDHT(bw);
            //inserire dati immagine
            writeSOSY(bw);
            writeImageDataY(bbw, YDC, YAC);
            //bw.Flush();
            //writeSOSCb(bw);
            //writeImageDataC(bbw, CbDC, CbAC);
            //bw.Flush();
            //writeSOSCr(bw);
            //writeImageDataC(bbw, CrDC, CrAC);
            //bw.Flush();
            writeEOI(bw);
            bw.Close();
            bbw.Close();
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

        public byte[] intToByteArray(UInt16[] Vector)
        {
            byte[] byteV = new byte[Vector.Length];
            for (int i = 0; i < Vector.Length; i++)
                byteV[i] = Convert.ToByte(Vector[i]);
            return byteV;
        }
          
        private void writeImageDataY(BitBinaryWriter bbw, int[] yDC, ArrayList yAC)
        {
            int numMCU = yDC.Length;
            for (int i = 0; i < numMCU; i++)
            {
                //writeYDC(bbw, yDC, i);
                writeYAC(bbw, yAC, i);
                //if (i == 1)
                //    break;
            }
        }

        private void writeImageDataC(BitBinaryWriter bbw, int[] cDC, ArrayList cAC)
        {
            int numMCU =cDC.Length;
            for (int i = 0; i < numMCU; i++)
            {
                writeCDC(bbw, cDC, i);
                writeCAC(bbw, cAC, i);
            }
        }

        private void writeYDC(BinaryWriter bw, int[] yDC, int i)
        {
            int val = yDC[i];
            writeCategoryAndValue(yDC[i], bw);
        }

        private void writeYAC(BitBinaryWriter bw, ArrayList yAC, int i)
        {
            int[] vi = (int[])yAC[i];
            for (int j = 0; j < vi.Length; j+=2)
            {
                int runlen = vi[j];
                int value = vi[j + 1];
                if (runlen == 0 && value == 0)
                {   //fine blocco AC
                    UInt16 endBlock = 0x00;
                    bw.Write(getHuffmanACCode(endBlock, JPEGUtility.COMPONENT_Y));
                }
                else if (runlen == 0 && value != 0)
                {   //nuovo numero fuori la sequenza zero-runlen
                        writeCategoryAndValueAC(value, bw);
                }
                else if(runlen != 0 && value != 0)
                {   //caso di runlen zero che precede un valore diverso da zero
                    if (runlen > 16)
                    {
                        int numZLR = runlen % 16;
                        if (numZLR == 0)
                        {   //num runlen zero minore di 16
                            writeRunLenCategoryAndValue(runlen, value, bw, JPEGUtility.COMPONENT_Y);
                        }
                        else
                        {   //scrivere ZLR tante volte quanti runlen di 16 zero sono presenti
                            for (int k = 0; k < numZLR; k++)
                            {
                                UInt16 ZLR = 0xf0;
                                bw.Write(getHuffmanACCode(ZLR, JPEGUtility.COMPONENT_Y));
                            }
                            int runLenOffset = runlen - (numZLR * 16);
                            writeRunLenCategoryAndValue(runLenOffset, value, bw, JPEGUtility.COMPONENT_Y);
                        }
                    }
                    else
                    {
                        writeRunLenCategoryAndValue(runlen, value, bw, JPEGUtility.COMPONENT_Y);
                    }
                }
            }
        }

        private void writeCDC(BinaryWriter bw, int[] cDC, int i)
        {
            int val = cDC[i];
            writeCategoryAndValue(cDC[i], bw);
        }

        private void writeCAC(BitBinaryWriter bw, ArrayList cAC, int i)
        {
            int[] vi = (int[]) cAC[i];
            for (int j = 0; j < vi.Length; j += 2)
            {
                int runlen = vi[j];
                int value = vi[j + 1];
                if (runlen == 0 && value == 0)
                {   //fine blocco AC
                    UInt16 endBlock = 0x00;
                    bw.Write(getHuffmanACCode(endBlock, JPEGUtility.COMPONENT_Cb));
                }
                else if (runlen == 0 && value != 0)
                {   //nuovo numero fuori la sequenza zero-runlen
                    writeCategoryAndValueAC(value, bw);
                }
                else if (runlen != 0 && value != 0)
                {   //caso di runlen zero che precede un valore diverso da zero
                    if (runlen > 16)
                    {
                        int numZLR = runlen % 16;
                        if (numZLR == 0)
                        {   //num runlen zero minore di 16
                            writeRunLenCategoryAndValue(runlen, value, bw, JPEGUtility.COMPONENT_Cb);
                        }
                        else
                        {   //scrivere ZLR tante volte quanti runlen di 16 zero sono presenti
                            for (int k = 0; k < numZLR; k++)
                            {
                                UInt16 ZLR = 0xf0;
                                bw.Write(getHuffmanACCode(ZLR, JPEGUtility.COMPONENT_Cb));
                            }
                            int runLenOffset = runlen - (numZLR * 16);
                            writeRunLenCategoryAndValue(runLenOffset, value, bw, JPEGUtility.COMPONENT_Cb);
                        }
                    }
                    else
                    {
                        writeRunLenCategoryAndValue(runlen, value, bw, JPEGUtility.COMPONENT_Cb);
                    }
                }
            }
        }

        private bool[] getHuffmanACCode(ushort hexACCode, int Component_ID)
        {
            //restituisce il codice Huffman degli hex per l'AC Encoding
            bool[] result = null;
            if (Component_ID == JPEGUtility.COMPONENT_Y)
            {
                int dimHexTable = JPEGUtility.DHTLuminanceAC.categoryWords.Length;
                for (int i=0; i<dimHexTable; i++)
                {
                    UInt16 hexWord = JPEGUtility.DHTLuminanceAC.categoryWords[i];
                    if (hexWord == hexACCode)
                    {
                        result = (bool[]) JPEGUtility.DHTLuminanceAC.categoryWordsHuffmanCodes[i];
                        return result;
                    } 
                }
            }
            else if(Component_ID == JPEGUtility.COMPONENT_Cb || Component_ID == JPEGUtility.COMPONENT_Cr)
            {
                int dimHexTable = JPEGUtility.DHTLuminanceAC.categoryWords.Length;
                for (int i = 0; i < dimHexTable; i++)
                {
                    UInt16 hexWord = JPEGUtility.DHTLuminanceAC.categoryWords[i];
                    if (hexWord == hexACCode)
                    {
                        result = (bool[])JPEGUtility.DHTLuminanceAC.categoryWordsHuffmanCodes[i];
                        return result;
                    }
                }
            }
            return result;
        }

        private void writeCbDC(BinaryWriter bw, int[] cbDC, int i)
        {
            int val = cbDC[i];
            writeCategoryAndValue(cbDC[i], bw);
        }

        private void writeCbAC(BinaryWriter bw, ArrayList cbAC, int i)
        {
            int[] vi = (int[])cbAC[i];
            for (int j = 0; j < vi.Length; j++)
            {
                writeHex(bw, vi[j]);
                //bw.Write(vi[j]);
            }
        }

        private void writeCrDC(BinaryWriter bw, int[] crDC, int i)
        {
            int val = crDC[i];
            writeCategoryAndValue(crDC[i], bw);
        }

        private void writeCrAC(BinaryWriter bw, ArrayList crAC, int i)
        {
            int[] vi = (int[])crAC[i];
            for (int j = 0; j < vi.Length; j++)
            {
                writeHex(bw, vi[j]);
                //bw.Write(vi[j]);
            }
        }

        private void writeCategoryAndValue(int val, BinaryWriter bw)
        {
            bool positive = true;
            if (val < 0)
                positive = false;
            int absvalue = Math.Abs(val);
            byte unsignedByte = 0;
            UInt16 unsignedInt = 0;
            int dimUnsignByte = -1;
            int category = -1;
            if (absvalue <= 255)
            {
                unsignedByte = Convert.ToByte(absvalue);
                dimUnsignByte = Convert.ToString(absvalue, 2).Length;                //dimensione del numero binario
                category = JPEGUtility.DHTLuminanceDC.categoryWords[dimUnsignByte]; //valore Hex della categoria
                if (!positive)
                {
                    int flipped = ~absvalue;         //flip dei bit di unsigned
                    byte b = Convert.ToByte(flipped & 0x000000FF);
                    byte bFinal = Convert.ToByte(b & binaryDimToHex[dimUnsignByte]);
                    int start = (dimUnsignByte -1);  //si eliminano i bit che precedono quelli da scrivere
                    bbw.Write((bool[])JPEGUtility.DHTLuminanceDC.categoryWordsHuffmanCodes[category]);
                    bbw.Write(bFinal, start);
                }
                else
                {
                    bbw.Write((bool[])JPEGUtility.DHTLuminanceDC.categoryWordsHuffmanCodes[category]);
                    bbw.Write(unsignedByte);
                }
            }
            else
            {
                unsignedInt = Convert.ToUInt16(absvalue);
                dimUnsignByte = Convert.ToString(absvalue, 2).Length;                //dimensione del numero binario
                category = JPEGUtility.DHTLuminanceDC.categoryWords[dimUnsignByte];     //valore Hex della categoria
                if (!positive)
                {
                    int flipped = ~absvalue;         //flip dei bit di unsigned
                    UInt16 b = Convert.ToUInt16(flipped & 0x000000FF);
                    UInt16 bFinal = Convert.ToUInt16(b & binaryDimToHex[dimUnsignByte]);
                    int start = (dimUnsignByte -1);  //si eliminano i bit che precedono quelli da scrivere
                    bbw.Write((bool[])JPEGUtility.DHTLuminanceDC.categoryWordsHuffmanCodes[category]);
                    bbw.Write(bFinal, start);
                }
                else
                {
                    bbw.Write((bool[])JPEGUtility.DHTLuminanceDC.categoryWordsHuffmanCodes[category]);
                    bbw.Write(unsignedInt);
                }
            }
        }

        private void writeCategoryAndValueAC(int val, BitBinaryWriter bw)
        {
            bool positive = true;
            if (val < 0)
                positive = false;
            int absvalue = Math.Abs(val);
            byte unsignedByte = 0;
            UInt16 unsignedInt = 0;
            int dimUnsignByte = -1;
            int category = -1;
            if (absvalue <= 255)
            {
                unsignedByte = Convert.ToByte(absvalue);
                dimUnsignByte = Convert.ToString(absvalue, 2).Length;                //dimensione del numero binario
                category = JPEGUtility.DHTLuminanceDC.categoryWords[dimUnsignByte]; //valore Hex della categoria
                if (!positive)
                {
                    int flipped = ~absvalue;         //flip dei bit di unsigned
                    byte b = Convert.ToByte(flipped & 0x000000FF);
                    byte bFinal = Convert.ToByte(b & binaryDimToHex[dimUnsignByte]);
                    int start = (dimUnsignByte - 1);  //si eliminano i bit che precedono quelli da scrivere
                    bbw.Write((bool[])JPEGUtility.DHTLuminanceAC.categoryWordsHuffmanCodes[category]);
                    bbw.Write(bFinal, start);
                }
                else
                {
                    bbw.Write((bool[])JPEGUtility.DHTLuminanceAC.categoryWordsHuffmanCodes[category]);
                    bbw.Write(unsignedByte);
                }
            }
            else
            {
                unsignedInt = Convert.ToUInt16(absvalue);
                dimUnsignByte = Convert.ToString(absvalue, 2).Length;                //dimensione del numero binario
                category = JPEGUtility.DHTLuminanceDC.categoryWords[dimUnsignByte];     //valore Hex della categoria
                if (!positive)
                {
                    int flipped = ~absvalue;         //flip dei bit di unsigned
                    UInt16 b = Convert.ToUInt16(flipped & 0x000000FF);
                    UInt16 bFinal = Convert.ToUInt16(b & binaryDimToHex[dimUnsignByte]);
                    int start = (dimUnsignByte - 1);  //si eliminano i bit che precedono quelli da scrivere
                    bbw.Write((bool[])JPEGUtility.DHTLuminanceAC.categoryWordsHuffmanCodes[category]);
                    bbw.Write(bFinal, start);
                }
                else
                {
                    bbw.Write((bool[])JPEGUtility.DHTLuminanceAC.categoryWordsHuffmanCodes[category]);
                    bbw.Write(unsignedInt);
                }
            }
        }


        private void writeRunLenCategoryAndValue(int runlen, int val, BinaryWriter bw, int componentType)
        {
            bool positive = true;
            if (val < 0)
                positive = false;
            int absvalue = Math.Abs(val);
            byte unsignedByte = 0;
            UInt16 unsignedInt = 0;
            int dimUnsignByte = -1;
            int category = -1;
            if (absvalue <= 255)
            {
                unsignedByte = Convert.ToByte(absvalue);
                dimUnsignByte = Convert.ToString(absvalue, 2).Length;                //dimensione del numero binario
                category = JPEGUtility.DHTLuminanceDC.categoryWords[dimUnsignByte]; //valore Hex della categoria
                if (!positive)
                {
                    int flipped = ~absvalue;         //flip dei bit di unsigned
                    byte b = Convert.ToByte(flipped & 0x000000FF);
                    byte bFinal = Convert.ToByte(b & binaryDimToHex[dimUnsignByte]);
                    int start = (dimUnsignByte -1);  //si eliminano i bit che precedono quelli da scrivere
                    byte hexVal = bbw.mergeByte(Convert.ToByte(runlen), Convert.ToByte(category));
                    bbw.Write(getHuffmanACCode(hexVal, componentType));
                    bbw.Write(bFinal, start);
                }
                else
                {
                    byte hexVal = bbw.mergeByte(Convert.ToByte(runlen), Convert.ToByte(category));
                    bbw.Write(getHuffmanACCode(hexVal, componentType));
                    bbw.Write(unsignedByte);
                }
            }
            else
            {
                //TODO
                unsignedInt = Convert.ToUInt16(absvalue);
                dimUnsignByte = Convert.ToString(absvalue, 2).Length;                //dimensione del numero binario
                category = JPEGUtility.DHTLuminanceDC.categoryWords[dimUnsignByte];     //valore Hex della categoria
                if (!positive)
                {
                    int flipped = ~absvalue;         //flip dei bit di unsigned
                    UInt16 b = Convert.ToUInt16(flipped & 0x000000FF);
                    UInt16 bFinal = Convert.ToUInt16(b & binaryDimToHex[dimUnsignByte]);
                    int start = (dimUnsignByte -1);  //si eliminano i bit che precedono quelli da scrivere
                    byte hexVal = bbw.mergeByte(Convert.ToByte(runlen), Convert.ToByte(category));
                    bbw.Write(getHuffmanACCode(hexVal, componentType));
                    bbw.Write(bFinal, start);
                }
                else
                {
                    byte hexVal = bbw.mergeByte(Convert.ToByte(runlen), Convert.ToByte(category));
                    bbw.Write(getHuffmanACCode(hexVal, componentType));
                    bbw.Write(unsignedInt);
                }
            }
        }

        public class BitBinaryWriter : System.IO.BinaryWriter
        {
            private bool[] curByte = new bool[8];
            private int curBitIndx = 0;
            private System.Collections.BitArray ba;
            
            public BitBinaryWriter(Stream s) : base(s) { }

            public BitBinaryWriter() { }    //DEBUG

            public override void Flush()
            {
                base.Write(ConvertToByte(curByte));
                base.Flush();
            }
            
            public void Write(bool[] boolArray)
            {
                for (int i = 0; i < boolArray.Length; i++)
                    this.Write(boolArray[i]);
            }

            public override void Write(bool value)
            {
                curByte[curBitIndx] = value;
                this.curBitIndx++;
                if (curBitIndx == 8)
                {
                    byte toWrite = ConvertToByte(curByte);
                    base.Write(toWrite);
                    if (toWrite == 0xff)        //controllo sulla scrittura del byte FF
                        base.Write(0);
                    this.curBitIndx = 0;
                    this.curByte = new bool[8];
                }
            }

            public override void Write(byte value)
            {
                bool firstOneBit = false;
                ba = new BitArray(new byte[] { value });
                for (int k = 7 ; k >= 0; k--)
                {
                    if (!firstOneBit && ba[k])
                        firstOneBit = true;
                    if (firstOneBit)
                        this.Write(ba[k]);
                }
                ba = null;
            }

            public void Write(byte value, int start)
            {
                ba = new BitArray(new byte[] { value });
                for (int k = start; k >= 0; k--)
                {
                    this.Write(ba[k]);
                }
                ba = null;
            }

            public void Write(ushort value, int start)
            {
                ba = new BitArray(BitConverter.GetBytes(value));
                for (int k = start; k >= 0; k--)
                {
                    this.Write(ba[k]);
                }
                ba = null;
            }

            public override void Write(byte[] buffer)
            {
                for (int i = 0; i < buffer.Length; i++)
                {
                    this.Write((byte)buffer[i]);
                }
            }

            public override void Write(uint value)
            {
                ba = new BitArray(BitConverter.GetBytes(value));
                for (int i = 0; i < 32; i++)
                {
                    this.Write(ba[i]);
                }
                ba = null;
            }

            public override void Write(ulong value)
            {
                ba = new BitArray(BitConverter.GetBytes(value));
                for (int i = 0; i < 64; i++)
                {
                    this.Write(ba[i]);
                }
                ba = null;
            }

            public override void Write(ushort value)
            {
                ba = new BitArray(BitConverter.GetBytes(value));
                for (int i = 0; i < 16; i++)
                {
                    this.Write(ba[i]);
                }
                ba = null;
            }

            public byte ConvertToByte(bool[] bools)
            {
                byte b = 0;

                byte bitIndex = 0;
                for (int i = 0; i < 8; i++)
                {
                    if (bools[i])
                    {
                        b |= (byte)(((byte)1) << 7 - bitIndex);
                    }
                    bitIndex++;
                }
                return b;
            }

            public byte mergeByte(byte b1, byte b2)
            {
                //prende i 4 bit meno significativi dei due byte, per poi unirli in un solo byte
                BitArray byte1 = new BitArray(new byte[] { b1 });
                BitArray byte2 = new BitArray(new byte[] { b2 });
                int cnt = 0;
                bool[] result = new bool[8];
                for (int i = 3; i >= 0; i--)
                {
                    result[cnt] = byte1[i];
                    result[4+cnt] = byte2[i];
                    cnt++;
                }
                return ConvertToByte(result);
            }
        } //BitBitBinaryWriter
    }
}
