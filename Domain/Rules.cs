using System;
using System.Collections.Generic;

namespace Domain
{
    public class Rules: ICloneable
    {
        public bool CanShipsTouch { get; set; }
        public int BoardRows { get; set; }
        public int BoardCols { get; set; }
        public string Name { get; set; }
        public List<(int size, int quantity)> BoatSizesAndQuantities { get; set; }

        public Rules(bool canShipsTouch, int boardRows, int boardCols, List<(int, int)> boatSizesAndQuantities)
        {
            CanShipsTouch = canShipsTouch;
            BoardRows = boardRows;
            BoardCols = boardCols;
            BoatSizesAndQuantities = boatSizesAndQuantities;
        }
        

        public static Rules GetDefaultRules()
        {
            var boats = new List<(int, int)>();
            boats.Add((1,1));
            boats.Add((2,1));
            boats.Add((3,1));
            boats.Add((4,1));
            boats.Add((5,1));
            Rules rules = new Rules(true, 10, 10, boats);
            rules.Name = "Standard rules";
            return rules;
        }

        public object Clone()
        {
            List<(int size, int quantity)> boats = new List<(int size, int quantity)>();
            BoatSizesAndQuantities.ForEach(tuple => boats.Add((tuple.size, tuple.quantity)));
            return new Rules(CanShipsTouch, BoardRows, BoardCols, boats);
        }
    }
}