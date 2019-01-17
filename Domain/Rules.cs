using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class Rules: ICloneable
    {
        public int RulesId { get; set; }

        public int CanShipsTouch { get; set; }
        
        public int BoardRows { get; set; }

        public int BoardCols { get; set; }

        [MaxLength(64)]
        public string Name { get; set; }

        public List<BoatRule> BoatRules { get; set; } = new List<BoatRule>();

        public Rules(int canShipsTouch, int boardRows, int boardCols, List<(int, int)> boatRules)
        {
            CanShipsTouch = canShipsTouch;
            BoardRows = boardRows;
            BoardCols = boardCols;
            boatRules.ForEach(tuple => BoatRules.Add(new BoatRule(tuple.Item1, tuple.Item2)));
        }

        private Rules()
        {
        }

        public int MaxShipTiles()
        {
            return BoardCols * BoardRows / 2 ;
        }


        public static Rules GetDefaultRules()
        {
            var boats = new List<(int, int)>();
            boats.Add((1,1));
            boats.Add((2,1));
            boats.Add((3,1));
            boats.Add((4,1));
            boats.Add((5,1));
            Rules rules = new Rules(1, 10, 10, boats);
            rules.Name = "Standard rules";
            return rules;
        }

        public object Clone()
        {
            List<(int size, int quantity)> boats = new List<(int size, int quantity)>();
            BoatRules.ForEach(rule => boats.Add((rule.Size, rule.Quantity)));
            return new Rules(CanShipsTouch, BoardRows, BoardCols, boats);
        }
    }
}