using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chess
{
    class Rating
    {
        public static int[] pieceVals = new int[] { 100, 500, 300, 300, 900, 20000 };
        //attribute to http://chessprogramming.wikispaces.com/Simplified+evaluation+function
        static int[][] positionValues = new int[][]{
            new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 50, 50, 50, 50, 50, 50, 50, 50, 10, 10, 20, 30, 30, 20, 10, 10, 5, 5, 10, 25, 25, 10, 5, 5, 0, 0, 0, 20, 20, 0, 0, 0, 5, -5, -10, 0, 0, -10, -5, 5, 5, 10, 10, -20, -20, 10, 10, 5, 0, 0, 0, 0, 0, 0, 0, 0 },
            new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 5, 10, 10, 10, 10, 10, 10, 5, -5, 0, 0, 0, 0, 0, 0, -5, -5, 0, 0, 0, 0, 0, 0, -5, -5, 0, 0, 0, 0, 0, 0, -5, -5, 0, 0, 0, 0, 0, 0, -5, -5, 0, 0, 0, 0, 0, 0, -5, 0, 0, 0, 5, 5, 0, 0, 0 },
            new int[] { -50, -40, -30, -30, -30, -30, -40, -50, -40, -20, 0, 0, 0, 0, -20, -40, -30, 0, 10, 15, 15, 10, 0, -30, -30, 5, 15, 20, 20, 15, 5, -30, -30, 0, 15, 20, 20, 15, 0, -30, -30, 5, 10, 15, 15, 10, 5, -30, -40, -20, 0, 5, 5, 0, -20, -40, -50, -40, -30, -30, -30, -30, -40, -50 },
            new int[] { -20, -10, -10, -10, -10, -10, -10, -20, -10, 0, 0, 0, 0, 0, 0, -10, -10, 0, 5, 10, 10, 5, 0, -10, -10, 5, 5, 10, 10, 5, 5, -10, -10, 0, 10, 10, 10, 10, 0, -10, -10, 10, 10, 10, 10, 10, 10, -10, -10, 5, 0, 0, 0, 0, 5, -10, -20, -10, -10, -10, -10, -10, -10, -20 },
            new int[] { -20, -10, -10, -5, -5, -10, -10, -20, -10, 0, 0, 0, 0, 0, 0, -10, -10, 0, 5, 5, 5, 5, 0, -10, -5, 0, 5, 5, 5, 5, 0, -5, 0, 0, 5, 5, 5, 5, 0, -5, -10, 5, 5, 5, 5, 5, 0, -10, -10, 0, 5, 0, 0, 0, 0, -10, -20, -10, -10, -5, -5, -10, -10, -20 },
            new int[] { -30, -40, -40, -50, -50, -40, -40, -30, -30, -40, -40, -50, -50, -40, -40, -30, -30, -40, -40, -50, -50, -40, -40, -30, -30, -40, -40, -50, -50, -40, -40, -30, -20, -30, -30, -40, -40, -30, -30, -20, -10, -20, -20, -20, -20, -20, -20, -10, 20, 20, 0, 0, 0, 0, 20, 20, 20, 30, 10, 0, 0, 10, 30, 20 },
            new int[] { -50, -40, -30, -20, -20, -30, -40, -50, -30, -20, -10, 0, 0, -10, -20, -30, -30, -10, 20, 30, 30, 20, -10, -30, -30, -10, 30, 40, 40, 30, -10, -30, -30, -10, 30, 40, 40, 30, -10, -30, -30, -10, 20, 30, 30, 20, -10, -30, -30, -30, 0, 0, 0, 0, -30, -30, -50, -30, -30, -30, -30, -30, -30, -50 }
        };

        public static int rating(bool isWhite, ChessBoard c, int possibleMoves, int depth)
        {
            int counter = 0;
            int material = rateMaterial(isWhite, c);
            counter += rateAttack(isWhite, c);
            counter += material;
            counter += rateMoveability(isWhite, c, possibleMoves, depth, material);
            counter += ratePositional(isWhite, c, material);

            isWhite = !isWhite;

            material = rateMaterial(isWhite, c);
            counter -= rateAttack(isWhite, c);
            counter -= material;
            counter -= rateMoveability(isWhite, c, possibleMoves, depth, material);
            counter -= ratePositional(isWhite, c, material);
            return -(counter+depth*50);
        }

        public static int rateAttack(bool isWhite, ChessBoard c)
        {
            BitboardLayer[] dict = c.getDict(isWhite);
            int counter = 0;
            for (int j = 0; j <= pieceIndex.KING; j++) //don't include king
            {
                foreach (int i in dict[j].getTrueIndicies())
                {
                    switch (j)
                    {
                        case pieceIndex.PAWN:
                            if (isUnderAttack(isWhite, c, i)) counter -= 64;
                            break;
                        case pieceIndex.ROOK:
                            if (isUnderAttack(isWhite, c, i)) counter -= 500;
                            break;
                        case pieceIndex.KNIGHT:
                            if (isUnderAttack(isWhite, c, i)) counter -= 300;
                            break;
                        case pieceIndex.BISHOP:
                            if (isUnderAttack(isWhite, c, i)) counter -= 300;
                            break;
                        case pieceIndex.QUEEN:
                            if (isUnderAttack(isWhite, c, i)) counter -= 900;
                            break;
                        case pieceIndex.KING:
                            if (isUnderAttack(isWhite, c, i)) counter -= 200;
                            break;
                    }
                }
            }
            return counter/2;
        }

        public static bool isUnderAttack(bool isWhite, ChessBoard c, int index)
        {
            BitboardLayer[] enemyDict = c.getDict(!isWhite);
            foreach (int i in enemyDict[pieceIndex.ALL_LOCATIONS].getTrueIndicies())
            {
                if (c.getValidMoves(isWhite, i).trueAtIndex(index)) return true;
            }
            return false;
        }

        public static int rateMaterial(bool isWhite, ChessBoard c)
        {
            int counter = 0;
            int numBishops = 0;
            BitboardLayer[] dict = c.getDict(isWhite);
            for (int j = 0; j <= pieceIndex.KING; j++)
            {
                foreach (int i in dict[j].getTrueIndicies()) { 
                    if (j != pieceIndex.BISHOP) counter += pieceVals[j];
                    else numBishops++;
                }
            }
            if (numBishops >= 2) counter += 300 * numBishops;
            else counter += 250 * numBishops;
            return counter;
        }

        public static int rateMoveability(bool isWhite, ChessBoard c, int possibleMoves, int depth, int material)
        {
            int counter = 0;
            counter += possibleMoves;
            if (possibleMoves == 0) //checkmate or stalemate
            {
                if (c.isInCheck(isWhite)) counter -= 200000 * depth;
                else counter -= 150000 * depth;
            }
            return counter;
        }

        public static int ratePositional(bool isWhite, ChessBoard c, int material)
        {
            int counter = 0;
            BitboardLayer[] dict = c.getDict(isWhite);
            for (int j = 0; j <= pieceIndex.KING; j++)
            {
                foreach (int i in dict[j].getTrueIndicies())
                {
                    counter += positionValues[j][i];
                    if (j == pieceIndex.KING)
                    {
                        if (material >= 1750) counter += positionValues[j][i];
                        else counter += positionValues[j + 1][i];
                    }
                }
            }
            return counter;
        }

        public static ulong setAtIndex(ulong state, int index, bool isTrue)
        {
            if (isTrue) return state | (ulong)(1uL << (63 - index));
            return state & ~((ulong)(1uL << 63 - index));
        }

        public static bool trueAtIndex(ulong t, int i)
        { //easier to think of the other way
            return (t & (ulong)(1uL << (63 - i))) > 0;
            //invert normal digit order (ie, index of 0 gives LBS of 63, which is the leftmost bit
        }
    }
}
