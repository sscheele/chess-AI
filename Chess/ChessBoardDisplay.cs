using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chess
{
    public class ChessBoardDisplay
    {
        MainForm frm;
        ChessBoard c;

        public ChessBoardDisplay(MainForm f)
        {
            frm = f;
            c = new ChessBoard(null, null);
        }

        public void genOverlay()
        {
            ulong[] white = c.getDict(true);
            ulong[] black = c.getDict(false);
            for (int i = 0; i < 64; i++)
            {
                frm.setOverlay(i, 15);
            }
            for (int s = 0; s <= pieceIndex.KING; s++)
            {
                ulong blackArr = black[s];
                ulong whiteArr = white[s];
                for (int i = 0; i < 64; i++)
                {
                    if (trueAtIndex(whiteArr, i)) frm.setOverlay(i, s);
                    if (trueAtIndex(blackArr, i)) frm.setOverlay(i, s + 6);
                }
            }
        }

        public ChessBoard getBoard() { return c; }

        public static bool trueAtIndex(ulong t, int i)
        { //easier to think of the other way
            return (t & (ulong)(1uL << (63 - i))) > 0;
            //invert normal digit order (ie, index of 0 gives LBS of 63, which is the leftmost bit
        }

    }
}
