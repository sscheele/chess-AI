using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chess
{
    public class AttackedSquaresGetter {
        ChessBoard c;

        BitboardLayer[] pieceLocations; //0 is white, 1 is black

        BitboardLayer[][] currAttackedSquares;
        BitboardLayer[] kingAttackedSquares;
        BitboardLayer[] allAttackedSq;

        BitboardLayer[][] currValidMoves;
        BitboardLayer[] allValidMoves;

        List<int[]>[] currPinnedPcs; //format: pinner, pinned piece, interval

        bool[][] canCastle; //white{right_castle, left_castle}, black{right_castle, left_castle}
        public AttackedSquaresGetter(ChessBoard c)
        {
            this.c = c;
            pieceLocations = new BitboardLayer[2];
            currAttackedSquares = new BitboardLayer[2][];
            kingAttackedSquares = new BitboardLayer[2];
            allAttackedSq = new BitboardLayer[2];
            allValidMoves = new BitboardLayer[2];
            currValidMoves = new BitboardLayer[2][];
            currPinnedPcs = new List<int[]>[2];
            for (int i = 0; i < 2; i++)
            {
                pieceLocations[i] = new BitboardLayer();
                currAttackedSquares[i] = new BitboardLayer[64];
                kingAttackedSquares[i] = new BitboardLayer();
                currValidMoves[i] = new BitboardLayer[64];
                allValidMoves[i] = new BitboardLayer();
                allAttackedSq[i] = new BitboardLayer();
                currPinnedPcs[i] = new List<int[]>();
                canCastle[i] = new bool[] { false, false };
            }
        }

        //returns the indicies we need to change getValidMoves / getAttackedSquares for
        int[] getChangedIndices(bool isWhite, int[] lastMove)
        {
            int colorIndex = isWhite ? 0 : 1;
            int oppositeColorIndex = isWhite ? 1 : 0;
            List<int> retVal = new List<int>();

            BitboardLayer oldAttackedSquares = currAttackedSquares[colorIndex][lastMove[0]];
            BitboardLayer changedIndicies = new BitboardLayer(pieceLocations[colorIndex].getLayerData() ^ c.getDict(isWhite)[pieceIndex.ALL_LOCATIONS].getLayerData());
            for (int i = 0; i < 64; i++){
                //cases for other pieces: was under attack by (b/c pins) or was attacking moved piece
                //pinned by
                foreach (int[] pin in currPinnedPcs[colorIndex]) {
                    if (pin[0] == i) retVal.Add(i);
                }
                if (currAttackedSquares[oppositeColorIndex][i].trueAtIndex(lastMove[0])) retVal.Add(i); //was attacking (opposite color)
                if (currAttackedSquares[oppositeColorIndex][i].trueAtIndex(lastMove[1])) retVal.Add(i); //is attacking (opposite color)
            }
            for (int i = 0; i < 64; i++)
            {
                if (currAttackedSquares[colorIndex][i].trueAtIndex(lastMove[0])) retVal.Add(i); //was attacking (same color)
                if (currAttackedSquares[colorIndex][i].trueAtIndex(lastMove[1])) retVal.Add(i); //is attacking (same color)
            }
            retVal.Add(lastMove[1]);
            retVal.AddRange(changedIndicies.getTrueIndicies());
            return retVal.ToArray();
        }

        //updates both black and white's attacked squares
        public void updatePosition(bool isWhite, int[] move)
        {
            int colorIndex = isWhite ? 0 : 1;
            int oppositeColorIndex = isWhite ? 1 : 0;

            //get changed indices before we update all locations
            int[] changedIndicies = getChangedIndices(isWhite, move);
            pieceLocations[colorIndex] = c.getDict(isWhite)[pieceIndex.ALL_LOCATIONS];

            int oldLocation = move[0];
            currAttackedSquares[colorIndex][oldLocation] = new BitboardLayer();

            //if move was a capture, nullify all valid moves, pins, and attacked squares
            if (pieceLocations[oppositeColorIndex].trueAtIndex(move[1])) {
                currValidMoves[oppositeColorIndex][move[1]] = new BitboardLayer();
                currAttackedSquares[oppositeColorIndex][move[1]] = new BitboardLayer();
                foreach(int[] pin in currPinnedPcs[oppositeColorIndex])
                {
                    if (pin[0] == move[1]) currPinnedPcs[oppositeColorIndex].Remove(pin);
                }
            }

            foreach (int location in changedIndicies)
            {
                getValidMoves(pieceLocations[1].trueAtIndex(location), location);
                //update valid moves and attacked squares for piece at location
                
                //king's valid moves should be XOR'd with attacked squares
                //because attacked squares won't include position unless being backed up, this works
            }
            //to make sure only valid moves are considered, bitwise-AND each non-king's valid moves with position of each piece attacking the king and their attack vector
            //if double-checked, this makes sure no positions will show up
            //otherwise will automatically limit to capture or block
            foreach (int[] pin in currPinnedPcs[colorIndex])
            {
                //format: pinner, pinned piece, interval
                ulong vMoveLayer = 0uL;
                for (int i = pin[0]; i >= 0 && i <= 63; i += pin[2])
                {
                    vMoveLayer |= 1uL << (63 - i);
                }
                currValidMoves[oppositeColorIndex][pin[1]].setLayerData(currValidMoves[oppositeColorIndex][pin[1]].getLayerData() & vMoveLayer);

            }
            foreach (int[] pin in currPinnedPcs[oppositeColorIndex])
            {
                ulong vMoveLayer = 0uL;
                for (int i = pin[0]; i >= 0 && i <= 63; i += pin[2])
                {
                    vMoveLayer |= 1uL << (63 - i);
                }
                currValidMoves[colorIndex][pin[1]].setLayerData(currValidMoves[oppositeColorIndex][pin[1]].getLayerData() & vMoveLayer);
            }
        }

        public void getValidMoves(bool isWhite, int index)
        {
            BitboardLayer[] w = c.getDict(true);
            BitboardLayer[] b = c.getDict(false);
            BitboardLayer[] dict = isWhite ? w : b;
            BitboardLayer newAllPos = dict[pieceIndex.ALL_LOCATIONS];
            BitboardLayer[] enemyDict = isWhite ? b : w;
            int white_ep = c.getEP(true);
            int black_ep = c.getEP(false);
            int colorIndex = isWhite ? 0 : 1;
            int oppositeColorIndex = isWhite ? 0 : 1;

            currAttackedSquares[colorIndex][index] = new BitboardLayer();
            currValidMoves[colorIndex][index] = new BitboardLayer();
            foreach (int[] pin in currPinnedPcs[colorIndex])
            {
                if (pin[0] == index) currPinnedPcs[colorIndex].Remove(pin);
            }
            for (int s = 0; s <= pieceIndex.KING; s++)
            {
                if (dict[s].trueAtIndex(index))
                {
                    bool isKingMove = false;
                    BitboardLayer pieceVM = currValidMoves[colorIndex][index];
                    BitboardLayer pieceAS = currAttackedSquares[colorIndex][index];
                    switch (s)
                    {
                        case pieceIndex.PAWN:
                            int enemy_ep = isWhite ? black_ep : white_ep;
                            int direction = isWhite ? -1 : 1;
                            int pawnFile = isWhite ? 6 : 1;
                            int moveIndex = index + (direction * 8);
                            //move one in dir
                            if (moveIndex >= 0 && moveIndex <= 63 && !enemyDict[pieceIndex.ALL_LOCATIONS].trueAtIndex(moveIndex) && !dict[pieceIndex.ALL_LOCATIONS].trueAtIndex(moveIndex))
                                pieceVM.setAtIndex(moveIndex, true);

                            //captures
                            if (moveIndex - 1 >= 0 && moveIndex - 1 <= 63 && (moveIndex - 1) / 8 == moveIndex / 8 && enemyDict[pieceIndex.ALL_LOCATIONS].trueAtIndex(moveIndex - 1))
                            {
                                pieceAS.setAtIndex(moveIndex - 1, true);
                                pieceVM.setAtIndex(moveIndex - 1, true);
                            }
                            if (moveIndex + 1 >= 0 && moveIndex + 1 <= 63 && (moveIndex + 1) / 8 == moveIndex / 8 && enemyDict[pieceIndex.ALL_LOCATIONS].trueAtIndex(moveIndex + 1)){
                                pieceAS.setAtIndex(moveIndex + 1, true);
                                pieceVM.setAtIndex(moveIndex + 1, true);
                            }

                            //move two in dir
                            if (index / 8 == pawnFile && !enemyDict[pieceIndex.ALL_LOCATIONS].trueAtIndex(moveIndex) && !dict[pieceIndex.ALL_LOCATIONS].trueAtIndex(moveIndex))
                            {
                                moveIndex += direction * 8;
                                if (moveIndex >= 0 && moveIndex <= 63 && !enemyDict[pieceIndex.ALL_LOCATIONS].trueAtIndex(moveIndex) && !dict[pieceIndex.ALL_LOCATIONS].trueAtIndex(moveIndex)) pieceVM.setAtIndex(moveIndex, true);
                            }

                            //en passant captures
                            if (index + direction * 7 == enemy_ep || index + direction * 9 == enemy_ep){
                                pieceAS.setAtIndex(enemy_ep, true);
                                pieceVM.setAtIndex(enemy_ep, true);
                            }
                            break;

                        case pieceIndex.ROOK:
                            int[] diffs = new int[] { 8, -8, 1, -1 };
                            foreach (int diff in diffs)
                            {
                                extendPath(isWhite, index, diff);
                            }
                            break;
                        case pieceIndex.KNIGHT:
                            diffs = new int[]{ 17, 15, 10, 6, -6, -10, -15, -17 };
                            int[] rightRows = { 16, 16, 8, 8, -8, -8, -16, -16 };
                            for (int i = 0; i < diffs.Length; i++)
                            {
                                int possibleMove = index + diffs[i];
                                int rightRow = index + rightRows[i];
                                if (possibleMove / 8 == rightRow / 8 && possibleMove >= 0 && possibleMove <= 63)
                                {
                                    if (checkCollision(isWhite, possibleMove) != 2)
                                    {
                                        pieceVM.setAtIndex(possibleMove, true);
                                    }
                                    pieceAS.setAtIndex(possibleMove, true);
                                }
                            }
                            break;
                        case pieceIndex.BISHOP:
                            diffs = new int[] { -7, 7, -9, 9 };
                            foreach (int diff in diffs)
                            {
                                /*
                                int prevTemp = index;
                                for (int ext = 1; ext < 8; ext++)
                                {
                                    int temp = index + (ext * diff);
                                    if (checkCollision(index, temp, newAllPos, enemyAllPos, fromAttackedSq) || temp < 0 || temp > 63 ||
                                        Math.Abs((prevTemp / 8) - (temp / 8)) != 1 || Math.Abs((prevTemp / 8) - (temp / 8)) != 1)
                                        break;
                                    prevTemp = temp;
                                    retVal.setAtIndex(temp, true);
                                }
                                */
                                extendPath(isWhite, index, diff);
                            }
                            break;
                        case pieceIndex.QUEEN:
                            //move like a rook
                            diffs = new int[] { -7, 7, -9, 9, 8, -8, 1, -1 };
                            foreach (int diff in diffs)
                            {
                                extendPath(isWhite, index, diff);
                            }
                            break;
                        case pieceIndex.KING:
                            isKingMove = true;
                            //int currRow = index / 8;
                            int currCol = index % 8;
                            for (int a = -8; a <= 8; a += 8)
                            {
                                for (int c = -1; c <= 1; c++)
                                {
                                    int newIndex = index + a + c;
                                    if (newIndex >= 0 && newIndex <= 63 && newIndex % 8 - currCol >= -1 && newIndex % 8 - currCol <= 1)
                                    {
                                        pieceAS.setAtIndex(newIndex, true);
                                        if (checkCollision(isWhite, newIndex) != 2) pieceVM.setAtIndex(newIndex, true);
                                    }
                                }
                            }
                            if ((dict[pieceIndex.FLAGS].getLayerData() & flagIndex.KING_CASTLE) > 0) //king can castle
                            {
                                if ((dict[pieceIndex.FLAGS].getLayerData() & flagIndex.RIGHT_ROOK_CASTLE) > 0 &&
                                    !enemyDict[pieceIndex.ALL_LOCATIONS].trueAtIndex(index + 1) && !dict[pieceIndex.ALL_LOCATIONS].trueAtIndex(index + 1) &&
                                    !enemyDict[pieceIndex.ALL_LOCATIONS].trueAtIndex(index + 2) && !dict[pieceIndex.ALL_LOCATIONS].trueAtIndex(index + 2))
                                    canCastle[colorIndex][1] = true; //king can castle right

                                if ((dict[pieceIndex.FLAGS].getLayerData() & flagIndex.LEFT_ROOK_CASTLE) > 0 &&
                                    !dict[pieceIndex.ALL_LOCATIONS].trueAtIndex(index - 1) && !enemyDict[pieceIndex.ALL_LOCATIONS].trueAtIndex(index - 1) &&
                                    !dict[pieceIndex.ALL_LOCATIONS].trueAtIndex(index - 2) && !enemyDict[pieceIndex.ALL_LOCATIONS].trueAtIndex(index - 2))
                                    canCastle[colorIndex][0] = true; //king can castle left
                            }
                            break;
                    }
                }
            }
        }

        //return 0 for empty, 1 for enemy, 2 for friend, 3 for enemy king
        int checkCollision(bool isWhite, int i)
        {
            int colorIndex = isWhite ? 0 : 1;
            int oppositeColorIndex = isWhite ? 1 : 0;

            if (pieceLocations[colorIndex].trueAtIndex(i)) return 2;
            if (pieceLocations[oppositeColorIndex].trueAtIndex(i))
            {
                if (c.getDict(!isWhite)[pieceIndex.KING].trueAtIndex(i)) return 3;
                return 1;
            }
            return 0;
        }

        void extendPath(bool isWhite, int begin, int interval) {
            int colorIndex = isWhite ? 0 : 1;
            int oppositeColorIndex = isWhite ? 1 : 0;
            int whatIsFor = 31; //11111
            /*
            1       valid moves
             1      attacked squares
              1     piece in ray (1)
               1    piece in ray (2)
                1   king in ray
            */
            int vMoveShift = 4, atSqShift = 3, onePieceShift = 2, twoPieceShift = 1;
            int s = begin;
            int pinnedPiece = -1;
            BitboardLayer atSq = currAttackedSquares[colorIndex][begin];
            BitboardLayer vMoves = currValidMoves[colorIndex][begin];
            while (whatIsFor != 0)
            { //we're still looking for things
                s += interval;
                if (s > 63 || s < 0 || ((interval == 1 || interval == -1) && s / 8 != begin / 8)) break;
                int collisionStatus = checkCollision(isWhite, s);
                
                //for all cases, add to attacked squares
                if ((whatIsFor & (1 << atSqShift)) > 0) atSq.setAtIndex(s, true);

                if (collisionStatus == 2) break; //attacking friend; since is already in attackedsquares we're done

                if ((whatIsFor & (1 << vMoveShift)) > 0 && collisionStatus != 2) vMoves.setAtIndex(s, true);

                if (collisionStatus == 3) {
                    //say we're attacking the king, then extend one more in dir and add to kingAttackedSquares
                    whatIsFor &= ~1;
                    if (s + interval < 64 && s + interval > -1) kingAttackedSquares[colorIndex].setAtIndex(s + interval, true);
                    break;
                } else if (collisionStatus == 1) {
                    if ((whatIsFor & (1 << onePieceShift)) > 0)
                    {
                        whatIsFor &= ~(1 << onePieceShift);
                        pinnedPiece = s;
                    }
                    else if ((whatIsFor & (1 << twoPieceShift)) > 0 && (whatIsFor & 1) > 0) whatIsFor &= ~(1 << twoPieceShift); 
                    //if there's already a piece on this 'ray' AND we still haven't hit the king, set twoPieceShift to 0
                }
            }
            if (pinnedPiece != -1 && (whatIsFor & (1 << twoPieceShift)) > 0 && (whatIsFor & (1 << onePieceShift)) == 0 && (whatIsFor & 1) == 0) currPinnedPcs[colorIndex].Add(new int[] { begin, pinnedPiece, interval });
        }

        //accessor method
        public BitboardLayer getAttackedSquares(bool isWhite)
        {
            int colorIndex = isWhite ? 0 : 1;
            return null;
        }
        /*
        bool trueAtIndex(ulong layerData, int i){
            return ((1uL << (63 - i)) & layerData) > 0;
        }
        */

    }
}
