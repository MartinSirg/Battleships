using System;
using Domain;

namespace BLL
{
    public class Game
    {
        public Rules Rules { get; set; }
        public Player Player1 { get; set; }
        public Player Player2 { get; set; }
        
        public Game(Rules rules)
        {
            Rules = rules;
            var board1 = new Board(rules.BoardRows, rules.BoardCols, rules.CanShipsTouch);
            var board2 = new Board(rules.BoardRows, rules.BoardCols, rules.CanShipsTouch);
            
            Player1 = new Player(board1);
            Player2 = new Player(board2);
        }

        public void Start(IInput input)
        {
            
        }



        //TODO: Generate battleships for board


        //TODO: add battleship to board(list?)

    }
}