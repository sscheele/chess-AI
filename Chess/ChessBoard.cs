/*
 * User: samsc
 * Date: 9/22/2015
 * Time: 12:11 PM
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

namespace Chess
{
	public class ChessBoard{
        List<int[]> moveList = new List<int[]>();
        int moveNum = 0;

        int[] hitCount = new int[10];

        BitboardLayer[] white = new BitboardLayer[10];
		BitboardLayer[] black = new BitboardLayer[10];
        int black_ep = -1;
        int white_ep = -1;
		
		public ChessBoard(BitboardLayer[] white, BitboardLayer[] black){
            if (white == null || black == null)
            {
                //pawn
                ulong temp = Convert.ToUInt64("0000000011111111000000000000000000000000000000000000000000000000", 2);
                this.black[pieceIndex.PAWN] = new BitboardLayer(temp);
                temp = Convert.ToUInt64("0000000000000000000000000000000000000000000000001111111100000000", 2);
                this.white[pieceIndex.PAWN] = new BitboardLayer(temp);
                //rook
                temp = Convert.ToUInt64("1000000100000000000000000000000000000000000000000000000000000000", 2);
                this.black[pieceIndex.ROOK] = new BitboardLayer(temp);
                temp = Convert.ToUInt64("0000000000000000000000000000000000000000000000000000000010000001", 2);
                this.white[pieceIndex.ROOK] = new BitboardLayer(temp);
                //knight
                temp = Convert.ToUInt64("0100001000000000000000000000000000000000000000000000000000000000", 2);
                this.black[pieceIndex.KNIGHT] = new BitboardLayer(temp);
                temp = Convert.ToUInt64("0000000000000000000000000000000000000000000000000000000001000010", 2);
                this.white[pieceIndex.KNIGHT] = new BitboardLayer(temp);
                //bishop
                temp = Convert.ToUInt64("0010010000000000000000000000000000000000000000000000000000000000", 2);
                this.black[pieceIndex.BISHOP] = new BitboardLayer(temp);
                temp = Convert.ToUInt64("0000000000000000000000000000000000000000000000000000000000100100", 2);
                this.white[pieceIndex.BISHOP] = new BitboardLayer(temp);
                //queen
                this.black[pieceIndex.QUEEN] = new BitboardLayer(Convert.ToUInt64("0001000000000000000000000000000000000000000000000000000000000000", 2));
                this.white[pieceIndex.QUEEN] = new BitboardLayer(Convert.ToUInt64("0000000000000000000000000000000000000000000000000000000000010000", 2));
                //king
                this.black[pieceIndex.KING] = new BitboardLayer(Convert.ToUInt64("0000100000000000000000000000000000000000000000000000000000000000", 2));
                this.white[pieceIndex.KING] = new BitboardLayer(Convert.ToUInt64("0000000000000000000000000000000000000000000000000000000000001000", 2));
                //set flags used for castling
                this.white[pieceIndex.FLAGS] = new BitboardLayer(Convert.ToUInt64("111", 2)); //castling flags - left rook, king, right rook
                this.black[pieceIndex.FLAGS] = new BitboardLayer(Convert.ToUInt64("111", 2));

                getAllLocations(true);
                getAllLocations(false);

                this.white[pieceIndex.ATTACKED_SQUARES] = new BitboardLayer();
                this.black[pieceIndex.ATTACKED_SQUARES] = new BitboardLayer();
                this.white[pieceIndex.ATTACKED_SQUARES] = getAttackedSquares(true);
                this.black[pieceIndex.ATTACKED_SQUARES] = getAttackedSquares(false);

                this.white[pieceIndex.VALID_MOVES] = new BitboardLayer();
                this.black[pieceIndex.VALID_MOVES] = new BitboardLayer();
            } else
            {
                this.white = white;
                this.black = black;
            }
		}
		
		public ChessBoard(ChessBoard c){
            for (int i = 0; i <= pieceIndex.FLAGS; i++)
            {
                white[i] = new BitboardLayer(c.getDict(true)[i]);
                black[i] = new BitboardLayer(c.getDict(false)[i]);
            }
            white_ep = c.getEP(true);
            black_ep = c.getEP(false);
            moveList = new List<int[]>(c.getMoveList());
            moveNum = c.getMoveNum();
		}

        public List<int[]> getMoveList(){ return moveList; }

        public int getMoveNum(){ return moveNum; }

        public int getEP(bool isWhite){ return isWhite ? white_ep : black_ep; }
		
		public BitboardLayer[] getDict(bool isWhite){ return isWhite ? white : black; }

        public void resetBoard()
        {
            //pawn
            ulong temp = Convert.ToUInt64("0000000011111111000000000000000000000000000000000000000000000000", 2);
            this.black[pieceIndex.PAWN] = new BitboardLayer(temp);
            temp = Convert.ToUInt64("0000000000000000000000000000000000000000000000001111111100000000", 2);
            this.white[pieceIndex.PAWN] = new BitboardLayer(temp);
            //rook
            temp = Convert.ToUInt64("1000000100000000000000000000000000000000000000000000000000000000", 2);
            this.black[pieceIndex.ROOK] = new BitboardLayer(temp);
            temp = Convert.ToUInt64("0000000000000000000000000000000000000000000000000000000010000001", 2);
            this.white[pieceIndex.ROOK] = new BitboardLayer(temp);
            //knight
            temp = Convert.ToUInt64("0100001000000000000000000000000000000000000000000000000000000000", 2);
            this.black[pieceIndex.KNIGHT] = new BitboardLayer(temp);
            temp = Convert.ToUInt64("0000000000000000000000000000000000000000000000000000000001000010", 2);
            this.white[pieceIndex.KNIGHT] = new BitboardLayer(temp);
            //bishop
            temp = Convert.ToUInt64("0010010000000000000000000000000000000000000000000000000000000000", 2);
            this.black[pieceIndex.BISHOP] = new BitboardLayer(temp);
            temp = Convert.ToUInt64("0000000000000000000000000000000000000000000000000000000000100100", 2);
            this.white[pieceIndex.BISHOP] = new BitboardLayer(temp);
            //queen
            this.black[pieceIndex.QUEEN] = new BitboardLayer(Convert.ToUInt64("0001000000000000000000000000000000000000000000000000000000000000", 2));
            this.white[pieceIndex.QUEEN] = new BitboardLayer(Convert.ToUInt64("0000000000000000000000000000000000000000000000000000000000010000", 2));
            //king
            this.black[pieceIndex.KING] = new BitboardLayer(Convert.ToUInt64("0000100000000000000000000000000000000000000000000000000000000000", 2));
            this.white[pieceIndex.KING] = new BitboardLayer(Convert.ToUInt64("0000000000000000000000000000000000000000000000000000000000001000", 2));
            //set flags used for castling
            this.white[pieceIndex.FLAGS] = new BitboardLayer(Convert.ToUInt64("111", 2)); //castling flags - left rook, king, right rook
            this.black[pieceIndex.FLAGS] = new BitboardLayer(Convert.ToUInt64("111", 2));

            getAllLocations(true);
            getAllLocations(false);
            this.white[pieceIndex.ATTACKED_SQUARES] = getAttackedSquares(true);
            this.black[pieceIndex.ATTACKED_SQUARES] = getAttackedSquares(false);

            moveList = new List<int[]>();
            moveNum = 0;

            white_ep = -1;
            black_ep = -1;
        }

        public void undoMove(bool isWhite)
        {
            bool pawnIsOn11 = black[pieceIndex.PAWN].trueAtIndex(11);
            BitboardLayer[] dict = isWhite ? white : black;
            BitboardLayer[] enemyDict = isWhite ? black : white;
            int[] lastMove = moveList[moveList.Count - 1];
            //special case for castling
            for (int i = 0; i <= pieceIndex.KING; i++)
            {
                if (dict[i].trueAtIndex(lastMove[1]))
                {
                    dict[i].setAtIndex(lastMove[0], true);
                    dict[i].setAtIndex(lastMove[1], false);
                }
            }

            if (dict[pieceIndex.KING].trueAtIndex(lastMove[1]) && Math.Abs(lastMove[0] - lastMove[1]) == 2)
            {
                //castling right, aka kingside
                if (dict[1].getLayerData() > dict[0].getLayerData())
                {
                    dict[pieceIndex.ROOK].setAtIndex(lastMove[0] + 1, false);
                    dict[pieceIndex.ROOK].setAtIndex(lastMove[0] + 3, true);
                } else { //castling left, aka queenside
                    dict[pieceIndex.ROOK].setAtIndex(lastMove[0] - 1, false);
                    dict[pieceIndex.ROOK].setAtIndex(lastMove[0] - 4, true);
                }
            } else {
                //pieces are restored from captures
                if (lastMove[2] >= 0) enemyDict[lastMove[2]].setAtIndex(lastMove[1], true);
                //pieces are removed from promotions
                if (lastMove[3] >= 0) dict[lastMove[3]].setAtIndex(lastMove[1], false);
            }

            white_ep = lastMove[4];
            black_ep = lastMove[5];

            getAllLocations(true);
            getAllLocations(false);
            white[pieceIndex.ATTACKED_SQUARES].setLayerData(getAttackedSquares(true).getLayerData());
            black[pieceIndex.ATTACKED_SQUARES].setLayerData(getAttackedSquares(false).getLayerData());
            moveList.RemoveAt(moveList.Count - 1);
        }
		
		public void movePiece(bool isWhite, int begin, int end, bool isTest = false){
            bool pawnIsOn11 = black[pieceIndex.PAWN].trueAtIndex(11);
            int captureLayer = -1;
            int promotionLayer = -1;
			BitboardLayer[] dict = isWhite ? white : black;
			BitboardLayer[] enemyDict = isWhite ? black : white;
			for (int i = 0; i <= pieceIndex.KING; i++){
				if (dict[i].trueAtIndex(begin)){
					dict[i].setAtIndex(begin, false);
					dict[i].setAtIndex(end, true);
					if (enemyDict[pieceIndex.ALL_LOCATIONS].trueAtIndex(end)){
						for (int j = 0; j <= pieceIndex.KING; j++){
                            if (enemyDict[j].trueAtIndex(end))
                            {
                                enemyDict[j].setAtIndex(end, false);
                                captureLayer = j;
                            }
                        }
					}
                    //if king or rook moving, invalidate castle
                    if (i == pieceIndex.KING) dict[pieceIndex.FLAGS].setLayerData(dict[pieceIndex.FLAGS].getLayerData() & ~flagIndex.KING_CASTLE);
                    if (i == pieceIndex.ROOK && (begin / 8 == 0 || begin / 8 == 7) && begin % 8 == 0 && (dict[pieceIndex.FLAGS].getLayerData() & flagIndex.LEFT_ROOK_CASTLE) > 0) dict[pieceIndex.FLAGS].setLayerData(dict[pieceIndex.FLAGS].getLayerData() & ~flagIndex.LEFT_ROOK_CASTLE);
                    if (i == pieceIndex.ROOK && (begin / 8 == 0 || begin / 8 == 7) && begin % 8 == 7 && (dict[pieceIndex.FLAGS].getLayerData() & flagIndex.RIGHT_ROOK_CASTLE) > 0) dict[pieceIndex.FLAGS].setLayerData(dict[pieceIndex.FLAGS].getLayerData() & ~flagIndex.RIGHT_ROOK_CASTLE);

                    //if castling, move rook to other side of king
                    if (i == pieceIndex.KING){
                        int rookIndex = isWhite ? 56 : 0;
                        int dir = begin < end ? 1 : -1;
                        if (dir == 1) rookIndex += 7;
                        if ((end - begin) * dir == 2) { //king is castling
                            dict[pieceIndex.ROOK].setAtIndex(rookIndex, false);
                            dict[pieceIndex.ROOK].setAtIndex(begin + dir, true);
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
                        if (end == enemy_ep) enemyDict[pieceIndex.PAWN].setAtIndex(enemy_ep - dir * 8, false);

                        //promotion
                        if (!isTest && ((end / 8 == 0 && isWhite) || (end / 8 == 7 && !isWhite))) promotionLayer = promotePiece(end, isWhite);
                    } else {
                        if (isWhite) white_ep = -1;
                        else black_ep = -1;
                    }
                }
            }
            moveList.Add(new int[] { begin, end, captureLayer, promotionLayer, white_ep, black_ep });
            moveNum++;
            getAllLocations(isWhite);
            if (captureLayer != -1) getAllLocations(!isWhite);
            dict[pieceIndex.ATTACKED_SQUARES].setLayerData(getAttackedSquares(isWhite).getLayerData());
            enemyDict[pieceIndex.ATTACKED_SQUARES].setLayerData(getAttackedSquares(!isWhite).getLayerData());
        }

        int promotePiece(int index, bool isWhite)
        {
            promotionForm pf = new promotionForm(isWhite);
            pf.ShowDialog();
            int promotionChoice = pf.result;
            BitboardLayer[] dict = isWhite ? white : black;
            dict[pieceIndex.PAWN].setAtIndex(index, false);
            dict[promotionChoice].setAtIndex(index, true);
            return promotionChoice;
        }

        void promotePiece(int index, bool isWhite, int promotionChoice)
        {
            BitboardLayer[] dict = isWhite ? white : black;
            dict[pieceIndex.PAWN].setAtIndex(index, false);
            dict[promotionChoice].setAtIndex(index, true);
        }
		
		public void getAllLocations(bool isWhite){
			BitboardLayer[] dict = isWhite ? white : black;
			dict[pieceIndex.ALL_LOCATIONS] = new BitboardLayer();
			for (int i = 0; i <= pieceIndex.KING; i++){
				dict[pieceIndex.ALL_LOCATIONS].setLayerData(dict[pieceIndex.ALL_LOCATIONS].getLayerData() | dict[i].getLayerData());
			}
		}		
		
		public BitboardLayer getValidMoves(bool isWhite, int index, BitboardLayer enemyAllPos = null, bool applyCheckLimits = true, bool fromAttackedSq = false){
            BitboardLayer[] w = white;
            BitboardLayer[] b = black;
			if (enemyAllPos == null) enemyAllPos = isWhite ? black[pieceIndex.ALL_LOCATIONS] : white[pieceIndex.ALL_LOCATIONS];
			BitboardLayer retVal = new BitboardLayer();
			BitboardLayer[] dict = isWhite ? w : b;
			BitboardLayer newAllPos = dict[pieceIndex.ALL_LOCATIONS];
			BitboardLayer[] enemyDict = isWhite ? b : w;
			for (int s = 0; s <= pieceIndex.KING; s++){
				if(dict[s].trueAtIndex(index)){
					bool isKingMove = false;
					switch(s){
						case pieceIndex.PAWN:
                            int enemy_ep = isWhite ? black_ep : white_ep;
							int direction = isWhite ? -1 : 1;
							int pawnFile = isWhite ? 6 : 1;
							int moveIndex = index + (direction * 8);
							//move one in dir
							if (moveIndex >= 0 && moveIndex <= 63 && 
							    !enemyDict[pieceIndex.ALL_LOCATIONS].trueAtIndex(moveIndex) &&
                                !fromAttackedSq) retVal.setAtIndex(moveIndex, true);
							
							//captures
							if (moveIndex - 1 >= 0 && moveIndex - 1 <= 63 && //in bounds
								(moveIndex - 1) / 8 == moveIndex / 8 && //on same rank
                                enemyDict[pieceIndex.ALL_LOCATIONS].trueAtIndex(moveIndex - 1)) retVal.setAtIndex(moveIndex - 1, true);
							if (moveIndex + 1 >= 0 && moveIndex + 1 <= 63 && //in bounds
								(moveIndex + 1) / 8 == moveIndex / 8 && //on same rank
                                enemyDict[pieceIndex.ALL_LOCATIONS].trueAtIndex(moveIndex + 1)) retVal.setAtIndex(moveIndex + 1, true);
							
							//move two in dir
							if (index / 8 == pawnFile && !enemyDict[pieceIndex.ALL_LOCATIONS].trueAtIndex(moveIndex))
                            {
								moveIndex += direction * 8;
								if (moveIndex >= 0 && moveIndex <= 63 && !enemyDict[pieceIndex.ALL_LOCATIONS].trueAtIndex(moveIndex) && !fromAttackedSq) retVal.setAtIndex(moveIndex, true);
							}

                            //en passant captures
                            if (index + direction * 7 == enemy_ep || index + direction * 9 == enemy_ep) retVal.setAtIndex(enemy_ep, true);
                            break;
							
						case pieceIndex.ROOK:
							for (int dir = -1; dir < 2; dir += 2){
								for (int ext = 1; ext <= 8; ext++){
									int testInd = index + (dir * ext);
									if (checkCollision(index, testInd, newAllPos, enemyAllPos, fromAttackedSq) || index / 8 != testInd / 8 || testInd < 0 || testInd > 63) break;
									retVal.setAtIndex(testInd, true);
								}
								for (int ext = 0; ext < 8; ext++) {
									int testInd = index + (8 * dir * ext);
									if (checkCollision(index, testInd, newAllPos, enemyAllPos, fromAttackedSq) || index % 8 != testInd % 8 || testInd < 0 || testInd > 63) break;
									retVal.setAtIndex(testInd, true);
								}
							}
							break;
						case pieceIndex.KNIGHT:
							int[] diffs = {17, 15, 10, 6, -6, -10, -15, -17};
							int[] rightRows = {16, 16, 8, 8, -8, -8, -16, -16};
							for (int i = 0; i < diffs.Length; i++){
								if ((index + diffs[i]) / 8 == (index + rightRows[i]) / 8 && index + diffs[i] >= 0 && index + diffs[i] <= 63 && !dict[pieceIndex.ALL_LOCATIONS].trueAtIndex(index + diffs[i])) retVal.setAtIndex(index + diffs[i], true);
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
									retVal.setAtIndex(temp, true);
								}
							}
							break;
						case pieceIndex.QUEEN:
                            //move like a rook
							for (int dir = -1; dir < 2; dir += 2){
								for (int ext = 1; ext <= 8; ext++){
									int testInd = index + (dir * ext);
									if (checkCollision(index, testInd, newAllPos, enemyAllPos, fromAttackedSq) || index / 8 != testInd / 8 || testInd < 0 || testInd > 63) break;
									retVal.setAtIndex(testInd, true);
								}
								for (int ext = 0; ext < 8; ext++) {
									int testInd = index + (8 * dir * ext);
									if (checkCollision(index, testInd, newAllPos, enemyAllPos, fromAttackedSq) || index % 8 != testInd % 8 || testInd < 0 || testInd > 63) break;
									retVal.setAtIndex(testInd, true);
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
									retVal.setAtIndex(temp, true);
								}
							}
							break;
						case pieceIndex.KING:
							isKingMove = true;
                            int currRow = index / 8;
							for (int a = -1; a <= 1; a++){
								for (int c = -1; c <= 1; c++){
									int newIndex = index + (8 * a) + c;
									if (newIndex >= 0 && newIndex <= 63 && !newAllPos.trueAtIndex(newIndex)) retVal.setAtIndex(newIndex, true);
								}
							}
                            if ((dict[pieceIndex.FLAGS].getLayerData() & flagIndex.KING_CASTLE) > 0 && !enemyDict[pieceIndex.ATTACKED_SQUARES].trueAtIndex(index)) //king can castle
                            {
                                if((dict[pieceIndex.FLAGS].getLayerData() & flagIndex.LEFT_ROOK_CASTLE) > 0 &&
                                    !enemyDict[pieceIndex.ATTACKED_SQUARES].trueAtIndex(index - 1) && !dict[pieceIndex.ALL_LOCATIONS].trueAtIndex(index - 1) &&
                                    !enemyDict[pieceIndex.ATTACKED_SQUARES].trueAtIndex(index - 2) && !dict[pieceIndex.ALL_LOCATIONS].trueAtIndex(index - 2))
                                    retVal.setAtIndex(index + 2, true); //king can castle right

                                if ((dict[pieceIndex.FLAGS].getLayerData() & flagIndex.RIGHT_ROOK_CASTLE) > 0 && 
                                    !enemyDict[pieceIndex.ATTACKED_SQUARES].trueAtIndex(index + 1) && !dict[pieceIndex.ALL_LOCATIONS].trueAtIndex(index + 1) &&
                                    !enemyDict[pieceIndex.ATTACKED_SQUARES].trueAtIndex(index + 2) && !dict[pieceIndex.ALL_LOCATIONS].trueAtIndex(index + 2)) retVal.setAtIndex(index + 2, true); //king can castle right
                            }
							break;
					}
					retVal.setAtIndex(index, false);
					if (applyCheckLimits) return applyCheck(isWhite, index, retVal, isKingMove);
					return retVal;
				}
			}
			return new BitboardLayer();
		}
		
		BitboardLayer applyCheck(bool isWhite, int beginIndex, BitboardLayer possibleMoves, bool isKingMove){
			BitboardLayer[] myDict = isWhite ? white : black;
			BitboardLayer[] enemyDict = isWhite ? black : white;
			BitboardLayer retVal = new BitboardLayer(possibleMoves);
			BitboardLayer myBoard;
			
            myBoard = new BitboardLayer(myDict[pieceIndex.ALL_LOCATIONS]);

            foreach (int i in possibleMoves.getTrueIndicies()){	//for each valid move, use a test allLocations to see if it's moving into check
                    BitboardLayer kingPos = new BitboardLayer(myDict[pieceIndex.KING]);
                    if (isKingMove) kingPos.setLayerData((ulong)1 << (63 - i));
                    ChessBoard cboard = new ChessBoard(this);
                    cboard.movePiece(isWhite, beginIndex, i, true);
                    BitboardLayer newAttackedSquares = cboard.getAttackedSquares(!isWhite);
                    if ((kingPos.getLayerData() & newAttackedSquares.getLayerData()) > 0) retVal.setAtIndex(i, false);
                    myDict[pieceIndex.ALL_LOCATIONS] = myBoard;
			}
			return retVal;
		}		               
		
		//POST: gives squares attacked by isWhite given the current configuration
		public BitboardLayer getAttackedSquares(bool isWhite){
			BitboardLayer newAllPos = isWhite ? black[pieceIndex.ALL_LOCATIONS] : white[pieceIndex.ALL_LOCATIONS];
			BitboardLayer retVal = new BitboardLayer();
			BitboardLayer[] myDict = isWhite ? white : black;
			BitboardLayer myPcs = new BitboardLayer(myDict[pieceIndex.ALL_LOCATIONS]);
			foreach (int i in myPcs.getTrueIndicies()) { 
                retVal.setLayerData(retVal.getLayerData() | getValidMoves(isWhite, i, newAllPos, false, true).getLayerData());
			}
			return retVal;
		}
		
		bool checkCollision(int beginIndex, int endIndex, BitboardLayer myPcs, BitboardLayer enemyPcs, bool attackedSq){
			int dir = beginIndex < endIndex ? 1 : -1;
			if ((beginIndex - endIndex) % 8 == 0){ //moving vertically
				return extendCurrPath(beginIndex, endIndex, myPcs, enemyPcs, 8, dir, attackedSq);
			} if (beginIndex / 8 == endIndex / 8) { //moving horizontally
				while(dir * beginIndex < dir * endIndex && beginIndex / 8 == endIndex / 8){
					beginIndex += dir;
					if (myPcs.trueAtIndex(beginIndex)) {
						if (attackedSq && beginIndex == endIndex) return false;
						return true;
					} if (enemyPcs.trueAtIndex(beginIndex)){
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
		
		bool extendCurrPath(int beginIndex, int endIndex, BitboardLayer myPcs, BitboardLayer enemyPcs, int extend, int dir, bool attackedSq){
			while(dir * beginIndex < dir * endIndex && (beginIndex >= 0 && beginIndex <= 63)){
					beginIndex += dir * extend;
					if (myPcs.trueAtIndex(beginIndex)) {
						if (attackedSq && beginIndex == endIndex) return false;
						return true;
					} if (enemyPcs.trueAtIndex(beginIndex)){
						if (beginIndex != endIndex) return true;
						return false;
					}
				}
			return false;
		}

        public bool isInCheck(bool isWhite)
        {
            BitboardLayer[] dict = isWhite ? white : black;
            BitboardLayer[] enemyDict = isWhite ? black : white;
            if ((enemyDict[pieceIndex.ATTACKED_SQUARES].getLayerData() & dict[pieceIndex.KING].getLayerData()) > 0) return true;
            return false;
        }
		
		public bool checkForMate(bool isWhite){ //checks to see if isWhite has checkmate
            BitboardLayer[] dict = isWhite ? white : black;
			BitboardLayer[] enemyDict = isWhite ? black : white;
			foreach (int i in enemyDict[pieceIndex.ALL_LOCATIONS].getTrueIndicies())
            {
                if (getValidMoves(!isWhite, i).getLayerData() != 0uL) return false;
			}
			return (dict[pieceIndex.KING].getLayerData() & enemyDict[pieceIndex.ATTACKED_SQUARES].getLayerData()) > 0;
		}
}
}
