using System.Collections.Generic;

namespace Domain
{
    public class SaveGame
    {
        
        public int SaveGameId { get; set; }
        public string Name { get; set; }
        public List<GameMove> GameMoves { get; set; }
        public Player Player1 { get; set; }
        public Player Player2 { get; set; }
        public Rules Rules { get; set; }
        public bool IsFinished { get; set; }
    }
}