using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using System.Drawing;
using DCTLib;
using System.Collections;

namespace JPEGEncoding
{
    class JPEGEncoder : JPEGEncoderIF
    {
        private Bitmap b;
        private DCT dct;

        public JPEGEncoder(string pathImage)
        {
            this.b = new Bitmap(pathImage);
            dct = new DCT(8, 8);
        }
        
        public int[] getImageDimensions()
        {
            int[] dimXYPixel = new int[2];
            dimXYPixel[0] = b.Height;
            dimXYPixel[1] = b.Width;
            return dimXYPixel;
        }

        public Tuple<byte[,], byte[,], byte[,]> getRGBMatrix(string pathFile)
        {
            int yPx = b.Width;
            int xPx = b.Height;
            byte[,] RMatrix = new byte[xPx, yPx];
            byte[,] GMatrix = new byte[xPx, yPx];
            byte[,] BMatrix = new byte[xPx, yPx];
            for (int i = 0; i < xPx; i++)
                for (int j = 0; j < yPx; j++)
                {
                    RMatrix[i, j] = b.GetPixel(j, i).R;
                    GMatrix[i, j] = b.GetPixel(j, i).G;
                    BMatrix[i, j] = b.GetPixel(j, i).B;
                }
            return Tuple.Create(RMatrix, GMatrix, BMatrix);
        }

        public Tuple<float[,], float[,], float[,]> getYCbCrMatrix(byte[,] RMatrix, byte[,] GMatrix, byte[,] BMatrix)
        {
            int xPx = RMatrix.GetLength(0);
            int yPx = RMatrix.GetLength(1);
            float[,] YMatrix = new float[xPx, yPx];
            float[,] CbMatrix = new float[xPx, yPx];
            float[,] CrMatrix = new float[xPx, yPx];
            for (int i = 0; i < xPx; i++)
                for (int j = 0; j < yPx; j++)
                {
                    RGB rgb = new RGB(RMatrix[i, j], GMatrix[i, j], BMatrix[i, j]);
                    YCbCr ycbcr = RGBToYCbCr(rgb);
                    YMatrix[i, j] = ycbcr.Y;
                    CbMatrix[i, j] = ycbcr.Cb;
                    CrMatrix[i, j] = ycbcr.Cr;
                }
            return Tuple.Create(YMatrix, CbMatrix, CrMatrix);
        }

        public Tuple<float[,], float[,]> espandiCbCr(float[,] cbMatrixSub, float[,] crMatrixSub)
        {
            int rows = cbMatrixSub.GetLength(0);
            int columns = cbMatrixSub.GetLength(1);
            float[,] CbExp = new float[rows * 2, columns * 2];
            float[,] CrExp = new float[rows * 2, columns * 2];
            for (int i=0; i < rows; i++) 
                for (int j=0; j < columns; j++)
                {
                    CbExp[2 * i, 2 * j] = cbMatrixSub[i, j];
                    CbExp[2 * i, 2 * j + 1 ] = cbMatrixSub[i, j];
                    CbExp[2 * i + 1, 2 * j] = cbMatrixSub[i, j];
                    CbExp[2 * i + 1, 2 * j + 1] = cbMatrixSub[i, j];
                    CrExp[2 * i, 2 * j] = crMatrixSub[i, j];
                    CrExp[2 * i, 2 * j + 1] = crMatrixSub[i, j];
                    CrExp[2 * i + 1, 2 * j] = crMatrixSub[i, j];
                    CrExp[2 * i + 1, 2 * j + 1] = crMatrixSub[i, j];
                }
            return Tuple.Create(CbExp, CrExp);
        }

        public Tuple<byte[,], byte[,], byte[,]> getRGBFromYCbCr(float[,] YMatrix, float[,] CbMatrix, float[,] CrMatrix)
        {
            int xPx = YMatrix.GetLength(0);
            int yPx = YMatrix.GetLength(1);
            byte[,] RMatrix = new byte[xPx, yPx];
            byte[,] GMatrix = new byte[xPx, yPx];
            byte[,] BMatrix = new byte[xPx, yPx];
            for (int i = 0; i < xPx; i++)
                for (int j = 0; j < yPx; j++)
                {
                    RGB rgb = YCbCrToRGB(new YCbCr(YMatrix[i,j], CbMatrix[i, j], CrMatrix[i, j]));
                    RMatrix[i, j] = rgb.R;
                    GMatrix[i, j] = rgb.G;
                    BMatrix[i, j] = rgb.B;
                }
            return Tuple.Create(RMatrix, GMatrix, BMatrix);
        }

        public Tuple<float[,], float[,]> get420Subsampling(float[,] Cb, float[,] Cr, int type)
        {
            //SI ASSUME PER ORA CHE LE MATRICI YCC ABBIANO DIMENSIONE MULTIPLA DI 16 px
            int rows = Cb.GetLength(0);
            int columns = Cb.GetLength(1);
            float[,] CbSub = new float[rows, columns];
            float[,] CrSub = new float[rows, columns];
            for (int i = 0; i < rows; i += 16)
                for (int j = 0; j < columns; j += 16)
                {
                    Tuple<float[,], float[,]> result = get420SubsamplingBlock(Cb, Cr, i, j, type);
                    float[,] CbBlock = result.Item1;
                    float[,] CrBlock = result.Item2;
                    for (int k = 0; k<16; k++)
                        for (int w = 0; w<16; w++)
                        {
                            CbSub[k+i,w+j] = CbBlock[k,w];
                            CrSub[k+i,w+j] = CrBlock[k,w];
                        }
                }
            return Tuple.Create(CbSub, CrSub);
        }

        public Tuple<float[,], float[,]> get422Subsampling(float[,] Cb, float[,] Cr, int type)
        {
            //SI ASSUME PER ORA CHE LE MATRICI YCC ABBIANO DIMENSIONE MULTIPLA DI 16 px
            int rows = Cb.GetLength(0);
            int columns = Cb.GetLength(1);
            float[,] CbSub = new float[rows, columns];
            float[,] CrSub = new float[rows, columns];
            for (int i = 0; i < rows; i += 16)
                for (int j = 0; j < columns; j += 16)
                {
                    Tuple<float[,], float[,]> result = get422SubsamplingBlock(Cb, Cr, i, j, type);
                    float[,] CbBlock = result.Item1;
                    float[,] CrBlock = result.Item2;
                    for (int k = 0; k < 16; k++)
                        for (int w = 0; w < 16; w++)
                        {
                            CbSub[k + i, w + j] = CbBlock[k, w];
                            CrSub[k + i, w + j] = CrBlock[k, w];
                        }
                }
            return Tuple.Create(CbSub, CrSub);
        }
        
        public Tuple<float[,], float[,]> get420SubsamplingBlock(float[,] Cb, float[,] Cr, int k, int w, int  paddingType)
        {
            //k = indice di riga da cui parte il blocco, w=indice di colonna
            //type = { 0 : padding di 0 su blocchi adiacenti; 1 : padding con copia del blocco compresso sui blocchi adiacenti }
            float[,] CbSub = new float[16, 16];
            float[,] CrSub = new float[16, 16];
            if (paddingType == JPEGUtility.ZERO_BLOCK_PADDING)
            {
                //padding sui blocchi adiacenti basato sul valore di type
                for (int i = 0; i < 16; i++)
                    for (int j = 0; j < 16; j++)
                    {
                        CbSub[i, j] = 0;
                        CrSub[i, j] = 0;
                    }
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        CbSub[i, j] = (Cb[k + 2 * i, w + 2 * j] + Cb[k + 2 * i, w + 2 * j + 1] + Cb[k + 2 * i + 1, w + 2 * j] + Cb[k + 2 * i + 1, w + 2 * j + 1]) / 4;
                        CrSub[i, j] = (Cr[k + 2 * i, w + 2 * j] + Cr[k + 2 * i, w + 2 * j + 1] + Cr[k + 2 * i + 1, w + 2 * j] + Cr[k + 2 * i + 1, w + 2 * j + 1]) / 4;
                    }
                }
            }
            else if (paddingType == JPEGUtility.COPY_BLOCK_PADDING)
            {
                for (int i = 0; i < 8; i++)
                    for (int j = 0; j < 8; j++)
                    {
                        float CbVal = (Cb[k + 2 * i, w + 2 * j] + Cb[k + 2 * i, w + 2 * j + 1] + Cb[k + 2 * i + 1, w + 2 * j] + Cb[k + 2 * i + 1, w + 2 * j + 1]) / 4;
                        float CrVal = (Cr[k + 2 * i, w + 2 * j] + Cr[k + 2 * i, w + 2 * j + 1] + Cr[k + 2 * i + 1, w + 2 * j] + Cr[k + 2 * i + 1, w + 2 * j + 1]) / 4;
                        CbSub[i, j] = CbVal;
                        CbSub[i, j+8] = CbVal;
                        CbSub[i+8, j] = CbVal;
                        CbSub[i+8, j+8] = CbVal;
                        CrSub[i, j] = CrVal;
                        CrSub[i, j + 8] = CrVal;
                        CrSub[i + 8, j] = CrVal;
                        CrSub[i + 8, j + 8] = CrVal;
                    }
            }
            return Tuple.Create(CbSub, CrSub);
        }

        public Tuple<float[,], float[,]> get422SubsamplingBlock(float[,] Cb, float[,] Cr, int k, int w, int paddingType)
        {
            //k = indice di riga da cui parte il blocco, w=indice di colonna
            //type = { 0 : padding di 0 su blocchi adiacenti; 1 : padding con copia del blocco compresso sui blocchi adiacenti }
            float[,] CbSub = new float[16, 16];
            float[,] CrSub = new float[16, 16];
            if (paddingType == JPEGUtility.ZERO_BLOCK_PADDING)
            {
                //padding sui blocchi adiacenti basato sul valore di type
                for (int i = 0; i < 16; i++)
                    for (int j = 0; j < 16; j++)
                    {
                        CbSub[i, j] = 0;
                        CrSub[i, j] = 0;
                    }
                for (int i = 0; i < 16; i++)
                    for (int j = 0; j < 8; j++)
                    {
                        CbSub[i, j] = (Cb[k + i, w + 2 * j] + Cb[k + i, w + 2 * j + 1]) / 2;
                        CrSub[i, j] = (Cr[k + i, w + 2 * j] + Cr[k + i, w + 2 * j + 1]) / 2;
                    }
            }
            else if (paddingType == JPEGUtility.COPY_BLOCK_PADDING)
            {
                for (int i = 0; i < 16; i++)
                    for (int j = 0; j < 8; j++)
                    {
                        float CbVal = (Cb[k + i, w + 2 * j] + Cb[k + i, w + 2 * j + 1]) / 2;
                        float CrVal = (Cr[k + i, w + 2 * j] + Cr[k + i, w + 2 * j + 1]) / 2;
                        CbSub[i, j] = CbVal;
                        CbSub[i, j + 8] = CbVal;
                        CrSub[i, j] = CrVal;
                        CrSub[i, j + 8] = CrVal;
                    }
            }
            return Tuple.Create(CbSub, CrSub);
        }

        public Tuple<double[,], double[,], double[,]> getDCTMatrices(double[,] Y, double[,] Cb, double[,] Cr, int subsamplingType, int paddingType)
        {
            int rows = Y.GetLength(0);
            int columns = Y.GetLength(1);
            double[,] Ydct = new double[rows, columns];
            double[,] Cbdct = new double[rows, columns];
            double[,] Crdct = new double[rows, columns];
            if (subsamplingType == JPEGUtility.NO_SUBSAMPLING)
            {
                //DCT su tutti i blocchi YCC
                for (int i = 0; i < rows; i += 8)
                    for (int j = 0; j < columns; j += 8)
                    {
                        double[,] Yblock = copyBlock(Y, i, j);
                        double[,] Cbblock = copyBlock(Cb, i, j);
                        double[,] Crblock = copyBlock(Cr, i, j);
                        double[,] YblockResult = dct.DCT2D(Yblock);
                        double[,] CbblockResult = dct.DCT2D(Cbblock);
                        double[,] CrblockResult = dct.DCT2D(Crblock);
                        insertBlock(Ydct, YblockResult, i, j);
                        insertBlock(Cbdct, CbblockResult, i, j);
                        insertBlock(Crdct, CrblockResult, i, j);
                    }
            }
            else if(subsamplingType == JPEGUtility.SUBSAMPLING_422)
            {
                Boolean calcolaCbCr = true;
                for (int i = 0; i < rows; i += 8)
                    for (int j = 0; j < columns; j += 8)
                    {
                        double[,] Yblock = copyBlock(Y, i, j);
                        double[,] YblockResult = dct.DCT2D(Yblock);
                        insertBlock(Ydct, YblockResult, i, j);
                        if (calcolaCbCr)
                        {
                            double[,] Cbblock = copyBlock(Cb, i, j);
                            double[,] Crblock = copyBlock(Cr, i, j);
                            double[,] CbblockResult = dct.DCT2D(Cbblock);
                            double[,] CrblockResult = dct.DCT2D(Crblock);
                            insertBlock(Cbdct, CbblockResult, i, j);
                            insertBlock(Crdct, CbblockResult, i, j);
                            calcolaCbCr = false;
                        }
                        else
                        {
                            if (paddingType == JPEGUtility.ZERO_BLOCK_PADDING)
                            {
                                zeroBlock(Cbdct, i, j);
                                zeroBlock(Crdct, i, j);
                            }
                            else if (paddingType == JPEGUtility.COPY_BLOCK_PADDING)
                            {
                                insertBlock(Cbdct, copyBlock(Cbdct, i, j-8), i, j);
                                insertBlock(Crdct, copyBlock(Crdct, i, j-8), i, j);
                            }
                            calcolaCbCr = true;
                        }
                    }
            }
            else if (subsamplingType == JPEGUtility.SUBSAMPLING_420)
            {
                Boolean calcolaRowCbCr = true, calcolaColCbCr = true;
                for (int i = 0; i < rows; i += 8)
                {
                    for (int j = 0; j < columns; j += 8)
                    {
                        double[,] Yblock = copyBlock(Y, i, j);
                        double[,] YblockResult = dct.DCT2D(Yblock);
                        insertBlock(Ydct, YblockResult, i, j);
                        if (calcolaRowCbCr && calcolaColCbCr)
                        {
                            double[,] Cbblock = copyBlock(Cb, i, j);
                            double[,] Crblock = copyBlock(Cr, i, j);
                            double[,] CbblockResult = dct.DCT2D(Cbblock);
                            double[,] CrblockResult = dct.DCT2D(Crblock);
                            insertBlock(Cbdct, CbblockResult, i, j);
                            insertBlock(Crdct, CbblockResult, i, j);
                            calcolaColCbCr = false;
                        }
                        else
                        {
                            if (paddingType == JPEGUtility.ZERO_BLOCK_PADDING)
                            {
                                zeroBlock(Cbdct, i, j);
                                zeroBlock(Crdct, i, j);
                            }
                            else if (paddingType == JPEGUtility.COPY_BLOCK_PADDING)
                            {
                                if (calcolaRowCbCr)
                                {
                                    insertBlock(Cbdct, copyBlock(Cbdct, i, j - 8), i, j);
                                    insertBlock(Crdct, copyBlock(Crdct, i, j - 8), i, j);
                                }
                                else
                                {
                                    insertBlock(Cbdct, copyBlock(Cbdct, i - 8, j), i, j);
                                    insertBlock(Crdct, copyBlock(Crdct, i - 8, j), i, j);
                                }
                            }
                            calcolaColCbCr = true;
                        }
                    }
                    if(calcolaRowCbCr)
                    {
                        calcolaRowCbCr = false;
                    }
                    else
                    {
                        calcolaRowCbCr = true;
                    }
                    //controllare che a fine riga calcolaColCbCr è sempre true
                }
            }
            return Tuple.Create(Ydct, Cbdct, Crdct);
        }

        private double[,] copyBlock(double[,] M, int k, int w)
        {
            double[,] copyBlock = new double[8, 8];
            for (int i=0; i<8; i++)
                for(int j=0; j<8; j++)
                {
                    copyBlock[i, j] = M[i + k, j + w];
                }
            return copyBlock;
        }

        private void insertBlock(double[,] result, double[,] block, int k, int w)
        {
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                {
                    result[i + k, j + w] = block[i, j];
                }
        }

        private void zeroBlock(double[,] result, int k, int w)
        {
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                {
                    result[i + k, j + w] = 0;
                }
        }

        public Tuple<double[,], double[,], double[,]> getQuantizedMatrices(double[,] Ydct, double[,] Cbdct, double[,] Crdct)
        {
            int rows = Ydct.GetLength(0);
            int columns = Ydct.GetLength(1);
            for (int i=0; i<rows; i+=8) 
                for (int j=0; j<columns; j+=8)
                {
                    blockQuantization(Ydct,Cbdct, Crdct, i,j);
                }
            return Tuple.Create(Ydct, Cbdct, Crdct);
        }

        public Tuple<int[,], int[,], int[,]> getRoundToIntMatrices(double[,] YQ, double[,] CbQ, double[,] CrQ)
        {
            int rows = YQ.GetLength(0);
            int columns = YQ.GetLength(1);
            int[,] YQRound = new int[rows, columns];
            int[,] CbQRound = new int[rows, columns];
            int[,] CrQRound = new int[rows, columns];
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < columns; j++)
                {
                    YQRound[i, j] = Convert.ToInt32(Math.Round(YQ[i, j]));
                    CbQRound[i, j] = Convert.ToInt32(Math.Round(CbQ[i, j]));
                    CrQRound[i, j] = Convert.ToInt32(Math.Round(CrQ[i, j]));
                }
            return Tuple.Create(YQRound, CbQRound, CrQRound);
        }
        
        public Tuple<ArrayList, ArrayList, ArrayList> getACEncoding(int[,] YMatrixQ, int[,] CbMatrixQ, int[,] CrMatrixQ)
        {
            int rows = YMatrixQ.GetLength(0);
            int columns = YMatrixQ.GetLength(1);
            ArrayList YACEncoding = new ArrayList();
            ArrayList CbACEncoding = new ArrayList();
            ArrayList CrACEncoding = new ArrayList();
            for (int i = 0; i < rows; i += 8)
                for (int j = 0; j < columns; j += 8)
                {
                    /*
                    Console.WriteLine("BLOCK");
                    printBlock(YMatrixQ, i, j);
                    Console.WriteLine("*******************");
                    */
                    YACEncoding.Add(getACFromBlock(YMatrixQ, i, j));
                    CbACEncoding.Add(getACFromBlock(CbMatrixQ, i, j));
                    CrACEncoding.Add(getACFromBlock(CrMatrixQ, i, j));
                }
            return Tuple.Create(YACEncoding, CbACEncoding, CrACEncoding);
        }

        public Tuple<int[], int[], int[]> getDCEncoding(int[,] YMatrixQ, int[,] CbMatrixQ, int[,] CrMatrixQ)
        {
            int rows = YMatrixQ.GetLength(0);
            int columns = YMatrixQ.GetLength(1);
            int blockNumber = (rows / 8) * (columns / 8);
            int[] YDCEnc = new int[blockNumber];
            int[] CbDCEnc = new int[blockNumber];
            int[] CrDCEnc = new int[blockNumber];
            int precYDC = YMatrixQ[0, 0];
            int precCbDC = CbMatrixQ[0, 0];
            int precCrDC = CrMatrixQ[0, 0];
            YDCEnc[0] = precYDC;
            CbDCEnc[0] = precCbDC;
            CrDCEnc[0] = precCrDC;
            int cntBlock = 1;
            for (int i=0; i<rows; i+=8)
                for (int j=0; j<columns; j+=8)
                {
                    if (i == 0 && j == 0)
                        continue;
                    YDCEnc[cntBlock] = (YMatrixQ[i, j] - precYDC);
                    precYDC = YMatrixQ[i, j];
                    CbDCEnc[cntBlock] = (CbMatrixQ[i, j] - precCbDC);
                    precCbDC = CbMatrixQ[i, j];
                    CrDCEnc[cntBlock] = (CrMatrixQ[i, j] - precCrDC);
                    precCrDC = CrMatrixQ[i, j];
                    cntBlock++;
                }
            return Tuple.Create(YDCEnc, CbDCEnc, CrDCEnc);
        }

        private void blockQuantization(double[,] Ydct,  double[,] Cbdct, double[,] Crdct, int k, int w)
        {
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                {
                    Ydct[i + k, j + w] /= JPEGUtility.QuantizationYMatrix[i, j];
                    Cbdct[i + k, j + w] /= JPEGUtility.QuantizationCMatrix[i, j];
                    Crdct[i + k, j + w] /= JPEGUtility.QuantizationCMatrix[i, j];
                }
        }
        /*
        private int[] getACFromBlock(int[,] Matrix, int k, int w)
        {
            /*
            si inseriscono nell'array AC valori diversi da zero; quando si incontra uno zero, si inserisce zero con
            di seguito la count degli zero consecutivi; All'uscita dal blocco si inserisce 00
            ( es. [ 2, 3, 0, 4 -> 4 zeri consecutivi, 5, 7, ..., 0, 0 -> fine blocco]. )
            
            LinkedList<int> encoding = new LinkedList<int>();
            int dimArray = ZigZagX.GetLength(0);
            Boolean counting = false;
            int numZeroCounter = 0;
            for (int i=0; i<dimArray; i++)
            {
                int current = Matrix[ZigZagX[i] + k, ZigZagY[i] + w];
                if (current != 0 && !counting)
                {// valore diverso da zero, fuori da una sequenza di zeri
                    encoding.AddLast(current);
                }
                else if (current == 0 && !counting)
                {
                    counting = true;
                    numZeroCounter++;
                }
                else if (current == 0 && counting)
                {
                    numZeroCounter++;
                }
                else if (current != 0 && counting)
                {
                    encoding.AddLast(0);
                    encoding.AddLast(numZeroCounter);
                    encoding.AddLast(current);
                    numZeroCounter = 0;
                    counting = false;
                }
                if (i+1 == dimArray)
                {
                    encoding.AddLast(0);
                    encoding.AddLast(0);
                }
            }
            int[] result = encoding.ToArray();
            //DEBUG
            Console.WriteLine("RESULT");
            for (int i = 0; i < encoding.Count; i++)
                Console.Write(result[i] + " ");
            Console.WriteLine();
            Console.WriteLine("**************************************");
            return result;
        }
        */

        private int[] getACFromBlock(int[,] Matrix, int k, int w)
        {
            LinkedList<int> encoding = new LinkedList<int>();
            int dimArray = JPEGUtility.ZigZagX.GetLength(0);
            Boolean counting = false;
            int numZeroCounter = 0;
            for (int i=0; i<dimArray; i++)
            {
                int current = Matrix[JPEGUtility.ZigZagX[i] + k, JPEGUtility.ZigZagY[i] + w];
                if (current != 0 && !counting)
                {// valore diverso da zero, fuori da una sequenza di zeri
                    encoding.AddLast(0);
                    encoding.AddLast(current);
                }
                else if (current == 0 && !counting)
                {
                    counting = true;
                    numZeroCounter++;
                }
                else if (current == 0 && counting)
                {
                    numZeroCounter++;
                }
                else if (current != 0 && counting)
                {
                    encoding.AddLast(numZeroCounter);
                    encoding.AddLast(current);
                    numZeroCounter = 0;
                    counting = false;
                }
                if (i+1 == dimArray)
                {
                    encoding.AddLast(0);
                    encoding.AddLast(0);
                }
            }
            int[] result = encoding.ToArray();
            //DEBUG
            Console.WriteLine("RESULT");
            for (int i = 0; i < encoding.Count; i++)
                Console.Write(result[i] + " ");
            Console.WriteLine();
            Console.WriteLine("**************************************");
            return result;
        }
        
        /*
        public Tuple<float[,], float[,]> get420SubsamplingBlock(float[,] Cb, float[,] Cr, int k, int w)
        {
            //k = indice di riga da cui parte il blocco, w=indice di colonna
            float[,] CbSub = new float[8, 8];
            float[,] CrSub = new float[8, 8];
            for (int i = 0; i<8; i++) 
                for(int j = 0; j<8; j++)
                {
                    CbSub[i,j] = (Cb[k+2*i,w+2*j] + Cb[k+2*i, w+2*j+1] + Cb[k+2*i+1,w+2*j] + Cb[k+2*i+1,w+2*j+1]) / 4;
                    CrSub[i,j] = (Cr[k+2*i,w+2*j] + Cr[k+2*i, w+2*j+1] + Cr[k+2*i+1,w+2*j] + Cr[k+2*i+1,w+2*j+1]) / 4;
                }
            return Tuple.Create(CbSub,CrSub);
        }
        */

        private YCbCr RGBToYCbCr(RGB rgb)
        {
            float fr = (float)rgb.R;
            float fg = (float)rgb.G;
            float fb = (float)rgb.B;

            float Y = (float)(0.299 * fr + 0.587 * fg + 0.114 * fb);
            float Cb = (float)(-0.1687 * fr - 0.3313 * fg + 0.5 * fb) + 128;
            float Cr = (float)(0.5 * fr - 0.4187 * fg - 0.0813 * fb) + 128; 

            return new YCbCr(Y, Cb, Cr);
        }
        
        private RGB YCbCrToRGB(YCbCr ycbcr)
        {
            float r = Math.Max(0.0f, Math.Min(1.0f, (float)(ycbcr.Y + 0.0000 * (ycbcr.Cb - 128) + 1.402 * (ycbcr.Cr - 128))));
            float g = Math.Max(0.0f, Math.Min(1.0f, (float)(ycbcr.Y - 0.34414 * (ycbcr.Cb - 128) - 0.71414 * (ycbcr.Cr - 128))));
            float b = Math.Max(0.0f, Math.Min(1.0f, (float)(ycbcr.Y + 1.772 * (ycbcr.Cb - 128) + 0.0000 * (ycbcr.Cr - 128))));

            return new RGB((byte)(r * 255), (byte)(g * 255), (byte)(b * 255));
        }

        public void printMatriciRGB(byte[,] RMatrix, byte[,] GMatrix, byte[,] BMatrix, int width, int height)
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

        private void debugBlockPrint(double x)
        {
            Console.Write(x.ToString("0.0") + " ");
        }

    }//RGBEncoder


    public struct RGB
        {
            private byte _r;
            private byte _g;
            private byte _b;

            public RGB(byte r, byte g, byte b)
            {
                this._r = r;
                this._g = g;
                this._b = b;
            }
            public byte R
            {
                get { return this._r; }
                set { this._r = value; }
            }
            public byte G
            {
                get { return this._g; }
                set { this._g = value; }
            }
            public byte B
            {
                get { return this._b; }
                set { this._b = value; }
            }
            public bool Equals(RGB rgb)
            {
                return (this.R == rgb.R) && (this.G == rgb.G) && (this.B == rgb.B);
            }
        }

        public struct YCbCr
        {
            private float _y;
            private float _cb;
            private float _cr;

            public YCbCr(float y, float cb, float cr)
            {
                this._y = y;
                this._cb = cb;
                this._cr = cr;
            }

            public float Y
            {
                get { return this._y; }
                set { this._y = value; }
            }

            public float Cb
            {
                get { return this._cb; }
                set { this._cb = value; }
            }

            public float Cr
            {
                get { return this._cr; }
                set { this._cr = value; }
            }

            public bool Equals(YCbCr ycbcr)
            {
                return (this.Y == ycbcr.Y) && (this.Cb == ycbcr.Cb) && (this.Cr == ycbcr.Cr);
            }
        }
}
