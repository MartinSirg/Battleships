namespace Domain
{
    public class Tile
    {
        public Battleship Battleship { get; set; }
        public bool IsBombed { get; set; }
        public int Row { get; set; }
        public int Col { get; set; }
        public Board Board { get; set; }
        public bool IsHighlightedStart { get; set; }
        public bool IsHighlightedEnd { get; set; }

        public Tile(int row, int col, Board board)
        {
            Board = board;
            Row = row;
            Col = col;
        }

        public Tile(Battleship battleship, int row, int col, Board board)
        {
            Board = board;
            Battleship = battleship;
            Row = row;
            Col = col;
        }

        public bool IsEmpty()
        {
            return Battleship == null;
        }
        
        
    }
}