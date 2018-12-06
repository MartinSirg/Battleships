using System;
using System.Collections.Generic;
using System.Linq;
using Domain;
using MenuSystem;

namespace BLL
{
    public class Game
    {
        public Rules Rules { get; set; }
        public Player Player1 { get; set; }
        public Player Player2 { get; set; }
        public Player CurrentPlayer { get; set; }
        public Player TargetPlayer { get; set; }
        public GameMoves GameMoves = new GameMoves();
        private LetterNumberSystem Converter = new LetterNumberSystem();
        private IUserInterface UI;

        public Game(IUserInterface ui)
        {
            Rules = Rules.GetDefaultRules();
            UI = ui;
            var board1 = new Board(Rules.BoardRows, Rules.BoardCols, Rules.CanShipsTouch);
            var board2 = new Board(Rules.BoardRows, Rules.BoardCols, Rules.CanShipsTouch);
            
            Player1 = new Player(board1, "Player1");
            Player2 = new Player(board2, "Player2");
            CurrentPlayer = Player1;
            TargetPlayer = Player2;
        }
        
        
        public string BombShip()
        {
            var target = TargetPlayer;
            string targetLocation = UI.GetTargetLocation(target.Board);
            
            if (string.IsNullOrEmpty(targetLocation))
            {
                throw new ArgumentException("Empty target location string");
            }

            string stringRow = "", stringCol = "";
            foreach (var c in targetLocation)
            {
                if (stringCol.Length == 0 && Char.IsLetter(c)) stringRow += c;
                else if (stringRow.Length > 0 && char.IsDigit(c)) stringCol += c;
                else throw new ArgumentException("Invalid location");
            }

            int row = Converter.GetNumberFromLetters(stringRow) - 1, col = int.Parse(stringCol) - 1;
            bool result = target.Board.BombLocation(row, col);
            BombingResult b = result ? BombingResult.Hit : BombingResult.Miss;
            GameMoves.Moves.Add((target, target.Board.Tiles[row][col], b));
            UI.ShowBombingResult(b, target.Board);
            
            //Switching player status
            Player temp = TargetPlayer;
            TargetPlayer = CurrentPlayer;
            CurrentPlayer = temp;
            UI.CurrentPlayer = CurrentPlayer;
            UI.TargetPlayer = TargetPlayer;
            return "";
        }
        
        //TODO: Generate battleships for board
        
        public string RunMenu(Menu menu)
        {
            bool done = false;
            do
            {
                //Check menu title name
                UI.DisplayNewMenu(menu);        //TODO: vaata yle kas oleks m6tekam kokku viia need kaks meetodit
                var chosenShortcut = UI.GetMenuShortcut().ToUpper();
                if (chosenShortcut == menu.Previous.Shortcut) return menu.Previous.Shortcut;
                if (chosenShortcut == "Q")
                {
                    //TODO: return to main menu  
                } 

                var menuItem = menu.MenuItems.FirstOrDefault(item => item.Shortcut.Equals(chosenShortcut));
                if (menuItem == null)
                {
                    
                    UI.Alert("No such shortcut");
                    UI.WaitForUser();
                    continue;
                }
                //TODO: check if commandtoexecute is not null

                var commandChosen = menuItem.CommandToExecute();
                menu.NameInTitle = CurrentPlayer.Name;
            } while (!done);

            return "";
        }

        public string SaveGame()
        {
            throw new NotImplementedException();
        }

        public string AddShipToRules()
        {
            throw new NotImplementedException();
        }

        public string EditShipInRules()
        {
            throw new NotImplementedException();
        }

        public string DeleteShipInRules()
        {
            throw new NotImplementedException();
        }

        public string EditShipsCanTouchRule()
        {
            throw new NotImplementedException();
        }

        public string EditBoardHeight()
        {
            throw new NotImplementedException();
        }

        public string EditBoardWidth()
        {
            throw new NotImplementedException();
        }

        public string SetRulesName()
        {
            throw new NotImplementedException();
        }

        public string SetStandardRules()
        {
            throw new NotImplementedException();
        }

        public string LoadCustomRulesPreset()
        {
            throw new NotImplementedException();
        }

        public string PlaceShipOnBoard()
        {
            throw new NotImplementedException();
        }

        public string DeleteShipFromBoard()
        {
            throw new NotImplementedException();
        }

        public string ChangePlayersName()
        {
            throw new NotImplementedException();
        }

        public string SwitchCurrentPlayer()
        {
            throw new NotImplementedException();
        }

        public bool CheckIfCanStartGame()
        {
            throw new NotImplementedException();
        }

        public string ShowShipsAndBombings()
        {
            throw new NotImplementedException();
        }

        public void DisplayRulesShips()
        {
            throw new NotImplementedException();
        }

        public void ShowBoardRules()
        {
            throw new NotImplementedException();
        }

        public void ShowCurrentShips()
        {
            throw new NotImplementedException();
        }

        public string GetTileOfDeleteableShip()
        {
            throw new NotImplementedException();
        }

        public void ShowCurrentRuleset()
        {
            throw new NotImplementedException();
        }

        public void ShowCurrentAndAvailableShips()
        {
            throw new NotImplementedException();
        }

        public string GetShipStartTile()
        {
            throw new NotImplementedException();
        }

        public string GetShipEndTile()
        {
            throw new NotImplementedException();
        }

        public void SetPlayerNotReady() //Current player.ready = true vms
        {
            throw new NotImplementedException();
        }

        public void SetSelectedMode(string modeName)
        {
            throw new NotImplementedException();
        }

        public void SetPlayerReady()
        {
            throw new NotImplementedException();
        }

        public void Alert(string message)
        {
            throw new NotImplementedException();
        }

        public void LoadGame()
        {
            throw new NotImplementedException();
        }

        public string ReaplayGame()
        {
            throw new NotImplementedException();
        }
    }
}