using System;
using System.Collections.Generic;
using System.Linq;
using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DAL
{
    public class AppDbContext : DbContext
    {
        public DbSet<GameMove> GameMoves { get; set; }
        public DbSet<SaveGame> SaveGames { get; set; }
        public DbSet<Battleship> Battleships { get; set; }
        public DbSet<Rules> Rules { get; set; }
        public DbSet<Tile> Tiles { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Board> Boards { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseMySQL(
                "server=alpha.akaver.com;" +
                "database=student2018_179563;" +
                "user=student2018" +
                "password=student2018");

//            optionsBuilder.UseSqlServer(
//                @"Server=(localdb)\mssqllocaldb;
//                Database=MyDatabase;
//                Trusted_Connection=True;
//                MultipleActiveResultSets=true");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //configure entities //TODO: make saved game class
//            modelBuilder.Entity<SaveGame>(game => game.Name).IsUnique(); // Save game ie saa olla siis sama nimega 
            
            //remove cascade delete
            foreach (var mutableForeignKey in modelBuilder
                .Model
                .GetEntityTypes()
                .Where(e => !e.IsOwned())
                .SelectMany(e => e.GetForeignKeys()))
            {
                mutableForeignKey.DeleteBehavior = DeleteBehavior.Restrict;
            }

//            modelBuilder
//                .Entity<Board>()
//                .Property(entity => entity.Bombings)
//                .HasConversion(converter);
            
            
            
            base.OnModelCreating(modelBuilder);
                  
        }
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        public List<List<GameMove>> GameMovesOld { get; set; } = new List<List<GameMove>>();
        public List<Player> PlayersOld { get; set; } = new List<Player>();
        public List<Board> BoardsOld { get; set; } = new List<Board>();
        public List<Rules> RulesOld { get; set; } = new List<Rules>();
        public List<Battleship> BattleshipsOld { get; set; } = new List<Battleship>();
        public List<Tile> TilesOld { get; set; } = new List<Tile>();
        public List<(string name, int gameMoves, int player1, int player2, int rules)> FinishedGamesOld { get; set; }= 
            new List<(string name, int gameMoves, int player1, int player2, int rules)>();
        public List<(string name, int gameMoves, int player1, int player2, int rules)> TotalGameOld { get; set;} = 
            new List<(string , int, int, int, int)>();
    }
}