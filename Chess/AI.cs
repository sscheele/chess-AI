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

        public int[][] getAIMove(ChessBoard cb, bool isWhite, int depth)
        {
            searchDepth = depth;
            ChessBoard tBoard = new ChessBoard(cb);
            int[][] retVal = alphaBeta(tBoard, isWhite, depth, Int32.MinValue, Int32.MaxValue, new int[0], -1);
            return retVal;
        }
        

        public int[][] alphaBeta(ChessBoard cb, bool isWhite, int depth, int alpha, int beta, int[] move, int player)
        {
            BitboardLayer[] possibleMoves = getPossibleMoves(cb, isWhite ^ player == 1);

            int numMoves = 0; 
            foreach(BitboardLayer i in possibleMoves) { 
                numMoves += i.getNumOnes(); //need this for rating later
            }
            if (depth == 0 || numMoves == 0) return new int[][] { move, new int[] { Rating.rating(isWhite, cb, numMoves, searchDepth) } };
            //TODO: sort for alphabeta
            player *= -1;
            for (int i = 0; i < 64; i++)
            {
                foreach (int j in possibleMoves[i].getTrueIndicies()) { 
                    cb.movePiece(isWhite, i, j, true);
                    int[][] retVal = alphaBeta(cb, isWhite, depth - 1, alpha, beta, new int[] { i, j }, player);
                    cb.undoMove((player == -1) ^ isWhite);
                    if (player == -1)
                    {
                        if (retVal[1][0] <= beta)
                        {
                            beta = retVal[1][0];
                            if (depth == searchDepth) move = retVal[0];
                        }
                    }
                    else
                    {
                        if (retVal[1][0] > alpha)
                        {
                            alpha = retVal[1][0];
                            if (depth == searchDepth) move = retVal[0];
                        }
                    }
                    if (alpha >= beta)
                    {
                        Debug.Print("Branch pruned at level " + (4 - depth) + " because alpha (" + alpha + ") >= beta (" + beta + ")");
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
