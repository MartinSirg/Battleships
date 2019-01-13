using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BLL;

namespace Domain
{
    public class Tile : ICloneable
    {
        [Key]
        public int TileId { get; set; }
        
        public Battleship Battleship { get; set; }
        public bool IsBombed { get; set; } = false;
        public int Row { get; set; }
        public int Col { get; set; }

        
        public Board Board { get; set; }
        
        [NotMapped]
        public bool IsHighlightedStart { get; set; }
        [NotMapped]
        public bool IsHighlightedEnd { get; set; }

        public Tile(int row, int col, Board board)
        {
            Board = board;
            Row = row;
            Col = col;
        }

        private Tile()
        {
        }

        public Tile(Battleship battleship, int row, int col, Board board)
        {
            Board = board;
            Battleship = battleship;
            Row = row;
            Col = col;
        }

        private Tile(int row, int col)
        {
            Row = row;
            Col = col;
        }

        public bool IsEmpty()
        {
            return Battleship == null;
        }

        public override string ToString()
        {
//            return $"Row: {Row}\tCol: {Col}\tHasShip: {!IsEmpty()}\tIsBombed: {IsBombed}\tBoard Hashcode: {Board.GetHashCode()}";
            return $"{new LetterNumberSystem().GetLetter(Row + 1)}{Col + 1}";
        }

        public object Clone()
        {
            var tileClone = new Tile(Row, Col);
            tileClone.IsBombed = IsBombed;
            return tileClone;
        }
    }
}