using System;
using System.Collections.Generic;
using Domain;

namespace MenuSystem
{
    public interface IUserInterface
    {
        Player CurrentPlayer { get; set; }
        Player TargetPlayer { get; set; }

        string ShipStartPoint { get; set; }
        string ShipEndPoint { get; set; }
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
//        Game Game { get; set; }

//        ActionEnum ShowMenu(Menu menu);

//        (int x, int y) AddShip();

//        int DeleteShip();
        
//        (int len, int quantity) AddShipToRules();

//        (int len, int newQuantity) EditShip();

//        int RemoveShipFromRules();

        int GetInteger(string message);

        string GetString(string message);


        string GetTargetLocation(Board enemyBoard);

        void ShowShipsAndBombings(Board enemy, Board current);
        
        void ShowBombingResult(BombingResult bombingResult, Board targetBoard);

        bool AskExitConfirmation();
        
        int GetShipSize();
        
        int GetShipQuantity();
        
        void DisplayRulesShips(IUserInterface ui);
        
        void ShowBoardRules(IUserInterface obj);
        
        void ShowCurrentRules(IUserInterface obj);
        
        void DisplayCurrentRules(IUserInterface obj);
        
        void DisplayAvailableShips(Board currentPlayerBoard, Rules gameRules);
        
        void DisplayCurrentShips(Board currentPlayerBoard);
        
        string GetShipStartPoint(IUserInterface obj);
        
        string GetShipEndPoint(IUserInterface obj);
        
        void Alert(string alert, int waitTime);
        
        string  GetMenuShortcut();
        
        void DisplayNewMenu(Menu menu);
        
        void WaitForUser();
        
        string GetSaveGameName();
        
        void DisplaySavedGames(List<string> names);
    }
}