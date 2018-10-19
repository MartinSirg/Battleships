using System;
using System.Collections.Generic;

namespace BLL
{
    public interface IInput
    {
//        bool DefaultRules { get; }
//        
//        ((int, int), (int, int)) GetShipStartAndEndLocations();
//        
//        (int, int) GetBombingLocation();
//
//        /*
//         *Parameters:
//         *     errorMessage: Error message given, if called again due to invalid input
//         *     correctInput: If the first value was correct, it is passed as well, otherwise it remains -1 
//         * 
//         * Returns: tuple(int, int)
//         *     Item1 - size of ship
//         *     Item2 - quantity of those ships 
//         */
//        (int, int) GetShipSizeAndQuantity(string errorMessage = "", int previousCorrectLength = -1);
//
//        (int, int, bool) GetCustomRules();
        int MaxWidth { get; set; }
        int MaxHeight { get; set; }

        int GetInteger(string message);

        string GetString(string message);



    }
}