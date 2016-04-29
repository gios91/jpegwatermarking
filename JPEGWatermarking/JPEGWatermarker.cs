using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace JPEGWatermarking
{
    class JPEGWatermarker : JPEGWatermarkerIF
    {
        public static byte EOB = 255;
        private BinaryFormatter bf = new BinaryFormatter();

        public Tuple<byte[,], byte[,], byte[,]> doRGBWatermarking(byte[,] R, byte[,] G, byte[,] B, byte[] byteString)
        {
            BitBinaryWriter bbw = new BitBinaryWriter(byteString);      //memorizzo l'array di byte in BitWriter per scansione bit a bit
            int rows = R.GetLength(0);
            int columns = R.GetLength(1);

            byte[,] RNew = new byte[rows, columns];
            byte[,] GNew = new byte[rows, columns];
            byte[,] BNew = new byte[rows, columns];

            int imageNumOfPx = R.GetLength(0) * R.GetLength(1) * 3;
            int textNumOfBit = byteString.Length * 8;
            
            if ( textNumOfBit <= imageNumOfPx )
            {
                for (int i = 0; i < rows; i++)
                    for (int j = 0; j < columns; j++)
                    {
                        if (bbw.getNumBitsRemaining() >= 3)
                        {
                            RNew[i, j] = bbw.changeLSB(R[i, j]);
                            GNew[i, j] = bbw.changeLSB(G[i, j]);
                            BNew[i, j] = bbw.changeLSB(B[i, j]);
                        }
                        else if (bbw.getNumBitsRemaining() == 2)
                        {
                            RNew[i, j] = bbw.changeLSB(R[i, j]);
                            GNew[i, j] = bbw.changeLSB(G[i, j]);
                        }
                        else if (bbw.getNumBitsRemaining() == 1)
                        {
                            RNew[i, j] = bbw.changeLSB(R[i, j]);
                        }
                        else
                        {
                            RNew[i, j] = R[i, j];
                            GNew[i, j] = G[i, j];
                            BNew[i, j] = B[i, j];
                        }
                    }
            }
            else
            {
                throw new Exception("Non è possibile inserire il dict perché di dimensioni maggiori dell'img");
            }
            return Tuple.Create(RNew, GNew, BNew);

        }//doRGBWatermarking


        public Tuple<byte[,], byte[,], byte[,]> doLuminanceRGBWatermarking(byte[,] R, byte[,] G, byte[,] B, byte[] byteString, List<int[]> blockSequence, int numLSBSelectedBlock, int numLSBNonSelectedBlock)
        {
            BitBinaryWriter bbw = new BitBinaryWriter(byteString);      //memorizzo l'array di byte in BitWriter per scansione bit a bit
            bool isPossibleToInsertWater = getLuminanceRGBNumBitRequired(R, byteString, blockSequence.Count, numLSBSelectedBlock, numLSBNonSelectedBlock);
            if (!isPossibleToInsertWater)
                throw new Exception("Non è possibile inserire il watermarking: dimensioni maggiori dei bit disponibili");
            int rows = R.GetLength(0);
            int columns = R.GetLength(1);
            byte[,] RNew = new byte[rows, columns];
            byte[,] GNew = new byte[rows, columns];
            byte[,] BNew = new byte[rows, columns];
            for (int i = 0; i < rows; i += 8)
                for (int j = 0; j < columns; j += 8)
                {
                    int[] rowCol = new int[2];
                    rowCol[0] = i; rowCol[1] = j;
                    if (isContained(blockSequence, rowCol))
                    {   //inserire il watermarking nei blocchi selezionati
                        switch (numLSBSelectedBlock)
                        {
                            case 1:
                                rgbWatermarkingLevel1Block(R, G, B, ref RNew, ref GNew, ref BNew, i, j, byteString, ref bbw);
                                break;
                            case 2:
                                rgbWatermarkingLevel2Block(R, G, B, ref RNew, ref GNew, ref BNew, i, j, byteString, ref bbw);
                                break;
                            case 3:
                                rgbWatermarkingLevel3Block(R, G, B, ref RNew, ref GNew, ref BNew, i, j, byteString, ref bbw);
                                break;
                            case 4:
                                rgbWatermarkingLevel4Block(R, G, B, ref RNew, ref GNew, ref BNew, i, j, byteString, ref bbw);
                                break;
                        }
                    }
                    else
                    {
                        switch (numLSBNonSelectedBlock)
                        {
                            case 1:
                                rgbWatermarkingLevel1Block(R, G, B, ref RNew, ref GNew, ref BNew, i, j, byteString, ref bbw);
                                break;
                            case 2:
                                rgbWatermarkingLevel2Block(R, G, B, ref RNew, ref GNew, ref BNew, i, j, byteString, ref bbw);
                                break;
                            case 3:
                                rgbWatermarkingLevel3Block(R, G, B, ref RNew, ref GNew, ref BNew, i, j, byteString, ref bbw);
                                break;
                            case 4:
                                rgbWatermarkingLevel4Block(R, G, B, ref RNew, ref GNew, ref BNew, i, j, byteString, ref bbw);
                                break;
                        }
                    }
                }
            return Tuple.Create(RNew, GNew, BNew);
        }

        private bool getLuminanceRGBNumBitRequired(byte[,] M, byte[] byteString, int numSelectedBlock, int numLSBSelectedBlock, int numLSBNonSelectedBlock)
        {
            int numBlock = (M.GetLength(0) * M.GetLength(1)) / 64;
            int numNonSelectedBlock = numBlock - numSelectedBlock;
            int numBitsAvailable = (64 * (numLSBSelectedBlock * 3) * numSelectedBlock) + (64 * (numLSBNonSelectedBlock * 3) * numNonSelectedBlock);

            Console.WriteLine("[++++++ TEST +++++++] num bit da scrivere = {0}", byteString.Length * 8);
            Console.WriteLine("[++++++ TEST +++++++] num bit disponibili = {0}", numBitsAvailable);

            if (byteString.Length * 8 > numBitsAvailable)
                return false;
            return true;

        }

        private bool isContained(List<int[]> blockSequence, int[] colRow)
        {
            foreach (int[] idx in blockSequence)
                if (idx[0] == colRow[0] && idx[1] == colRow[1])
                    return true;
            return false;
        }

        public Tuple<byte[,], byte[,], byte[,], int> doAdvancedRGBWatermarking(byte[,] R, byte[,] G, byte[,] B, byte[] byteString)
        {
            /*
             * level è un valore fissato [1,4] che indica il massimo num di LSB da alterare con l'inserimento del watermarking
             */
            BitBinaryWriter bbw = new BitBinaryWriter(byteString);      //memorizzo l'array di byte in BitWriter per scansione bit a bit
            int numLevelRequired = getLevelRequired(R, byteString);
            if (numLevelRequired == -1)
                throw new Exception("Non è possibile inserire il watermarking: dimensioni maggiori dell'RGB level 4");
            int rows = R.GetLength(0);
            int columns = R.GetLength(1);
            byte[,] RNew = new byte[rows, columns];
            byte[,] GNew = new byte[rows, columns];
            byte[,] BNew = new byte[rows, columns];

            switch (numLevelRequired)
            {
                case 1:
                    rgbWatermarkingLevel1(R, G, B, ref RNew, ref GNew, ref BNew, byteString, bbw);
                    break;
                case 2:
                    rgbWatermarkingLevel2(R, G, B, ref RNew, ref GNew, ref BNew, byteString, bbw);
                    break;
                case 3:
                    rgbWatermarkingLevel3(R, G, B, ref RNew, ref GNew, ref BNew, byteString, bbw);
                    break;
                case 4:
                    rgbWatermarkingLevel4(R, G, B, ref RNew, ref GNew, ref BNew, byteString, bbw);
                    break;
            }
            return Tuple.Create(RNew, GNew, BNew, numLevelRequired);
        }

        private void rgbWatermarkingLevel1(byte[,] R, byte[,] G, byte[,] B, ref byte[,] RNew, ref byte[,] GNew, ref byte[,] BNew, byte[] byteString, BitBinaryWriter bbw)
        {
            int rows = R.GetLength(0);
            int columns = R.GetLength(1);
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < columns; j++)
                {
                    if (bbw.getNumBitsRemaining() >= 3)
                    {
                        RNew[i, j] = bbw.changeLSB(R[i, j]);
                        GNew[i, j] = bbw.changeLSB(G[i, j]);
                        BNew[i, j] = bbw.changeLSB(B[i, j]);
                    }
                    else if (bbw.getNumBitsRemaining() == 2)
                    {
                        RNew[i, j] = bbw.changeLSB(R[i, j]);
                        GNew[i, j] = bbw.changeLSB(G[i, j]);
                    }
                    else if (bbw.getNumBitsRemaining() == 1)
                    {
                        RNew[i, j] = bbw.changeLSB(R[i, j]);
                    }
                    else
                    {
                        RNew[i, j] = R[i, j];
                        GNew[i, j] = G[i, j];
                        BNew[i, j] = B[i, j];
                    }
                }
        }

        private void rgbWatermarkingLevel1Block(byte[,] R, byte[,] G, byte[,] B, ref byte[,] RNew, ref byte[,] GNew, ref byte[,] BNew, int x, int y, byte[] byteString, ref BitBinaryWriter bbw)
        {
            int rows = R.GetLength(0);
            int columns = R.GetLength(1);
            for (int i = x; i < x+8 ; i++)
                for (int j = y; j < y+8 ; j++)
                {
                    if (bbw.getNumBitsRemaining() >= 3)
                    {
                        RNew[i, j] = bbw.changeLSB(R[i, j]);
                        GNew[i, j] = bbw.changeLSB(G[i, j]);
                        BNew[i, j] = bbw.changeLSB(B[i, j]);
                    }
                    else if (bbw.getNumBitsRemaining() == 2)
                    {
                        RNew[i, j] = bbw.changeLSB(R[i, j]);
                        GNew[i, j] = bbw.changeLSB(G[i, j]);
                    }
                    else if (bbw.getNumBitsRemaining() == 1)
                    {
                        RNew[i, j] = bbw.changeLSB(R[i, j]);
                    }
                    else
                    {
                        RNew[i, j] = R[i, j];
                        GNew[i, j] = G[i, j];
                        BNew[i, j] = B[i, j];
                    }
                }
        }


        private void rgbWatermarkingLevel2(byte[,] R, byte[,] G, byte[,] B, ref byte[,] RNew, ref byte[,] GNew, ref byte[,] BNew, byte[] byteString, BitBinaryWriter bbw)
        {
            int rows = R.GetLength(0);
            int columns = R.GetLength(1);
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < columns; j++)
                {
                    if (bbw.getNumBitsRemaining() >= 6)
                    {
                        byte RTemp = bbw.changeLSB(R[i, j], 0);
                        RNew[i, j] = bbw.changeLSB(RTemp, 1);
                        byte GTemp = bbw.changeLSB(G[i, j], 0);
                        GNew[i, j] = bbw.changeLSB(GTemp, 1);
                        byte BTemp = bbw.changeLSB(B[i, j], 0);
                        BNew[i, j] = bbw.changeLSB(BTemp, 1);
                    }
                    else if (bbw.getNumBitsRemaining() == 5)
                    {
                        byte RTemp = bbw.changeLSB(R[i, j], 0);
                        RNew[i, j] = bbw.changeLSB(RTemp, 1);
                        byte GTemp = bbw.changeLSB(G[i, j], 0);
                        GNew[i, j] = bbw.changeLSB(GTemp, 1);
                        BNew[i, j] = bbw.changeLSB(B[i, j], 0);
                    }
                    else if (bbw.getNumBitsRemaining() == 4)
                    {
                        byte RTemp = bbw.changeLSB(R[i, j], 0);
                        RNew[i, j] = bbw.changeLSB(RTemp, 1);
                        byte GTemp = bbw.changeLSB(G[i, j], 0);
                        GNew[i, j] = bbw.changeLSB(GTemp, 1);
                    }
                    else if (bbw.getNumBitsRemaining() == 3)
                    {
                        byte RTemp = bbw.changeLSB(R[i, j], 0);
                        RNew[i, j] = bbw.changeLSB(RTemp, 1);
                        GNew[i, j] = bbw.changeLSB(G[i, j], 0);
                    }
                    else if (bbw.getNumBitsRemaining() == 2)
                    {
                        byte RTemp = bbw.changeLSB(R[i, j], 0);
                        RNew[i, j] = bbw.changeLSB(RTemp, 1);
                    }
                    else if (bbw.getNumBitsRemaining() == 1)
                    {
                        RNew[i, j] = bbw.changeLSB(R[i, j], 0);
                    }
                    else
                    {
                        RNew[i, j] = R[i, j];
                        GNew[i, j] = G[i, j];
                        BNew[i, j] = B[i, j];
                    }
                }
        }

        private void rgbWatermarkingLevel2Block(byte[,] R, byte[,] G, byte[,] B, ref byte[,] RNew, ref byte[,] GNew, ref byte[,] BNew, int x, int y, byte[] byteString, ref BitBinaryWriter bbw)
        {
            int rows = R.GetLength(0);
            int columns = R.GetLength(1);
            for (int i = x; i < x+8; i++)
                for (int j = y; j < y+8; j++)
                {
                    if (bbw.getNumBitsRemaining() >= 6)
                    {
                        byte RTemp = bbw.changeLSB(R[i, j], 0);
                        RNew[i, j] = bbw.changeLSB(RTemp, 1);
                        byte GTemp = bbw.changeLSB(G[i, j], 0);
                        GNew[i, j] = bbw.changeLSB(GTemp, 1);
                        byte BTemp = bbw.changeLSB(B[i, j], 0);
                        BNew[i, j] = bbw.changeLSB(BTemp, 1);
                    }
                    else if (bbw.getNumBitsRemaining() == 5)
                    {
                        byte RTemp = bbw.changeLSB(R[i, j], 0);
                        RNew[i, j] = bbw.changeLSB(RTemp, 1);
                        byte GTemp = bbw.changeLSB(G[i, j], 0);
                        GNew[i, j] = bbw.changeLSB(GTemp, 1);
                        BNew[i, j] = bbw.changeLSB(B[i, j], 0);
                    }
                    else if (bbw.getNumBitsRemaining() == 4)
                    {
                        byte RTemp = bbw.changeLSB(R[i, j], 0);
                        RNew[i, j] = bbw.changeLSB(RTemp, 1);
                        byte GTemp = bbw.changeLSB(G[i, j], 0);
                        GNew[i, j] = bbw.changeLSB(GTemp, 1);
                    }
                    else if (bbw.getNumBitsRemaining() == 3)
                    {
                        byte RTemp = bbw.changeLSB(R[i, j], 0);
                        RNew[i, j] = bbw.changeLSB(RTemp, 1);
                        GNew[i, j] = bbw.changeLSB(G[i, j], 0);
                    }
                    else if (bbw.getNumBitsRemaining() == 2)
                    {
                        byte RTemp = bbw.changeLSB(R[i, j], 0);
                        RNew[i, j] = bbw.changeLSB(RTemp, 1);
                    }
                    else if (bbw.getNumBitsRemaining() == 1)
                    {
                        RNew[i, j] = bbw.changeLSB(R[i, j], 0);
                    }
                    else
                    {
                        RNew[i, j] = R[i, j];
                        GNew[i, j] = G[i, j];
                        BNew[i, j] = B[i, j];
                    }
                }
        }

        private void rgbWatermarkingLevel3(byte[,] R, byte[,] G, byte[,] B, ref byte[,] RNew, ref byte[,] GNew, ref byte[,] BNew, byte[] byteString, BitBinaryWriter bbw)
        {
            int rows = R.GetLength(0);
            int columns = R.GetLength(1);
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < columns; j++)
                {
                    if (bbw.getNumBitsRemaining() >= 9)
                    {
                        byte RTemp1 = bbw.changeLSB(R[i, j], 0);
                        byte RTemp2 = bbw.changeLSB(RTemp1, 1);
                        RNew[i, j] = bbw.changeLSB(RTemp2, 2);
                        byte GTemp1 = bbw.changeLSB(G[i, j], 0);
                        byte GTemp2 = bbw.changeLSB(GTemp1, 1);
                        GNew[i, j] = bbw.changeLSB(GTemp2, 2);
                        byte BTemp1 = bbw.changeLSB(B[i, j], 0);
                        byte BTemp2 = bbw.changeLSB(BTemp1, 1);
                        BNew[i, j] = bbw.changeLSB(BTemp2, 2);
                    }
                    else if (bbw.getNumBitsRemaining() == 8)
                    {
                        byte RTemp1 = bbw.changeLSB(R[i, j], 0);
                        byte RTemp2 = bbw.changeLSB(RTemp1, 1);
                        RNew[i, j] = bbw.changeLSB(RTemp2, 2);
                        byte GTemp1 = bbw.changeLSB(G[i, j], 0);
                        byte GTemp2 = bbw.changeLSB(GTemp1, 1);
                        GNew[i, j] = bbw.changeLSB(GTemp2, 2);
                        byte BTemp1 = bbw.changeLSB(B[i, j], 0);
                        BNew[i, j] = bbw.changeLSB(BTemp1, 1);
                    }
                    else if (bbw.getNumBitsRemaining() == 7)
                    {
                        byte RTemp1 = bbw.changeLSB(R[i, j], 0);
                        byte RTemp2 = bbw.changeLSB(RTemp1, 1);
                        RNew[i, j] = bbw.changeLSB(RTemp2, 2);
                        byte GTemp1 = bbw.changeLSB(G[i, j], 0);
                        byte GTemp2 = bbw.changeLSB(GTemp1, 1);
                        GNew[i, j] = bbw.changeLSB(GTemp2, 2);
                        BNew[i, j] = bbw.changeLSB(B[i, j], 0);
                    }
                    else if (bbw.getNumBitsRemaining() == 6)
                    {
                        byte RTemp1 = bbw.changeLSB(R[i, j], 0);
                        byte RTemp2 = bbw.changeLSB(RTemp1, 1);
                        RNew[i, j] = bbw.changeLSB(RTemp2, 2);
                        byte GTemp1 = bbw.changeLSB(G[i, j], 0);
                        byte GTemp2 = bbw.changeLSB(GTemp1, 1);
                        GNew[i, j] = bbw.changeLSB(GTemp2, 2);
                    }
                    else if (bbw.getNumBitsRemaining() == 5)
                    {
                        byte RTemp1 = bbw.changeLSB(R[i, j], 0);
                        byte RTemp2 = bbw.changeLSB(RTemp1, 1);
                        RNew[i, j] = bbw.changeLSB(RTemp2, 2);
                        byte GTemp1 = bbw.changeLSB(G[i, j], 0);
                        GNew[i, j] = bbw.changeLSB(GTemp1, 1);
                    }
                    else if (bbw.getNumBitsRemaining() == 4)
                    {
                        byte RTemp1 = bbw.changeLSB(R[i, j], 0);
                        byte RTemp2 = bbw.changeLSB(RTemp1, 1);
                        RNew[i, j] = bbw.changeLSB(RTemp2, 2);
                        GNew[i, j] = bbw.changeLSB(G[i, j], 0);
                    }
                    else if (bbw.getNumBitsRemaining() == 3)
                    {
                        byte RTemp1 = bbw.changeLSB(R[i, j], 0);
                        byte RTemp2 = bbw.changeLSB(RTemp1, 1);
                        RNew[i, j] = bbw.changeLSB(RTemp2, 2);
                    }
                    else if (bbw.getNumBitsRemaining() == 2)
                    {
                        byte RTemp1 = bbw.changeLSB(R[i, j], 0);
                        RNew[i, j] = bbw.changeLSB(RTemp1, 1);
                    }
                    else if (bbw.getNumBitsRemaining() == 1)
                    {
                        RNew[i, j] = bbw.changeLSB(R[i, j], 0);
                    }
                    else
                    {
                        RNew[i, j] = R[i, j];
                        GNew[i, j] = G[i, j];
                        BNew[i, j] = B[i, j];
                    }
                }
        }

        private void rgbWatermarkingLevel3Block(byte[,] R, byte[,] G, byte[,] B, ref byte[,] RNew, ref byte[,] GNew, ref byte[,] BNew, int x, int y, byte[] byteString, ref BitBinaryWriter bbw)
        {
            int rows = R.GetLength(0);
            int columns = R.GetLength(1);
            for (int i = x; i < x+8; i++)
                for (int j = y; j < y+8; j++)
                {
                    if (bbw.getNumBitsRemaining() >= 9)
                    {
                        byte RTemp1 = bbw.changeLSB(R[i, j], 0);
                        byte RTemp2 = bbw.changeLSB(RTemp1, 1);
                        RNew[i, j] = bbw.changeLSB(RTemp2, 2);
                        byte GTemp1 = bbw.changeLSB(G[i, j], 0);
                        byte GTemp2 = bbw.changeLSB(GTemp1, 1);
                        GNew[i, j] = bbw.changeLSB(GTemp2, 2);
                        byte BTemp1 = bbw.changeLSB(B[i, j], 0);
                        byte BTemp2 = bbw.changeLSB(BTemp1, 1);
                        BNew[i, j] = bbw.changeLSB(BTemp2, 2);
                    }
                    else if (bbw.getNumBitsRemaining() == 8)
                    {
                        byte RTemp1 = bbw.changeLSB(R[i, j], 0);
                        byte RTemp2 = bbw.changeLSB(RTemp1, 1);
                        RNew[i, j] = bbw.changeLSB(RTemp2, 2);
                        byte GTemp1 = bbw.changeLSB(G[i, j], 0);
                        byte GTemp2 = bbw.changeLSB(GTemp1, 1);
                        GNew[i, j] = bbw.changeLSB(GTemp2, 2);
                        byte BTemp1 = bbw.changeLSB(B[i, j], 0);
                        BNew[i, j] = bbw.changeLSB(BTemp1, 1);
                    }
                    else if (bbw.getNumBitsRemaining() == 7)
                    {
                        byte RTemp1 = bbw.changeLSB(R[i, j], 0);
                        byte RTemp2 = bbw.changeLSB(RTemp1, 1);
                        RNew[i, j] = bbw.changeLSB(RTemp2, 2);
                        byte GTemp1 = bbw.changeLSB(G[i, j], 0);
                        byte GTemp2 = bbw.changeLSB(GTemp1, 1);
                        GNew[i, j] = bbw.changeLSB(GTemp2, 2);
                        BNew[i, j] = bbw.changeLSB(B[i, j], 0);
                    }
                    else if (bbw.getNumBitsRemaining() == 6)
                    {
                        byte RTemp1 = bbw.changeLSB(R[i, j], 0);
                        byte RTemp2 = bbw.changeLSB(RTemp1, 1);
                        RNew[i, j] = bbw.changeLSB(RTemp2, 2);
                        byte GTemp1 = bbw.changeLSB(G[i, j], 0);
                        byte GTemp2 = bbw.changeLSB(GTemp1, 1);
                        GNew[i, j] = bbw.changeLSB(GTemp2, 2);
                    }
                    else if (bbw.getNumBitsRemaining() == 5)
                    {
                        byte RTemp1 = bbw.changeLSB(R[i, j], 0);
                        byte RTemp2 = bbw.changeLSB(RTemp1, 1);
                        RNew[i, j] = bbw.changeLSB(RTemp2, 2);
                        byte GTemp1 = bbw.changeLSB(G[i, j], 0);
                        GNew[i, j] = bbw.changeLSB(GTemp1, 1);
                    }
                    else if (bbw.getNumBitsRemaining() == 4)
                    {
                        byte RTemp1 = bbw.changeLSB(R[i, j], 0);
                        byte RTemp2 = bbw.changeLSB(RTemp1, 1);
                        RNew[i, j] = bbw.changeLSB(RTemp2, 2);
                        GNew[i, j] = bbw.changeLSB(G[i, j], 0);
                    }
                    else if (bbw.getNumBitsRemaining() == 3)
                    {
                        byte RTemp1 = bbw.changeLSB(R[i, j], 0);
                        byte RTemp2 = bbw.changeLSB(RTemp1, 1);
                        RNew[i, j] = bbw.changeLSB(RTemp2, 2);
                    }
                    else if (bbw.getNumBitsRemaining() == 2)
                    {
                        byte RTemp1 = bbw.changeLSB(R[i, j], 0);
                        RNew[i, j] = bbw.changeLSB(RTemp1, 1);
                    }
                    else if (bbw.getNumBitsRemaining() == 1)
                    {
                        RNew[i, j] = bbw.changeLSB(R[i, j], 0);
                    }
                    else
                    {
                        RNew[i, j] = R[i, j];
                        GNew[i, j] = G[i, j];
                        BNew[i, j] = B[i, j];
                    }
                }
        }

        private void rgbWatermarkingLevel4(byte[,] R, byte[,] G, byte[,] B, ref byte[,] RNew, ref byte[,] GNew, ref byte[,] BNew, byte[] byteString, BitBinaryWriter bbw)
        {
            int rows = R.GetLength(0);
            int columns = R.GetLength(1);
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < columns; j++)
                {
                    if (bbw.getNumBitsRemaining() >= 12)
                    {
                        byte RTemp1 = bbw.changeLSB(R[i, j], 0);
                        byte RTemp2 = bbw.changeLSB(RTemp1, 1);
                        byte RTemp3 = bbw.changeLSB(RTemp2, 2);
                        RNew[i, j] = bbw.changeLSB(RTemp3, 3);
                        byte GTemp1 = bbw.changeLSB(G[i, j], 0);
                        byte GTemp2 = bbw.changeLSB(GTemp1, 1);
                        byte GTemp3 = bbw.changeLSB(GTemp2, 2);
                        GNew[i, j] = bbw.changeLSB(GTemp3, 3);
                        byte BTemp1 = bbw.changeLSB(B[i, j], 0);
                        byte BTemp2 = bbw.changeLSB(BTemp1, 1);
                        byte BTemp3 = bbw.changeLSB(BTemp2, 2);
                        BNew[i, j] = bbw.changeLSB(BTemp3, 3);
                    }
                    else if (bbw.getNumBitsRemaining() == 11)
                    {
                        byte RTemp1 = bbw.changeLSB(R[i, j], 0);
                        byte RTemp2 = bbw.changeLSB(RTemp1, 1);
                        byte RTemp3 = bbw.changeLSB(RTemp2, 2);
                        RNew[i, j] = bbw.changeLSB(RTemp3, 3);
                        byte GTemp1 = bbw.changeLSB(G[i, j], 0);
                        byte GTemp2 = bbw.changeLSB(GTemp1, 1);
                        byte GTemp3 = bbw.changeLSB(GTemp2, 2);
                        GNew[i, j] = bbw.changeLSB(GTemp3, 3);
                        byte BTemp1 = bbw.changeLSB(B[i, j], 0);
                        byte BTemp2 = bbw.changeLSB(BTemp1, 1);
                        BNew[i, j] = bbw.changeLSB(BTemp2, 2);
                    }
                    else if (bbw.getNumBitsRemaining() == 10)
                    {
                        byte RTemp1 = bbw.changeLSB(R[i, j], 0);
                        byte RTemp2 = bbw.changeLSB(RTemp1, 1);
                        byte RTemp3 = bbw.changeLSB(RTemp2, 2);
                        RNew[i, j] = bbw.changeLSB(RTemp3, 3);
                        byte GTemp1 = bbw.changeLSB(G[i, j], 0);
                        byte GTemp2 = bbw.changeLSB(GTemp1, 1);
                        byte GTemp3 = bbw.changeLSB(GTemp2, 2);
                        GNew[i, j] = bbw.changeLSB(GTemp3, 3);
                        byte BTemp1 = bbw.changeLSB(B[i, j], 0);
                        BNew[i , j] = bbw.changeLSB(BTemp1, 1);
                    }
                    else if (bbw.getNumBitsRemaining() == 9)
                    {
                        byte RTemp1 = bbw.changeLSB(R[i, j], 0);
                        byte RTemp2 = bbw.changeLSB(RTemp1, 1);
                        byte RTemp3 = bbw.changeLSB(RTemp2, 2);
                        RNew[i, j] = bbw.changeLSB(RTemp3, 3);
                        byte GTemp1 = bbw.changeLSB(G[i, j], 0);
                        byte GTemp2 = bbw.changeLSB(GTemp1, 1);
                        byte GTemp3 = bbw.changeLSB(GTemp2, 2);
                        GNew[i, j] = bbw.changeLSB(GTemp3, 3);
                        BNew[i, j] = bbw.changeLSB(B[i, j], 0);
                    }
                    else if (bbw.getNumBitsRemaining() == 8)
                    {
                        byte RTemp1 = bbw.changeLSB(R[i, j], 0);
                        byte RTemp2 = bbw.changeLSB(RTemp1, 1);
                        byte RTemp3 = bbw.changeLSB(RTemp2, 2);
                        RNew[i, j] = bbw.changeLSB(RTemp3, 3);
                        byte GTemp1 = bbw.changeLSB(G[i, j], 0);
                        byte GTemp2 = bbw.changeLSB(GTemp1, 1);
                        byte GTemp3 = bbw.changeLSB(GTemp2, 2);
                        GNew[i, j] = bbw.changeLSB(GTemp3, 3);
                    }
                    else if (bbw.getNumBitsRemaining() == 7)
                    {
                        byte RTemp1 = bbw.changeLSB(R[i, j], 0);
                        byte RTemp2 = bbw.changeLSB(RTemp1, 1);
                        byte RTemp3 = bbw.changeLSB(RTemp2, 2);
                        RNew[i, j] = bbw.changeLSB(RTemp3, 3);
                        byte GTemp1 = bbw.changeLSB(G[i, j], 0);
                        byte GTemp2 = bbw.changeLSB(GTemp1, 1);
                        GNew[i, j] = bbw.changeLSB(GTemp2, 2);
                    }
                    else if (bbw.getNumBitsRemaining() == 6)
                    {
                        byte RTemp1 = bbw.changeLSB(R[i, j], 0);
                        byte RTemp2 = bbw.changeLSB(RTemp1, 1);
                        byte RTemp3 = bbw.changeLSB(RTemp2, 2);
                        RNew[i, j] = bbw.changeLSB(RTemp3, 3);
                        byte GTemp1 = bbw.changeLSB(G[i, j], 0);
                        GNew[i ,j] = bbw.changeLSB(GTemp1, 1);
                    }
                    else if (bbw.getNumBitsRemaining() == 5)
                    {
                        byte RTemp1 = bbw.changeLSB(R[i, j], 0);
                        byte RTemp2 = bbw.changeLSB(RTemp1, 1);
                        byte RTemp3 = bbw.changeLSB(RTemp2, 2);
                        RNew[i, j] = bbw.changeLSB(RTemp3, 3);
                        GNew[i ,j] = bbw.changeLSB(G[i, j], 0);
                    }
                    else if (bbw.getNumBitsRemaining() == 4)
                    {
                        byte RTemp1 = bbw.changeLSB(R[i, j], 0);
                        byte RTemp2 = bbw.changeLSB(RTemp1, 1);
                        byte RTemp3 = bbw.changeLSB(RTemp2, 2);
                        RNew[i, j] = bbw.changeLSB(RTemp3, 3);
                    }
                    else if (bbw.getNumBitsRemaining() == 3)
                    {
                        byte RTemp1 = bbw.changeLSB(R[i, j], 0);
                        byte RTemp2 = bbw.changeLSB(RTemp1, 1);
                        RNew[i, j] = bbw.changeLSB(RTemp2, 2);
                    }
                    else if (bbw.getNumBitsRemaining() == 2)
                    {
                        byte RTemp1 = bbw.changeLSB(R[i, j], 0);
                        RNew[i, j] = bbw.changeLSB(RTemp1, 1);
                    }
                    else if (bbw.getNumBitsRemaining() == 1)
                    {
                        RNew[i, j] = bbw.changeLSB(R[i, j], 0);
                    }
                    else
                    {
                        RNew[i, j] = R[i, j];
                        GNew[i, j] = G[i, j];
                        BNew[i, j] = B[i, j];
                    }
                }
        }

        private void rgbWatermarkingLevel4Block(byte[,] R, byte[,] G, byte[,] B, ref byte[,] RNew, ref byte[,] GNew, ref byte[,] BNew, int x, int y, byte[] byteString, ref BitBinaryWriter bbw)
        {
            int rows = R.GetLength(0);
            int columns = R.GetLength(1);
            for (int i = x; i < x+8; i++)
                for (int j = y; j < y+8; j++)
                {
                    if (bbw.getNumBitsRemaining() >= 12)
                    {
                        byte RTemp1 = bbw.changeLSB(R[i, j], 0);
                        byte RTemp2 = bbw.changeLSB(RTemp1, 1);
                        byte RTemp3 = bbw.changeLSB(RTemp2, 2);
                        RNew[i, j] = bbw.changeLSB(RTemp3, 3);
                        byte GTemp1 = bbw.changeLSB(G[i, j], 0);
                        byte GTemp2 = bbw.changeLSB(GTemp1, 1);
                        byte GTemp3 = bbw.changeLSB(GTemp2, 2);
                        GNew[i, j] = bbw.changeLSB(GTemp3, 3);
                        byte BTemp1 = bbw.changeLSB(B[i, j], 0);
                        byte BTemp2 = bbw.changeLSB(BTemp1, 1);
                        byte BTemp3 = bbw.changeLSB(BTemp2, 2);
                        BNew[i, j] = bbw.changeLSB(BTemp3, 3);
                    }
                    else if (bbw.getNumBitsRemaining() == 11)
                    {
                        byte RTemp1 = bbw.changeLSB(R[i, j], 0);
                        byte RTemp2 = bbw.changeLSB(RTemp1, 1);
                        byte RTemp3 = bbw.changeLSB(RTemp2, 2);
                        RNew[i, j] = bbw.changeLSB(RTemp3, 3);
                        byte GTemp1 = bbw.changeLSB(G[i, j], 0);
                        byte GTemp2 = bbw.changeLSB(GTemp1, 1);
                        byte GTemp3 = bbw.changeLSB(GTemp2, 2);
                        GNew[i, j] = bbw.changeLSB(GTemp3, 3);
                        byte BTemp1 = bbw.changeLSB(B[i, j], 0);
                        byte BTemp2 = bbw.changeLSB(BTemp1, 1);
                        BNew[i, j] = bbw.changeLSB(BTemp2, 2);
                    }
                    else if (bbw.getNumBitsRemaining() == 10)
                    {
                        byte RTemp1 = bbw.changeLSB(R[i, j], 0);
                        byte RTemp2 = bbw.changeLSB(RTemp1, 1);
                        byte RTemp3 = bbw.changeLSB(RTemp2, 2);
                        RNew[i, j] = bbw.changeLSB(RTemp3, 3);
                        byte GTemp1 = bbw.changeLSB(G[i, j], 0);
                        byte GTemp2 = bbw.changeLSB(GTemp1, 1);
                        byte GTemp3 = bbw.changeLSB(GTemp2, 2);
                        GNew[i, j] = bbw.changeLSB(GTemp3, 3);
                        byte BTemp1 = bbw.changeLSB(B[i, j], 0);
                        BNew[i, j] = bbw.changeLSB(BTemp1, 1);
                    }
                    else if (bbw.getNumBitsRemaining() == 9)
                    {
                        byte RTemp1 = bbw.changeLSB(R[i, j], 0);
                        byte RTemp2 = bbw.changeLSB(RTemp1, 1);
                        byte RTemp3 = bbw.changeLSB(RTemp2, 2);
                        RNew[i, j] = bbw.changeLSB(RTemp3, 3);
                        byte GTemp1 = bbw.changeLSB(G[i, j], 0);
                        byte GTemp2 = bbw.changeLSB(GTemp1, 1);
                        byte GTemp3 = bbw.changeLSB(GTemp2, 2);
                        GNew[i, j] = bbw.changeLSB(GTemp3, 3);
                        BNew[i, j] = bbw.changeLSB(B[i, j], 0);
                    }
                    else if (bbw.getNumBitsRemaining() == 8)
                    {
                        byte RTemp1 = bbw.changeLSB(R[i, j], 0);
                        byte RTemp2 = bbw.changeLSB(RTemp1, 1);
                        byte RTemp3 = bbw.changeLSB(RTemp2, 2);
                        RNew[i, j] = bbw.changeLSB(RTemp3, 3);
                        byte GTemp1 = bbw.changeLSB(G[i, j], 0);
                        byte GTemp2 = bbw.changeLSB(GTemp1, 1);
                        byte GTemp3 = bbw.changeLSB(GTemp2, 2);
                        GNew[i, j] = bbw.changeLSB(GTemp3, 3);
                    }
                    else if (bbw.getNumBitsRemaining() == 7)
                    {
                        byte RTemp1 = bbw.changeLSB(R[i, j], 0);
                        byte RTemp2 = bbw.changeLSB(RTemp1, 1);
                        byte RTemp3 = bbw.changeLSB(RTemp2, 2);
                        RNew[i, j] = bbw.changeLSB(RTemp3, 3);
                        byte GTemp1 = bbw.changeLSB(G[i, j], 0);
                        byte GTemp2 = bbw.changeLSB(GTemp1, 1);
                        GNew[i, j] = bbw.changeLSB(GTemp2, 2);
                    }
                    else if (bbw.getNumBitsRemaining() == 6)
                    {
                        byte RTemp1 = bbw.changeLSB(R[i, j], 0);
                        byte RTemp2 = bbw.changeLSB(RTemp1, 1);
                        byte RTemp3 = bbw.changeLSB(RTemp2, 2);
                        RNew[i, j] = bbw.changeLSB(RTemp3, 3);
                        byte GTemp1 = bbw.changeLSB(G[i, j], 0);
                        GNew[i, j] = bbw.changeLSB(GTemp1, 1);
                    }
                    else if (bbw.getNumBitsRemaining() == 5)
                    {
                        byte RTemp1 = bbw.changeLSB(R[i, j], 0);
                        byte RTemp2 = bbw.changeLSB(RTemp1, 1);
                        byte RTemp3 = bbw.changeLSB(RTemp2, 2);
                        RNew[i, j] = bbw.changeLSB(RTemp3, 3);
                        GNew[i, j] = bbw.changeLSB(G[i, j], 0);
                    }
                    else if (bbw.getNumBitsRemaining() == 4)
                    {
                        byte RTemp1 = bbw.changeLSB(R[i, j], 0);
                        byte RTemp2 = bbw.changeLSB(RTemp1, 1);
                        byte RTemp3 = bbw.changeLSB(RTemp2, 2);
                        RNew[i, j] = bbw.changeLSB(RTemp3, 3);
                    }
                    else if (bbw.getNumBitsRemaining() == 3)
                    {
                        byte RTemp1 = bbw.changeLSB(R[i, j], 0);
                        byte RTemp2 = bbw.changeLSB(RTemp1, 1);
                        RNew[i, j] = bbw.changeLSB(RTemp2, 2);
                    }
                    else if (bbw.getNumBitsRemaining() == 2)
                    {
                        byte RTemp1 = bbw.changeLSB(R[i, j], 0);
                        RNew[i, j] = bbw.changeLSB(RTemp1, 1);
                    }
                    else if (bbw.getNumBitsRemaining() == 1)
                    {
                        RNew[i, j] = bbw.changeLSB(R[i, j], 0);
                    }
                    else
                    {
                        RNew[i, j] = R[i, j];
                        GNew[i, j] = G[i, j];
                        BNew[i, j] = B[i, j];
                    }
                }
        }


        private int getLevelRequired(byte[,] RMatrix, byte[] byteString)
        {
            int rows = RMatrix.GetLength(0);
            int columns = RMatrix.GetLength(1);
            int numWatermarkingBit = byteString.Length * 8;
            int numLSBLevel1 = rows * columns * 3;
            int numLSBLevel2 = rows * columns * 6;
            int numLSBLevel3 = rows * columns * 9;
            int numLSBLevel4 = rows * columns * 12;
            if (numWatermarkingBit <= numLSBLevel1)
                return 1;
            else if (numWatermarkingBit <= numLSBLevel2)
                return 2;
            else if (numWatermarkingBit <= numLSBLevel3)
                return 3;
            else if (numWatermarkingBit <= numLSBLevel4)
                return 4;
            else
                return -1;
        }

        public byte[] getRGBWatermarking(byte[,] R, byte[,] G, byte[,] B, int EOS)
        {
            BitBinaryWriter bbw = new BitBinaryWriter();      //memorizzo l'array di byte in BitWriter per scansione bit a bit
            int rows = R.GetLength(0);
            int columns = R.GetLength(1);
            bool[] temp = new bool[rows * columns * 3]; //massima dimensione dei dati memorizzabile nell'immagine (in bit)
            int cntTemp = 0;
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < columns; j++)
                {
                    temp[cntTemp] = bbw.getLSB(R[i, j]);
                    temp[cntTemp + 1] = bbw.getLSB(G[i, j]);
                    temp[cntTemp + 2] = bbw.getLSB(B[i, j]);
                    cntTemp += 3;
                }
            byte[] v = bbw.getByteArray(temp);
            int numElem = 0;
            int cntEOB = 0;
            bool contaEOB = false;
            for (int k = 0; k < v.Length; k++)
            {
                if (v[k] == EOB)
                {
                    if (!contaEOB)
                    {
                        cntEOB++;
                        contaEOB = true;
                        numElem++;
                    }
                    else if (contaEOB)
                    {
                        cntEOB++;
                        numElem++;
                    }
                }
                else
                {
                    if (contaEOB)
                    {
                        if (cntEOB == EOS)
                        {
                            break;
                        }
                        else
                        {
                            contaEOB = false;
                            cntEOB = 0;
                            numElem++;
                        }
                    }
                    else
                    {
                        numElem++;
                    }
                }
            }
            byte[] result = new byte[numElem];
            for (int w = 0; w < numElem; w++)
                result[w] = v[w];
            return result;
        }
        
        public byte[] getAdvancedRGBWatermarking(byte[,] R, byte[,] G, byte[,] B, int EOS, int level)
        {
            BitBinaryWriter bbw = new BitBinaryWriter();      //memorizzo l'array di byte in BitWriter per scansione bit a bit
            int[] numLSBForLevel = { 0, 3, 6, 9, 12 };
            int rows = R.GetLength(0);
            int columns = R.GetLength(1);
            bool[] temp = new bool[rows * columns * numLSBForLevel[level]]; //massima dimensione dei dati memorizzabile nell'immagine (in bit)
            int cntTemp = 0;
            switch(level)
            {
                case 1:
                    getRGBWatermarkingLevel1(R, G, B, ref temp, ref cntTemp, ref bbw);
                    break;
                case 2:
                    getRGBWatermarkingLevel2(R, G, B, ref temp, ref cntTemp, ref bbw);
                    break;
                case 3:
                    getRGBWatermarkingLevel3(R, G, B, ref temp, ref cntTemp, ref bbw);
                    break;
                case 4:
                    getRGBWatermarkingLevel4(R, G, B, ref temp, ref cntTemp, ref bbw);
                    break;
            }
            byte[] v = bbw.getByteArray(temp);
            int numElem = 0;
            int cntEOB = 0;
            bool contaEOB = false;
            for (int k = 0; k < v.Length; k++)
            {
                if (v[k] == EOB)
                {
                    if (!contaEOB)
                    {
                        cntEOB++;
                        contaEOB = true;
                        numElem++;
                    }
                    else if (contaEOB)
                    {
                        cntEOB++;
                        numElem++;
                    }
                }
                else
                {
                    if (contaEOB)
                    {
                        if (cntEOB == EOS)
                        {
                            break;
                        }
                        else
                        {
                            contaEOB = false;
                            cntEOB = 0;
                            numElem++;
                        }
                    }
                    else
                    {
                        numElem++;
                    }
                }
            }
            byte[] result = new byte[numElem];
            for (int w = 0; w < numElem; w++)
                result[w] = v[w];
            return result;
        }

        public byte[] getLuminanceRGBWatermarking(byte[,] R, byte[,] G, byte[,] B, int EOS, List<int[]> blockSequence, int numLSBSelectedBlock, int numLSBNonSelectedBlock)
        {
            BitBinaryWriter bbw = new BitBinaryWriter();      //memorizzo l'array di byte in BitWriter per scansione bit a bit
            int[] numLSBForLevel = { 0, 3, 6, 9, 12 };
            int rows = R.GetLength(0);
            int columns = R.GetLength(1);
            bool[] temp = new bool[rows * columns * numLSBForLevel[numLSBSelectedBlock]]; //massima dimensione dei dati memorizzabile nell'immagine (in bit)
            int cntTemp = 0;
            for (int i = 0; i < rows; i += 8)
                for (int j = 0; j < columns; j += 8)
                {
                    int[] rowCol = new int[2];
                    rowCol[0] = i; rowCol[1] = j;
                    if (isContained(blockSequence,rowCol))
                    {   //inserire il watermarking nei blocchi selezionati
                        if (numLSBSelectedBlock == 1)
                            getRGBWatermarkingLevel1Block(R, G, B, ref temp, ref cntTemp, i, j, ref bbw);
                        else if (numLSBSelectedBlock == 2)
                            getRGBWatermarkingLevel2Block(R, G, B, ref temp, ref cntTemp, i, j, ref bbw);
                        else if (numLSBSelectedBlock == 3)
                            getRGBWatermarkingLevel3Block(R, G, B, ref temp, ref cntTemp, i, j, ref bbw);
                        else if (numLSBSelectedBlock == 4)
                            getRGBWatermarkingLevel4Block(R, G, B, ref temp, ref cntTemp, i, j, ref bbw);
                    }
                    else
                    {
                        if (numLSBNonSelectedBlock == 1)
                            getRGBWatermarkingLevel1Block(R, G, B, ref temp, ref cntTemp, i, j, ref bbw);
                        else if (numLSBNonSelectedBlock == 2)
                            getRGBWatermarkingLevel2Block(R, G, B, ref temp, ref cntTemp, i, j, ref bbw);
                        else if (numLSBNonSelectedBlock == 3)
                            getRGBWatermarkingLevel3Block(R, G, B, ref temp, ref cntTemp, i, j, ref bbw);
                        else if (numLSBNonSelectedBlock == 4)
                            getRGBWatermarkingLevel4Block(R, G, B, ref temp, ref cntTemp, i, j, ref bbw);
                    }
                }
            byte[] v = bbw.getByteArray(temp);
            int numElem = 0;
            int cntEOB = 0;
            bool contaEOB = false;
            for (int k = 0; k < v.Length; k++)
            {
                if (v[k] == EOB)
                {
                    if (!contaEOB)
                    {
                        cntEOB++;
                        contaEOB = true;
                        numElem++;
                    }
                    else if (contaEOB)
                    {
                        cntEOB++;
                        numElem++;
                    }
                }
                else
                {
                    if (contaEOB)
                    {
                        if (cntEOB == EOS)
                        {
                            break;
                        }
                        else
                        {
                            contaEOB = false;
                            cntEOB = 0;
                            numElem++;
                        }
                    }
                    else
                    {
                        numElem++;
                    }
                }
            }
            byte[] result = new byte[numElem];
            for (int w = 0; w < numElem; w++)
                result[w] = v[w];
            return result;
        }
        

        private void getRGBWatermarkingLevel1(byte[,] R, byte[,] G, byte[,] B, ref bool[] temp, ref int cntTemp, ref BitBinaryWriter bbw)
        {
            int rows = R.GetLength(0);
            int columns = R.GetLength(1);
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < columns; j++)
                {
                    temp[cntTemp] = bbw.getLSB(R[i, j]);
                    temp[cntTemp + 1] = bbw.getLSB(G[i, j]);
                    temp[cntTemp + 2] = bbw.getLSB(B[i, j]);
                    cntTemp += 3;
                }
        }

        private void getRGBWatermarkingLevel1Block(byte[,] R, byte[,] G, byte[,] B, ref bool[] temp, ref int cntTemp, int x, int y, ref BitBinaryWriter bbw)
        {
            for (int i = x; i < x + 8; i++)
                for (int j = y; j < y + 8; j++)
                {
                    temp[cntTemp] = bbw.getLSB(R[i, j]);
                    temp[cntTemp + 1] = bbw.getLSB(G[i, j]);
                    temp[cntTemp + 2] = bbw.getLSB(B[i, j]);
                    cntTemp += 3;
                }
        }

        private void getRGBWatermarkingLevel2(byte[,] R, byte[,] G, byte[,] B, ref bool[] temp, ref int cntTemp, ref BitBinaryWriter bbw)
        {
            int rows = R.GetLength(0);
            int columns = R.GetLength(1);
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < columns; j++)
                {
                    temp[cntTemp] = bbw.getLSB(R[i, j], 0);
                    temp[cntTemp + 1] = bbw.getLSB(R[i, j], 1);
                    temp[cntTemp + 2] = bbw.getLSB(G[i, j], 0);
                    temp[cntTemp + 3] = bbw.getLSB(G[i, j], 1);
                    temp[cntTemp + 4] = bbw.getLSB(B[i, j], 0);
                    temp[cntTemp + 5] = bbw.getLSB(B[i, j], 1);
                    cntTemp += 6;
                }
        }

        private void getRGBWatermarkingLevel2Block(byte[,] R, byte[,] G, byte[,] B, ref bool[] temp, ref int cntTemp, int x, int y, ref BitBinaryWriter bbw)
        {
            for (int i = x; i < x+8; i++)
                for (int j = y; j < y+8; j++)
                {
                    temp[cntTemp] = bbw.getLSB(R[i, j], 0);
                    temp[cntTemp + 1] = bbw.getLSB(R[i, j], 1);
                    temp[cntTemp + 2] = bbw.getLSB(G[i, j], 0);
                    temp[cntTemp + 3] = bbw.getLSB(G[i, j], 1);
                    temp[cntTemp + 4] = bbw.getLSB(B[i, j], 0);
                    temp[cntTemp + 5] = bbw.getLSB(B[i, j], 1);
                    cntTemp += 6;
                }
        }

        private void getRGBWatermarkingLevel3(byte[,] R, byte[,] G, byte[,] B, ref bool[] temp, ref int cntTemp, ref BitBinaryWriter bbw)
        {
            int rows = R.GetLength(0);
            int columns = R.GetLength(1);
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < columns; j++)
                {
                    temp[cntTemp] = bbw.getLSB(R[i, j], 0);
                    temp[cntTemp + 1] = bbw.getLSB(R[i, j], 1);
                    temp[cntTemp + 2] = bbw.getLSB(R[i, j], 2);
                    temp[cntTemp + 3] = bbw.getLSB(G[i, j], 0);
                    temp[cntTemp + 4] = bbw.getLSB(G[i, j], 1);
                    temp[cntTemp + 5] = bbw.getLSB(G[i, j], 2);
                    temp[cntTemp + 6] = bbw.getLSB(B[i, j], 0);
                    temp[cntTemp + 7] = bbw.getLSB(B[i, j], 1);
                    temp[cntTemp + 8] = bbw.getLSB(B[i, j], 2);
                    cntTemp += 9;
                }
        }

        private void getRGBWatermarkingLevel3Block(byte[,] R, byte[,] G, byte[,] B, ref bool[] temp, ref int cntTemp, int x, int y, ref BitBinaryWriter bbw)
        {
            for (int i = x; i < x+8; i++)
                for (int j = y; j < y+8; j++)
                {
                    temp[cntTemp] = bbw.getLSB(R[i, j], 0);
                    temp[cntTemp + 1] = bbw.getLSB(R[i, j], 1);
                    temp[cntTemp + 2] = bbw.getLSB(R[i, j], 2);
                    temp[cntTemp + 3] = bbw.getLSB(G[i, j], 0);
                    temp[cntTemp + 4] = bbw.getLSB(G[i, j], 1);
                    temp[cntTemp + 5] = bbw.getLSB(G[i, j], 2);
                    temp[cntTemp + 6] = bbw.getLSB(B[i, j], 0);
                    temp[cntTemp + 7] = bbw.getLSB(B[i, j], 1);
                    temp[cntTemp + 8] = bbw.getLSB(B[i, j], 2);
                    cntTemp += 9;
                }
        }

        private void getRGBWatermarkingLevel4(byte[,] R, byte[,] G, byte[,] B, ref bool[] temp, ref int cntTemp, ref BitBinaryWriter bbw)
        {
            int rows = R.GetLength(0);
            int columns = R.GetLength(1);
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < columns; j++)
                {
                    temp[cntTemp] = bbw.getLSB(R[i, j], 0);
                    temp[cntTemp + 1] = bbw.getLSB(R[i, j], 1);
                    temp[cntTemp + 2] = bbw.getLSB(R[i, j], 2);
                    temp[cntTemp + 3] = bbw.getLSB(R[i, j], 3);
                    temp[cntTemp + 4] = bbw.getLSB(G[i, j], 0);
                    temp[cntTemp + 5] = bbw.getLSB(G[i, j], 1);
                    temp[cntTemp + 6] = bbw.getLSB(G[i, j], 2);
                    temp[cntTemp + 7] = bbw.getLSB(G[i, j], 3);
                    temp[cntTemp + 8] = bbw.getLSB(B[i, j], 0);
                    temp[cntTemp + 9] = bbw.getLSB(B[i, j], 1);
                    temp[cntTemp + 10] = bbw.getLSB(B[i, j], 2);
                    temp[cntTemp + 11] = bbw.getLSB(B[i, j], 3);
                    cntTemp += 12;
                }
        }

        private void getRGBWatermarkingLevel4Block(byte[,] R, byte[,] G, byte[,] B, ref bool[] temp, ref int cntTemp, int x, int y, ref BitBinaryWriter bbw)
        {
            for (int i = x; i < x+8; i++)
                for (int j = y; j < y+8; j++)
                {
                    temp[cntTemp] = bbw.getLSB(R[i, j], 0);
                    temp[cntTemp + 1] = bbw.getLSB(R[i, j], 1);
                    temp[cntTemp + 2] = bbw.getLSB(R[i, j], 2);
                    temp[cntTemp + 3] = bbw.getLSB(R[i, j], 3);
                    temp[cntTemp + 4] = bbw.getLSB(G[i, j], 0);
                    temp[cntTemp + 5] = bbw.getLSB(G[i, j], 1);
                    temp[cntTemp + 6] = bbw.getLSB(G[i, j], 2);
                    temp[cntTemp + 7] = bbw.getLSB(G[i, j], 3);
                    temp[cntTemp + 8] = bbw.getLSB(B[i, j], 0);
                    temp[cntTemp + 9] = bbw.getLSB(B[i, j], 1);
                    temp[cntTemp + 10] = bbw.getLSB(B[i, j], 2);
                    temp[cntTemp + 11] = bbw.getLSB(B[i, j], 3);
                    cntTemp += 12;
                }
        }


        public List<int[]> getDictByteDecoding(byte[] dictArray)
        {
            MemoryStream dictStreamDes = new MemoryStream();
            dictStreamDes.Write(dictArray, 0, dictArray.Length);
            dictStreamDes.Position = 0;
            List<int[]> dictFromByte = bf.Deserialize(dictStreamDes) as List<int[]>;
            return dictFromByte;
        }

        public Dictionary<int, string> getDictNewCharsByteDecoding(byte[] dictArray)
        {
            MemoryStream dictNewCharsStreamDes = new MemoryStream();
            dictNewCharsStreamDes.Write(dictArray, 0, dictArray.Length);
            dictNewCharsStreamDes.Position = 0;
            Dictionary<int, string> dictNewCharsFromByte = bf.Deserialize(dictNewCharsStreamDes) as Dictionary<int, string>;
            return dictNewCharsFromByte;
        }

        public List<Int16[]> getDictByteCompactDecoding(byte[] dictArray)
        {
            MemoryStream dictStreamDes = new MemoryStream();
            dictStreamDes.Write(dictArray, 0, dictArray.Length);
            dictStreamDes.Position = 0;
            List<Int16[]> dictFromByte = bf.Deserialize(dictStreamDes) as List<Int16[]>;
            return dictFromByte;
        }

        public string[] getDictNewCharsByteCompactDecoding(byte[] dictArray)
        {
            MemoryStream dictNewCharsStreamDes = new MemoryStream();
            dictNewCharsStreamDes.Write(dictArray, 0, dictArray.Length);
            dictNewCharsStreamDes.Position = 0;
            string[] dictNewCharsFromByte = bf.Deserialize(dictNewCharsStreamDes) as string[];
            return dictNewCharsFromByte;
        }

        private int getMaxEOBSequence(byte[] stream)
        {
            int cntmaxEOB = 0;
            int cntEOB = 0;
            bool conta = false;
            for (int i = 0; i < stream.Length; i++)
            {
                if (stream[i] == EOB)
                {
                    if (!conta)
                    {
                        cntEOB++;
                        conta = true;
                    }
                    else if (conta)
                    {
                        cntEOB++;
                    }
                }
                else
                {
                    if (conta)
                    {
                        if (cntmaxEOB < cntEOB)
                            cntmaxEOB = cntEOB;
                        conta = false;
                        cntEOB = 0;
                    }
                }
            }
            return cntmaxEOB;
        }

        private int getEOS(byte[] dict, byte[] dictNewChars)
        {
            int maxEOBSeq = 0;  //massimo tra max seq EOB di dict e max seq EOB di dictNewChars
            int maxSeqEOBDict = getMaxEOBSequence(dict);
            int maxSeqEOBDictNewChars = getMaxEOBSequence(dictNewChars);
            if (maxSeqEOBDict > maxSeqEOBDictNewChars)
                maxEOBSeq = maxSeqEOBDict;
            else
                maxEOBSeq = maxSeqEOBDictNewChars;
            int EOS = maxEOBSeq + 2;            //numero di EOB che determinano la fine del flusso
            return EOS;
        }


        private int getEOD(byte[] dict, byte[] dictNewChars)
        {
            int maxEOBSeq = 0;  //massimo tra max seq EOB di dict e max seq EOB di dictNewChars
            int maxSeqEOBDict = getMaxEOBSequence(dict);
            int maxSeqEOBDictNewChars = getMaxEOBSequence(dictNewChars);
            if (maxSeqEOBDict > maxSeqEOBDictNewChars)
                maxEOBSeq = maxSeqEOBDict;
            else
                maxEOBSeq = maxSeqEOBDictNewChars;
            int EOD = maxEOBSeq + 1;            //numero di EOB che determinano la fine del flusso
            return EOD;
        }


        public Tuple<byte[],int,int> createWatermarkingString (byte[] dict, byte[] dictNewChars)
        {
            /* Lo stream finale è così composto: 
             * | byte di dict | EOD = EOB * maxEOBSeq + 1 | byte di dictNewChars | EOS = EOB *  maxEOBSeq + 2 |  
             */
            int maxEOBSeq = 0;  //massimo tra max seq EOB di dict e max seq EOB di dictNewChars
            int maxSeqEOBDict = getMaxEOBSequence(dict);
            int maxSeqEOBDictNewChars = getMaxEOBSequence(dictNewChars);
            if (maxSeqEOBDict > maxSeqEOBDictNewChars)
                maxEOBSeq = maxSeqEOBDict;
            else
                maxEOBSeq = maxSeqEOBDictNewChars;
            int EOD = maxEOBSeq + 1;            //numero di EOB che dividono dict da dictNewChars
            int EOS = maxEOBSeq + 2;            //numero di EOB che determinano la fine del flusso
            int kDictNewCh = 0;
            byte[] finalStream = new byte[dict.Length + EOD + dictNewChars.Length + EOS];
            for (int i=0; i<finalStream.Length; i++)
            {
                if (i < dict.Length)
                {
                    finalStream[i] = dict[i];
                }
                else if (i < dict.Length+EOD)
                {//EOD
                    finalStream[i] = EOB;
                }
                else if (i < dict.Length+EOD+dictNewChars.Length)
                {
                    finalStream[i] = dictNewChars[kDictNewCh];
                    kDictNewCh++;
                }
                else
                {//EOS
                    finalStream[i] = EOB;
                }
            }
            return Tuple.Create(finalStream,EOD, EOS);
        }

        public Tuple<byte[],byte[]> decodeWatermarkingString(byte[] byteString, int EOD, int EOS)
        {
            ArrayList currentBytes = new ArrayList();
            byte[] dict = null;
            byte[] dictNewChars = null;
            bool contaEOB = false;
            int cntEOB = 0;
            for (int i=0; i < byteString.Length; i++)
            {
                if (byteString[i] == EOB)
                {
                    if (!contaEOB)
                    {
                        cntEOB++;
                        contaEOB = true;
                        currentBytes.Add(byteString[i]);
                    }
                    else if (contaEOB)
                    {
                        cntEOB++;
                        currentBytes.Add(byteString[i]);
                        if (i == byteString.Length - 1 )
                        {
                            dictNewChars = new byte[currentBytes.Count - EOS];
                            for (int k = 0; k < dictNewChars.Length; k++)
                                dictNewChars[k] = (byte)currentBytes[k];
                            contaEOB = false;
                            cntEOB = 0;
                        }
                    }
                }
                else
                {
                    if (contaEOB)
                    {
                        if (cntEOB == EOD)
                        {
                            dict = new byte[currentBytes.Count - EOD];
                            for (int k = 0; k < dict.Length; k++)
                                dict[k] = (byte)currentBytes[k];
                            currentBytes.Clear();
                            // fine dict ed inizio dictNewChars
                            currentBytes.Add(byteString[i]);
                            contaEOB = false;
                            cntEOB = 0;
                        }
                        /*
                        else if (cntEOB == EOS)
                        {
                            dictNewChars = new byte[currentBytes.Count - EOS];
                            for (int k = 0; k < dictNewChars.Length; k++)
                                dictNewChars[k] = (byte)currentBytes[k];
                            contaEOB = false;
                            cntEOB = 0;
                        }
                        */
                        else
                        {
                            contaEOB = false;
                            cntEOB = 0;
                            currentBytes.Add(byteString[i]);
                        }
                    }
                    else
                    {
                        currentBytes.Add(byteString[i]);
                    }
                }
            }
            return Tuple.Create(dict, dictNewChars);
        }

        public byte[] getDictByteEncoding(List<int[]> dict)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream dictStream = new MemoryStream();
            bf.Serialize(dictStream, dict);
            byte[] dictArray = dictStream.ToArray();
            return dictArray;
        }

        public byte[] getDictNewCharsByteEncoding(Dictionary<int, string> dictNewChars)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream dictStream = new MemoryStream();
            bf.Serialize(dictStream, dictNewChars);
            byte[] dictNewCharsArray = dictStream.ToArray();
            return dictNewCharsArray;
        }
        

        private double getAverageLuminanceValue(float[,] Y, float delta)
        {
            int rows = Y.GetLength(0);
            int columns = Y.GetLength(1);
            double numPixel = Y.GetLength(0) * Y.GetLength(1);
            double sumLogY = 0;
            for (int i=0; i< rows; i++)
                for (int j=0; j<columns; j++)
                {
                    double pixLogValue = Math.Log(Y[i, j] + delta);
                    sumLogY += pixLogValue;
                }
            double avgSumLogY = sumLogY / numPixel;
            return Math.Exp(avgSumLogY);
        }


        public List<int[]> getBlocksForYWatermarking(float[,] Y, float delta)
        {
            int rows = Y.GetLength(0);
            int columns = Y.GetLength(1);
            List<int[]> blockSequ = new List<int[]>();
            double YAvgLuminance = getAverageLuminanceValue(Y, delta);
            for (int i = 0; i < rows; i += 8)
                for (int j = 0; j < columns; j += 8)
                {
                    //calcolo il valore di Y medio per ogni blocco
                    float[,] block = copyBlock(Y, i, j);
                    double blockAvgLuminance = getAverageLuminanceValue(block, delta);
                    if (blockAvgLuminance >= YAvgLuminance)
                    {
                        //il blocco può essere usato per LSBAdvancedWatermarking
                        int[] rowColIndex = new int[2];
                        rowColIndex[0] = i; rowColIndex[1] = j;
                        blockSequ.Add(rowColIndex);
                    }
                }
            return blockSequ;
        }

        private float[,] copyBlock(float[,] M, int k, int w)
        {
            float[,] copyBlock = new float[8, 8];
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                {
                    copyBlock[i, j] = M[i + k, j + w];
                }
            return copyBlock;
        }

        public class BitBinaryWriter : System.IO.BinaryWriter
        {
            private bool[] bitArray;
            private int curBitIndx = 0;
            private System.Collections.BitArray ba;

            public string encoding = "";
            public int cntWrittenByte = 0;

            public BitBinaryWriter(Stream s) : base(s) { }

            public BitBinaryWriter() { } //DEBUG       

            public BitBinaryWriter(byte[] array)
            {
                bitArray = new bool[array.Length * 8];
                initBitArray(array);
            }

            private void initBitArray(byte[] array)
            {
                BitArray ba = new BitArray(array);
                for (int i = 0; i < ba.Length; i++)
                    bitArray[i] = ba[i];
            }

            public int getNumBitsRemaining()
            {
                return bitArray.Length - curBitIndx;
            }

            public bool getLSB(byte b)
            {
                BitArray ba = new BitArray(new byte[] { b });
                return ba[0];
            }

            public bool getLSB(byte b, int position)
            {
                BitArray ba = new BitArray(new byte[] { b });
                return ba[position];
            }

            public byte[] getByteArray(bool[] v)
            {
                byte[] result = new byte[v.Length / 8];
                int cntByte = 0;
                for (int i = 0; i < v.Length; i += 8)
                {
                    bool[] b = new bool[8];
                    int cnt = 0;
                    for (int j = i; j < i+8; j++)
                    {
                        b[cnt] = v[j];
                        cnt++;
                    }
                    byte currentByte = ConvertToByte(b);
                    result[cntByte] = currentByte;
                    cntByte++;
                }
                return result;
            }

            public byte[] getByteArray(BitArray v)
            {
                byte[] result = new byte[v.Length / 8];
                int cntByte = 0;
                for (int i = 0; i < v.Length; i += 8)
                {
                    bool[] b = new bool[8];
                    int cnt = 0;
                    for (int j = i; j < i + 8; j++)
                    {
                        b[cnt] = v[j];
                        cnt++;
                    }
                    byte currentByte = ConvertToByte(b);
                    result[cntByte] = currentByte;
                    cntByte++;
                }
                return result;
            }

            /*
            public override void Flush()
            {
                base.Write(ConvertToByte(curByte));
                base.Flush();
            }
            */

            public void Write(bool[] boolArray)
            {
                for (int i = 0; i < boolArray.Length; i++)
                    this.Write(boolArray[i]);
            }
            /*
            public override void Write(bool value)
            {
                curByte[curBitIndx] = value;
                this.curBitIndx++;
                if (curBitIndx == 8)
                {
                    byte toWrite = ConvertToByte(curByte);
                    base.Write(toWrite);
                    this.encoding += Convert.ToString(toWrite, 2).PadLeft(8, '0');
                    cntWrittenByte++;
                    if (toWrite == 255)
                    {
                        base.Write(0x00);
                        this.encoding += Convert.ToString(0x00, 2).PadLeft(8, '0');
                        cntWrittenByte++;
                        //controllo sulla scrittura del byte FF
                    }
                    this.curBitIndx = 0;
                    this.curByte = new bool[8];
                }
            }
            */

            public override void Write(byte value)
            {
                bool firstOneBit = false;
                ba = new BitArray(new byte[] { value });
                for (int k = 7; k >= 0; k--)
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
                        b |= (byte)(((byte)1) << bitIndex);
                    }
                    bitIndex++;
                }
                return b;
            }

            public byte ConvertToByte(BitArray bools)
            {
                byte b = 0;

                byte bitIndex = 0;
                for (int i = 0; i < 8; i++)
                {
                    if (bools[i])
                    {
                        b |= (byte)(((byte)1) << bitIndex);
                    }
                    bitIndex++;
                }
                return b;
            }

            public byte changeLSB(byte b)
            {
                BitArray bArray = new BitArray(new byte[] { b });
                bArray[0] = this.bitArray[curBitIndx];
                bool[] result = new bool[8];
                curBitIndx++;
                return ConvertToByte(bArray);
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
                    result[4 + cnt] = byte2[i];
                    cnt++;
                }
                return ConvertToByte(result);
            }

            public BitArray getREncoding(byte[] stream, int numR)
            {
                BitArray v = new BitArray(stream);
                int lengthEnc = stream.Length * 8 * numR;
                BitArray enc = new BitArray(lengthEnc);
                for(int i=0; i<v.Length; i++)
                    for (int j=0; j<numR; j++)
                    {
                        enc[i * numR + j] = v[i];
                    }
                return enc;
            }

            internal byte changeLSB(byte b, int position)
            {
                BitArray bArray = new BitArray(new byte[] { b });
                bArray[position] = this.bitArray[curBitIndx];
                bool[] result = new bool[8];
                curBitIndx++;
                return ConvertToByte(bArray);
            }
        } //BitBitBinaryWriter
    }

}
