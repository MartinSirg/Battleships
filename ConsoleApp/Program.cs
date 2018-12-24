using System;
using System.Collections.Generic;
using System.Linq;
using BLL;
using DAL;
using Domain;
using Initializers;
using MenuSystem;
using Microsoft.EntityFrameworkCore;
using UI;


namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            IUserInterface ui = new ConsoleUI();
            
            NewDbContext ctx = new NewDbContext();
//            
//            appDbContext.BoardsOld.Add(new Board(10,10,true));
//            appDbContext.BoardsOld[0].AddBattleship((0, 0), (0, 2), new Battleship(3));
//            appDbContext.BoardsOld.Add(new Board(10,10,true));
//            appDbContext.PlayersOld.Add(new Player(appDbContext.BoardsOld[0], "Player 1"));
//            appDbContext.PlayersOld.Add(new Player(appDbContext.BoardsOld[1], "Player 2"));
//            appDbContext.GameMovesOld.Add(new List<GameMove>());
//            appDbContext.RulesOld.Add(Rules.GetDefaultRules());
//            appDbContext.TotalGameOld.Add((name: "sample1", gameMoves: 0, player1: 0, player2: 1, rules: 0));
            
            
            Game game = new Game(ui, ctx);

            ApplicationMenu applicationMenu = new ApplicationMenu(game);
            Menu main = applicationMenu.GetMain();
            game.RunMenu(menu: main);


        }
    }
}