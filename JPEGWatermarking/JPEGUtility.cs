﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPEGEncoding
{
    class JPEGUtility
    {
        public static int NO_SUBSAMPLING = 0;
        public static int SUBSAMPLING_422 = 1;
        public static int SUBSAMPLING_420 = 2;

        public static int ZERO_BLOCK_PADDING = 0;
        public static int COPY_BLOCK_PADDING = 1;

        public static int DHT_CLASS_DC = 0;
        public static int DHT_CLASS_AC = 1;
        
        public static int DHT_ID_Y = 0;
        public static int DHT_ID_CbCr = 1;

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

        public static int[] ZigZagX =
        { 0, 1, 2, 1, 0, 0, 1, 2, 3, 4, 3, 2, 1, 0, 0, 1, 2, 3, 4, 5, 6, 5, 4, 3, 2, 1, 0,
          0, 1, 2, 3, 4, 5, 6, 7, 7, 6, 5, 4, 3, 2, 1, 2, 3, 4, 5, 6, 7, 7, 6, 5, 4, 3, 4, 5,
          6, 7, 7, 6, 5, 6, 7, 7 };

        public static int[] ZigZagY =
        { 1, 0, 0, 1, 2, 3, 2, 1, 0, 0, 1, 2, 3, 4, 5, 4, 3, 2, 1, 0, 0, 1, 2, 3, 4, 5, 6,
          7, 6, 5, 4, 3, 2, 1, 0, 1, 2, 3, 4, 5, 6, 7, 7, 6, 5, 4, 3, 2, 3, 4, 5, 6, 7, 7, 6,
          5, 4, 5, 6, 7, 7, 6, 7 };

        public static JPEGHuffmanTable DHTLuminanceDC = new JPEGHuffmanTable
            ( JPEGUtility.DHT_CLASS_DC, 
              JPEGUtility.DHT_ID_Y,
              new int[] { 0, 1, 5, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0 }, 
              new int[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b } 
            ); 

        public static JPEGHuffmanTable DHTChrominanceDC = new JPEGHuffmanTable
            ( JPEGUtility.DHT_CLASS_DC, 
              JPEGUtility.DHT_ID_CbCr,
              new int[] { 0, 1, 5, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0 }, 
              new int[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b } 
            );

        public static JPEGHuffmanTable DHTLuminanceAC = new JPEGHuffmanTable
            (JPEGUtility.DHT_CLASS_AC,
              JPEGUtility.DHT_ID_Y,
              new int[] { 0, 2, 1, 3, 3, 2, 4, 3, 5, 5, 4, 4, 0, 0, 1, 0x7d },
              new int[] {     0x01, 0x02, 0x03, 0x00, 0x04, 0x11, 0x05, 0x12,
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
							  0xf9, 0xfa }
            );

        public static JPEGHuffmanTable DHTChrominanceAC = new JPEGHuffmanTable
            (JPEGUtility.DHT_CLASS_AC,
              JPEGUtility.DHT_ID_CbCr,
              new int[] { 0, 2, 1, 3, 3, 2, 4, 3, 5, 5, 4, 4, 0, 0, 1, 0x7d },
              new int[] {     0x00, 0x01, 0x02, 0x03, 0x11, 0x04, 0x05, 0x21,
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
                              0xf9, 0xfa }
            );

        public class JPEGHuffmanTable
        {
            public int DHTClass;
            public int ID;
            public int[] categoryWordCounts { get; }
            public int[] categoryWords { get; }

            public JPEGHuffmanTable(int DHTClass, int ID, int[] categoryWordCounts, int[] categoryWords)
            {
                this.DHTClass = DHTClass;
                this.ID = ID;
                this.categoryWordCounts = categoryWordCounts;
                this.categoryWords = categoryWords;
            }
        }
    }
}