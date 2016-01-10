/*
 * User: samsc
 * Date: 9/22/2015
 * Time: 12:10 PM
 */
using System;

namespace Chess
{
	public static class pieceIndex{
		public const int PAWN = 0;
		public const int ROOK = 1;
		public const int KNIGHT = 2;
		public const int BISHOP = 3;
		public const int QUEEN = 4;
		public const int KING = 5;
		public const int ALL_LOCATIONS = 6;
		public const int ATTACKED_SQUARES = 7;
		public const int VALID_MOVES = 8;
		public const int FLAGS = 9;
	}

    public static class flagIndex
    {
        public const UInt64 RIGHT_ROOK_CASTLE = 1;
        public const UInt64 KING_CASTLE = 2;
        public const UInt64 LEFT_ROOK_CASTLE = 4;
    }
}
