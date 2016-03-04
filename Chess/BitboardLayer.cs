using System.Collections.Generic;

namespace Chess
{
    public class BitboardLayer
    {
        //by adding commonly used data fields to the ulong, this class is intended to 
        //sacrifice negligible memory for more significant gains in time while maintaining
        //many of the advantages of using the bitboard
        ulong layerData;
        HashSet<int> trueIndicies;

        public BitboardLayer()
        {
            layerData = 0uL;
            trueIndicies = new HashSet<int>();
        }

        public BitboardLayer(ulong layerData)
        {
            this.layerData = layerData;
            initializeMetadata();
        }

        public BitboardLayer(BitboardLayer b)
        {
            layerData = b.getLayerData();
            trueIndicies = new HashSet<int>();
            foreach (int i in b.getTrueIndicies()) { trueIndicies.Add(i); }
        }

        void initializeMetadata()
        {
            trueIndicies = new HashSet<int>();
            for (int i = 0; i < 64; i++)
            {
                if (trueAtIndex(i)) trueIndicies.Add(i);
            }
        }

        public ulong setAtIndex(int index, bool isTrue)
        {
            if (isTrue)
            {
                layerData |= (ulong)(1uL << (63 - index));
                trueIndicies.Add(index);
            }
            else
            {
                layerData &= ~((ulong)(1uL << 63 - index));
                /*if (trueIndicies.Contains(index))*/ trueIndicies.Remove(index);
            }
            return layerData;
        }

        public bool trueAtIndex(int i) { //less clear, but needed for initializeMetadata
            return ((1uL << (63 - i)) & layerData) > 0;
        }

        public ulong getLayerData() { return layerData; }

        public int[] getTrueIndicies() {
            int[] retVal = new int[trueIndicies.Count];
            trueIndicies.CopyTo(retVal);
            return retVal;
        }

        public int getNumOnes() { return trueIndicies.Count; }

        public void setLayerData(ulong d) //not really necessary, but prettier in code than calling the constructor all over again
        {
            layerData = d;
            initializeMetadata();
        }
    }
}
