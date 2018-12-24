using System;
using System.Collections.Generic;
using System.Threading;
using Domain;
using MenuSystem;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp
{
    public class UiController : IUserInterface
    {
        public string ShipStartPoint { get; set; }
        public string ShipEndPoint { get; set; }
        public int MaxWidth { get; set; }
        public int MaxHeight { get; set; }
        public PageModel CurrentPageModel { get; set; }
        
        public int GetInteger(string message)
        {
            throw new System.NotImplementedException();
        }

        public string GetString(string message)
        {
            throw new System.NotImplementedException();
        }

        public string GetTargetLocation(Board enemyBoard, Player currentPlayer)
        {
            throw new System.NotImplementedException();
        }

        public void DisplayShipsAndBombings(Board enemy, Board current)
        {
            throw new System.NotImplementedException();
        }

        public void DisplayBombingResult(BombingResult bombingResult, Board targetBoard, Player currentPlayer)
        {
            throw new System.NotImplementedException();
        }

        public bool AskExitConfirmation()
        {
            throw new System.NotImplementedException();
        }

        public string GetShipSize(Rules rules)
        {
            throw new System.NotImplementedException();
        }

        public string GetShipQuantity(int size)
        {
            throw new System.NotImplementedException();
        }

        public void DisplayRulesShips(Rules rules)
        {
            throw new System.NotImplementedException();
        }

        public void DisplayBoardRules(Rules rules)
        {
            throw new System.NotImplementedException();
        }

        public void DisplayCurrentRules(Rules rules)
        {
            throw new System.NotImplementedException();
        }

        public void DisplayAvailableShips(List<KeyValuePair<int, int>> availableShips)
        {
            throw new System.NotImplementedException();
        }

        public void DisplayCurrentShips(Board currentPlayerBoard, string type)
        {
            throw new System.NotImplementedException();
        }

        public string GetShipEndPoint(Board currentPlayerBoard, List<KeyValuePair<int, int>> availableShips)
        {
            throw new System.NotImplementedException();
        }

        public string GetShipStartPoint(Board currentPlayerBoard, List<KeyValuePair<int, int>> availableShips)
        {
            throw new System.NotImplementedException();
        }

        public void Alert(string alert, int waitTime)
        {
            Console.WriteLine("ERROR");
        }

        public string GetMenuShortcut()
        {
            return "";
        }

        public void DisplayNewMenu(Menu menu)
        {
            CurrentPageModel.ViewData["CurrentMenu"] = menu;
            var x  = CurrentPageModel.RedirectToPage("/UiPages/Start");
        }

        public void WaitForUser()
        {
            throw new System.NotImplementedException();
        }

        public string GetSaveGameName()
        {
            throw new System.NotImplementedException();
        }

        public void DisplaySavedGames(List<SaveGame> saves)
        {
            throw new System.NotImplementedException();
        }

        public string GetDeletableShipTile()
        {
            throw new System.NotImplementedException();
        }

        public string GetRulesetName()
        {
            throw new System.NotImplementedException();
        }

        public string ConfirmBoatsOverride()
        {
            throw new System.NotImplementedException();
        }

        public string GetShipsCanTouch(int current)
        {
            throw new System.NotImplementedException();
        }

        public string GetNewBoardHeight(int currentRows)
        {
            throw new System.NotImplementedException();
        }

        public string GetNewBoardWidth(int currentCols)
        {
            throw new System.NotImplementedException();
        }

        public string GetExistingShipSize(Rules rules)
        {
            throw new System.NotImplementedException();
        }

        public void Continue()
        {
            throw new System.NotImplementedException();
        }
    }
}