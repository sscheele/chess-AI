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
            BitboardLayer[] white = c.getDict(true);
            BitboardLayer[] black = c.getDict(false);
            for (int i = 0; i < 64; i++)
            {
                frm.setOverlay(i, 15);
            }
            for (int s = 0; s <= pieceIndex.KING; s++)
            {
                BitboardLayer blackArr = black[s];
                BitboardLayer whiteArr = white[s];
                foreach (int i in whiteArr.getTrueIndicies())
                {
                    frm.setOverlay(i, s);
                }
                foreach (int i in blackArr.getTrueIndicies())
                {
                    frm.setOverlay(i, s + 6);
                }
            }
        }

        public ChessBoard getBoard() { return c; }

    }
}
