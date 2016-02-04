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
		
		public AI(ChessBoard c, bool isWhite, int searchDepth){
			transTable = new Dictionary<ulong[][], int>();
			expectedMovesTable = new Dictionary<ulong[][], int[]>();
			this.searchDepth = searchDepth;
		}
		
		public AI(ChessBoard c, bool isWhite, int searchDepth, AI parentAI){
			transTable = new Dictionary<ulong[][], int>();
			expectedMovesTable = new Dictionary<ulong[][], int[]>();

			this.searchDepth = searchDepth;
		}
		

        BitboardLayer[] getPossibleMoves(ChessBoard c, bool isWhite)
        {
            BitboardLayer[] retVal = new BitboardLayer[64];
            BitboardLayer[] dict = c.getDict(isWhite);
            BitboardLayer[] enemyDict = c.getDict(!isWhite);

            int[] allLocs = dict[pieceIndex.ALL_LOCATIONS].getTrueIndicies();
            for (int i = 0; i < 64; i++)
            {
                if (Array.IndexOf(allLocs, i) != -1){
                    retVal[i] = c.getValidMoves(isWhite, i, enemyDict[pieceIndex.ALL_LOCATIONS], true, false);
                } else
                {
                    retVal[i] = new BitboardLayer();
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
            searchDepth = depth;
            //ChessBoard tBoard = new ChessBoard(cb);
            //int[][] retVal = alphaBeta(tBoard, isWhite, depth, Int32.MinValue, Int32.MaxValue, new int[0], -1);
            int[][] retVal = alphaBeta(cb, isWhite, depth, Int32.MinValue, Int32.MaxValue, new int[0], -1);
            Debug.Print("Alphabeta is done.");
            return retVal;
        }
        

        public int[][] alphaBeta(ChessBoard cb, bool isWhite, int depth, int alpha, int beta, int[] move, int player)
        {
            numIterations++;
            //GOAL: minimize beta (starts at infinity) and maximize alpha (starts at neg. infinity)
            bool isWhiteMove = isWhite ^ (player == 1);
            BitboardLayer[] possibleMoves = getPossibleMoves(cb, isWhiteMove);

            int numMoves = 0; 
            foreach(BitboardLayer i in possibleMoves) { 
                numMoves += i.getNumOnes(); //need this for rating later
            }

            if (depth == 0 || numMoves == 0)
            {
                int i = -1 * player * Rating.rating(isWhite, cb, numMoves, searchDepth);
                Debug.Print("Found rating of: " + i);
                return new int[][] { move, new int[] { i } };
            }
            //TODO: sort for alphabeta
            player *= -1;
            isWhiteMove = isWhite ^ (player == -1);
            for (int i = 0; i < 64; i++)
            {
                foreach (int j in possibleMoves[i].getTrueIndicies()) {
                    if (depth == searchDepth)
                    {
                        Debug.Print("Currently searching move: [" + i + ", " + j + "] (alpha = " + alpha + ", beta = " + beta + ")");
                    }
                    cb.movePiece(isWhiteMove, i, j, true);
                    int[][] retVal = alphaBeta(cb, isWhite, depth - 1, alpha, beta, new int[] { i, j }, player);
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
            }
            if (player == -1) return new int[][] { move, new int[] { beta } };
            else return new int[][] { move, new int[] { alpha } };
        }
	}
}
