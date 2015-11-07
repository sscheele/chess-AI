/*
 * User: samsc
 * Date: 9/24/2015
 * Time: 11:27 AM
 */
using System;
using System.Collections.Generic;

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
		
		public static ulong setAtIndex(ulong state, int index, bool isTrue){
			if (isTrue) return state | (ulong)(1uL << (63 - index));
			return state & ~((ulong)(1uL << 63 - index));
		}
		
		public static bool trueAtIndex(ulong t, int i){ //easier to think of the other way
			return (t & (ulong)(1uL << (63 - i))) > 0;
			//invert normal digit order (ie, index of 0 gives LBS of 63, which is the leftmost bit
		}
		
		public int[] getAIMove(){
			ulong[] myBoard = new ulong[64];
			ulong[] enemyBoard = new ulong[64];
			Array.Copy(c.getDict(isWhite), myBoard, 64);
			Array.Copy(c.getDict(!isWhite), enemyBoard, 64);
			return getAIMove(myBoard, enemyBoard, 0, searchDepth, 0, null);
		}
		
		
		//args are for purposes of searchAIMove - see below
		public int[] getAIMove(ulong[] myBoard, ulong[] enemyBoard, int movesSearched, int ply, int currValue, int[] rootMove){
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
							int[] currMove = new int[]{index, to};
							int val = searchAIMove(myBoard, enemyBoard, movesSearched, ply, currValue, rootMove);
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
			return new int[]{-1, -1};
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
					ulong[] myTBoard = null;
					ulong[] enemyTBoard = null;
					Array.Copy(myBoard, myTBoard, myBoard.Length);
					Array.Copy(enemyBoard, enemyTBoard, enemyBoard.Length);
					if (movesSearched % 2 == 0 ^ isTheoretical){ //means we're searching our move. XOR by theoretical means that we search even moves and our opponent searches odd
						if (movesSearched == 0){
							ulong[][] newboards;
							if (rootMove != null){
								newboards = movePiece(myTBoard, enemyTBoard, rootMove[0], rootMove[1]);
							} else {
								int[] move = getAIMove(myTBoard, enemyTBoard, movesSearched + 1, ply, currValue, rootMove);
								newboards = movePiece(myTBoard, enemyTBoard, move[0], move[1]);
							}
							myTBoard = newboards[0]; //make the root move
							enemyTBoard = newboards[1];
							movesSearched++;
						} else {
							int[] bestMoves = this.getAIMove(myBoard, enemyBoard, movesSearched, ply, currValue, rootMove);
							
						}
					}
					return searchAIMove(myTBoard);
			} else {
				return currValue;
			}
			return currValue;
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
		
		int getValueAt(ulong[] board, int index){
			if (trueAtIndex(board[pieceIndex.ALL_LOCATIONS], index)){
				for (int p = 0; p <= pieceIndex.KING; p++){
					if (trueAtIndex(board[p], index)){
						switch (p){
							case pieceIndex.PAWN:
								return pieceVals.PAWN;
							case pieceIndex.ROOK:
								return pieceVals.ROOK;
							case pieceIndex.KNIGHT:
								return pieceVals.KNIGHT;
							case pieceIndex.BISHOP:
								return pieceVals.BISHOP;
							case pieceIndex.QUEEN:
								return pieceVals.QUEEN;
							case pieceIndex.KING:
								return pieceVals.KING;							
						}
					}
				}
			}
			return -666;
		}
		
		ulong[][] movePiece(ulong[] dict, ulong[] enemyDict, int begin, int end){
			for (int i = 0; i <= pieceIndex.KING; i++){
				if (trueAtIndex(dict[i], begin)){
					dict[i] = setAtIndex(dict[i], begin, false);
					dict[i] = setAtIndex(dict[i], end, true);
					if (trueAtIndex(enemyDict[pieceIndex.ALL_LOCATIONS], end)){
						for (int j = 0; j <= pieceIndex.KING; j++){
							if (trueAtIndex(enemyDict[j], end)) enemyDict[j] = setAtIndex(enemyDict[j], end, false);
						}
					}
				}
			}
			return new ulong[][]{dict, enemyDict};
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
