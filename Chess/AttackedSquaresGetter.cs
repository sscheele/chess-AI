using System.Collections.Generic;
using System.Diagnostics;

namespace Chess
{
    public class AttackedSquaresGetter {
        ChessBoard c;

        BitboardLayer[] pieceLocations; //0 is white, 1 is black

        BitboardLayer[][] currAttackedSquares;
        BitboardLayer[] kingAttackedSquares;
        BitboardLayer[] allAttackedSq;

        int[] lastUpdated;

        BitboardLayer[][] currValidMoves;
        BitboardLayer[] allValidMoves;

        List<int[]>[] currPinnedPcs; //format: pinner, pinned piece, vector
        List<int[]>[] currCheckers; //format: checker, vector

        bool[][] canCastle; //white{right_castle, left_castle}, black{right_castle, left_castle}

        int numIterations;
        public AttackedSquaresGetter(ChessBoard c)
        {
            lastUpdated = new int[64];
            this.c = c;
            pieceLocations = new BitboardLayer[2];
            currAttackedSquares = new BitboardLayer[2][];
            kingAttackedSquares = new BitboardLayer[2];
            allAttackedSq = new BitboardLayer[2];
            allValidMoves = new BitboardLayer[2];
            currValidMoves = new BitboardLayer[2][];
            currPinnedPcs = new List<int[]>[2];
            currCheckers = new List<int[]>[2];
            canCastle = new bool[2][];
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
                currCheckers[i] = new List<int[]>();

                for (int j = 0; j < 64; j++)
                {
                    currAttackedSquares[i][j] = new BitboardLayer();
                    currValidMoves[i][j] = new BitboardLayer();
                }
            }
            numIterations = 0;
            updatePosition(true, new int[] { 63, 63 });
            updatePosition(false, new int[] { 0, 0 });
        }

        public BitboardLayer getValidMoves(bool isWhite, int index)
        {
            return new BitboardLayer(currValidMoves[isWhite ? 0 : 1][index]);
        }

        public BitboardLayer getAllValidMoves(bool isWhite)
        {
            return new BitboardLayer(allValidMoves[isWhite ? 0 : 1]);
        }

        public BitboardLayer getAttackedSquares(bool isWhite, int index)
        {
            return new BitboardLayer(currAttackedSquares[isWhite ? 0 : 1][index]);
        }

        public BitboardLayer getAllAttackedSquares(bool isWhite)
        {
            return new BitboardLayer(allAttackedSq[isWhite ? 0 : 1]);
        }

        int[] getChangedIndices(bool isWhite, int[] lastMove)
        {
            int colorIndex = isWhite ? 0 : 1;
            int oppositeColorIndex = isWhite ? 1 : 0;
            HashSet<int> retVal = new HashSet<int>();
            BitboardLayer nums = new BitboardLayer(c.getDict(true)[pieceIndex.ALL_LOCATIONS].getLayerData() | c.getDict(true)[pieceIndex.ALL_LOCATIONS].getLayerData() | (pieceLocations[colorIndex].getLayerData() ^ c.getDict(isWhite)[pieceIndex.ALL_LOCATIONS].getLayerData()));
            foreach(int i in nums.getTrueIndicies())
            {
                retVal.Add(i);
            }
            int[] arrayForm = new int[retVal.Count];
            retVal.CopyTo(arrayForm);
            return arrayForm;
        }

        //returns the indicies we need to change findAttackMove / getAttackedSquares for
        /*
        int[] getChangedIndices(bool isWhite, int[] lastMove)
        {
            int colorIndex = isWhite ? 0 : 1;
            int oppositeColorIndex = isWhite ? 1 : 0;
            HashSet<int> retVal = new HashSet<int>();

            BitboardLayer oldAttackedSquares = new BitboardLayer(currAttackedSquares[colorIndex][lastMove[0]].getLayerData() | currValidMoves[colorIndex][lastMove[0]].getLayerData());
            BitboardLayer changedIndicies = new BitboardLayer(pieceLocations[colorIndex].getLayerData() ^ c.getDict(isWhite)[pieceIndex.ALL_LOCATIONS].getLayerData());
            int[] sidesInOrder;
            if (currCheckers[oppositeColorIndex].Count != 0) sidesInOrder = new int[] { oppositeColorIndex, colorIndex };
            else sidesInOrder = new int[] { colorIndex, oppositeColorIndex };

            int currColorIndex = sidesInOrder[0];
            //int consideredIndex = 1 - currColorIndex; //consideredIndex is the index of the opposite side

            foreach (int index in c.getDict(currColorIndex == 0)[pieceIndex.ALL_LOCATIONS].getTrueIndicies())
            {
                BitboardLayer valAttack = new BitboardLayer(currAttackedSquares[currColorIndex][index].getLayerData() | currValidMoves[currColorIndex][index].getLayerData());
                //BitboardLayer valAttack = new BitboardLayer(currAttackedSquares[consideredIndex][index].getLayerData() | currValidMoves[consideredIndex][index].getLayerData());
                foreach (int changedIndex in changedIndicies.getTrueIndicies())
                {
                    if (valAttack.trueAtIndex(changedIndex)) retVal.Add(index); //was attacking (opposite color)
                }
            }

            currColorIndex = 1 - currColorIndex;
            if (currCheckers[1 - currColorIndex].Count != 0) //if the first side was checking the second side
            {
                foreach (int index in c.getDict(currColorIndex == 0)[pieceIndex.ALL_LOCATIONS].getTrueIndicies())
                {
                    retVal.Add(index);
                }
            } else {
                foreach (int index in c.getDict(currColorIndex == 0)[pieceIndex.ALL_LOCATIONS].getTrueIndicies())
                {
                    BitboardLayer valAttack = new BitboardLayer(currAttackedSquares[currColorIndex][index].getLayerData() | currValidMoves[currColorIndex][index].getLayerData());
                    //BitboardLayer valAttack = new BitboardLayer(currAttackedSquares[consideredIndex][index].getLayerData() | currValidMoves[consideredIndex][index].getLayerData());
                    foreach (int changedIndex in changedIndicies.getTrueIndicies())
                    {
                        if (valAttack.trueAtIndex(changedIndex)) retVal.Add(index); //was attacking (opposite color)
                    }
                    //if (valAttack.trueAtIndex(c.getDict(isWhite)[pieceIndex.KING].getTrueIndicies()[0])) retVal.Add(index); //is checking (opposite color)
                }
            }
            foreach (List<int[]> pinList in currPinnedPcs)
            {
                foreach (int[] pin in pinList)
                {
                    retVal.Add(pin[0]);
                    retVal.Add(pin[1]);
                }
            }
            retVal.Add(lastMove[1]);
            foreach (int i in changedIndicies.getTrueIndicies())
            {
                retVal.Add(i);
            }
            int[] arrayForm = new int[retVal.Count];
            retVal.CopyTo(arrayForm);
            return arrayForm;
        }
        */

        //updates both black and white's attacked squares
        public void updatePosition(bool isWhite, int[] move)
        {
            int colorIndex = isWhite ? 0 : 1;
            int oppositeColorIndex = isWhite ? 1 : 0;

            //get changed indices before we update all locations
            int[] changedIndicies = getChangedIndices(isWhite, move);
            pieceLocations[colorIndex] = c.getDict(isWhite)[pieceIndex.ALL_LOCATIONS];

            /*
            int oldLocation = move[0];
            currAttackedSquares[colorIndex][oldLocation] = new BitboardLayer();
            */
            for (int i = 0; i < 2; i++) {
                kingAttackedSquares[i] = new BitboardLayer();
                currCheckers[i] = new List<int[]>();
                currPinnedPcs[i] = new List<int[]>();
            }
            //if move was a capture, nullify all valid moves and attacked squares
            //pins and checks are cleared and re-revaluated every move, so no need to directly nullify them
            if (pieceLocations[oppositeColorIndex].trueAtIndex(move[1])) {
                currValidMoves[oppositeColorIndex][move[1]] = new BitboardLayer();
                currAttackedSquares[oppositeColorIndex][move[1]] = new BitboardLayer();
            }
            
            BitboardLayer allOccupiedSquares = new BitboardLayer(c.getDict(true)[pieceIndex.ALL_LOCATIONS].getLayerData() | c.getDict(false)[pieceIndex.ALL_LOCATIONS].getLayerData());
            bool ftupdate = false;
            foreach (int location in changedIndicies)
            {
                numIterations++;
                /*
                if (numIterations == 244438)
                {
                    Debug.Print("Found bug!");
                }
                */
                if (allOccupiedSquares.trueAtIndex(location))
                {
                    findAttackMove(pieceLocations[0].trueAtIndex(location), location);
                    if (location == 57 && currValidMoves[0][57].getLayerData() == 10489856)
                    {
                        Debug.Print("Knight updated! New value: " + currAttackedSquares[0][52].getLayerData() + " numIterations: " + numIterations);
                    }
                }
                else
                {
                    for (int i = 0; i < 2; i++)
                    {
                        currAttackedSquares[i][location] = new BitboardLayer();
                        currValidMoves[i][location] = new BitboardLayer();
                    }
                }
                //update valid moves and attacked squares for piece at location
            }
            foreach (int[] pin in currPinnedPcs[colorIndex])
            {
                //format: pinner, pinned piece, interval
                ulong vMoveLayer = 0uL;
                for (int i = pin[0]; checkCollision(oppositeColorIndex == 1, i) != 3; i += pin[2])
                {
                    vMoveLayer |= 1uL << (63 - i);
                }
                currValidMoves[oppositeColorIndex][pin[1]].setLayerData(currValidMoves[oppositeColorIndex][pin[1]].getLayerData() & vMoveLayer);
            }
            foreach (int[] pin in currPinnedPcs[oppositeColorIndex])
            {
                ulong vMoveLayer = 0uL;
                for (int i = pin[0]; checkCollision(colorIndex == 1, i) != 3; i += pin[2])
                {
                    vMoveLayer |= 1uL << (63 - i);
                }
                currValidMoves[colorIndex][pin[1]].setLayerData(currValidMoves[oppositeColorIndex][pin[1]].getLayerData() & vMoveLayer);
            }
            //get all attacked squares so we can get king's valid moves
            for (int i = 0; i < 2; i++)
            {
                ulong fullAttackedSq = 0;
                foreach(BitboardLayer pieceAttackedSq in currAttackedSquares[i])
                {
                    fullAttackedSq |= pieceAttackedSq.getLayerData();
                }
                allAttackedSq[i] = new BitboardLayer(fullAttackedSq);
            }

            //to make sure only valid moves are considered, bitwise-AND each non-king's valid moves with position of each piece attacking the king and their attack vector
            //if double-checked, this makes sure no positions will show up
            //otherwise will automatically limit to capture or block
            for (int i = 0; i < 2; i++)
            {
                int kingPos = c.getDict(i == 1)[pieceIndex.KING].getTrueIndicies()[0];
                //if opponent is checking, limit valid moves
                foreach (int[] checker in currCheckers[1 - i])
                {
                    ulong vMoveMask = 0;
                    for (int j = checker[0]; checkCollision(i == 1, j) != 3; j += checker[1])
                    {
                        vMoveMask |= 1uL << (63 - j);
                    }
                    for(int j = 0; j < 64; j++)
                    {
                        currValidMoves[i][j].setLayerData(currValidMoves[i][j].getLayerData() & vMoveMask);
                    }
                }

                //limit king's valid moves
                BitboardLayer kingVMoves = new BitboardLayer(currAttackedSquares[i][kingPos].getLayerData());
                foreach (int possibleMove in kingVMoves.getTrueIndicies())
                {
                    if (checkCollision(i == 0, i) == 2) kingVMoves.setAtIndex(possibleMove, false);
                    if (allAttackedSq[1 - i].trueAtIndex(possibleMove)) kingVMoves.setAtIndex(possibleMove, false);
                    if (kingAttackedSquares[1 - i].trueAtIndex(possibleMove)) kingVMoves.setAtIndex(possibleMove, false);
                }

                //castling
                if (canCastle[i][1] && !allAttackedSq[1 - i].trueAtIndex(kingPos + 1) && !allAttackedSq[1 - i].trueAtIndex(kingPos + 2)) //can castle right
                {
                    kingVMoves.setAtIndex(kingPos + 2, true);
                }
                if (canCastle[i][0] && !allAttackedSq[1 - i].trueAtIndex(kingPos - 1) && !allAttackedSq[1 - i].trueAtIndex(kingPos - 2))
                {
                    kingVMoves.setAtIndex(kingPos - 2, true);
                }
                currValidMoves[i][kingPos] = new BitboardLayer(kingVMoves);
            }
            for (int i = 0; i < 2; i++)
            {
                ulong fullValidMoves = 0;
                foreach (BitboardLayer pieceValidMoves in currValidMoves[i])
                {
                    fullValidMoves |= pieceValidMoves.getLayerData();
                    pieceValidMoves.setAtIndex(c.getDict(i == 1)[pieceIndex.KING].getTrueIndicies()[0], false);
                }
                allValidMoves[i] = new BitboardLayer(fullValidMoves);
            }
        }

        void findAttackMove(bool isWhite, int index)
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
            for (int i = 0; i < currPinnedPcs[colorIndex].Count; i++)
            {
                int[] pin = currPinnedPcs[oppositeColorIndex][i];
                if (pin[0] == index) currPinnedPcs[colorIndex].Remove(pin);
            }
            for (int s = 0; s <= pieceIndex.KING; s++)
            {
                if (dict[s].trueAtIndex(index))
                {
                    lastUpdated[index] = numIterations;
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
                            if (moveIndex - 1 >= 0 && moveIndex - 1 <= 63 && (moveIndex - 1) / 8 == moveIndex / 8)
                            {
                                pieceAS.setAtIndex(moveIndex - 1, true);
                                if (enemyDict[pieceIndex.ALL_LOCATIONS].trueAtIndex(moveIndex - 1)) pieceVM.setAtIndex(moveIndex - 1, true);
                            }
                            if (moveIndex + 1 >= 0 && moveIndex + 1 <= 63 && (moveIndex + 1) / 8 == moveIndex / 8){
                                pieceAS.setAtIndex(moveIndex + 1, true);
                                if (enemyDict[pieceIndex.ALL_LOCATIONS].trueAtIndex(moveIndex + 1)) pieceVM.setAtIndex(moveIndex + 1, true);
                            }

                            //move two in dir
                            if (index / 8 == pawnFile && !enemyDict[pieceIndex.ALL_LOCATIONS].trueAtIndex(moveIndex) && !dict[pieceIndex.ALL_LOCATIONS].trueAtIndex(moveIndex))
                            {
                                moveIndex += direction * 8;
                                if (moveIndex >= 0 && moveIndex <= 63 && !enemyDict[pieceIndex.ALL_LOCATIONS].trueAtIndex(moveIndex) && !dict[pieceIndex.ALL_LOCATIONS].trueAtIndex(moveIndex)) pieceVM.setAtIndex(moveIndex, true);
                            }

                            //en passant captures
                            if (enemy_ep != -1 && index + direction * 7 == enemy_ep || index + direction * 9 == enemy_ep){
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
                                extendPath(isWhite, index, diff, true);
                            }
                            break;
                        case pieceIndex.QUEEN:
                            diffs = new int[] { -7, 7, -9, 9 };
                            foreach (int diff in diffs)
                            {
                                extendPath(isWhite, index, diff, true);
                            }
                            diffs = new int[] { 8, -8, 1, -1 };
                            foreach (int diff in diffs)
                            {
                                extendPath(isWhite, index, diff);
                            }
                            break;
                        case pieceIndex.KING:
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
                                    !dict[pieceIndex.ALL_LOCATIONS].trueAtIndex(index - 2) && !enemyDict[pieceIndex.ALL_LOCATIONS].trueAtIndex(index - 2) &&
                                    !dict[pieceIndex.ALL_LOCATIONS].trueAtIndex(index - 3) && !enemyDict[pieceIndex.ALL_LOCATIONS].trueAtIndex(index - 3))
                                    canCastle[colorIndex][0] = true; //king can castle left
                            }
                            break;
                    }
                    break;
                }
            }
        }

        //return 0 for empty, 1 for enemy, 2 for friend, 3 for enemy king
        int checkCollision(bool isWhite, int i)
        {
            int colorIndex = isWhite ? 0 : 1;
            int oppositeColorIndex = isWhite ? 1 : 0;

            if (c.getDict(isWhite)[pieceIndex.ALL_LOCATIONS].trueAtIndex(i)) return 2;
            if (c.getDict(!isWhite)[pieceIndex.ALL_LOCATIONS].trueAtIndex(i))
            {
                if (c.getDict(!isWhite)[pieceIndex.KING].trueAtIndex(i)) return 3;
                return 1;
            }
            return 0;
        }

        void extendPath(bool isWhite, int begin, int interval, bool isBishop = false) {
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
                if (s > 63 || s < 0 || ((interval == 1 || interval == -1) && s / 8 != begin / 8) || (isBishop && ((((s-interval)/8) - (s/8)) != 1 && (((s - interval) / 8) - (s / 8)) != -1))) break;
                int collisionStatus = checkCollision(isWhite, s);
                
                //for all cases, add to attacked squares
                if ((whatIsFor & (1 << atSqShift)) > 0) atSq.setAtIndex(s, true);

                if (collisionStatus == 2) break; //attacking friend; since is already in attackedsquares we're done

                if ((whatIsFor & (1 << vMoveShift)) > 0 && collisionStatus != 2) vMoves.setAtIndex(s, true);

                if (collisionStatus == 3) {
                    //say we're attacking the king, then extend one more in dir and add to kingAttackedSquares
                    whatIsFor &= ~1;
                    currCheckers[colorIndex].Add(new int[] { begin, interval });
                    if ((whatIsFor & (1 << atSqShift)) > 0 && s + interval < 64 && s + interval > -1)
                    {
                        kingAttackedSquares[colorIndex].setAtIndex(s + interval, true);
                    }
                    break; //there isn't any reason to continue searching in this direction
                } else if (collisionStatus == 1) {
                    if ((whatIsFor & (1 << onePieceShift)) > 0){
                        whatIsFor &= ~(1 << onePieceShift);
                        pinnedPiece = s;
                    }
                    else if ((whatIsFor & (1 << twoPieceShift)) > 0 && (whatIsFor & 1) > 0) whatIsFor &= ~(1 << twoPieceShift);
                    whatIsFor &= ~(1 << vMoveShift);
                    whatIsFor &= ~(1 << atSqShift);
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
    }
}
