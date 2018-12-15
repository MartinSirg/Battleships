using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DAL;
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
        private DbContext DbContext;
        public string SelectedMode { get; set; }

        public Game(IUserInterface ui, DbContext dbContext)
        {
            Rules = Rules.GetDefaultRules();
            UI = ui;
            DbContext = dbContext;
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
            
            //TODO: check if anyone has won!
            return "CHANGE NAME";
        }
        
        //TODO: Generate battleships for board
        
        public string RunMenu(Menu menu)
        {
            bool done = false;
            do
            {
                //Check menu title name
                UI.DisplayNewMenu(menu);
                var chosenShortcut = UI.GetMenuShortcut().ToUpper();
                if (chosenShortcut == menu.Previous.Shortcut) return menu.Previous.Shortcut;
                if (chosenShortcut == "Q" && !menu.Title.Equals("Main menu"))
                {
                    //TODO: more checks(not in game)
                    return "Q";

                } 

                var menuItem = menu.MenuItems.FirstOrDefault(item => item.Shortcut.Equals(chosenShortcut));
                if (menuItem == null)
                {
                    
                    UI.Alert("No such shortcut", 500);
                    continue;
                }

                if (menuItem.CommandToExecute == null)
                {
                    UI.Alert("Command not specified", 500);
                    continue;
                    
                }

                var postActionCommand = menuItem.CommandToExecute();
                if (postActionCommand.Equals("Q") && !menu.Title.Equals("Main menu"))
                {
                    return "Q";
                }
                if (postActionCommand.Equals("FINISHED"))
                {
                    break;
                }

                if (postActionCommand.Equals("CHANGE NAME"))
                {
                    String namedTitle = menu.TitleWithName;
                    menu.Title = namedTitle.Replace("PLAYER_NAME", CurrentPlayer.Name);
                }
//                menu.NameInTitle = CurrentPlayer.Name;
            } while (!done);

            return "";
        }

        public string SaveGame()
        {
            DbContext.Rules.Add(Rules);
            DbContext.Boards.Add(Player1.Board);
            DbContext.Boards.Add(Player2.Board);
            DbContext.Players.Add(Player1);
            DbContext.Players.Add(Player2);
            DbContext.GameMoves.Add(GameMoves);
            string name = UI.GetSaveGameName();
            while (DbContext.TotalGame.Exists(tuple => tuple.name.Equals(name)))
            {
                UI.Alert($"Enter a different name {name} is already taken.", 0);
                name = UI.GetSaveGameName();
            }
            
            DbContext.TotalGame.Add((
                name,
                DbContext.GameMoves.FindIndex(moves => moves == GameMoves),
                DbContext.Players.FindIndex(player => player == Player1),
                DbContext.Players.FindIndex(player => player == Player2),
                DbContext.Boards.FindIndex(board => board == Player1.Board),
                DbContext.Boards.FindIndex(board => board == Player2.Board),
                DbContext.Rules.FindIndex(rules => rules == Rules)
                ));
            CurrentPlayer = new Player(new Board(), "Player 1" );
            TargetPlayer = new Player(new Board(), "Player 2" );
            return "Q";
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
            CurrentPlayer.Name = UI.GetString("Enter your name");
            return "CHANGE NAME";
        }

        public void SwitchCurrentPlayer()
        {
            Player temp = CurrentPlayer;
            CurrentPlayer = TargetPlayer;
            TargetPlayer = temp;
        }

        public bool CheckIfCanStartGame()
        {
            if (SelectedMode == null)
            {
                UI.Alert("Game mode is not selected", 1000);
                return false;
            }
            
            //TODO: check if all ships have been placed
            
            if (SelectedMode.Equals("SP")) return CurrentPlayer.IsReady;
            return CurrentPlayer.IsReady && TargetPlayer.IsReady;
        }

        public string ShowShipsAndBombings()
        {
            UI.ShowShipsAndBombings(TargetPlayer.Board, CurrentPlayer.Board);
            return "";
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
            CurrentPlayer.IsReady = false;
        }

        public void SetSelectedMode(string modeName)
        {
            switch (modeName)
            {
                    case "MP" : 
                        SelectedMode = "MP";
                        break;
                    case "SP" :
                        SelectedMode = "SP";
                        break;
                    default:
                        throw new Exception("Unknown exception at Game.SetSelectedMode");
            }
        }

        

        public void SetPlayerReady()
        {
            CurrentPlayer.IsReady = true;
        }

        public void Alert(string message, int waitTime)
        {
            UI.Alert(message, waitTime);
        }

        public string LoadGame()
        {
            List<string> names = new List<string>();
            DbContext.TotalGame.ForEach(tuple => names.Add(tuple.name));
            UI.DisplaySavedGames(names);
            int num;
            while (true)
            {
                string saveGameNumber = UI.GetString("Enter saved game number or Q to go back");

                if (saveGameNumber.ToUpper().Equals("Q")) return "Q";
                if (!int.TryParse(saveGameNumber, out num))
                {
                    UI.Alert("Not a number", 0);
                    continue;
                }

                if (int.Parse(saveGameNumber) <= 0 || int.Parse(saveGameNumber) > names.Count)
                {
                    UI.Alert("No such save game number", 0);
                    continue;
                }
                break;
            }

            (string name, int gameMoves, int player1, int player2, int p1board, int p2board, int rules) totalgame =
                DbContext.TotalGame[num - 1];
            Player1 = DbContext.Players[totalgame.player1];
            Player2 = DbContext.Players[totalgame.player2];
            Rules = DbContext.Rules[totalgame.rules];
            GameMoves = DbContext.GameMoves[totalgame.gameMoves];
            if (GameMoves.Moves.Count == 0)
            {
                TargetPlayer = Player2;
                CurrentPlayer = Player1;
            }
            else
            {
                CurrentPlayer = GameMoves.Moves[GameMoves.Moves.Count - 1].target;
                TargetPlayer = CurrentPlayer == Player1 ? Player2 : Player1;
            }

            return "";
        }

        public string ReplayGame()
        {
            throw new NotImplementedException();
        }

//        public void ChangeMenuTitle(MenuEnum menuEnum)
//        {
//            throw new NotImplementedException();
//        }
        public void NewGame()
        {
            Board board1 = new Board(), board2 = new Board();
            Player1 = new Player(board1, "Player 1");
            Player2 = new Player(board2, "Player 2");
            CurrentPlayer = Player1;
            TargetPlayer = Player2;
        }
    }
}