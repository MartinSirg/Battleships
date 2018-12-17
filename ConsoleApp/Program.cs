using System;
using BLL;
using DAL;
using Domain;
using MenuSystem;
using UI;


namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
//            var board = new Board(totalCols: 25, totalRows:25);
//            var ship = new Battleship(5);
//            var ship2 = new Battleship(4);
//            var ui = new ConsoleUI();
//            board.AddBattleship((0,0), (0,3), ship2);
//            board.AddBattleship((4,4), (4,8), ship);
//            board.BombLocation(4, 4);
//            board.BombLocation(4, 5);
//            board.BombLocation(4, 3);
//            ui.PrintFriendlyShips(board);
//            ui.PrintBombedLocations(board);
//            ui.PrintBombedLocationsAndFriendlyShips(board, board);
//            Console.ReadLine();
            IUserInterface ui = new ConsoleUI();
            DbContext dbContext = new DbContext();
            
            dbContext.Boards.Add(new Board());
            dbContext.Boards[0].AddBattleship((0, 0), (0, 2), new Battleship(3));
            dbContext.Boards.Add(new Board());
            dbContext.Players.Add(new Player(dbContext.Boards[0], "Player 1"));
            dbContext.Players.Add(new Player(dbContext.Boards[1], "Player 2"));
            dbContext.GameMoves.Add(new GameMoves());
            dbContext.Rules.Add(Rules.GetDefaultRules());
            dbContext.TotalGame.Add((name: "sample1", gameMoves: 0, player1: 0, player2: 1, p1board: 0, p2board: 1, rules: 0));
            
            Game game = new Game(ui, dbContext);
            game.DisplayCurrentRuleset();
            Console.ReadLine();

//            ApplicationMenu applicationMenu = new ApplicationMenu(game);
//            Menu main = applicationMenu.GetMain();
//            game.RunMenu(menu: main);
        }
    }
}