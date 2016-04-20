using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPEGEncoding
{
    class JPEGUtility
    {
        private static bool I = true;
        private static bool O = false;

        public static int NO_SUBSAMPLING = 0;
        public static int SUBSAMPLING_422 = 1;
        public static int SUBSAMPLING_420 = 2;

        public static int ZERO_BLOCK_PADDING = 0;
        public static int COPY_BLOCK_PADDING = 1;

        public static int DHT_CLASS_DC = 0;
        public static int DHT_CLASS_AC = 1;

        public static int DHT_ID_Y = 0;
        public static int DHT_ID_CbCr = 1;

        public static int COMPONENT_Y = 1;
        public static int COMPONENT_Cb = 2;
        public static int COMPONENT_Cr = 3;

        







        public static Byte[] Category = new Byte[65535];
        public static BitString[] BitCode = new BitString[65535];

        public static BitString[] Y_DC_Huffman_Table = new BitString[12]; 
        public static BitString[] Cb_DC_Huffman_Table = new BitString[12];
        public static BitString[] Y_AC_Huffman_Table = new BitString[256];
        public static BitString[] Cb_AC_Huffman_Table = new BitString[256];

        public static BitString[] getYDCHCode()
        {
            return Y_DC_Huffman_Table;
        }

        public static BitString[] getYACHCode()
        {
            return Y_AC_Huffman_Table;
        }

        public static BitString[] getCbDCHCode()
        {
            return Cb_DC_Huffman_Table;
        }

        public static BitString[] getCbACHCode()
        {
            return Cb_AC_Huffman_Table;
        }

        public static Byte[] QuantizationYVector =
        {
             16, 11, 10, 16, 24, 40, 51, 61 ,
             12, 12, 14, 19, 26, 58, 60, 55 ,
             14, 13, 16, 24, 40, 57, 69, 56 ,
             14, 17, 22, 29, 51, 87, 80, 62 ,
             18, 22, 37, 56, 68, 109, 103, 77 ,
             24, 35, 55, 64, 81, 104, 113, 92 ,
            49, 64, 78, 87, 103, 121, 120, 101 ,
             72, 92, 95, 98, 112, 100, 103, 99 

        };

        public static Byte[,] QuantizationYMatrix =
       {
            { 16, 11, 10, 16, 24, 40, 51, 61 },
            { 12, 12, 14, 19, 26, 58, 60, 55 },
            { 14, 13, 16, 24, 40, 57, 69, 56 },
            { 14, 17, 22, 29, 51, 87, 80, 62 },
            { 18, 22, 37, 56, 68, 109, 103, 77 },
            { 24, 35, 55, 64, 81, 104, 113, 92 },
            { 49, 64, 78, 87, 103, 121, 120, 101 },
            { 72, 92, 95, 98, 112, 100, 103, 99 }

        };

        public static Byte[] QuantizationCVector =
       {
             17, 18, 24, 47, 99, 99, 99, 99 ,
             18, 21, 26, 66, 99, 99, 99, 99 ,
             24, 26, 56, 99, 99, 99, 99, 99 ,
             47, 66, 99, 99, 99, 99, 99, 99 ,
             99, 99, 99, 99, 99, 99, 99, 99 ,
             99, 99, 99, 99, 99, 99, 99, 99 ,
             99, 99, 99, 99, 99, 99, 99, 99 ,
             99, 99, 99, 99, 99, 99, 99, 99 
        };

        public static Byte[,] QuantizationCMatrix =
        {
            { 17, 18, 24, 47, 99, 99, 99, 99 },
            { 18, 21, 26, 66, 99, 99, 99, 99 },
            { 24, 26, 56, 99, 99, 99, 99, 99 },
            { 47, 66, 99, 99, 99, 99, 99, 99 },
            { 99, 99, 99, 99, 99, 99, 99, 99 },
            { 99, 99, 99, 99, 99, 99, 99, 99 },
            { 99, 99, 99, 99, 99, 99, 99, 99 },
            { 99, 99, 99, 99, 99, 99, 99, 99 }
        };

        public static int[,] ZigZag =
        {
            { 0, 1, 5, 6, 14, 15, 27, 28 },
            { 2, 4, 7, 13, 16, 26, 29, 42 },
            { 3, 8, 12, 17, 25, 30, 41, 43 },
            { 9, 11, 18, 24, 31, 40, 44, 53 },
            { 10, 19, 23, 32, 39, 45, 52, 54 },
            { 20, 22, 33, 38, 46, 51, 55, 60 },
            { 21, 34, 37, 47, 50, 56, 59, 61 },
            { 35, 36, 48, 49, 57, 58, 62, 63 }
        };

        public static int[] ZigZagVector =
        {
             0, 1, 5, 6, 14, 15, 27, 28 ,
             2, 4, 7, 13, 16, 26, 29, 42 ,
             3, 8, 12, 17, 25, 30, 41, 43 ,
             9, 11, 18, 24, 31, 40, 44, 53 ,
             10, 19, 23, 32, 39, 45, 52, 54 ,
             20, 22, 33, 38, 46, 51, 55, 60 ,
             21, 34, 37, 47, 50, 56, 59, 61 ,
             35, 36, 48, 49, 57, 58, 62, 63 
        };


        public static int[] ZigZagX =
        { 0, 1, 2, 1, 0, 0, 1, 2, 3, 4, 3, 2, 1, 0, 0, 1, 2, 3, 4, 5, 6, 5, 4, 3, 2, 1, 0,
          0, 1, 2, 3, 4, 5, 6, 7, 7, 6, 5, 4, 3, 2, 1, 2, 3, 4, 5, 6, 7, 7, 6, 5, 4, 3, 4, 5,
          6, 7, 7, 6, 5, 6, 7, 7 };

        public static int[] ZigZagY =
        { 1, 0, 0, 1, 2, 3, 2, 1, 0, 0, 1, 2, 3, 4, 5, 4, 3, 2, 1, 0, 0, 1, 2, 3, 4, 5, 6,
          7, 6, 5, 4, 3, 2, 1, 0, 1, 2, 3, 4, 5, 6, 7, 7, 6, 5, 4, 3, 2, 3, 4, 5, 6, 7, 7, 6,
          5, 4, 5, 6, 7, 7, 6, 7 };

        public static JPEGHuffmanTable DHTLuminanceDC = new JPEGHuffmanTable
            (JPEGUtility.DHT_CLASS_DC,
              JPEGUtility.DHT_ID_Y,
              new byte[] { 0, 1, 5, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0 },
              new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b },
              getHuffmanCodesLuminanceDC()
             );

        public static JPEGHuffmanTable DHTChrominanceDC = new JPEGHuffmanTable
            (JPEGUtility.DHT_CLASS_DC,
              JPEGUtility.DHT_ID_CbCr,
              new byte[] { 0, 3, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0 },
              new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b },
              getHuffmanCodesChrominanceDC()
            );

        public static JPEGHuffmanTable DHTLuminanceAC = new JPEGHuffmanTable
            (JPEGUtility.DHT_CLASS_AC,
              JPEGUtility.DHT_ID_Y,
              new byte[] { 0, 2, 1, 3, 3, 2, 4, 3, 5, 5, 4, 4, 0, 0, 1, 0x7d },
              new byte[] {     0x01, 0x02, 0x03, 0x00, 0x04, 0x11, 0x05, 0x12,
                              0x21, 0x31, 0x41, 0x06, 0x13, 0x51, 0x61, 0x07,
                              0x22, 0x71, 0x14, 0x32, 0x81, 0x91, 0xa1, 0x08,
                              0x23, 0x42, 0xb1, 0xc1, 0x15, 0x52, 0xd1, 0xf0,
                              0x24, 0x33, 0x62, 0x72, 0x82, 0x09, 0x0a, 0x16,
                              0x17, 0x18, 0x19, 0x1a, 0x25, 0x26, 0x27, 0x28,
                              0x29, 0x2a, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39,
                              0x3a, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49,
                              0x4a, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59,
                              0x5a, 0x63, 0x64, 0x65, 0x66, 0x67, 0x68, 0x69,
                              0x6a, 0x73, 0x74, 0x75, 0x76, 0x77, 0x78, 0x79,
                              0x7a, 0x83, 0x84, 0x85, 0x86, 0x87, 0x88, 0x89,
                              0x8a, 0x92, 0x93, 0x94, 0x95, 0x96, 0x97, 0x98,
                              0x99, 0x9a, 0xa2, 0xa3, 0xa4, 0xa5, 0xa6, 0xa7,
                              0xa8, 0xa9, 0xaa, 0xb2, 0xb3, 0xb4, 0xb5, 0xb6,
                              0xb7, 0xb8, 0xb9, 0xba, 0xc2, 0xc3, 0xc4, 0xc5,
                              0xc6, 0xc7, 0xc8, 0xc9, 0xca, 0xd2, 0xd3, 0xd4,
                              0xd5, 0xd6, 0xd7, 0xd8, 0xd9, 0xda, 0xe1, 0xe2,
                              0xe3, 0xe4, 0xe5, 0xe6, 0xe7, 0xe8, 0xe9, 0xea,
                              0xf1, 0xf2, 0xf3, 0xf4, 0xf5, 0xf6, 0xf7, 0xf8,
                              0xf9, 0xfa },
              getHuffmanCodesLuminanceAC()
            );

        public static JPEGHuffmanTable DHTChrominanceAC = new JPEGHuffmanTable
            (JPEGUtility.DHT_CLASS_AC,
              JPEGUtility.DHT_ID_CbCr,
              new byte[] { 0, 2, 1, 3, 3, 2, 4, 3, 5, 5, 4, 4, 0, 0, 1, 0x7d },
              new byte[] {     0x00, 0x01, 0x02, 0x03, 0x11, 0x04, 0x05, 0x21,
                              0x31, 0x06, 0x12, 0x41, 0x51, 0x07, 0x61, 0x71,
                              0x13, 0x22, 0x32, 0x81, 0x08, 0x14, 0x42, 0x91,
                              0xa1, 0xb1, 0xc1, 0x09, 0x23, 0x33, 0x52, 0xf0,
                              0x15, 0x62, 0x72, 0xd1, 0x0a, 0x16, 0x24, 0x34,
                              0xe1, 0x25, 0xf1, 0x17, 0x18, 0x19, 0x1a, 0x26,
                              0x27, 0x28, 0x29, 0x2a, 0x35, 0x36, 0x37, 0x38,
                              0x39, 0x3a, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48,
                              0x49, 0x4a, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58,
                              0x59, 0x5a, 0x63, 0x64, 0x65, 0x66, 0x67, 0x68,
                              0x69, 0x6a, 0x73, 0x74, 0x75, 0x76, 0x77, 0x78,
                              0x79, 0x7a, 0x82, 0x83, 0x84, 0x85, 0x86, 0x87,
                              0x88, 0x89, 0x8a, 0x92, 0x93, 0x94, 0x95, 0x96,
                              0x97, 0x98, 0x99, 0x9a, 0xa2, 0xa3, 0xa4, 0xa5,
                              0xa6, 0xa7, 0xa8, 0xa9, 0xaa, 0xb2, 0xb3, 0xb4,
                              0xb5, 0xb6, 0xb7, 0xb8, 0xb9, 0xba, 0xc2, 0xc3,
                              0xc4, 0xc5, 0xc6, 0xc7, 0xc8, 0xc9, 0xca, 0xd2,
                              0xd3, 0xd4, 0xd5, 0xd6, 0xd7, 0xd8, 0xd9, 0xda,
                              0xe2, 0xe3, 0xe4, 0xe5, 0xe6, 0xe7, 0xe8, 0xe9,
                              0xea, 0xf2, 0xf3, 0xf4, 0xf5, 0xf6, 0xf7, 0xf8,
                              0xf9, 0xfa },
              getHuffmanCodesChrominanceAC()
            );

        private static ArrayList getHuffmanCodesChrominanceDC()
        {
            ArrayList codes = new ArrayList();
            codes.Add(new bool[] { O, O });                         //category 0
            codes.Add(new bool[] { O, I, O });                      //category 1 
            codes.Add(new bool[] { O, I, I });                      //category 2
            codes.Add(new bool[] { I, O, O });                      //category 3
            codes.Add(new bool[] { I, O, I });                      //category 4
            codes.Add(new bool[] { I, I, O });                      //category 5
            codes.Add(new bool[] { I, I, I, O });                   //category 6
            codes.Add(new bool[] { I, I, I, I, O });                //category 7
            codes.Add(new bool[] { I, I, I, I, I, O });             //category 8
            codes.Add(new bool[] { I, I, I, I, I, I, O });          //category 9
            codes.Add(new bool[] { I, I, I, I, I, I, I, O });       //category 10
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, O });    //category 11
            return codes;
        }

        private static ArrayList getHuffmanCodesLuminanceDC()
        {
            ArrayList codes = new ArrayList();
            codes.Add(new bool[] { O, O });                         //category 0
            codes.Add(new bool[] { O, I, O });                      //category 1 
            codes.Add(new bool[] { O, I, I });                      //category 2
            codes.Add(new bool[] { I, O, O });                      //category 3
            codes.Add(new bool[] { I, O, I });                      //category 4
            codes.Add(new bool[] { I, I, O });                      //category 5
            codes.Add(new bool[] { I, I, I, O });                   //category 6
            codes.Add(new bool[] { I, I, I, I, O });                //category 7
            codes.Add(new bool[] { I, I, I, I, I, O });             //category 8
            codes.Add(new bool[] { I, I, I, I, I, I, O });          //category 9
            codes.Add(new bool[] { I, I, I, I, I, I, I, O });       //category 10
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, O });    //category 11
            return codes;
        }

        private static ArrayList getHuffmanCodesLuminanceAC()
        {
            ArrayList codes = new ArrayList();
            codes.Add(new bool[] { O, O });//01
            codes.Add(new bool[] { O, I });//02    
            codes.Add(new bool[] { I, O, O });//03   
            codes.Add(new bool[] { I, O, I, O });//00 EOB
            codes.Add(new bool[] { I, O, I, I });//04
            codes.Add(new bool[] { I, I, O, O });//11       
            codes.Add(new bool[] { I, I, O, I, O });//05
            codes.Add(new bool[] { I, I, O, I, I });//12
            //++++++++++++++++++++++++++++++++++++++++++++++
            codes.Add(new bool[] { I, I, I, O, O });//21 ok
            codes.Add(new bool[] { I, I, I, O, I, O });//31
            codes.Add(new bool[] { I, I, I, O, I, I });//41
            codes.Add(new bool[] { I, I, I, I, O, O, O });//06
            codes.Add(new bool[] { I, I, I, I, O, O, I });//13
            codes.Add(new bool[] { I, I, I, I, O, I, O });//51
            codes.Add(new bool[] { I, I, I, I, I, O, I, I });//61
            codes.Add(new bool[] { I, I, I, I, I, O, O, O });//07
            codes.Add(new bool[] { I, I, I, I, I, O, O, I });//22
            //++++++++++++++++++++++++++++++++++++++++++++++
            codes.Add(new bool[] { I, I, I, I, I, O, I, O });//71 ok
            codes.Add(new bool[] { I, I, I, I, I, O, I, I, O });//14
            codes.Add(new bool[] { I, I, I, I, I, O, I, I, I });//32
            codes.Add(new bool[] { I, I, I, I, I, I, O, O, O });//81
            codes.Add(new bool[] { I, I, I, I, I, I, O, O, I });//91 ok
            codes.Add(new bool[] { I, I, I, I, I, I, O, I, O });//a1
            codes.Add(new bool[] { I, I, I, I, I, I, O, I, I, O });//08
            codes.Add(new bool[] { I, I, I, I, I, I, O, I, I, I });//23
            //++++++++++++++++++++++++++++++++++++++++++++++
            codes.Add(new bool[] { I, I, I, I, I, I, I, O, O, O });//42
            codes.Add(new bool[] { I, I, I, I, I, I, I, O, O, I });//b1
            codes.Add(new bool[] { I, I, I, I, I, I, I, O, I, O });//c1
            codes.Add(new bool[] { I, I, I, I, I, I, I, O, I, I, O });//15
            codes.Add(new bool[] { I, I, I, I, I, I, I, O, I, I, I });//52
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, O, O, O });//d1
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, O, O, I });//f0 ZRL k
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, O, O, O, O, I, O });//09
            //++++++++++++++++++++++++++++++++++++++++++++++
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, O, I, O, O });//24
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, O, I, O, I });//33
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, O, I, I, O });//62
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, O, I, I, I });//72
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, O, O, O, O, O, O });//82
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, O, O, O, O, I, I });//0a
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, O, O, O, I, O, O });//16
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, O, O, I, I, I, I });//34 ok
            //++++++++++++++++++++++++++++++++++++++++++++++
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, I, O, I, O, I, I });//e1
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, O, O, I, O, O, I });//25
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, I, I, O, I, O, I });//f1
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, O, O, O, I, O, I });//17
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, O, O, O, I, I, I });//18
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, O, O, O, I, I, I });//19
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, O, O, I, O, O, O });//1a
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, O, O, I, O, I, O });//26 ok
            //++++++++++++++++++++++++++++++++++++++++++++++
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, O, O, I, O, I, I });//27
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, O, O, I, I, O, O });//28
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, O, O, I, I, O, I });//29
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, O, O, I, I, I, O });//2a
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, O, I, O, O, O, O });//35
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, O, I, O, O, O, I });//36
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, O, I, O, O, I, O });//37
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, O, I, O, O, I, I });//38 ok
            //++++++++++++++++++++++++++++++++++++++++++++++
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, O, I, O, I, O, O });//39
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, O, I, O, I, O, I });//3a
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, O, I, O, I, I, O });//43
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, O, I, O, I, I, I });//44
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, O, I, I, O, O, O });//45
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, O, I, I, O, O, I });//46
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, O, I, I, O, I, O });//47
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, O, I, I, O, I, I });//48 ok
            //++++++++++++++++++++++++++++++++++++++++++++++
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, O, I, I, I, O, O });//49
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, O, I, I, I, O, I });//4a
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, O, I, I, I, I, O });//53
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, O, I, I, I, I, I });//54
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, I, O, O, O, O, O });//55
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, I, O, O, O, O, I });//56
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, I, O, O, O, I, O });//57
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, I, O, O, O, I, I });//58 ok
            //++++++++++++++++++++++++++++++++++++++++++++++
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, I, O, O, I, O, O });//59
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, I, O, O, I, O, I });//5a
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, I, O, O, I, I, O });//63
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, I, O, O, I, I, I });//64
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, I, O, I, O, O, O });//65
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, I, O, I, O, O, I });//66
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, I, O, I, O, I, O });//67
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, I, O, I, O, I, I });//68 ok
            //++++++++++++++++++++++++++++++++++++++++++++++
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, I, O, I, I, O, O });//69
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, I, O, I, I, O, I });//6a
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, I, O, I, I, I, O });//73
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, I, O, I, I, I, I });//74
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, I, I, O, O, O, O });//75
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, I, I, O, O, O, I });//76
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, I, I, O, O, I, O });//77
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, I, I, O, O, I, I });//78 ok
            //++++++++++++++++++++++++++++++++++++++++++++++
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, I, I, O, I, O, O });//79
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, I, I, O, I, O, I });//7a
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, I, I, O, I, I, O });//83
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, I, I, O, I, I, I });//84
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, I, I, I, O, O, O });//85
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, I, I, I, O, O, I });//86
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, I, I, I, O, I, O });//87 ok
            //++++++++++++++++++++++++++++++++++++++++++++++
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, I, I, I, O, I, I });//88
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, I, I, I, I, O, O });//89
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, I, I, I, I, O, I });//8a
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, I, I, I, I, I, O });//92
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, I, I, I, I, I, I });//93
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, O, O, O, O, O, O });//94
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, O, O, O, O, O, I });//95
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, O, O, O, O, I, O });//96 ok
            //++++++++++++++++++++++++++++++++++++++++++++++
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, O, O, O, O, I, I });//97
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, O, O, O, I, O, O });//98
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, O, O, O, I, O, I });//99
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, O, O, O, I, I, O });//9a
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, O, O, O, I, I, I });//a2
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, O, O, I, O, O, O });//a3
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, O, O, I, O, O, I });//a4
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, O, O, I, O, I, O });//a5 ok
            //++++++++++++++++++++++++++++++++++++++++++++++
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, O, O, I, O, I, I });//a6
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, O, O, I, I, O, O });//a7
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, O, O, I, I, O, I });//a8
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, O, O, I, I, I, O });//a9
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, O, O, I, I, I, I });//aa
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, O, I, O, O, O, O });//b2
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, O, I, O, O, O, I });//b3
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, O, I, O, O, I, O });//b4 ok
            //++++++++++++++++++++++++++++++++++++++++++++++
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, O, I, O, O, I, I });//b5
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, O, I, O, I, O, O });//b6
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, O, I, O, I, O, I });//b7
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, O, I, O, I, I, O });//b8
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, O, I, O, I, I, I });//b9
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, O, I, I, O, O, O });//ba
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, O, I, I, O, O, I });//c2
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, O, I, I, O, I, O });//c3 ok
            //++++++++++++++++++++++++++++++++++++++++++++++
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, O, I, I, O, I, I });//c4
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, O, I, I, I, O, O });//c5
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, O, I, I, I, O, I });//c6
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, O, I, I, I, I, O });//c7
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, O, I, I, I, I, I });//c8
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, I, O, O, O, O, O });//c9
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, I, O, O, O, O, I });//ca
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, I, O, O, O, I, O });//d2 ok
            //++++++++++++++++++++++++++++++++++++++++++++++
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, I, O, O, O, I, I });//d3
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, I, O, O, I, O, O });//d4
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, I, O, O, I, O, I });//d5
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, I, O, O, I, I, O });//d6
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, I, O, O, I, I, I });//d7
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, I, O, I, O, O, O });//d8
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, I, O, I, O, O, I });//d9
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, I, O, I, O, I, O });//da ok
            //++++++++++++++++++++++++++++++++++++++++++++++
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, I, O, I, I, O, O });//e2
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, I, O, I, I, O, I });//e3
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, I, O, I, I, I, O });//e4
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, I, O, I, I, I, I });//e5
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, I, I, O, O, O, O });//e6
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, I, I, O, O, O, I });//e7
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, I, I, O, O, I, O });//e8
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, I, I, O, O, I, I });//e9 ok
            //++++++++++++++++++++++++++++++++++++++++++++++
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, I, I, O, I, O, O });//ea
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, I, I, O, I, I, O });//f2
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, I, I, O, I, I, I });//f3
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, I, I, I, O, O, O });//f4
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, I, I, I, O, O, I });//f5
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, I, I, I, O, I, O });//f6
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, I, I, I, O, I, I });//f7
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, I, I, I, I, O, O });//f8
            //++++++++++++++++++++++++++++++++++++++++++++++
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, I, I, I, I, O, I });//f9
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, I, I, I, I, I, O });//fa
            //++++++++++++++++++++++++++++++++++++++++++++++
            return codes;
        }

        private static ArrayList getHuffmanCodesChrominanceAC()
        {
            ArrayList codes = new ArrayList();
            codes.Add(new bool[] { I, O, I, O });//00
            codes.Add(new bool[] { O, O });//01
            codes.Add(new bool[] { O, I });//02    
            codes.Add(new bool[] { I, O, O });//03   
            codes.Add(new bool[] { I, I, O, O });//11       
            codes.Add(new bool[] { I, O, I, I });//04
            codes.Add(new bool[] { I, I, O, I, O });//05
            codes.Add(new bool[] { I, I, I, O, O });//21 ok
            //++++++++++++++++++++++++++++++++++++++++++++++
            codes.Add(new bool[] { I, I, I, O, I, O });//31
            codes.Add(new bool[] { I, I, I, I, O, O, O });//06
            codes.Add(new bool[] { I, I, O, I, I });//12
            codes.Add(new bool[] { I, I, I, O, I, I });//41
            codes.Add(new bool[] { I, I, I, I, O, I, O });//51
            codes.Add(new bool[] { I, I, I, I, I, O, O, O });//07
            codes.Add(new bool[] { I, I, I, I, I, O, I, I });//61
            codes.Add(new bool[] { I, I, I, I, I, O, I, O });//71 ok
            //++++++++++++++++++++++++++++++++++++++++++++++
            codes.Add(new bool[] { I, I, I, I, O, O, I });//13
            codes.Add(new bool[] { I, I, I, I, I, O, O, I });//22
            codes.Add(new bool[] { I, I, I, I, I, O, I, I, I });//32
            codes.Add(new bool[] { I, I, I, I, I, I, O, O, O });//81
            codes.Add(new bool[] { I, I, I, I, I, I, O, I, I, O });//08
            codes.Add(new bool[] { I, I, I, I, I, O, I, I, O });//14
            codes.Add(new bool[] { I, I, I, I, I, I, I, O, O, O });//42
            codes.Add(new bool[] { I, I, I, I, I, I, O, O, I });//91 ok
            //++++++++++++++++++++++++++++++++++++++++++++++
            codes.Add(new bool[] { I, I, I, I, I, I, O, I, O });//a1
            codes.Add(new bool[] { I, I, I, I, I, I, I, O, O, I });//b1
            codes.Add(new bool[] { I, I, I, I, I, I, I, O, I, O });//c1
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, O, O, O, O, I, O });//09
            codes.Add(new bool[] { I, I, I, I, I, I, O, I, I, I });//23
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, O, I, O, I });//33
            codes.Add(new bool[] { I, I, I, I, I, I, I, O, I, I, I });//52
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, O, O, I });//f0 ZRL k
            //++++++++++++++++++++++++++++++++++++++++++++++
            codes.Add(new bool[] { I, I, I, I, I, I, I, O, I, I, O });//15
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, O, I, I, O });//62
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, O, I, I, I });//72
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, O, O, O });//d1
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, O, O, O, O, I, I });//0a
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, O, O, O, I, O, O });//16
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, O, I, O, O });//24
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, O, O, I, I, I, I });//34 ok
            //++++++++++++++++++++++++++++++++++++++++++++++
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, I, O, I, O, I, I });//e1
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, O, O, I, O, O, I });//25
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, I, I, O, I, O, I });//f1
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, O, O, O, I, O, I });//17
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, O, O, O, I, I, I });//18
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, O, O, O, I, I, I });//19
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, O, O, I, O, O, O });//1a
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, O, O, I, O, I, O });//26 ok
            //++++++++++++++++++++++++++++++++++++++++++++++
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, O, O, I, O, I, I });//27
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, O, O, I, I, O, O });//28
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, O, O, I, I, O, I });//29
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, O, O, I, I, I, O });//2a
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, O, I, O, O, O, O });//35
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, O, I, O, O, O, I });//36
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, O, I, O, O, I, O });//37
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, O, I, O, O, I, I });//38 ok
            //++++++++++++++++++++++++++++++++++++++++++++++
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, O, I, O, I, O, O });//39
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, O, I, O, I, O, I });//3a
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, O, I, O, I, I, O });//43
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, O, I, O, I, I, I });//44
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, O, I, I, O, O, O });//45
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, O, I, I, O, O, I });//46
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, O, I, I, O, I, O });//47
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, O, I, I, O, I, I });//48 ok
            //++++++++++++++++++++++++++++++++++++++++++++++
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, O, I, I, I, O, O });//49
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, O, I, I, I, O, I });//4a
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, O, I, I, I, I, O });//53
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, O, I, I, I, I, I });//54
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, I, O, O, O, O, O });//55
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, I, O, O, O, O, I });//56
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, I, O, O, O, I, O });//57
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, I, O, O, O, I, I });//58 ok
            //++++++++++++++++++++++++++++++++++++++++++++++
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, I, O, O, I, O, O });//59
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, I, O, O, I, O, I });//5a
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, I, O, O, I, I, O });//63
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, I, O, O, I, I, I });//64
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, I, O, I, O, O, O });//65
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, I, O, I, O, O, I });//66
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, I, O, I, O, I, O });//67
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, I, O, I, O, I, I });//68 ok
            //++++++++++++++++++++++++++++++++++++++++++++++
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, I, O, I, I, O, O });//69
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, I, O, I, I, O, I });//6a
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, I, O, I, I, I, O });//73
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, I, O, I, I, I, I });//74
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, I, I, O, O, O, O });//75
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, I, I, O, O, O, I });//76
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, I, I, O, O, I, O });//77
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, I, I, O, O, I, I });//78 ok
            //++++++++++++++++++++++++++++++++++++++++++++++
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, I, I, O, I, O, O });//79
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, I, I, O, I, O, I });//7a
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, O, O, O, O, O, O });//82
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, I, I, O, I, I, O });//83
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, I, I, O, I, I, I });//84
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, I, I, I, O, O, O });//85
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, I, I, I, O, O, I });//86
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, I, I, I, O, I, O });//87 ok
            //++++++++++++++++++++++++++++++++++++++++++++++
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, I, I, I, O, I, I });//88
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, I, I, I, I, O, O });//89
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, I, I, I, I, O, I });//8a
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, I, I, I, I, I, O });//92
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, O, I, I, I, I, I, I });//93
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, O, O, O, O, O, O });//94
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, O, O, O, O, O, I });//95
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, O, O, O, O, I, O });//96 ok
            //++++++++++++++++++++++++++++++++++++++++++++++
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, O, O, O, O, I, I });//97
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, O, O, O, I, O, O });//98
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, O, O, O, I, O, I });//99
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, O, O, O, I, I, O });//9a
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, O, O, O, I, I, I });//a2
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, O, O, I, O, O, O });//a3
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, O, O, I, O, O, I });//a4
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, O, O, I, O, I, O });//a5 ok
            //++++++++++++++++++++++++++++++++++++++++++++++
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, O, O, I, O, I, I });//a6
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, O, O, I, I, O, O });//a7
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, O, O, I, I, O, I });//a8
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, O, O, I, I, I, O });//a9
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, O, O, I, I, I, I });//aa
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, O, I, O, O, O, O });//b2
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, O, I, O, O, O, I });//b3
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, O, I, O, O, I, O });//b4 ok
            //++++++++++++++++++++++++++++++++++++++++++++++
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, O, I, O, O, I, I });//b5
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, O, I, O, I, O, O });//b6
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, O, I, O, I, O, I });//b7
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, O, I, O, I, I, O });//b8
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, O, I, O, I, I, I });//b9
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, O, I, I, O, O, O });//ba
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, O, I, I, O, O, I });//c2
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, O, I, I, O, I, O });//c3 ok
            //++++++++++++++++++++++++++++++++++++++++++++++
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, O, I, I, O, I, I });//c4
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, O, I, I, I, O, O });//c5
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, O, I, I, I, O, I });//c6
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, O, I, I, I, I, O });//c7
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, O, I, I, I, I, I });//c8
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, I, O, O, O, O, O });//c9
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, I, O, O, O, O, I });//ca
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, I, O, O, O, I, O });//d2 ok
            //++++++++++++++++++++++++++++++++++++++++++++++
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, I, O, O, O, I, I });//d3
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, I, O, O, I, O, O });//d4
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, I, O, O, I, O, I });//d5
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, I, O, O, I, I, O });//d6
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, I, O, O, I, I, I });//d7
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, I, O, I, O, O, O });//d8
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, I, O, I, O, O, I });//d9
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, I, O, I, O, I, O });//da ok
            //++++++++++++++++++++++++++++++++++++++++++++++
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, I, O, I, I, O, O });//e2
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, I, O, I, I, O, I });//e3
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, I, O, I, I, I, O });//e4
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, I, O, I, I, I, I });//e5
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, I, I, O, O, O, O });//e6
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, I, I, O, O, O, I });//e7
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, I, I, O, O, I, O });//e8
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, I, I, O, O, I, I });//e9 ok
            //++++++++++++++++++++++++++++++++++++++++++++++
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, I, I, O, I, O, O });//ea
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, I, I, O, I, I, O });//f2
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, I, I, O, I, I, I });//f3
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, I, I, I, O, O, O });//f4
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, I, I, I, O, O, I });//f5
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, I, I, I, O, I, O });//f6
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, I, I, I, O, I, I });//f7
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, I, I, I, I, O, O });//f8
            //++++++++++++++++++++++++++++++++++++++++++++++
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, I, I, I, I, O, I });//f9
            codes.Add(new bool[] { I, I, I, I, I, I, I, I, I, I, I, I, I, I, I, O });//fa
            //++++++++++++++++++++++++++++++++++++++++++++++
            return codes;
        }

        public static void InitializeCategoryAndBitcode()
        {
            Int32 nr;
            Int32 nr_lower, nr_upper;
            Byte cat;

            nr_lower = 1;
            nr_upper = 2;
            for (cat = 1; cat <= 15; cat++)
            {
                //Positive numbers
                for (nr = nr_lower; nr < nr_upper; nr++)
                {
                    Category[32767 + nr] = cat;
                    BitCode[32767 + nr] = new BitString();
                    BitCode[32767 + nr].setLength(cat);
                    BitCode[32767 + nr] = new BitString();
                    BitCode[32767 + nr].setValue((ushort)nr);
                }
                //Negative numbers
                for (nr = -(nr_upper - 1); nr <= -nr_lower; nr++)
                {
                    Category[32767 + nr] = cat;
                    BitCode[32767 + nr] = new BitString();
                    BitCode[32767 + nr].setLength(cat);
                    BitCode[32767 + nr] = new BitString();
                    BitCode[32767 + nr].setValue((ushort)nr);
                }
                nr_lower <<= 1;
                nr_upper <<= 1;
            }
        }

        static void Compute_Huffman_Table(Byte[] nrCodes, Byte[] std_table, ref BitString[] Huffman_Table)
        {
            Byte k, j;
            Byte pos_in_table;
            UInt16 code_value;

            code_value = 0;
            pos_in_table = 0;
            for (k = 0; k < 16; k++)
            {
                for (j = 1; j <= nrCodes[k]; j++)
                {
                    Huffman_Table[std_table[pos_in_table]].value = code_value;
                    byte length = k;
                    length++;
                    Huffman_Table[std_table[pos_in_table]].length = length;
                    pos_in_table++;
                    code_value++;
                }
                code_value <<= 1;
            }
        }

        public static void InitializeHuffmanTables()
        {
            // Compute the Huffman tables used for encoding
            zeroHuffDCTable(ref Y_DC_Huffman_Table, ref Y_AC_Huffman_Table, ref Cb_DC_Huffman_Table, ref Cb_AC_Huffman_Table);
            Compute_Huffman_Table(DHTLuminanceDC.categoryWordCounts, DHTLuminanceDC.categoryWords, ref Y_DC_Huffman_Table);
            Compute_Huffman_Table(DHTLuminanceAC.categoryWordCounts, DHTLuminanceAC.categoryWords, ref Y_AC_Huffman_Table);
            Compute_Huffman_Table(DHTChrominanceDC.categoryWordCounts, DHTChrominanceDC.categoryWords, ref Cb_DC_Huffman_Table);
            Compute_Huffman_Table(DHTChrominanceAC.categoryWordCounts, DHTChrominanceAC.categoryWords, ref Cb_AC_Huffman_Table);
        }


        private static void zeroHuffDCTable(ref BitString[] a, ref BitString[] b, ref BitString[] c, ref BitString[] d)
        {
            int dimA = a.Length, dimB = b.Length, dimC = c.Length, dimD = d.Length;
            for (int i = 0; i < dimA; i++)
                a[i] = new BitString();
            for (int i = 0; i < dimB; i++)
                b[i] = new BitString();
            for (int i = 0; i < dimC; i++)
                c[i] = new BitString();
            for (int i = 0; i < dimD; i++)
                d[i] = new BitString();
        }

        public static void printMatriciRGB(byte[,] RMatrix, byte[,] GMatrix, byte[,] BMatrix, int width, int height)
        {
            Console.WriteLine("R Matrix");
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    debugBlockPrint(RMatrix[i, j]);
                }
                Console.WriteLine();
            }
            Console.WriteLine("G Matrix");
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    debugBlockPrint(GMatrix[i, j]);
                }
                Console.WriteLine();
            }
            Console.WriteLine("B Matrix");
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    debugBlockPrint(BMatrix[i, j]);
                }
                Console.WriteLine();
            }
        }

        private void printBlock(int[,] M, int k, int w)
        {
            int rows = M.GetLength(0);
            int columns = M.GetLength(1);
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Console.Write(M[i + k, j + w] + "    ");
                }
                Console.WriteLine();
            }
        }

        public void printMatriciYCbCr(float[,] YMatrix, float[,] CbMatrix, float[,] CrMatrix, int width, int height)
        {
            Console.WriteLine("Y Matrix");
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    debugBlockPrint(YMatrix[i, j]);
                }
                Console.Write(" " + (i));
                Console.WriteLine();
            }
            Console.WriteLine("Cb Matrix");
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    debugBlockPrint(CbMatrix[i, j]);
                }
                Console.Write(" " + (i));
                Console.WriteLine();
            }
            Console.WriteLine("Cr Matrix");
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    debugBlockPrint(CrMatrix[i, j]);
                }
                Console.Write(" " + (i));
                Console.WriteLine();
            }
        }

        public void printMatrice(float[,] M, int row, int column)
        {
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < column; j++)
                {
                    debugBlockPrint(M[i, j]);
                }
                Console.WriteLine();
            }
        }

        public void printMatrice(int[,] M, int row, int column)
        {
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < column; j++)
                {
                    debugBlockPrint(M[i, j]);
                }
                Console.WriteLine();
            }
        }

        public void printMatrice(double[,] M, int row, int column)
        {
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < column; j++)
                {
                    debugBlockPrint(M[i, j]);
                }
                Console.WriteLine();
            }
        }

        public void printMatrice(Int16[,] M)
        {
            int row = M.GetLength(0);
            int columns = M.GetLength(1);

            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    debugBlockPrint(M[i, j]);
                }
                Console.WriteLine();
            }
        }

        public void printMatrice(float[,] M)
        {
            int row = M.GetLength(0);
            int columns = M.GetLength(1);

            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    debugBlockPrint(M[i, j]);
                }
                Console.WriteLine();
            }
        }

        public void printMatrice(byte[,] M)
        {
            int row = M.GetLength(0);
            int columns = M.GetLength(1);

            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    debugBlockPrint(M[i, j]);
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }


        public void printMatrice(int[,] M)
        {
            int row = M.GetLength(0);
            int columns = M.GetLength(1);

            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    debugBlockPrint(M[i, j]);
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        public void printMatrice(double[,] M)
        {
            int row = M.GetLength(0);
            int columns = M.GetLength(1);

            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    debugBlockPrint(M[i, j]);
                }
                Console.WriteLine();
            }
        }


        public void printMatrici(float[,] CbSub, float[,] CrSub)
        {
            int x = CbSub.GetLength(0);
            int y = CbSub.GetLength(1);
            Console.WriteLine("Matrix  Cb");
            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    debugBlockPrint(CbSub[i, j]);
                }
                Console.WriteLine();
            }
            Console.WriteLine("Matrix  Cr");
            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    debugBlockPrint(CrSub[i, j]);
                }
                Console.WriteLine();
            }
        }

        private void debugBlockPrint(float x)
        {
            Console.Write(x.ToString("0.0") + " ");
        }

        private void debugBlockPrint(int x)
        {
            Console.Write(x.ToString("0.0") + " ");
        }

        private static void debugBlockPrint(double x)
        {
            Console.Write(x.ToString("0.0") + " ");
        }

        private static void debugBlockPrint(byte x)
        {
            Console.Write(x.ToString("0.0") + " ");
        }

        public class BitString
        {
            public Byte length; 
            public UInt16 value; 

            public BitString()
            {
                length = 0;
                value = 0;
            }

            public BitString(Byte len, UInt16 val)
            {
                length = len;
                value = val;
            }

            public void setLength(Byte len)
            {
                this.length = len;
            }

            public void setValue(UInt16 val)
            {
                this.value = val;
            }

            public string print()
            {
                return Convert.ToString(value, 2).PadLeft(8, '0')+" len = "+length;
            }

        }//BitString

        public class JPEGHuffmanTable
        {
            public int DHTClass;
            public int ID;
            public byte[] categoryWordCounts { get; }
            public byte[] categoryWords { get; }
            public ArrayList categoryWordsHuffmanCodes { get; }
            
            public JPEGHuffmanTable(int DHTClass, int ID, byte[] categoryWordCounts, byte[] categoryWords, ArrayList categoryWordsHuffmanCodes)
            {
                this.DHTClass = DHTClass;
                this.ID = ID;
                this.categoryWordCounts = categoryWordCounts;
                this.categoryWords = categoryWords;
                this.categoryWordsHuffmanCodes = categoryWordsHuffmanCodes;
            }
        }//JPEGHuffmanTable
    }
}
