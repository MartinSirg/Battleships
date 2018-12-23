using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace DAL
{
    public class NewDbContext : DbContext
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
            optionsBuilder
//                .UseLoggerFactory(MyLoggerFactory)
                .UseMySQL(
                "server=alpha.akaver.com;" +
                "database=student2018_179563;" +
                "user=student2018;" +
                "password=student2018");

//            optionsBuilder.UseSqlServer(
//                @"Server=(localdb)\mssqllocaldb;
//                Database=MyDatabase;
//                Trusted_Connection=True;
//                MultipleActiveResultSets=true");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var converter = new BoolToStringConverter("N","Y");
            
            modelBuilder
                .Entity<SaveGame>()
                .HasIndex(game => game.Name)
                .IsUnique(); // Save game ie saa olla siis sama nimega

            modelBuilder
                .Entity<SaveGame>()
                .Property(game => game.IsFinished)
                .HasConversion(converter);

            modelBuilder
                .Entity<Tile>()
                .Property(tile => tile.IsBombed)
                .HasDefaultValue(false)
                .HasConversion(converter);
                
            
            base.OnModelCreating(modelBuilder);
                  
        }

        private List<Tile> ToOneDimensions(List<List<Tile>> input)
        {
            var result = new List<Tile>();
            input.ForEach(list => list.ForEach(tile => result.Add(tile)));
            return result;
        }
        
        private List<List<Tile>> ToTwoDimensions(List<Tile> input)
        {
            var result = new List<List<Tile>>();
            input.ForEach(tile => result[tile.Row][tile.Col] = tile);
            return result;
        }
        
        public static readonly LoggerFactory MyLoggerFactory
            = new LoggerFactory(new[] {
                new ConsoleLoggerProvider((category, level)
                    => category == DbLoggerCategory.Database.Command.Name
                       && level == LogLevel.Information, true)
            });

    }
}