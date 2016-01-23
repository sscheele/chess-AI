/*
 * User: samsc
 * Date: 9/22/2015
 * Time: 12:11 PM
 */
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Chess
{
	public class ChessBoard{
        List<int[]> moveList = new List<int[]>();
        int moveNum = 0;

		ulong[] white = new ulong[10];
		ulong[] black = new ulong[10];
        int black_ep = -1;
        int white_ep = -1;
		
		public ChessBoard(ulong[] white, ulong[] black){
            if (white == null || black == null)
            {
                //pawn
                ulong temp = Convert.ToUInt64("0000000011111111000000000000000000000000000000000000000000000000", 2);
                this.black[pieceIndex.PAWN] = temp;
                temp = Convert.ToUInt64("0000000000000000000000000000000000000000000000001111111100000000", 2);
                this.white[pieceIndex.PAWN] = temp;
                //rook
                temp = Convert.ToUInt64("1000000100000000000000000000000000000000000000000000000000000000", 2);
                this.black[pieceIndex.ROOK] = temp;
                temp = Convert.ToUInt64("0000000000000000000000000000000000000000000000000000000010000001", 2);
                this.white[pieceIndex.ROOK] = temp;
                //knight
                temp = Convert.ToUInt64("0100001000000000000000000000000000000000000000000000000000000000", 2);
                this.black[pieceIndex.KNIGHT] = temp;
                temp = Convert.ToUInt64("0000000000000000000000000000000000000000000000000000000001000010", 2);
                this.white[pieceIndex.KNIGHT] = temp;
                //bishop
                temp = Convert.ToUInt64("0010010000000000000000000000000000000000000000000000000000000000", 2);
                this.black[pieceIndex.BISHOP] = temp;
                temp = Convert.ToUInt64("0000000000000000000000000000000000000000000000000000000000100100", 2);
                this.white[pieceIndex.BISHOP] = temp;
                //queen
                this.black[pieceIndex.QUEEN] = Convert.ToUInt64("0001000000000000000000000000000000000000000000000000000000000000", 2);
                this.white[pieceIndex.QUEEN] = Convert.ToUInt64("0000000000000000000000000000000000000000000000000000000000010000", 2);
                //king
                this.black[pieceIndex.KING] = Convert.ToUInt64("0000100000000000000000000000000000000000000000000000000000000000", 2);
                this.white[pieceIndex.KING] = Convert.ToUInt64("0000000000000000000000000000000000000000000000000000000000001000", 2);
                //set flags used for castling
                this.white[pieceIndex.FLAGS] = Convert.ToUInt64("111", 2); //castling flags - left rook, king, right rook
                this.black[pieceIndex.FLAGS] = Convert.ToUInt64("111", 2);

                getAllLocations(true);
                getAllLocations(false);
                this.white[pieceIndex.ATTACKED_SQUARES] = getAttackedSquares(true);
                this.black[pieceIndex.ATTACKED_SQUARES] = getAttackedSquares(false);
            } else
            {
                this.white = white;
                this.black = black;
            }
		}
		
		public ChessBoard(ChessBoard c){
            Array.Copy(c.getDict(true), white, white.Length);
			Array.Copy(c.getDict(false), black, black.Length);
            white_ep = c.getEP(true);
            black_ep = c.getEP(false);
            moveList = new List<int[]>(c.getMoveList());
            moveNum = c.getMoveNum();
		}

        public List<int[]> getMoveList(){ return moveList; }

        public int getMoveNum(){ return moveNum; }

        public int getEP(bool isWhite){ return isWhite ? white_ep : black_ep; }
		
		public ulong[] getDict(bool isWhite){ return isWhite ? white : black; }

        public void resetBoard()
        {
            //pawn
            ulong temp = Convert.ToUInt64("0000000011111111000000000000000000000000000000000000000000000000", 2);
            this.black[pieceIndex.PAWN] = temp;
            temp = Convert.ToUInt64("0000000000000000000000000000000000000000000000001111111100000000", 2);
            this.white[pieceIndex.PAWN] = temp;
            //rook
            temp = Convert.ToUInt64("1000000100000000000000000000000000000000000000000000000000000000", 2);
            this.black[pieceIndex.ROOK] = temp;
            temp = Convert.ToUInt64("0000000000000000000000000000000000000000000000000000000010000001", 2);
            this.white[pieceIndex.ROOK] = temp;
            //knight
            temp = Convert.ToUInt64("0100001000000000000000000000000000000000000000000000000000000000", 2);
            this.black[pieceIndex.KNIGHT] = temp;
            temp = Convert.ToUInt64("0000000000000000000000000000000000000000000000000000000001000010", 2);
            this.white[pieceIndex.KNIGHT] = temp;
            //bishop
            temp = Convert.ToUInt64("0010010000000000000000000000000000000000000000000000000000000000", 2);
            this.black[pieceIndex.BISHOP] = temp;
            temp = Convert.ToUInt64("0000000000000000000000000000000000000000000000000000000000100100", 2);
            this.white[pieceIndex.BISHOP] = temp;
            //queen
            this.black[pieceIndex.QUEEN] = Convert.ToUInt64("0001000000000000000000000000000000000000000000000000000000000000", 2);
            this.white[pieceIndex.QUEEN] = Convert.ToUInt64("0000000000000000000000000000000000000000000000000000000000010000", 2);
            //king
            this.black[pieceIndex.KING] = Convert.ToUInt64("0000100000000000000000000000000000000000000000000000000000000000", 2);
            this.white[pieceIndex.KING] = Convert.ToUInt64("0000000000000000000000000000000000000000000000000000000000001000", 2);
            //set flags used for castling
            this.white[pieceIndex.FLAGS] = Convert.ToUInt64("111", 2); //castling flags - left rook, king, right rook
            this.black[pieceIndex.FLAGS] = Convert.ToUInt64("111", 2);

            getAllLocations(true);
            getAllLocations(false);
            this.white[pieceIndex.ATTACKED_SQUARES] = getAttackedSquares(true);
            this.black[pieceIndex.ATTACKED_SQUARES] = getAttackedSquares(false);

            moveList = new List<int[]>();
            moveNum = 0;

            white_ep = -1;
            black_ep = -1;
        }

        public void undoMove()
        {
            var oldMoveList = new List<int[]>(moveList);
            resetBoard();

            bool currMove = true;
            for (int i = 0; i < oldMoveList.Count - 1; i++)
            {
                movePiece(currMove, oldMoveList[i][0], oldMoveList[i][1]);
                currMove = !currMove;
            }
            getAllLocations(true);
            getAllLocations(false);
            white[pieceIndex.ATTACKED_SQUARES] = getAttackedSquares(true);
            black[pieceIndex.ATTACKED_SQUARES] = getAttackedSquares(false);
        }
		
		public void movePiece(bool isWhite, int begin, int end){
			ulong[] dict = isWhite ? white : black;
			ulong[] enemyDict = isWhite ? black : white;
			for (int i = 0; i <= pieceIndex.KING; i++){
				if (trueAtIndex(dict[i], begin)){
					dict[i] = setAtIndex(dict[i], begin, false);
					dict[i] = setAtIndex(dict[i], end, true);
					if (trueAtIndex(enemyDict[pieceIndex.ALL_LOCATIONS], end)){
						for (int j = 0; j <= pieceIndex.KING; j++){
                            if (trueAtIndex(enemyDict[j], end)) enemyDict[j] = setAtIndex(enemyDict[j], end, false);
						}
					}
                    //if king or rook moving, invalidate castle
                    if (i == pieceIndex.KING && (dict[pieceIndex.FLAGS] & flagIndex.KING_CASTLE) > 0) dict[pieceIndex.FLAGS] &= ~flagIndex.KING_CASTLE;
                    if (i == pieceIndex.ROOK && (begin / 8 == 0 || begin / 8 == 7) && begin % 8 == 0 && (dict[pieceIndex.FLAGS] & flagIndex.LEFT_ROOK_CASTLE) > 0) dict[pieceIndex.FLAGS] &= ~flagIndex.LEFT_ROOK_CASTLE;
                    if (i == pieceIndex.ROOK && (begin / 8 == 0 || begin / 8 == 7) && begin % 8 == 7 && (dict[pieceIndex.FLAGS] & flagIndex.RIGHT_ROOK_CASTLE) > 0) dict[pieceIndex.FLAGS] &= ~flagIndex.RIGHT_ROOK_CASTLE;

                    //if castling, move rook to other side of king
                    if (i == pieceIndex.KING){
                        int rookIndex = isWhite ? 56 : 0;
                        int dir = begin < end ? 1 : -1;
                        if (dir == 1) rookIndex += 7;
                        if ((end - begin) * dir == 2) { //king is castling
                            dict[pieceIndex.ROOK] = setAtIndex(dict[pieceIndex.ROOK], rookIndex, false);
                            dict[pieceIndex.ROOK] = setAtIndex(dict[pieceIndex.ROOK], begin + dir, true);
                        }
                    }
                    //add in en passant
                    if (i == pieceIndex.PAWN){
                        int dir = begin < end ? 1 : -1;
                        //set en passant of necessary
                        if ((dir * (end - begin) / 8) == 2){
                            if (isWhite) white_ep = begin + dir * 8;
                            else black_ep = begin + dir * 8;
                        } else {
                            if (isWhite) white_ep = -1;
                            else black_ep = -1;
                        }
                        //if capturing en passant, remove enemy pawn
                        int enemy_ep = isWhite ? black_ep : white_ep;
                        if (end == enemy_ep) enemyDict[pieceIndex.PAWN] = setAtIndex(enemyDict[pieceIndex.PAWN], enemy_ep - dir * 8, false);

                        //promotion
                        if ((end / 8 == 0 && isWhite) || (end / 8 == 7 && !isWhite)) promotePiece(end, isWhite);
                    } else {
                        if (isWhite) white_ep = -1;
                        else black_ep = -1;
                    }
                }
            }
            moveList.Add(new int[] { begin, end });
            moveNum++;
            getAllLocations(isWhite);
            dict[pieceIndex.ATTACKED_SQUARES] = getAttackedSquares(isWhite);
		}

        void promotePiece(int index, bool isWhite)
        {
            promotionForm pf = new promotionForm(isWhite);
            pf.ShowDialog();
            int promotionChoice = pf.result;
            ulong[] dict = isWhite ? white : black;
            dict[pieceIndex.PAWN] = setAtIndex(dict[pieceIndex.PAWN], index, false);
            dict[promotionChoice] = setAtIndex(dict[promotionChoice], index, true);
        }

        void promotePiece(int index, bool isWhite, int promotionChoice)
        {
            ulong[] dict = isWhite ? white : black;
            dict[pieceIndex.PAWN] = setAtIndex(dict[pieceIndex.PAWN], index, false);
            dict[promotionChoice] = setAtIndex(dict[promotionChoice], index, true);
        }
		
		public void getAllLocations(bool isWhite){
			ulong[] dict = isWhite ? white : black;
			dict[pieceIndex.ALL_LOCATIONS] = 0;
			for (int i = 0; i <= pieceIndex.KING; i++){
				dict[pieceIndex.ALL_LOCATIONS] |= dict[i];
			}
		}		
		
		public ulong getValidMoves(bool isWhite, int index, ulong enemyAllPos = 0, bool applyCheckLimits = true, bool fromAttackedSq = false){
            ulong[] w = white;
            ulong[] b = black;
			if (enemyAllPos == 0) enemyAllPos = isWhite ? black[pieceIndex.ALL_LOCATIONS] : white[pieceIndex.ALL_LOCATIONS];
			ulong retVal = 0;
			ulong[] dict = isWhite ? w : b;
			ulong newAllPos = dict[pieceIndex.ALL_LOCATIONS];
			ulong[] enemyDict = isWhite ? b : w;
			for (int s = 0; s <= pieceIndex.KING; s++){
				if(trueAtIndex(dict[s], index)){
					bool isKingMove = false;
					switch(s){
						case pieceIndex.PAWN:
                            int enemy_ep = isWhite ? black_ep : white_ep;
							int direction = isWhite ? -1 : 1;
							int pawnFile = isWhite ? 6 : 1;
							int moveIndex = index + (direction * 8);
							//move one in dir
							if (moveIndex >= 0 && moveIndex <= 63 && 
							    !checkCollision(index, moveIndex, newAllPos, enemyAllPos, fromAttackedSq) &&
                                !fromAttackedSq) retVal = setAtIndex(retVal, moveIndex, true);
							
							//captures
							if (moveIndex - 1 >= 0 && moveIndex - 1 <= 63 && //in bounds
								(moveIndex - 1) / 8 == moveIndex / 8 && //on same rank
								trueAtIndex(enemyDict[pieceIndex.ALL_LOCATIONS], moveIndex - 1)) retVal = setAtIndex(retVal, moveIndex - 1, true);
							if (moveIndex + 1 >= 0 && moveIndex + 1 <= 63 && //in bounds
								(moveIndex + 1) / 8 == moveIndex / 8 && //on same rank
								trueAtIndex(enemyDict[pieceIndex.ALL_LOCATIONS], moveIndex + 1)) retVal = setAtIndex(retVal, moveIndex + 1, true);
							
							//move two in dir
							if (index / 8 == pawnFile){
								moveIndex += direction * 8;
								if (!checkCollision(index, moveIndex, newAllPos, enemyAllPos, fromAttackedSq) && !fromAttackedSq) retVal = setAtIndex(retVal, moveIndex, true);
							}

                            //en passant captures
                            if (index + direction * 7 == enemy_ep || index + direction * 9 == enemy_ep) retVal = setAtIndex(retVal, enemy_ep, true);
                            break;
							
						case pieceIndex.ROOK:
							for (int dir = -1; dir < 2; dir += 2){
								for (int ext = 1; ext <= 8; ext++){
									int testInd = index + (dir * ext);
									if (checkCollision(index, testInd, newAllPos, enemyAllPos, fromAttackedSq) || index / 8 != testInd / 8 || testInd < 0 || testInd > 63) break;
									retVal = setAtIndex(retVal, testInd, true);
								}
								for (int ext = 0; ext < 8; ext++) {
									int testInd = index + (8 * dir * ext);
									if (checkCollision(index, testInd, newAllPos, enemyAllPos, fromAttackedSq) || index % 8 != testInd % 8 || testInd < 0 || testInd > 63) break;
									retVal = setAtIndex(retVal, testInd, true);
								}
							}
							break;
						case pieceIndex.KNIGHT:
							int[] diffs = {17, 15, 10, 6, -6, -10, -15, -17};
							int[] rightRows = {16, 16, 8, 8, -8, -8, -16, -16};
							for (int i = 0; i < diffs.Length; i++){
								if ((index + diffs[i]) / 8 == (index + rightRows[i]) / 8 && index + diffs[i] >= 0 && index + diffs[i] <= 63 && !trueAtIndex(dict[pieceIndex.ALL_LOCATIONS], index + diffs[i])) retVal = setAtIndex(retVal, index + diffs[i], true);
							}
							break;
						case pieceIndex.BISHOP:
							diffs = new int[]{-7, 7, -9, 9};
							foreach (int diff in diffs){
								int prevTemp = index;
								for (int ext = 1; ext < 8; ext++){
									int temp = index + (ext * diff);
									if (checkCollision(index, temp, newAllPos, enemyAllPos, fromAttackedSq) || temp < 0 || temp > 64 || 
									    Math.Abs((prevTemp / 8) - (temp / 8)) != 1 || Math.Abs((prevTemp / 8) - (temp / 8)) != 1) break;
									prevTemp = temp;
									retVal = setAtIndex(retVal, temp, true);
								}
							}
							break;
						case pieceIndex.QUEEN:
                            //move like a rook
							for (int dir = -1; dir < 2; dir += 2){
								for (int ext = 1; ext <= 8; ext++){
									int testInd = index + (dir * ext);
									if (checkCollision(index, testInd, newAllPos, enemyAllPos, fromAttackedSq) || index / 8 != testInd / 8 || testInd < 0 || testInd > 63) break;
									retVal = setAtIndex(retVal, testInd, true);
								}
								for (int ext = 0; ext < 8; ext++) {
									int testInd = index + (8 * dir * ext);
									if (checkCollision(index, testInd, newAllPos, enemyAllPos, fromAttackedSq) || index % 8 != testInd % 8 || testInd < 0 || testInd > 63) break;
									retVal = setAtIndex(retVal, testInd, true);
								}
							}
                            //move like a bishop
							diffs = new int[]{-7, 7, -9, 9};
							foreach (int diff in diffs){
								int prevTemp = index;
								for (int ext = 1; ext < 8; ext++){
									int temp = index + (ext * diff);
									if (checkCollision(index, temp, newAllPos, enemyAllPos, fromAttackedSq) || temp < 0 || temp > 64 || 
									    Math.Abs((prevTemp / 8) - (temp / 8)) != 1 || Math.Abs((prevTemp / 8) - (temp / 8)) != 1) break;
									prevTemp = temp;
									retVal = setAtIndex(retVal, temp, true);
								}
							}
							break;
						case pieceIndex.KING:
							isKingMove = true;
                            int currRow = index / 8;
							for (int a = -1; a <= 1; a++){
								for (int c = -1; c <= 1; c++){
									int newIndex = index + (8 * a) + c;
									if (newIndex >= 0 && newIndex <= 63 && !trueAtIndex(newAllPos, newIndex)) retVal = setAtIndex(retVal, newIndex, true);
								}
							}
                            if ((dict[pieceIndex.FLAGS] & flagIndex.KING_CASTLE) > 0 && !trueAtIndex(enemyDict[pieceIndex.ATTACKED_SQUARES], index)) //king can castle
                            {
                                if((dict[pieceIndex.FLAGS] & flagIndex.LEFT_ROOK_CASTLE) > 0 &&
                                    !trueAtIndex(enemyDict[pieceIndex.ATTACKED_SQUARES], index - 1) && !trueAtIndex(dict[pieceIndex.ALL_LOCATIONS], index - 1) &&
                                    !trueAtIndex(enemyDict[pieceIndex.ATTACKED_SQUARES], index - 2) && !trueAtIndex(dict[pieceIndex.ALL_LOCATIONS], index - 2)) retVal = setAtIndex(retVal, index + 2, true); //king can castle right

                                if ((dict[pieceIndex.FLAGS] & flagIndex.RIGHT_ROOK_CASTLE) > 0 && 
                                    !trueAtIndex(enemyDict[pieceIndex.ATTACKED_SQUARES], index + 1) && !trueAtIndex(dict[pieceIndex.ALL_LOCATIONS], index + 1) &&
                                    !trueAtIndex(enemyDict[pieceIndex.ATTACKED_SQUARES], index + 2) && !trueAtIndex(dict[pieceIndex.ALL_LOCATIONS], index + 2)) retVal = setAtIndex(retVal, index + 2, true); //king can castle right
                            }
							break;
					}
					retVal = setAtIndex(retVal, index, false);
					if (applyCheckLimits) return applyCheck(isWhite, index, retVal, isKingMove);
					return retVal;
				}
			}
			return 0;
		}
		
		ulong applyCheck(bool isWhite, int beginIndex, ulong possibleMoves, bool isKingMove){
			ulong[] myDict = isWhite ? white : black;
			ulong[] enemyDict = isWhite ? black : white;
			ulong retVal = possibleMoves;
			ulong myBoard = 0;
			
			getAttackedSquares(!isWhite);
            myBoard = myDict[pieceIndex.ALL_LOCATIONS];

            for (int i = 0; i < 64; i++){
				if (trueAtIndex(possibleMoves, i)){	//for each valid move, use a test allLocations to see if it's moving into check
                    ulong kingPos = myDict[pieceIndex.KING];
                    if (isKingMove) kingPos = (ulong)1 << (63 - i);
                    ChessBoard cboard = new ChessBoard(this);
                    cboard.movePiece(isWhite, beginIndex, i);
                    ulong newAttackedSquares = cboard.getAttackedSquares(!isWhite);
                    if ((kingPos & newAttackedSquares) > 0) retVal = setAtIndex(retVal, i, false);
                    myDict[pieceIndex.ALL_LOCATIONS] = myBoard;
				}
			}
			return retVal;
		}		               
		
		//POST: gives squares attacked by isWhite given the current configuration
		public ulong getAttackedSquares(bool isWhite){
			ulong newAllPos = isWhite ? black[pieceIndex.ALL_LOCATIONS] : white[pieceIndex.ALL_LOCATIONS];
			ulong retVal = 0;
			ulong[] myDict = isWhite ? white : black;
			ulong myPcs = myDict[pieceIndex.ALL_LOCATIONS];
			for (int i = 0; i < 64; i++){
				if (trueAtIndex(myPcs, i)){
					retVal |= getValidMoves(isWhite, i, newAllPos, false, true);
				}
			}
			return retVal;
		}
		
		bool checkCollision(int beginIndex, int endIndex, ulong myPcs, ulong enemyPcs, bool attackedSq){
			int dir = beginIndex < endIndex ? 1 : -1;
			if ((beginIndex - endIndex) % 8 == 0){ //moving vertically
				return extendCurrPath(beginIndex, endIndex, myPcs, enemyPcs, 8, dir, attackedSq);
			} if (beginIndex / 8 == endIndex / 8) { //moving horizontally
				while(dir * beginIndex < dir * endIndex && beginIndex / 8 == endIndex / 8){
					beginIndex += dir;
					if (trueAtIndex(myPcs, beginIndex)) {
						if (attackedSq && beginIndex == endIndex) return false;
						return true;
					} if (trueAtIndex(enemyPcs, beginIndex)){
						if (beginIndex != endIndex) return true;
						return false;
					}
				}
			} else { //moving diagonally
				if ((beginIndex - endIndex) % 9 == 0){ //between low-left and up-right
					return extendCurrPath(beginIndex, endIndex, myPcs, enemyPcs, 9, dir, attackedSq);
				} else {
					return extendCurrPath(beginIndex, endIndex, myPcs, enemyPcs, 7, dir, attackedSq);
				}
			}
			return false;
		}
		
		bool extendCurrPath(int beginIndex, int endIndex, ulong myPcs, ulong enemyPcs, int extend, int dir, bool attackedSq){
			while(dir * beginIndex < dir * endIndex && (beginIndex >= 0 && beginIndex <= 63)){
					beginIndex += dir * extend;
					if (trueAtIndex(myPcs, beginIndex)) {
						if (attackedSq && beginIndex == endIndex) return false;
						return true;
					} if (trueAtIndex(enemyPcs, beginIndex)){
						if (beginIndex != endIndex) return true;
						return false;
					}
				}
			return false;
		}
		
		public bool checkForMate(bool isWhite){ //checks to see if isWhite has checkmate
			ulong[] dict = isWhite ? black : white;
			for (int i = 0; i < 64; i++){
				if (trueAtIndex(dict[pieceIndex.ALL_LOCATIONS], i)){
					if (getValidMoves(!isWhite, i) != 0uL) return false;
				}
			}
			return true;
		}
		
		public static ulong setAtIndex(ulong state, int index, bool isTrue){
			if (isTrue) return state | (ulong)(1uL << (63 - index));
			return state & ~((ulong)(1uL << 63 - index));
		}
		
		public static bool trueAtIndex(ulong t, int i){ //easier to think of the other way
			return (t & (ulong)(1uL << (63 - i))) > 0;
			//invert normal digit order (ie, index of 0 gives LBS of 63, which is the leftmost bit
		}
}
}
