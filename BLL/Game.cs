using System;
using System.Collections.Generic;
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
                Console.Clear();
                Print();
                var input = Console.ReadLine().ToUpper();
                if (input == menu.Previous.Shortcut) return menu.Previous.Shortcut;
                
                var menuItem = menu.MenuItems.FirstOrDefault( (MenuItem m) => m.Shortcut == input);
                if (menuItem == null)
                {

                    Console.WriteLine($"Incorrect choice: {input}");
                    WaitForUser();
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
    }
}