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
		
		static ulong setAtIndex(ulong state, int index, bool isTrue){
			if (isTrue) return state | (ulong)(1uL << (63 - index));
			return state & ~((ulong)(1uL << 63 - index));
		}
		
		static bool trueAtIndex(ulong t, int i){ //easier to think of the other way
			return (t & (ulong)(1uL << (63 - i))) > 0;
			//invert normal digit order (ie, index of 0 gives LBS of 63, which is the leftmost bit
		}

        ulong[] getPossibleMoves(ChessBoard c)
        {
            ulong[] retVal = new ulong[64];
            ulong[] dict = c.getDict(isWhite);
            ulong[] enemyDict = c.getDict(!isWhite);
            for (int i = 0; i < 64; i++)
            {
                if (trueAtIndex(dict[pieceIndex.ALL_LOCATIONS], i))
                {
                    retVal[i] = c.getValidMoves(isWhite, i, enemyDict[pieceIndex.ALL_LOCATIONS], true, false);
                }
            }
            return retVal;
        }
        

        int[][] alphaBeta(int depth, int alpha, int beta, int[] move, int player)
        {
            ulong[] possibleMoves = getPossibleMoves(c);
            bool hasValidMoves = false;
            for (int i = 0; i < 64; i++)
            {
                if (possibleMoves[i] > 0)
                {
                    hasValidMoves = true;
                    break;
                }
            }
            if (depth == 0 || !hasValidMoves) return new int[][] { move, new int[] { player * boardEval() } };
            //TODO: sort for alphabeta
            player = -1 * player;
            for (int i = 0; i < 64; i++)
            {
                if (possibleMoves[i] > 0)
                {
                    for (int j = 0; i < 64; j++)
                    {
                        if (trueAtIndex(possibleMoves[i], j))
                        {
                            ChessBoard tempBoard = new ChessBoard(c);
                            c.movePiece(player == -1 ^ !isWhite, i, j);
                            int[][] retVal = alphaBeta(depth - 1, alpha, beta, new int[] { i, j }, player);
                            c = tempBoard;
                        }
                    }
                }
            }
            return null;
        }
		
		public int[] getAIMove(){
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
							newboards = movePiece(myTBoard, enemyTBoard, rootMove[0], rootMove[1]);
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
    
        int boardEval()
        {
            return 0; //placeholder
        }
		
		int rawMoveValue(int index, ulong[] board){
			if(trueAtIndex(board[pieceIndex.ALL_LOCATIONS], index)){
				if(trueAtIndex(board[pieceIndex.PAWN], index)) return pieceVals.PAWN;
				if(trueAtIndex(board[pieceIndex.ROOK], index)) return pieceVals.ROOK;
				if(trueAtIndex(board[pieceIndex.KNIGHT], index)) return pieceVals.KNIGHT;
				if(trueAtIndex(board[pieceIndex.BISHOP], index)) return pieceVals.BISHOP;
				if(trueAtIndex(board[pieceIndex.QUEEN], index)) return pieceVals.QUEEN;
				return pieceVals.KING;
			}
			return 0;
		}
		
		ulong[][] movePiece(ChessBoard ctb, ulong[] dict, ulong[] enemyDict, int begin, int end){
            ChessBoard newBoard = new ChessBoard(ctb);
            int white_ep = newBoard.getEP(true);
            int black_ep = newBoard.getEP(false);
            for (int i = 0; i <= pieceIndex.KING; i++)
            {
                if (trueAtIndex(dict[i], begin))
                {
                    dict[i] = setAtIndex(dict[i], begin, false);
                    dict[i] = setAtIndex(dict[i], end, true);
                    if (trueAtIndex(enemyDict[pieceIndex.ALL_LOCATIONS], end))
                    {
                        for (int j = 0; j <= pieceIndex.KING; j++)
                        {
                            if (trueAtIndex(enemyDict[j], end)) enemyDict[j] = setAtIndex(enemyDict[j], end, false);
                        }
                    }
                    //if king or rook moving, invalidate castle
                    if (i == pieceIndex.KING && (dict[pieceIndex.FLAGS] & flagIndex.KING_CASTLE) > 0) dict[pieceIndex.FLAGS] &= ~flagIndex.KING_CASTLE;
                    if (i == pieceIndex.ROOK && (begin / 8 == 0 || begin / 8 == 7) && begin % 8 == 0 && (dict[pieceIndex.FLAGS] & flagIndex.LEFT_ROOK_CASTLE) > 0) dict[pieceIndex.FLAGS] &= ~flagIndex.LEFT_ROOK_CASTLE;
                    if (i == pieceIndex.ROOK && (begin / 8 == 0 || begin / 8 == 7) && begin % 8 == 7 && (dict[pieceIndex.FLAGS] & flagIndex.RIGHT_ROOK_CASTLE) > 0) dict[pieceIndex.FLAGS] &= ~flagIndex.RIGHT_ROOK_CASTLE;

                    //if castling, move rook to other side of king
                    if (i == pieceIndex.KING)
                    {
                        int rookIndex = isWhite ? 56 : 0;
                        int dir = begin < end ? 1 : -1;
                        if (dir == 1) rookIndex += 7;
                        if ((end - begin) * dir == 2)
                        { //king is castling
                            dict[pieceIndex.ROOK] = setAtIndex(dict[pieceIndex.ROOK], rookIndex, false);
                            dict[pieceIndex.ROOK] = setAtIndex(dict[pieceIndex.ROOK], begin + dir, true);
                        }
                    }
                    //add in en passant
                    if (i == pieceIndex.PAWN)
                    {
                        int dir = begin < end ? 1 : -1;
                        //set en passant of necessary
                        if ((dir * (end - begin) / 8) == 2)
                        {
                            if (isWhite) white_ep = begin + dir * 8;
                            else black_ep = begin + dir * 8;
                        }
                        else
                        {
                            if (isWhite) white_ep = -1;
                            else black_ep = -1;
                        }
                        //if capturing en passant, remove enemy pawn
                        int enemy_ep = isWhite ? black_ep : white_ep;
                        if (end == enemy_ep) enemyDict[pieceIndex.PAWN] = setAtIndex(enemyDict[pieceIndex.PAWN], enemy_ep - dir * 8, false);

                        //promotion
                        if ((end / 8 == 0 && isWhite) || (end / 8 == 7 && !isWhite))
                        {
                            dict[pieceIndex.PAWN] = setAtIndex(dict[pieceIndex.PAWN], i, false);
                            dict[pieceIndex.QUEEN] = setAtIndex(dict[pieceIndex.QUEEN], i, true);
                        }
                    }
                    else
                    {
                        if (isWhite) white_ep = -1;
                        else black_ep = -1;
                    }
                }
            }
            return new ulong[][] { dict, enemyDict };
        }
	}
	
	public static class pieceVals{
		public const int PAWN = 1;
		public const int ROOK = 5;
		public const int KNIGHT = 3;
		public const int BISHOP = 3;
		public const int QUEEN = 9;
		public const int KING = Int32.MaxValue;
	}
}
