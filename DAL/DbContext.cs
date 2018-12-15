using System;
using System.Collections.Generic;
using Domain;

namespace DAL
{
    public class DbContext
    {
        public List<GameMoves> GameMoves { get; set; } = new List<GameMoves>();
        public List<Player> Players { get; set; } = new List<Player>();
        public List<Board> Boards { get; set; } = new List<Board>();
        public List<Rules> Rules { get; set; } = new List<Rules>();
        
        /**
         * 1. String = game name
         * 2. int = GameMoves primary index
         * 3. int = Player 1 primary index in Players
         * 4. int = Player 2  primary index in Players
         * 5. int = Player 1 Board primary index in Boards
         * 6. int = Player 2 Board primary index in Boards
         * 7. int = Rules primary index
         */
        public List<(string name, int gameMoves, int player1, int player2, int p1board, int p2board, int rules)> TotalGame { get; set;} = 
            new List<(string , int, int, int, int, int, int)>();
        
    }
}