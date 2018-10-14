using System.Collections.Generic;

namespace Domain
{
    public class Battleship
    {
        public int Size { get; set; }
        public int LivesLeft { get; set; }
        public Board RelatedBoard { get; set; }
        public List<Tile> Locations { get; set; } = new List<Tile>();
        
        public Battleship(int size)
        {
            Size = size;
            LivesLeft = size;
        }
    }
}