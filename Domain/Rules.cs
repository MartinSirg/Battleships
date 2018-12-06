using System.Collections.Generic;

namespace Domain
{
    public class Rules
    {
        public bool CanShipsTouch { get; set; }
        public int BoardRows { get; set; }
        public int BoardCols { get; set; }
        public string Name { get; set; }
        public List<(int, int)> BoatSizesAndQuantities { get; set; }

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
    }
}