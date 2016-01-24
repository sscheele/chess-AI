using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chess
{
    public class BitboardLayer
    {
        //by adding commonly used data fields to the ulong, this class is intended to 
        //sacrifice negligible memory for more significant gains in time while maintaining
        //many of the advantages of using the bitboard
        ulong layerData;
        List<int> trueIndicies;
        int numOnes;

        public BitboardLayer()
        {
            layerData = 0uL;
            trueIndicies = new List<int>();
            numOnes = 0;
        }

        public BitboardLayer(ulong layerData)
        {
            this.layerData = layerData;
            initializeMetadata();
        }

        public BitboardLayer(BitboardLayer b)
        {
            layerData = b.getLayerData();
            trueIndicies = new List<int>();
            foreach (int i in b.getTrueIndicies()) { trueIndicies.Add(i); }
            numOnes = b.getNumOnes();
        }

        void initializeMetadata()
        {
            trueIndicies = new List<int>();
            numOnes = 0;
            for (int i = 0; i < 64; i++)
            {
                if (trueAtIndex(i))
                {
                    trueIndicies.Add(i);
                    numOnes++;
                }
            }
        }

        public ulong setAtIndex(int index, bool isTrue)
        {
            if (isTrue)
            {
                layerData |= (ulong)(1uL << (63 - index));
                if (!trueIndicies.Contains(index))
                {
                    trueIndicies.Add(index);
                    numOnes++;
                }
            }
            else
            {
                layerData &= ~((ulong)(1uL << 63 - index));
                if (trueIndicies.Contains(index))
                {
                    trueIndicies.Remove(index);
                    numOnes--;
                }
            }
            return layerData;
        }

        public bool trueAtIndex(int i) { //easier to think of the other way
            //I would do this with trueIndicies, but I think this is actually slightly faster (consumes only a few clock cycles compared to a relatively inefficient O(n) search)
            return (layerData & (ulong)(1uL << (63 - i))) > 0;
                //invert normal digit order (ie, index of 0 gives LBS of 63, which is the leftmost bit
        }

        public ulong getLayerData() { return layerData; }

        public int[] getTrueIndicies() { return trueIndicies.ToArray(); }

        public int getNumOnes() { return numOnes; }

        public void setLayerData(ulong d) //not really necessary, but prettier in code than calling the constructor all over again
        {
            layerData = d;
            initializeMetadata();
        }
    }
}
