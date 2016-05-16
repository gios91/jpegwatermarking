using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPEGWatermarking
{
    public class ChannelError : ChannerErrorIF
    {
        private int numBitSingleError = -1;

        public BitArray singleError(BitArray stream, double alpha)
        {
            ArrayList posError = new ArrayList();   //contiene le posizioni correntemente alterate dal canale
            BitArray res = new BitArray(stream.Length);
            for (int k = 0; k < stream.Length; k++)
                res[k] = stream[k];
            //in base al valore di alpha, altera i bit all'interno di stream
            int numError = Convert.ToInt32(alpha * stream.Length);
            for (int i=0; i<numError; i++)
            {
                int randomPos = new Random(Guid.NewGuid().GetHashCode()).Next(0, (stream.Length - 1));
                while (isContained(randomPos, posError))
                {
                    randomPos = new Random(Guid.NewGuid().GetHashCode()).Next(0, (stream.Length - 1));
                }
                if (res[randomPos]) res[randomPos] = false;
                else res[randomPos] = true;
                posError.Add(randomPos);
            }
            numBitSingleError = posError.Count;
            return res;
        }

        public int getNumBitSingleError()
        {
            return numBitSingleError;
        }

        private bool isContained(int randomPos, ArrayList posError)
        {
            for (int i = 0; i < posError.Count; i++)
                if ((int)posError[i] == randomPos)
                    return true;
            return false;
        }

        public Tuple<BitArray,BitArray> gilberElliotBurstError(BitArray stream, double p, double r)
        {
            BitArray noiseVector = new BitArray(stream.Length);
            getGilbertBurstNoiseVector(ref noiseVector, stream.Length, p, r);
            BitArray res = new BitArray(stream.Length);
            //ogni bit 1 del noise vector porta un'alterazione nel msg stream (bit flip del relativo bit di stream)
            for (int i=0; i<res.Length; i++)
            {
                if (noiseVector[i])
                {   //vi è un errore in noise vector, quindi flip bit di stream
                    if (stream[i]) res[i] = false;
                    else  res[i] = true;
                }
                else
                {
                    res[i] = stream[i];
                }
            }
            return Tuple.Create(res,noiseVector);
        }


        public BitArray getGilbertBurstNoiseVector(ref BitArray noiseVector, int noiseVectorLen, double p, double r)
        {
            /*     Matrice di Gilber Elliot (catena di Markov)
             *           G                B
             *      _______________________________
             *  G  |  P(G|G) = 1-p   P(B|G) = p    |
             *     |                               |
             *  B  |  P(G|B) = r     P(B|B) = 1-r  |
             *     |_______________________________|
             *
             */

            // lo stato corrente state può essere G (good = true) o B (bad = false); ad ogni passo k, 
            // se lo stato corrente è G => noise[k] = 0, se invece stato corrente è B => noise[k] = 1
            bool state = false;
            //si definiscono i valori di probabilità
            
            //noiseVectorLen = 1000;

            for (int k=0; k < noiseVectorLen; k++)
            {
                if (!state)
                {   //se sono nello stato G, cerco l'evento P(G|G) o P(B|G)
                    double probEvent = new Random(Guid.NewGuid().GetHashCode()).NextDouble();
                    if (probEvent <= p)
                    {
                        //transizione di stato G -> B
                        state = true;
                        noiseVector[k] = true;
                    }
                    else
                    {
                        //retroazione nello stato: G -> G
                        noiseVector[k] = false;
                    }
                }
                else
                {   //se sono nello stato B, cerco l'evento P(B|B) o P(G|B)
                    double probEvent = new Random(Guid.NewGuid().GetHashCode()).NextDouble();
                    if (probEvent <= r)
                    {
                        //transizione di stato B -> G
                        state = false;
                        noiseVector[k] = false;
                    }
                    else
                    {
                        //retroazione nello stato: B -> B
                        noiseVector[k] = true;
                    }
                }
                Console.Write(state ? "1" : "0");
            }
            //printArray(noiseVector, "NOISE FINE METODO");
            return noiseVector;
        }

        /* TEST MAIN
        public static void Main(string[] args)
        {
            ChannelEncoderIF enc = new ChannelEncoder();
            ChannerErrorIF err = new ChannelError();
            ChannelDecoderIF dec = new ChannelDecoder();

            byte[] test = { 1,0,0,0,0,0,0,0,0 };
            printByteArray(test, "+ array originario");
            //BitArray testArray = new BitArray(test);
            int R = 50;
            BitArray coded = enc.RipetizioneEncoding(test, R);
            double p = 0.3;
            double r = 0.7;
            BitArray errorArray = err.gilberElliotBurstError(coded, p, r);
            int numError = getNumError(coded, errorArray);
            BitArray decoded = dec.RipetizioneDecoding(errorArray, R);
            printArray(coded, "codificato");
            printArray(errorArray, "decodificato");
            printByteArray(bitArrayToByteArray(decoded), "+ array decodificato");
        }
        */

        private static byte[] bitArrayToByteArray(BitArray decodedStream)
        {
            byte[] decArray = new byte[decodedStream.Length / 8];
            decodedStream.CopyTo(decArray, 0);
            return decArray;
        }

        private static int getNumError(BitArray v1, BitArray v2)
        {
            int cnt = 0;
            for (int i = 0; i < v1.Length; i++)
                if (v1[i] != v2[i]) cnt++;
            return cnt;
        }

        private static void printByteArray(byte[] v, string s)
        {
            Console.WriteLine("> Stampa array = {0}", s);
            Console.Write("[");
            for (int i = 0; i < v.Length; i++)
                Console.Write(v[i]+" ");
            Console.WriteLine("]");
        }

        private static void printArray(BitArray v, string info)
        {
            Console.WriteLine("> Stampa array = {0}", info);
            Console.Write("[");
            for (int i = 0; i < v.Length; i++)
                Console.Write(v[i]?"1":"0");
            Console.WriteLine("]");
        }

        private static bool equalArray(BitArray v1, BitArray v2)
        {
            for (int i = 0; i < v1.Length; i++)
                if (v1[i] != v2[i]) return false;
            return true;
        }
    }
}
