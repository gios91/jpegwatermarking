using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPEGEncoding
{
    class JPEGDebugger
    {
        public Boolean Subsampling420Debugger(float[,] CbSub, float[,] CrSub, float[,] Cb, float[,] Cr, int subsamplingType)
        {
            //SI ASSUME PER ORA CHE LE MATRICI YCC ABBIANO DIMENSIONE MULTIPLA DI 16 px
            int rows = Cb.GetLength(0);
            int columns = Cb.GetLength(1);
            for (int i = 0; i < rows; i += 16)
                for (int j = 0; j < columns; j += 16)
                {
                    Boolean rightBlock = check420SubsamplingBlock(Cb, Cr, CbSub, CrSub, i, j, subsamplingType);
                    if (!rightBlock)
                        return false;
                }
            return true;
        }

        private Boolean check420SubsamplingBlock(float[,] Cb, float[,] Cr, float[,] CbSub, float[,] CrSub, int k, int w, int type)
        {
            //k = indice di riga da cui parte il blocco, w=indice di colonna
            //type = { 0 : padding di 0 su blocchi adiacenti; 1 : padding con copia del blocco compresso sui blocchi adiacenti }
            if (type == 0)
            {
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        float CbVal = (Cb[k + 2 * i, w + 2 * j] + Cb[k + 2 * i, w + 2 * j + 1] + Cb[k + 2 * i + 1, w + 2 * j] + Cb[k + 2 * i + 1, w + 2 * j + 1]) / 4;
                        if (CbSub[k + i , w + j] != CbVal)
                            return false;
                        float CrVal = (Cr[k + 2 * i, w + 2 * j] + Cr[k + 2 * i, w + 2 * j + 1] + Cr[k + 2 * i + 1, w + 2 * j] + Cr[k + 2 * i + 1, w + 2 * j + 1]) / 4;
                        if (CrSub[k + i, w + j] != CrVal)
                            return false; 
                    }
                }
            }
            else if (type == 1)
            {
                for (int i = 0; i < 8; i++)
                    for (int j = 0; j < 8; j++)
                    {
                        float CbVal = (Cb[k + 2 * i, w + 2 * j] + Cb[k + 2 * i, w + 2 * j + 1] + Cb[k + 2 * i + 1, w + 2 * j] + Cb[k + 2 * i + 1, w + 2 * j + 1]) / 4;
                        float CrVal = (Cr[k + 2 * i, w + 2 * j] + Cr[k + 2 * i, w + 2 * j + 1] + Cr[k + 2 * i + 1, w + 2 * j] + Cr[k + 2 * i + 1, w + 2 * j + 1]) / 4;
                        if (CbSub[k + i, w + j] != CbVal)
                            return false;
                        if (CbSub[k + i, w + j + 8] != CbVal)
                            return false;
                        if (CbSub[k + i + 8, w + j] != CbVal)
                            return false;
                        if (CbSub[k + i + 8, w + j + 8] != CbVal)
                            return false;
                        if (CrSub[k + i, w + j] != CrVal)
                            return false;
                        if (CrSub[k + i, w + j + 8] != CrVal)
                            return false;
                        if (CrSub[k + i + 8, w + j] != CrVal)
                            return false;
                        if (CrSub[k + i + 8, w + j + 8] != CrVal)
                            return false;
                    }
            }
            return true;
        }

        public Boolean Subsampling422Debugger(float[,] CbSub, float[,] CrSub, float[,] Cb, float[,] Cr, int subsamplingType)
        {
            //SI ASSUME PER ORA CHE LE MATRICI YCC ABBIANO DIMENSIONE MULTIPLA DI 16 px
            int rows = Cb.GetLength(0);
            int columns = Cb.GetLength(1);
            for (int i = 0; i < rows; i += 16)
                for (int j = 0; j < columns; j += 16)
                {
                    Boolean rightBlock = check422SubsamplingBlock(Cb, Cr, CbSub, CrSub, i, j, subsamplingType);
                    if (!rightBlock)
                        return false;
                }
            return true;
        }

        private Boolean check422SubsamplingBlock(float[,] Cb, float[,] Cr, float[,] CbSub, float[,] CrSub, int k, int w, int type)
        {
            //k = indice di riga da cui parte il blocco, w=indice di colonna
            //type = { 0 : padding di 0 su blocchi adiacenti; 1 : padding con copia del blocco compresso sui blocchi adiacenti }
            if (type == 0)
            {
                for (int i = 0; i < 16; i++)
                    for (int j = 0; j < 8; j++)
                    {
                        float CbVal = (Cb[k + i, w + 2 * j] + Cb[k + i, w + 2 * j + 1]) / 2; 
                        float CrVal = (Cr[k + i, w + 2 * j] + Cr[k + i, w + 2 * j + 1]) / 2;
                        if (CbSub[k + i, w + j] != CbVal)
                            return false;
                        if (CrSub[k + i, w + j] != CrVal)
                            return false;
                    }
            }
            else if (type == 1)
            {
                for (int i = 0; i < 16; i++)
                    for (int j = 0; j < 8; j++)
                    {
                        float CbVal = (Cb[k + i, w + 2 * j] + Cb[k + i, w + 2 * j + 1]) / 2;
                        float CrVal = (Cr[k + i, w + 2 * j] + Cr[k + i, w + 2 * j + 1]) / 2;
                        if (CbSub[k + i, w + j] != CbVal)
                            return false;
                        if (CbSub[k + i, w + j + 8] != CbVal)
                            return false;
                        if (CrSub[k + i, w + j] != CrVal)
                            return false;
                        if (CrSub[k + i, w + j + 8] != CrVal)
                            return false;
                    }
            }
            return true;
        }
    }
}
