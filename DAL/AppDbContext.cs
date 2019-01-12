using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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

        public AppDbContext(DbContextOptions options) : base(options)
        {  
        }

        public AppDbContext()
        {
            
        }

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
    }
}