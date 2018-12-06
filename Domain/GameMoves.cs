using System.Collections.Generic;

namespace Domain
{
    public class GameMoves
    {
        public List<(Player target, Tile tile, BombingResult result)> Moves { get; set; } =
            new List<(Player target, Tile tile, BombingResult result)>();
    }
}