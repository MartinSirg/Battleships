using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class SaveGame
    {
        public const int MaxLength = 32;
        public int SaveGameId { get; set; }
        [MaxLength(MaxLength)]
        public string Name { get; set; }
        public List<GameMove> GameMoves { get; set; }
        public Player Player1 { get; set; }
        public Player Player2 { get; set; }
        public Rules Rules { get; set; }
        public bool IsFinished { get; set; }
        [MaxLength(2)]
        public string Mode { get; set; }
    }
}