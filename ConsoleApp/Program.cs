using System;
using System.Collections.Generic;
using System.Linq;
using BLL;
using DAL;
using Domain;
using Initializers;
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
            
            NewDbContext ctx = new NewDbContext();
            AppDbContext appDbContext = new AppDbContext();
//            
//            appDbContext.BoardsOld.Add(new Board(10,10,true));
//            appDbContext.BoardsOld[0].AddBattleship((0, 0), (0, 2), new Battleship(3));
//            appDbContext.BoardsOld.Add(new Board(10,10,true));
//            appDbContext.PlayersOld.Add(new Player(appDbContext.BoardsOld[0], "Player 1"));
//            appDbContext.PlayersOld.Add(new Player(appDbContext.BoardsOld[1], "Player 2"));
//            appDbContext.GameMovesOld.Add(new List<GameMove>());
//            appDbContext.RulesOld.Add(Rules.GetDefaultRules());
//            appDbContext.TotalGameOld.Add((name: "sample1", gameMoves: 0, player1: 0, player2: 1, rules: 0));
            
            
            Game game = new Game(ui, appDbContext);
            game.Ctx = ctx;
//            game.DisplayCurrentRuleset();
//            Console.ReadLine();


            ApplicationMenu applicationMenu = new ApplicationMenu(game);
            Menu main = applicationMenu.GetMain();
            game.RunMenu(menu: main);
            
//            var board = new Board(10,10,0);
//            ctx.Boards.Update(board);
//            ctx.SaveChanges();
//            ctx.Rules.Add(new Rules(0, 10, 10, new List<(int, int)> {(1, 1), (2, 1), (3, 1), (4, 1)}));
//            ctx.Boards.Add(new Board(10, 10, 1));
//            ctx.SaveChanges();
            
//            Player p = new Player(board, "Matu");
//            ctx.Players.Add(p);
        }
    }
}