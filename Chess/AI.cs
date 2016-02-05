/*
 * User: samsc
 * Date: 9/24/2015
 * Time: 11:27 AM
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Chess
{
	/// <summary>
	/// Description of AI.
	/// </summary>
	public class AI
	{
		Dictionary<ulong[][], int> transTable; //stores transpositions - pre-calculated positions we've already searched for and already know the point value of
		Dictionary<ulong[][], int[]> expectedMovesTable; //stores the sequence of moves we're hoping for/expecting

        int numIterations = 0;
		int searchDepth;

        MainForm gui;
		
		public AI(ChessBoard c, bool isWhite, int searchDepth, MainForm f){
			transTable = new Dictionary<ulong[][], int>();
			expectedMovesTable = new Dictionary<ulong[][], int[]>();
			this.searchDepth = searchDepth;

            gui = f;
		}
		

        List<int[]> getPossibleMoves(ChessBoard c, bool isWhite)
        {
            var retVal = new List<int[]>();
            BitboardLayer[] dict = c.getDict(isWhite);
            BitboardLayer[] enemyDict = c.getDict(!isWhite);

            foreach (int i in dict[pieceIndex.ALL_LOCATIONS].getTrueIndicies()) { 
                BitboardLayer pieceMoves = c.getValidMoves(isWhite, i);
                foreach (int j in pieceMoves.getTrueIndicies())
                {
                    retVal.Add(new int[] { i, j });
                }
            }
            return retVal;
        }

        public string displayBoard(ChessBoard c)
        {
            //for debugging purposes
            //capital letters are white, n's are knights
            char[] whiteLetters = new char[] { Convert.ToChar("P"), Convert.ToChar("R"), Convert.ToChar("N"), Convert.ToChar("B"), Convert.ToChar("Q"), Convert.ToChar("K") };
            char[] blackLetters = new char[] { Convert.ToChar("p"), Convert.ToChar("r"), Convert.ToChar("n"), Convert.ToChar("b"), Convert.ToChar("q"), Convert.ToChar("k") };

            char[] retVal = new char[64];
            for (int i = 0; i < 64; i++) retVal[i] = Convert.ToChar("+");
            BitboardLayer[] white = c.getDict(true);
            BitboardLayer[] black = c.getDict(false);
            for (int i = 0; i <= pieceIndex.KING; i++){
                foreach (int j in white[i].getTrueIndicies()) retVal[j] = whiteLetters[i];
                foreach (int j in black[i].getTrueIndicies()) retVal[j] = blackLetters[i];
            }
            string s = "";
            for (int i = 0; i < 64; i++){
                s += retVal[i];
                if (i % 8 == 7) s += "\n";
            }
            var ml = c.getMoveList();
            for (int i = 0; i < ml.Count; i++)
            {
                s += (i % 2 == 0 ? "White" : "Black") + ": [" + ml[i][0] + ", " + ml[i][1] + "]\n";
            }
            return s;
        }

        public bool matchesMoveList(ChessBoard cb)
        {
            ChessBoard test = new ChessBoard(null, null);
            var moveList = cb.getMoveList();
            for (int i = 0; i < moveList.Count; i++)
            {
                test.movePiece(i % 2 == 0, moveList[i][0], moveList[i][1]);
            }
            return cb.equals(test);
        }

        public int[][] getAIMove(ChessBoard cb, bool isWhite, int depth)
        {
            gui.resetProgBar();
            searchDepth = depth;
            int[][] retVal = alphaBeta(cb, isWhite, depth, Int32.MinValue, Int32.MaxValue, new int[0], -1);
            gui.addText("***\nAlphabeta is done.\n***");
            return retVal;
        }

        public List<int[]> sortAlphaBeta(ChessBoard cb, bool isWhite, List<int[]> possibleMoves)
        {
            List<int[]> retVal = new List<int[]>();
            List<int> valueSet = new List<int>();
            for (int i = 0; i < possibleMoves.Count; i++)
            {
                int[] currMove = possibleMoves[i];
                cb.movePiece(isWhite, currMove[0], currMove[1], true);
                valueSet.Add(Rating.rating(isWhite, cb, possibleMoves.Count, searchDepth));
                cb.undoMove(isWhite);
            }
            while (valueSet.Count > 0)
            {
                int index = indexOfMax(valueSet);
                retVal.Add(possibleMoves[index]);
                possibleMoves.RemoveAt(index);
                valueSet.RemoveAt(index);
            }
            return retVal;
        }

        int indexOfMax(List<int> l)
        {
            int retVal = 0;
            int currMax = l[0];
            for (int i = 0; i < l.Count; i++)
            {
                if (l[i] > currMax)
                {
                    retVal = i;
                    currMax = l[i];
                }
            }
            return retVal;
        }
        

        public int[][] alphaBeta(ChessBoard cb, bool isWhite, int depth, int alpha, int beta, int[] move, int player)
        {
            numIterations++;
            //GOAL: minimize beta (starts at infinity) and maximize alpha (starts at neg. infinity)
            bool isWhiteMove = isWhite ^ (player == 1);

            List<int[]> possibleMoves = depth > 1 ? sortAlphaBeta(cb, isWhiteMove, getPossibleMoves(cb, isWhiteMove)) : getPossibleMoves(cb, isWhiteMove);
            int numMoves = possibleMoves.Count;

            if (depth == 0 || numMoves == 0)
            {
                //int i = -1 * player * Rating.rating(isWhite, cb, numMoves, searchDepth);
                int i = Rating.rating(isWhite, cb, numMoves, searchDepth);
                //Debug.Print("Found rating of: " + i);
                return new int[][] { move, new int[] { i } };
            }
            //TODO: sort for alphabeta
            player *= -1;
            foreach (int[] currMove in possibleMoves) {   
                if (depth == searchDepth)
                {
                    gui.stepProgBarBy((int)(100 / possibleMoves.Count));
                    gui.addText("Currently searching move: [" + currMove[0] + ", " + currMove[1] + "] (alpha = " + alpha + ", beta = " + beta + ")\n");
                }
                cb.movePiece(isWhiteMove, currMove[0], currMove[1], true);
                int[][] retVal = alphaBeta(cb, isWhite, depth - 1, alpha, beta, currMove, player);
                cb.undoMove(isWhiteMove);
                /*
                if (!matchesMoveList(cb))
                {
                    Debug.Print("Chess board does not match moves!");
                }
                */
                if (player == -1) //is min node
                {
                    if (retVal[1][0] <= beta)
                    {
                        beta = retVal[1][0];
                        if (depth == searchDepth) move = retVal[0];
                    }
                }
                else //is max node
                {
                    if (retVal[1][0] > alpha)
                    {
                        alpha = retVal[1][0];
                        if (depth == searchDepth) move = retVal[0];
                    }
                }
                if (alpha >= beta)
                {
                    if (player == -1) return new int[][] { move, new int[] { beta } };
                    else return new int[][] { move, new int[] { alpha } };
                }
            }
            if (player == -1) return new int[][] { move, new int[] { beta } };
            else return new int[][] { move, new int[] { alpha } };
        }
	}
}
