using System;
using System.Collections.Generic;
using System.Linq;
using Domain;

namespace DAL
{
    public class AppDbContext
    {
        public List<List<GameMove>> GameMovesOld { get; set; } = new List<List<GameMove>>();
        public List<Player> PlayersOld { get; set; } = new List<Player>();
        public List<Board> BoardsOld { get; set; } = new List<Board>();
        public List<Rules> RulesOld { get; set; } = new List<Rules>();
        public List<Battleship> BattleshipsOld { get; set; } = new List<Battleship>();
//        public List<Tile> TilesOld { get; set; } = new List<Tile>();
        public List<(string name, int gameMoves, int player1, int player2, int rules)> FinishedGamesOld { get; set; }= 
            new List<(string name, int gameMoves, int player1, int player2, int rules)>();
        public List<(string name, int gameMoves, int player1, int player2, int rules)> TotalGameOld { get; set;} = 
            new List<(string , int, int, int, int)>();
    }
}