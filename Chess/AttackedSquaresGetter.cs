using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chess
{
    class AttackedSquaresGetter {
        ChessBoard c;

        BitboardLayer[] pieceLocations; //0 is white, 1 is black

        BitboardLayer[][] currAttackedSquares;
        BitboardLayer[] kingAttackedSquares;
        BitboardLayer[] allAttackedSq;

        BitboardLayer[][] currValidMoves;
        BitboardLayer[] allValidMoves;

        List<int>[] currPinnedPcs;
        public AttackedSquaresGetter(ChessBoard c)
        {
            this.c = c;
            pieceLocations = new BitboardLayer[2];
            currAttackedSquares = new BitboardLayer[2][];
            kingAttackedSquares = new BitboardLayer[2];
            allAttackedSq = new BitboardLayer[2];
            allValidMoves = new BitboardLayer[2];
            currValidMoves = new BitboardLayer[2][];
            currPinnedPcs = new List<int>[2];
            for (int i = 0; i < 2; i++)
            {
                pieceLocations[i] = new BitboardLayer();
                currAttackedSquares[i] = new BitboardLayer[64];
                kingAttackedSquares[i] = new BitboardLayer();
                currValidMoves[i] = new BitboardLayer[64];
                allValidMoves[i] = new BitboardLayer();
                allAttackedSq[i] = new BitboardLayer();
                currPinnedPcs[i] = new List<int>();
            }
        }

        //returns the indicies we need to change getValidMoves / getAttackedSquares for
        int[] getChangedIndices(bool isWhite)
        {
            int colorIndex = isWhite ? 0 : 1;
            int oppositeColorIndex = isWhite ? 1 : 0;
            List<int> retVal = new List<int>();
            int[] lastMove = c.getLastMove();

            BitboardLayer oldAttackedSquares = currAttackedSquares[colorIndex][lastMove[0]];
            BitboardLayer changedIndicies = new BitboardLayer(pieceLocations[colorIndex].getLayerData() ^ c.getDict(isWhite)[pieceIndex.ALL_LOCATIONS].getLayerData());
            for (int i = 0; i < 64; i++)
            {
                //cases for other pieces: was under attack by (b/c pins) or was attacking moved piece

                //attacked by && has no valid moves (therefore may be pinned)
                if (oldAttackedSquares.trueAtIndex(i) && currValidMoves[oppositeColorIndex][i].getLayerData() == 0uL) retVal.Add(i);

                //attacking
                else if (currAttackedSquares[oppositeColorIndex][i].trueAtIndex(lastMove[0])) retVal.Add(i); //attacking (opposite color)
                else if (currAttackedSquares[colorIndex][i].trueAtIndex(lastMove[0])) retVal.Add(i); //attacking (same color)
            }
            retVal.Add(lastMove[1]);
            return retVal.ToArray();
        }

        //updates both black and white's attacked squares
        public void updatePosition(bool isWhite)
        {
            int colorIndex = isWhite ? 0 : 1;
            int oppositeColorIndex = isWhite ? 1 : 0;

            //get changed indices before we update all locations
            int[] changedIndicies = getChangedIndices(isWhite);
            pieceLocations[colorIndex] = c.getDict(isWhite)[pieceIndex.ALL_LOCATIONS];

            int oldLocation = c.getLastMove()[1];
            currAttackedSquares[colorIndex][oldLocation] = new BitboardLayer();

            foreach (int location in changedIndicies)
            {
                bool currPieceWhite = false;
                if (pieceLocations[0].trueAtIndex(location)) currPieceWhite = true;

            }
        }

        public void updateMoves()
        {
            //first update attackedSquares and getValidMoves
            //because that allows us to get valid moves
        }

        public BitboardLayer getValidMoves(bool isWhite, int index, BitboardLayer enemyAllPos = null, bool applyCheckLimits = true, bool fromAttackedSq = false)
        {
            BitboardLayer[] w = c.getDict(true);
            BitboardLayer[] b = c.getDict(false);
            if (enemyAllPos == null) enemyAllPos = isWhite ? b[pieceIndex.ALL_LOCATIONS] : w[pieceIndex.ALL_LOCATIONS];
            BitboardLayer retVal = new BitboardLayer();
            BitboardLayer[] dict = isWhite ? w : b;
            BitboardLayer newAllPos = dict[pieceIndex.ALL_LOCATIONS];
            BitboardLayer[] enemyDict = isWhite ? b : w;
            int white_ep = c.getEP(true);
            int black_ep = c.getEP(false);
            int colorIndex = isWhite ? 0 : 1;
            int oppositeColorIndex = isWhite ? 0 : 1;
            for (int s = 0; s <= pieceIndex.KING; s++)
            {
                if (dict[s].trueAtIndex(index))
                {
                    bool isKingMove = false;
                    switch (s)
                    {
                        case pieceIndex.PAWN:
                            int enemy_ep = isWhite ? black_ep : white_ep;
                            int direction = isWhite ? -1 : 1;
                            int pawnFile = isWhite ? 6 : 1;
                            int moveIndex = index + (direction * 8);
                            //move one in dir
                            if (moveIndex >= 0 && moveIndex <= 63 &&
                                !enemyDict[pieceIndex.ALL_LOCATIONS].trueAtIndex(moveIndex) && !dict[pieceIndex.ALL_LOCATIONS].trueAtIndex(moveIndex) &&
                                !fromAttackedSq)
                                retVal.setAtIndex(moveIndex, true);

                            //captures
                            if (moveIndex - 1 >= 0 && moveIndex - 1 <= 63 && //in bounds
                                (moveIndex - 1) / 8 == moveIndex / 8 && //on same rank
                                enemyDict[pieceIndex.ALL_LOCATIONS].trueAtIndex(moveIndex - 1))
                                retVal.setAtIndex(moveIndex - 1, true);
                            if (moveIndex + 1 >= 0 && moveIndex + 1 <= 63 && //in bounds
                                (moveIndex + 1) / 8 == moveIndex / 8 && //on same rank
                                enemyDict[pieceIndex.ALL_LOCATIONS].trueAtIndex(moveIndex + 1))
                                retVal.setAtIndex(moveIndex + 1, true);

                            //move two in dir
                            if (index / 8 == pawnFile && !enemyDict[pieceIndex.ALL_LOCATIONS].trueAtIndex(moveIndex) && !dict[pieceIndex.ALL_LOCATIONS].trueAtIndex(moveIndex))
                            {
                                moveIndex += direction * 8;
                                if (moveIndex >= 0 && moveIndex <= 63 && !enemyDict[pieceIndex.ALL_LOCATIONS].trueAtIndex(moveIndex) && !dict[pieceIndex.ALL_LOCATIONS].trueAtIndex(moveIndex) && !fromAttackedSq) retVal.setAtIndex(moveIndex, true);
                            }

                            //en passant captures
                            if (index + direction * 7 == enemy_ep || index + direction * 9 == enemy_ep) retVal.setAtIndex(enemy_ep, true);
                            break;

                        case pieceIndex.ROOK:
                            for (int dir = -1; dir < 2; dir += 2)
                            {
                                for (int ext = 1; ext <= 8; ext++)
                                {
                                    int testInd = index + (dir * ext);
                                    if (checkCollision(index, testInd, newAllPos, enemyAllPos, fromAttackedSq) || index / 8 != testInd / 8 || testInd < 0 || testInd > 63) break;
                                    retVal.setAtIndex(testInd, true);
                                }
                                for (int ext = 0; ext < 8; ext++)
                                {
                                    int testInd = index + (8 * dir * ext);
                                    if (checkCollision(index, testInd, newAllPos, enemyAllPos, fromAttackedSq) || index % 8 != testInd % 8 || testInd < 0 || testInd > 63) break;
                                    retVal.setAtIndex(testInd, true);
                                }
                            }
                            break;
                        case pieceIndex.KNIGHT:
                            int[] diffs = { 17, 15, 10, 6, -6, -10, -15, -17 };
                            int[] rightRows = { 16, 16, 8, 8, -8, -8, -16, -16 };
                            for (int i = 0; i < diffs.Length; i++)
                            {
                                if ((index + diffs[i]) / 8 == (index + rightRows[i]) / 8 && index + diffs[i] >= 0 && index + diffs[i] <= 63 && !dict[pieceIndex.ALL_LOCATIONS].trueAtIndex(index + diffs[i])) retVal.setAtIndex(index + diffs[i], true);
                            }
                            break;
                        case pieceIndex.BISHOP:
                            diffs = new int[] { -7, 7, -9, 9 };
                            foreach (int diff in diffs)
                            {
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
                            }
                            break;
                        case pieceIndex.QUEEN:
                            //move like a rook
                            for (int dir = -1; dir < 2; dir += 2)
                            {
                                for (int ext = 1; ext <= 8; ext++)
                                {
                                    int testInd = index + (dir * ext);
                                    if (checkCollision(index, testInd, newAllPos, enemyAllPos, fromAttackedSq) || index / 8 != testInd / 8 || testInd < 0 || testInd > 63) break;
                                    retVal.setAtIndex(testInd, true);
                                }
                                for (int ext = 0; ext < 8; ext++)
                                {
                                    int testInd = index + (8 * dir * ext);
                                    if (checkCollision(index, testInd, newAllPos, enemyAllPos, fromAttackedSq) || index % 8 != testInd % 8 || testInd < 0 || testInd > 63) break;
                                    retVal.setAtIndex(testInd, true);
                                }
                            }
                            //move like a bishop
                            diffs = new int[] { -7, 7, -9, 9 };
                            foreach (int diff in diffs)
                            {
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
                            }
                            break;
                        case pieceIndex.KING:
                            isKingMove = true;
                            //int currRow = index / 8;
                            int currCol = index % 8;
                            for (int a = -1; a <= 1; a++)
                            {
                                for (int c = -1; c <= 1; c++)
                                {
                                    int newIndex = index + (8 * a) + c;
                                    if (newIndex >= 0 && newIndex <= 63 && !newAllPos.trueAtIndex(newIndex) && newIndex % 8 - currCol >= -1 && newIndex % 8 - currCol <= 1) retVal.setAtIndex(newIndex, true);
                                }
                            }
                            if ((dict[pieceIndex.FLAGS].getLayerData() & flagIndex.KING_CASTLE) > 0 && !enemyDict[pieceIndex.ATTACKED_SQUARES].trueAtIndex(index)) //king can castle
                            {
                                if ((dict[pieceIndex.FLAGS].getLayerData() & flagIndex.RIGHT_ROOK_CASTLE) > 0 &&
                                    !enemyDict[pieceIndex.ATTACKED_SQUARES].trueAtIndex(index + 1) && !enemyDict[pieceIndex.ALL_LOCATIONS].trueAtIndex(index + 1) && !dict[pieceIndex.ALL_LOCATIONS].trueAtIndex(index + 1) &&
                                    !enemyDict[pieceIndex.ATTACKED_SQUARES].trueAtIndex(index + 2) && !enemyDict[pieceIndex.ALL_LOCATIONS].trueAtIndex(index + 2) && !dict[pieceIndex.ALL_LOCATIONS].trueAtIndex(index + 2))
                                    retVal.setAtIndex(index + 2, true); //king can castle right

                                if ((dict[pieceIndex.FLAGS].getLayerData() & flagIndex.LEFT_ROOK_CASTLE) > 0 &&
                                    !enemyDict[pieceIndex.ATTACKED_SQUARES].trueAtIndex(index - 1) && !dict[pieceIndex.ALL_LOCATIONS].trueAtIndex(index - 1) && !enemyDict[pieceIndex.ALL_LOCATIONS].trueAtIndex(index - 1) &&
                                    !enemyDict[pieceIndex.ATTACKED_SQUARES].trueAtIndex(index - 2) && !dict[pieceIndex.ALL_LOCATIONS].trueAtIndex(index - 2) && !enemyDict[pieceIndex.ALL_LOCATIONS].trueAtIndex(index - 2))
                                    retVal.setAtIndex(index - 2, true); //king can castle left
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

        void extendPath(bool isWhite, int begin, int interval)
        {
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
            while (whatIsFor != 0)
            { //we're still looking for things
                s += interval;
                if (s > 63 || s < 0 || ((interval == 1 || interval == -1) && s / 8 == begin / 8)) break;
                if (checkCollision(isWhite, s) == 2)
                {
                    if ((whatIsFor & (1 << atSqShift)) > 0) currAttackedSquares[colorIndex][begin].setAtIndex(s, true);
                    break;
                }
                if (checkCollision(isWhite, s) == 1 && ((whatIsFor & (1 << atSqShift)) > 0)) //1 means
                {
                    currAttackedSquares[colorIndex][begin].setAtIndex(s, true);
                }

            }
        }

        //accessor method
        public BitboardLayer getAttackedSquares(bool isWhite)
        {
            int colorIndex = isWhite ? 0 : 1;
            return null;
        }

        bool trueAtIndex(ulong layerData, int i){
            return ((1uL << (63 - i)) & layerData) > 0;
        }

    }
}
