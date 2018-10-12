using System.Collections.Generic;

namespace Domain
{
    public class Board
    {
        public List<List<Battleship>> GameBoard { get; set; }

        Board(int width = 10, int height = 10)
        {
            
        }
    }
}