using System;
using BLL;
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
            Game game = new Game(ui);
            
            ApplicationMenu applicationMenu = new ApplicationMenu(game);
            Menu main = applicationMenu.GetMain();
            game.RunMenu(menu: main);
        }
    }
}