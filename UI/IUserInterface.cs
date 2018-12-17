using System;
using System.Collections.Generic;
using Domain;

namespace MenuSystem
{
    public interface IUserInterface
    {
        string ShipStartPoint { get; set; }

        string ShipEndPoint { get; set; }

        int MaxWidth { get; set; }
        
        int MaxHeight { get; set; }

        int GetInteger(string message);

        string GetString(string message);

        string GetTargetLocation(Board enemyBoard);

        void DisplayShipsAndBombings(Board enemy, Board current);
        
        void DisplayBombingResult(BombingResult bombingResult, Board targetBoard);

        bool AskExitConfirmation();
        
        int GetShipSize();
        
        int GetShipQuantity();
        
        void DisplayRulesShips(IUserInterface ui);
        
        void DisplayBoardRules(Rules rules);
        
        void DisplayCurrentRules(Rules rules);
        
        void DisplayAvailableShips(List<KeyValuePair<int,int>> availableShips);
        
        void DisplayCurrentShips(Board currentPlayerBoard, string type);
        
        string GetShipEndPoint(Board currentPlayerBoard, List<KeyValuePair<int,int>> availableShips);

        string GetShipStartPoint(Board currentPlayerBoard, List<KeyValuePair<int,int>> availableShips);
        
        void Alert(string alert, int waitTime);

        string GetMenuShortcut();

        void DisplayNewMenu(Menu menu);

        void WaitForUser();

        string GetSaveGameName();

        void DisplaySavedGames(List<string> names);

        string GetDeletableShipTile();
        
        string GetRulesetName();
        
        string ConfirmBoatsOverride();
        
        string GetShipsCanTouch(bool current);
        
        string GetNewBoardHeight(int currentRows);
        
        string GetNewBoardWidth(int currentCols);
    }
}