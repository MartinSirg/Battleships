using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class Player
    {
        public int PlayerId { get; set; }
        
//        public int BoardId { get; set; } // without annotations
        [Required]
        public Board Board { get; set; }
        
        [MaxLength(64)]
        public string Name { get; set; }
        [NotMapped]
        public bool IsReady { get; set; }

        public Player(Board board, string name)
        {
            Name = name;
            Board = board;
        }

        private Player()
        {
        }
    }
}