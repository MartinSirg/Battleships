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


            //RULES OK
//            SaveGame s = ctx.SaveGames
//                .Include(saveGame => saveGame.Rules)
//                .ThenInclude(rules => rules.BoatRules)
//                .Include(saveGame => saveGame.Player1)
//                .ThenInclude(player => player.Board)
//                .Include(saveGame => saveGame.Player2)
//                .ThenInclude(player => player.Board)
//                .Include(saveGame => saveGame.GameMoves)
//                .First(saveGame => saveGame.SaveGameId == 1);
//            
//            Console.WriteLine($"Rows: {s.Rules.BoardRows} \nCols :{s.Rules.BoardCols}");
//            
//            Player p1 = s.Player1;
//            List<List<Tile>> player1BoardTiles = new List<List<Tile>>(s.Rules.BoardRows);
//            List<Tile> tilesFromDb = ctx.Tiles.Include(tile => tile.Board).Include(tile => tile.Battleship).Where(tile => tile.Board == s.Player1.Board).ToList();
//            
//            for (int i = 0; i < s.Rules.BoardRows; i++)
//            {
//                player1BoardTiles.Add(new List<Tile>(s.Rules.BoardCols));
//                
//                for (int j = 0; j < s.Rules.BoardCols; j++)
//                {
//                    if (tilesFromDb.Any(tile => tile.Row == i && tile.Col == j)){
//                        player1BoardTiles[i].Add(tilesFromDb.Find(tile => tile.Row == i && tile.Col == j));}
//                    else
//                        player1BoardTiles[i].Add(new Tile(i, j, p1.Board));
//                }
//            }
//            p1.Board.Tiles = player1BoardTiles;
//            p1.Board.Tiles.ForEach(row => row.ForEach(Console.WriteLine));
            
            //TODO: give clone existing objects?
            


            ApplicationMenu applicationMenu = new ApplicationMenu(game);
            Menu main = applicationMenu.GetMain();
            game.RunMenu(menu: main);


        }
    }
}