﻿/*
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

        int numIterations = 0;
		int searchDepth;

        MainForm gui;

        int[] testedMoves = new int[] { 52, 36, 6, 21, 59, 52 };
		
		public AI(ChessBoard c, bool isWhite, int searchDepth, MainForm f){
			transTable = new Dictionary<ulong[][], int>();
			expectedMovesTable = new Dictionary<ulong[][], int[]>();
			this.searchDepth = searchDepth;

            gui = f;
		}
		

        List<int[]> getPossibleMoves(ChessBoard c, bool isWhite)
        {
            var retVal = new List<int[]>();
            BitboardLayer[] dict = c.getDict(isWhite);
            BitboardLayer[] enemyDict = c.getDict(!isWhite);

            foreach (int i in dict[pieceIndex.ALL_LOCATIONS].getTrueIndicies()) { 
                BitboardLayer pieceMoves = c.getValidMoves(isWhite, i);
                foreach (int j in pieceMoves.getTrueIndicies())
                {
                    retVal.Add(new int[] { i, j });
                }
            }
            return retVal;
        }

        public string displayBoard(ChessBoard c)
        {
            //for debugging purposes
            //capital letters are white, n's are knights
            char[] whiteLetters = new char[] { 'P', 'R', 'N', 'B', 'Q', 'K' };
            char[] blackLetters = new char[] {  'p', 'r', 'n', 'b', 'q', 'k' };

            char[] retVal = new char[64];
            for (int i = 0; i < 64; i++) retVal[i] = Convert.ToChar("+");
            BitboardLayer[] white = c.getDict(true);
            BitboardLayer[] black = c.getDict(false);
            for (int i = 0; i <= pieceIndex.KING; i++){
                foreach (int j in white[i].getTrueIndicies()) retVal[j] = whiteLetters[i];
                foreach (int j in black[i].getTrueIndicies()) retVal[j] = blackLetters[i];
            }
            string s = "";
            for (int i = 0; i < 64; i++){
                s += retVal[i];
                if (i % 8 == 7) s += "\n";
            }
            var ml = c.getMoveList();
            for (int i = 0; i < ml.Count; i++)
            {
                s += (i % 2 == 0 ? "White" : "Black") + ": [" + ml[i][0] + ", " + ml[i][1] + "]\n";
            }
            return s;
        }

        public int[] getMoveList(ChessBoard cb)
        {
            List<int> retVal = new List<int>();
            var moveList = cb.getMoveList();
            foreach (int[] i in moveList)
            {
                retVal.Add(i[0]);
                retVal.Add(i[1]);
            }
            return retVal.ToArray();
        }

        public bool matchesMoveList(ChessBoard cb)
        {
            ChessBoard test = new ChessBoard(null, null);
            var moveList = cb.getMoveList();
            for (int i = 0; i < moveList.Count; i++)
            {
                test.movePiece(i % 2 == 0, moveList[i][0], moveList[i][1], true);
            }
            //return cb.equals(test);
            return displayBoard(cb).Equals(displayBoard(test));
        }

        public int[][] getAIMove(ChessBoard cb, bool isWhite, int depth)
        {
            gui.resetProgBar();
            searchDepth = depth;
            int[][] retVal = alphaBeta(cb, isWhite, depth, int.MinValue, int.MaxValue, new int[0], false);
            gui.addText("***\nAlphabeta is done.\n***");
            gui.stepProgBarBy(100);
            return retVal;
        }

        public List<int[]> sortAlphaBeta(ChessBoard cb, bool isWhite, List<int[]> possibleMoves)
        {
            List<int[]> retVal = new List<int[]>();
            List<int> valueSet = new List<int>();
            for (int i = 0; i < possibleMoves.Count; i++)
            {
                numIterations++;
                if (numIterations == 33000)
                {
                    gui.addText("Found bug");
                }
                int[] currMove = possibleMoves[i];
                cb.movePiece(isWhite, currMove[0], currMove[1], true);
                valueSet.Add(Rating.rating(isWhite, cb, possibleMoves.Count, searchDepth));
                cb.undoMove(isWhite);
                if (!matchesMoveList(cb))
                {
                    gui.addText("More errors!");
                }
            }
            while (valueSet.Count > 0)
            {
                int index = indexOfMax(valueSet);
                retVal.Add(possibleMoves[index]);
                possibleMoves.RemoveAt(index);
                valueSet.RemoveAt(index);
            }
            return retVal;
        }

        int indexOfMax(List<int> l)
        {
            int retVal = 0;
            int currMax = l[0];
            for (int i = 0; i < l.Count; i++)
            {
                if (l[i] > currMax)
                {
                    retVal = i;
                    currMax = l[i];
                }
            }
            return retVal;
        }

        bool equalsTestedMoves(int[] i) {
            if (i.Length != testedMoves.Length) return false;
            for (int j = 0; j < testedMoves.Length; j++)
            {
                if (i[j] != testedMoves[j]) return false;
            }
            return true;
        }

        

        public int[][] alphaBeta(ChessBoard cb, bool isWhite, int depth, int alpha, int beta, int[] move, bool isMyMove)
        {
            numIterations++;
            if (numIterations == 32974)
            {
                gui.addText("Found error");
            }
            //GOAL: minimize beta (starts at infinity) and maximize alpha (starts at neg. infinity)
            bool isWhiteMove = isWhite ^ isMyMove;
            //problem is in sortalphabeta
            List<int[]> possibleMoves = depth > 1 ? sortAlphaBeta(cb, isWhiteMove, getPossibleMoves(cb, isWhiteMove)) : getPossibleMoves(cb, isWhiteMove);
            int numMoves = possibleMoves.Count;

            if (depth == 0 || numMoves == 0) {
                int i = Rating.rating(isWhite, cb, numMoves, searchDepth);
                return new int[][] { move, new int[] { i } };
            }
            isMyMove = !isMyMove;
            foreach (int[] currMove in possibleMoves) {
                numIterations++;
                if (depth == searchDepth) {
                    gui.stepProgBarBy((int)(100 / possibleMoves.Count));
                    gui.addText("Currently searching move: [" + currMove[0] + ", " + currMove[1] + "] (alpha = " + alpha + ", beta = " + beta + ")\n");
                }
                if (numIterations == 5053)
                {
                    gui.addText("Found error!");
                }
                cb.movePiece(isWhiteMove, currMove[0], currMove[1], true);
                int[][] retVal = alphaBeta(cb, isWhite, depth - 1, alpha, beta, currMove, isMyMove);
                cb.undoMove(isWhiteMove);
                if (!matchesMoveList(cb))
                {
                    gui.addText("Found error!");
                }
                if (!isMyMove) { //is min node
                    if (retVal[1][0] <= beta) {
                        beta = retVal[1][0];
                        if (depth == searchDepth) move = retVal[0];
                    }
                }
                else { //is max node
                    if (retVal[1][0] > alpha) {
                        alpha = retVal[1][0];
                        if (depth == searchDepth) move = retVal[0];
                    }
                }
                if (alpha >= beta) {
                    if (!isMyMove) return new int[][] { move, new int[] { beta } };
                    else return new int[][] { move, new int[] { alpha } };
                }
            }
            if (!isMyMove) return new int[][] { move, new int[] { beta } };
            else return new int[][] { move, new int[] { alpha } };
        }
	}
}
