/*
 * User: samsc
 * Date: 9/22/2015
 * Time: 12:11 PM
 */
using System;
using System.Collections.Generic;

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
        AttackedSquaresGetter ASG;
		
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
            } else
            {
                this.white = white;
                this.black = black;
            }
            ASG = new AttackedSquaresGetter(this);
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
            ASG = c.getASG();
		}

        public AttackedSquaresGetter getASG() { return ASG; }

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

            moveList = new List<int[]>();
            moveNum = 0;

            white_ep = -1;
            black_ep = -1;

            ASG = new AttackedSquaresGetter(this);
        }

        public int[] getLastMove()
        {
            if (moveList.Count > 0) return moveList[moveList.Count - 1];
            return null;
        }

        public void undoMove(bool isWhite)
        {
            //format of lastMove: { begin, end, captureLayer, promotionLayer, white_ep, black_ep }
            BitboardLayer[] dict = isWhite ? white : black;
            BitboardLayer[] enemyDict = isWhite ? black : white;
            int[] lastMove = moveList[moveList.Count - 1];

            white_ep = lastMove[4];
            black_ep = lastMove[5];

            int i = 0;
            for (i = 0; i <= pieceIndex.KING; i++)
            {
                if (dict[i].trueAtIndex(lastMove[1]))
                {
                    dict[i].setAtIndex(lastMove[0], true);
                    dict[i].setAtIndex(lastMove[1], false);
                    break;
                }
            }

            if (dict[pieceIndex.KING].trueAtIndex(lastMove[1]) && Math.Abs(lastMove[0] - lastMove[1]) == 2)
            {
                //castling right, aka kingside
                if (lastMove[0] < lastMove[1])
                {
                    dict[pieceIndex.ROOK].setAtIndex(lastMove[0] + 1, false);
                    dict[pieceIndex.ROOK].setAtIndex(lastMove[0] + 3, true);
                }
                else
                { //castling left, aka queenside
                    dict[pieceIndex.ROOK].setAtIndex(lastMove[0] - 1, false);
                    dict[pieceIndex.ROOK].setAtIndex(lastMove[0] - 4, true);
                }
                //move king back
                dict[pieceIndex.KING].setAtIndex(lastMove[0], true);
                dict[pieceIndex.KING].setAtIndex(lastMove[1], false);
            } else {
                //take care of regular king moves
                if (dict[pieceIndex.KING].trueAtIndex(lastMove[1]))
                {
                    dict[pieceIndex.KING].setAtIndex(lastMove[0], true);
                    dict[pieceIndex.KING].setAtIndex(lastMove[1], false);
                }
                //pieces are restored from captures
                if (lastMove[2] >= 0) enemyDict[lastMove[2]].setAtIndex(lastMove[1], true);
                if ((!isWhite ? white_ep : black_ep) == lastMove[1] && i == pieceIndex.PAWN)
                {
                    int dir = isWhite ? 1 : -1;
                    enemyDict[pieceIndex.PAWN].setAtIndex(lastMove[1] + (8 * dir), true);
                }
                //pieces are removed from promotions
                if (lastMove[3] >= 0) dict[lastMove[3]].setAtIndex(lastMove[1], false);
            }

            white[pieceIndex.FLAGS].setLayerData((ulong)lastMove[6]);
            black[pieceIndex.FLAGS].setLayerData((ulong)lastMove[7]);

            getAllLocations(true);
            getAllLocations(false);
            /*
            if (black[pieceIndex.KING].getLayerData() == 0uL)
            {
                i++;
                i--;
            }
            */
            ASG.updatePosition(isWhite, new int[] { moveList[moveList.Count - 1][1], moveList[moveList.Count - 1][0] }); //send reversed move so it can know what's going on
            moveList.RemoveAt(moveList.Count - 1);
        }
		
		public void movePiece(bool isWhite, int begin, int end, bool isTest = false){
            int captureLayer = -1;
            int promotionLayer = -1;
            int wFlags = (int)white[pieceIndex.FLAGS].getLayerData();
            int bFlags = (int)black[pieceIndex.FLAGS].getLayerData();
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
                        if ((end / 8 == 0 && isWhite) || (end / 8 == 7 && !isWhite))
                        {
                            if (isTest) promotionLayer = pieceIndex.QUEEN;
                            else promotionLayer = promotePiece(end, isWhite);
                        } 
                    } else {
                        if (isWhite) white_ep = -1;
                        else black_ep = -1;
                    }
                    break;
                }
            }
            moveList.Add(new int[] { begin, end, captureLayer, promotionLayer, white_ep, black_ep, wFlags, bFlags });
            moveNum++;
            getAllLocations(isWhite);
            if (captureLayer != -1) getAllLocations(!isWhite);
            ASG.updatePosition(isWhite, new int[] { begin, end });
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
        
        public bool equals(ChessBoard c)
        {
            BitboardLayer[] test_white = c.getDict(true);
            BitboardLayer[] test_black = c.getDict(false);
            for (int i = 0; i <= pieceIndex.FLAGS; i++)
            {
                if (test_white[i].getLayerData() != white[i].getLayerData()) return false;
                if (test_black[i].getLayerData() != black[i].getLayerData()) return false;
            }
            return true;
        }		

        public BitboardLayer getAllAttackedSq(bool isWhite)
        {
            return ASG.getAllAttackedSquares(isWhite);
        }

        public BitboardLayer getValidMoves(bool isWhite, int index)
        {
            return ASG.getValidMoves(isWhite, index);
        }

        public bool isInCheck(bool isWhite)
        {
            BitboardLayer[] dict = isWhite ? white : black;
            return ((getAllAttackedSq(!isWhite).getLayerData() & dict[pieceIndex.KING].getLayerData()) > 0);
        }
		
		public bool checkForMate(bool isWhite){ //checks to see if isWhite has checkmate
            return isInCheck(isWhite) && ASG.getAllValidMoves(isWhite).getLayerData() == 0;
		}
}
}
