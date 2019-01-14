using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class Player : ICloneable
    {
        public int PlayerId { get; set; }
        
//        public int BoardId { get; set; } // without annotations
        [Required]
        public Board Board { get; set; }

        public const int MaxLength = 64;
        public const int MinLength = 2;
        [MaxLength(MaxLength)]
        [MinLength(MinLength)]
        [Required]
        public string Name { get; set; }
        [NotMapped]
        public bool IsReady { get; set; }

        public Player(Board board, string name)
        {
            Name = name;
            Board = board;
        }

        public Player()
        {
        }

        public object Clone()
        {
            return new Player((Board) Board.Clone(), Name);
        }
    }
}