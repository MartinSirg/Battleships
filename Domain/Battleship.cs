using System.Collections.Generic;

namespace Domain
{
    public class Battleship
    {
        public int BattleshipId { get; set; }
        public int Size { get; set; }
        public int LivesLeft { get; set; }
        public List<Tile> Locations { get; set; } = new List<Tile>();
        
        public Battleship(int size)
        {
            Size = size;
            LivesLeft = size;
        }

        public bool IsAlive()
        {
            return LivesLeft > 0;
        }
    }
}