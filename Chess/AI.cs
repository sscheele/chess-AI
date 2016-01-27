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
		ChessBoard c;
		bool isWhite;
		int searchDepth;
		bool isTheoretical = false;
		AI parent_child;
		
		public AI(ChessBoard c, bool isWhite, int searchDepth){
			transTable = new Dictionary<ulong[][], int>();
			expectedMovesTable = new Dictionary<ulong[][], int[]>();
			this.c = c;
			this.isWhite = isWhite;
			this.searchDepth = searchDepth;
			parent_child = new AI(c, !isWhite, searchDepth, this);
		}
		
		public AI(ChessBoard c, bool isWhite, int searchDepth, AI parentAI){
			transTable = new Dictionary<ulong[][], int>();
			expectedMovesTable = new Dictionary<ulong[][], int[]>();
			this.c = c;
			this.isWhite = isWhite;
			this.searchDepth = searchDepth;
			isTheoretical = true;
			parent_child = parentAI;
		}	
		

        BitboardLayer[] getPossibleMoves(ChessBoard c, bool isMyMove)
        {
            BitboardLayer[] retVal = new BitboardLayer[64];
            bool isWhiteMove = isWhite ^ !isMyMove;
            BitboardLayer[] dict = c.getDict(isWhiteMove);
            BitboardLayer[] enemyDict = c.getDict(!isWhiteMove);

            int[] allLocs = dict[pieceIndex.ALL_LOCATIONS].getTrueIndicies();
            for (int i = 0; i < 64; i++)
            {
                if (Array.IndexOf(allLocs, i) != -1){
                    retVal[i] = c.getValidMoves(isWhiteMove, i, enemyDict[pieceIndex.ALL_LOCATIONS], true, false);
                } else
                {
                    retVal[i] = new BitboardLayer();
                }
            }
            return retVal;
        }
        

        public int[][] alphaBeta(int depth, int alpha, int beta, int[] move, int player)
        {
            BitboardLayer[] possibleMoves = getPossibleMoves(c, player == -1);

            int numMoves = 0; 
            foreach(BitboardLayer i in possibleMoves) { 
                numMoves += i.getNumOnes(); //need this for rating later
            }
            if (depth == 0 || numMoves == 0) return new int[][] { move, new int[] { Rating.rating((player == -1) ^ isWhite, c, numMoves, searchDepth) } };
            //TODO: sort for alphabeta
            player *= -1;
            for (int i = 0; i < 64; i++)
            {
                foreach (int j in possibleMoves[i].getTrueIndicies()) { 
                    c.movePiece((player == -1) ^ isWhite, i, j, true);
                    int[][] retVal = alphaBeta(depth - 1, alpha, beta, new int[] { i, j }, player);
                    c.undoMove((player == -1) ^ isWhite);
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
		
		public int[] alphaBeta(){
            /*
			ulong[] myBoard = new ulong[pieceIndex.FLAGS + 1];
			ulong[] enemyBoard = new ulong[pieceIndex.FLAGS + 1];
			Array.Copy(c.getDict(isWhite), myBoard, pieceIndex.FLAGS + 1);
			Array.Copy(c.getDict(!isWhite), enemyBoard, pieceIndex.FLAGS + 1);
			return getAIMove(myBoard, enemyBoard, 0, searchDepth, 0);
            */
            return null; //temp value to make compile
		}
		
		/*
		//args are for purposes of searchAIMove - see below
		public int[] getAIMove(ulong[] myBoard, ulong[] enemyBoard, int movesSearched, int ply, int currValue){
			var maxPointVal = Int32.MinValue;
			var minMoveVal = Int32.MaxValue;
			ulong[] white = isWhite ? myBoard : enemyBoard;
			ulong[] black = isWhite ? enemyBoard : myBoard;
			var currBestMove = new int[]{-1, -1};
			for (int index = 0; index < 64; index++){ //go through each valid move and find its value
				if (trueAtIndex(myBoard[pieceIndex.ALL_LOCATIONS], index)){
					ulong validMoves = c.getValidMoves(isWhite, index, enemyBoard[pieceIndex.ALL_LOCATIONS], true, white, black, false);
					for (int to = 0; to < 64; to++){
						if (trueAtIndex(validMoves, to)){
							int[] currMove = {index, to};
                            Debug.Print("Searching AI moves at depth " + ply + " (current depth: " + movesSearched + ")");
							int val = searchAIMove(myBoard, enemyBoard, movesSearched, ply, currValue, currMove);
							if (val > maxPointVal){ //this is the "greediest" move, so update best move
								maxPointVal = val;
								minMoveVal = movesSearched;
								currBestMove = currMove;
							} else if (val == maxPointVal){ //in case of a tie, choose the shortest path
								if (minMoveVal >= movesSearched){
									maxPointVal = val;
									minMoveVal = movesSearched;
									currBestMove = currMove;
								}
							}
						}
					}
				}
			}
			return currBestMove;
		}
		
		//myBoard - board we're assuming is current board - may differ from actual board, since method is recursive
		//enemyBoard - board we're assuming is the enemy's board
		//movesSearched - number of times we've recursed
		//currValue - how many points the current "string" of moves will give us
		//rootMove - the "root" move we're currently searching (ie, the one were're being required to make by getAIMove)
		int searchAIMove(ulong[] myBoard, ulong[] enemyBoard, int movesSearched, int ply, int currValue, int[] rootMove){
			var whiteBoard = isWhite ? myBoard : enemyBoard;
			var blackBoard = isWhite ? enemyBoard : myBoard;
			int retVal = currValue;
			if (movesSearched < ply){ //we will recurse
					ulong[] myTBoard = new ulong[myBoard.Length];
					ulong[] enemyTBoard = new ulong[myBoard.Length];
					Array.Copy(myBoard, myTBoard, myBoard.Length);
					Array.Copy(enemyBoard, enemyTBoard, enemyBoard.Length);
					ulong[][] newboards;
					if (movesSearched % 2 == 0 ^ isTheoretical){ //means we're searching our move. XOR by theoretical means that we search even moves and our opponent searches odd
						//if (movesSearched == 0){
						if (rootMove != null){
							newboards = 
                           iece(myTBoard, enemyTBoard, rootMove[0], rootMove[1]);
                            Debug.Print("Making theoretical move: " + rootMove[0] + " to " + rootMove[1]);
							/*} else {
								int[] move = getAIMove(myTBoard, enemyTBoard, movesSearched + 1, ply, currValue, null);
								newboards = movePiece(myTBoard, enemyTBoard, move[0], move[1]);
							}
							myTBoard = newboards[0]; //make the root move
							enemyTBoard = newboards[1];
							movesSearched++;*/
                            /*
						} else {
							int[] bestMove = this.getAIMove(myBoard, enemyBoard, movesSearched + 1, ply, currValue);
							newboards = movePiece(myTBoard, enemyTBoard, bestMove[0], bestMove[1]);
							retVal += rawMoveValue(bestMove[1], enemyBoard);
						}
					} else {
						int[] bestMove = parent_child.getAIMove(enemyBoard, myBoard, movesSearched + 1, ply, currValue);
						newboards = movePiece(enemyTBoard, myTBoard, bestMove[0], bestMove[1]);
						retVal -= rawMoveValue(bestMove[1], myBoard);
					}
					return searchAIMove(newboards[0], newboards[1], movesSearched + 1, ply, retVal, null);
			} 
			return currValue;
		}
        */
	}
}
