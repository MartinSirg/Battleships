using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class GameMove
    {
        public int GameMoveId { get; set; }

        public Player Target { get; set; }
        public Tile Tile { get; set; }

        public int MoveNumber { get; set; }

        [NotMapped] public static int NextMoveNumber = 0;

        public GameMove(Player target, Tile tile)
        {
            Target = target;
            Tile = tile;
            MoveNumber = NextMoveNumber++;
        }

        private GameMove()
        {
        }

        public override string ToString()
        {
            return $"{Target.Name} is bombed at:\t\t {Tile}";
        }
    }
}