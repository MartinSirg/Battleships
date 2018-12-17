using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
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
        public string SelectedMode { get; set; } = "";

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
            Tile targetTile;
            while (true)
            {
                string userInput = UI.GetTargetLocation(target.Board).ToUpper();
                if (userInput.Equals("X")) return "";
                targetTile = GetTile(userInput, target.Board);
                if (targetTile == null) UI.Alert("Invalid location", 1000);
                else if(targetTile.IsBombed) UI.Alert("Target already bombed", 1000);
                else break;
            }
            bool result = target.Board.BombLocation(targetTile.Row, targetTile.Col);
            BombingResult b = result ? BombingResult.Hit : BombingResult.Miss;
            GameMoves.Moves.Add((target, targetTile, b));
            UI.ShowBombingResult(b, target.Board);
            //TODO: check if anyone has won!
            //Switching player status
            if (SelectedMode.Equals("MP"))
            {
                Player temp = TargetPlayer;
                TargetPlayer = CurrentPlayer;
                CurrentPlayer = temp;
                return "CHANGE NAME";
            }
            
            //TODO: possible error: after going into game enemy board resets
            List<Tile> tiles = new List<Tile>();
            CurrentPlayer.Board.Tiles.ForEach(row => row.ForEach(tile => tiles.Add(tile))); //Adds all tiles to list
            CurrentPlayer.Board.Bombings.ForEach(tuple => tiles.Remove(tuple.tile)); //Removes bombed tiles from list
            Tile computerTarget = tiles[new Random().Next(0, tiles.Count - 1)];
            bool computerResultbool = CurrentPlayer.Board.BombLocation(computerTarget.Row, computerTarget.Col);
            BombingResult computerResult = computerResultbool ? BombingResult.Hit : BombingResult.Miss;
            GameMoves.Moves.Add((CurrentPlayer, computerTarget, computerResult));
            UI.ShowBombingResult(computerResult, CurrentPlayer.Board);
            return "";
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
                if (postActionCommand.Equals("Q") && !menu.Title.Equals("Main menu")) return "Q";
                if (postActionCommand.Equals("FINISHED"))break;
                PostAction(menu, postActionCommand);

//                menu.NameInTitle = CurrentPlayer.Name;
            } while (!done);

            return "";
        }

        private void PostAction(Menu menu, string postActionCommand)
        {
            //Changes menu's title
            if (postActionCommand.Equals("CHANGE NAME"))
            {
                String namedTitle = menu.TitleWithName;
                menu.Title = namedTitle.Replace("PLAYER_NAME", CurrentPlayer.Name);
            }
            //Highlights spot on board
            else if (postActionCommand.Contains("HIGHLIGHT_START"))
            {
                
            } 
            else if (postActionCommand.Contains("HIGHLIGHT_END"))
            {
                if (CurrentPlayer.Board.HighLightedEnd != null)
                    CurrentPlayer.Board.HighLightedEnd.IsHighlightedEnd = false;
                string stringRow = "", stringCol = "", tile = postActionCommand.Replace("HIGHLIGHT_END:", "");
                foreach (var c in tile)
                {
                    if (stringCol.Length == 0 && Char.IsLetter(c)) stringRow += c;
                    else if (stringRow.Length > 0 && char.IsDigit(c)) stringCol += c;
                    else throw new ArgumentException("Invalid location");
                }
                int row = Converter.GetNumberFromLetters(stringRow) - 1, col = int.Parse(stringCol) - 1;
                CurrentPlayer.Board.Tiles[row][col].IsHighlightedEnd = true;
                CurrentPlayer.Board.HighLightedEnd = CurrentPlayer.Board.Tiles[row][col];
            }
        }

        public string SaveGame()
        {
            string name = UI.GetSaveGameName();
            if (name.ToUpper().Equals("X")) return "";
            
            while (DbContext.TotalGame.Exists(tuple => tuple.name.Equals(name)))
            {
                UI.Alert($"Enter a different name {name} is already taken.", 0);
                name = UI.GetSaveGameName();
                if (name.ToUpper().Equals("X")) return "";
            }
            
            
            DbContext.Rules.Add(Rules);
            DbContext.Boards.Add(Player1.Board);
            DbContext.Boards.Add(Player2.Board);
            DbContext.Players.Add(Player1);
            DbContext.Players.Add(Player2);
            DbContext.GameMoves.Add(GameMoves);
            
            
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
            //Key size, Value Quantity;
            Dictionary<int, int>  boats = new Dictionary<int, int>();
            Rules.BoatSizesAndQuantities.ForEach(tuple => boats.Add(tuple.size, tuple.quantity));
            CurrentPlayer.Board.Battleships.ForEach(battleship => boats[battleship.Size]--);
            Tile start = CurrentPlayer.Board.HighlightedStart, end = CurrentPlayer.Board.HighLightedEnd;
            if (start == null || end == null)
            {
                UI.Alert("Start and End not specified", 1000);
                return "";
            }

            bool hasCommonAxis = start.Row - end.Row == 0 || start.Col - end.Col == 0;
            if (!hasCommonAxis)
            {
                UI.Alert("Cant place ship. Start and end tile must have one common axis", 4000);
                return "";
            }

            var length = start.Row - end.Row == 0 ? Math.Abs(start.Col - end.Col) + 1: Math.Abs(start.Row - end.Row) + 1;
            if (!boats.ContainsKey(length))
            {
                UI.Alert($"size {length} ship is not allowed.", 2000);
                return "";
            }
            if (boats[length] <= 0)
            {
                UI.Alert($"You can't place more size {length} ships", 2000);
                return "";
            }

            try
            {
                CurrentPlayer.Board.AddBattleship((start.Row, start.Col), (end.Row, end.Col), new Battleship(length));
                start.IsHighlightedStart = false;
                end.IsHighlightedEnd = false;
                CurrentPlayer.Board.HighLightedEnd = null;
                CurrentPlayer.Board.HighlightedStart = null;
                return "";
            }
            catch (Exception e)
            {
                UI.Alert(e.Message, 5000);
                return "";
            }
        }

        public string DeleteShipFromBoard()
        {
            Tile deletable = CurrentPlayer.Board.HighlightedStart;
            if (deletable == null)
            {
                UI.Alert("Tile of occupying ship not specified", 1500);
                return "";
            }

            if (deletable.IsEmpty())
            {
                UI.Alert("Selected tile does not contain a ship", 1000);
                return "";
            }
            CurrentPlayer.Board.DeleteShipFromBoard(deletable);
            deletable.IsHighlightedStart = false;
            return "";
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

        public void ShowCurrentShipsRegular()
        {
            UI.DisplayCurrentShips(CurrentPlayer.Board, "REGULAR");
        }

        public string GetTileOfDeleteableShip()
        {
            if (CurrentPlayer.Board.HighlightedStart != null)
                CurrentPlayer.Board.HighlightedStart.IsHighlightedStart = false;
            Tile startTile;
            while (true)
            {
                string userInput = UI.GetDeletableShipTile().ToUpper();
                if (userInput.Equals("X")) return "";
                startTile = GetTile(userInput, CurrentPlayer.Board);
                if (startTile == null) UI.Alert("Invalid location", 0);
                else if (startTile.IsEmpty()) UI.Alert("Tile doesn't contain a ship", 0);
                else break;
            }

            startTile.IsHighlightedStart = true;
            CurrentPlayer.Board.HighlightedStart = startTile;
            return "";
        }

        public void ShowCurrentRuleset()
        {
            throw new NotImplementedException();
        }

        public void ShowCurrentAndAvailableShips()
        {
            UI.DisplayCurrentShips(CurrentPlayer.Board, "ADDING");
            
            Dictionary<int, int>  ships = new Dictionary<int, int>();
            Rules.BoatSizesAndQuantities.ForEach(tuple => ships.Add(tuple.size, tuple.quantity));
            CurrentPlayer.Board.Battleships.ForEach(battleship => ships[battleship.Size]--);
            
            UI.DisplayAvailableShips(ships.ToList());
        }

        public string GetShipStartTile()
        {
            if (CurrentPlayer.Board.HighlightedStart != null)
                CurrentPlayer.Board.HighlightedStart.IsHighlightedStart = false;
            Tile startTile;
            while (true)
            {
                ShowCurrentAndAvailableShips();
                string userInput = UI.GetShipStartPoint(CurrentPlayer.Board, AvailableShipsList(CurrentPlayer).ToList()).ToUpper();
                if (userInput.Equals("X")) return "";
                startTile = GetTile(userInput, CurrentPlayer.Board);
                if (startTile == null) UI.Alert("Invalid location", 0);
                else if (startTile.IsEmpty() == false) UI.Alert("This tile already contains a ship", 0);
                else break;
            }

            startTile.IsHighlightedStart = true;
            CurrentPlayer.Board.HighlightedStart = startTile;
            return "";
        }

        private Dictionary<int, int> AvailableShipsList(Player player)
        {
            Dictionary<int, int>  availableShips = new Dictionary<int, int>();
            Rules.BoatSizesAndQuantities.ForEach(tuple => availableShips.Add(tuple.size, tuple.quantity));
            player.Board.Battleships.ForEach(battleship => availableShips[battleship.Size]--);
            return availableShips;
        }

        private Tile GetTile(string location, Board board)
        {
            string stringRow = "", stringCol = "";
            foreach (var c in location)
            {
                if (stringCol.Length == 0 && Char.IsLetter(c)) stringRow += c;
                else if (stringRow.Length > 0 && char.IsDigit(c)) stringCol += c;
                else
                {
                    return null;
                }
            }

            try
            {
                int row = Converter.GetNumberFromLetters(stringRow) - 1, col = int.Parse(stringCol) - 1;
                return board.Tiles[row]?[col];
            }
            catch (IndexOutOfRangeException e)
            {
                return null;
            }
            catch (ArgumentException e)
            {
                return null;
            }
            catch (FormatException e)
            {
                return null;
            }
        }

        public string GetShipEndTile()
        {
            if (CurrentPlayer.Board.HighLightedEnd != null)
                CurrentPlayer.Board.HighLightedEnd.IsHighlightedEnd = false;
            Tile endTile;
            while (true)
            {
                string userInput = UI.GetShipEndPoint(CurrentPlayer.Board, AvailableShipsList(CurrentPlayer).ToList()).ToUpper();
                if (userInput.Equals("X")) return "";

                endTile = GetTile(userInput, CurrentPlayer.Board);
                if (endTile == null) UI.Alert("Invalid location", 0);
                else if(endTile.IsEmpty() == false) UI.Alert("This tile already contains a ship", 0);
                else break;
            }
            
            endTile.IsHighlightedEnd = true;
            CurrentPlayer.Board.HighLightedEnd = endTile;
            return "";
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

        public void ShowCurrentShipsDeleting()
        {
            UI.DisplayCurrentShips(CurrentPlayer.Board, "DELETING");
        }

        public void ClearCurrentHighlights()
        {
            if (CurrentPlayer.Board.HighlightedStart != null)
            {
                CurrentPlayer.Board.HighlightedStart.IsHighlightedStart = false;
                CurrentPlayer.Board.HighlightedStart = null;   
            }

            if (CurrentPlayer.Board.HighLightedEnd != null)
            {
                CurrentPlayer.Board.HighLightedEnd.IsHighlightedEnd = false;
                CurrentPlayer.Board.HighLightedEnd = null;   
            }
        }

        public string GenerateOpponent()
        {
            TargetPlayer.Name = "Computer";
            GenerateRandomBoard(TargetPlayer);
            
            UI.DisplayCurrentShips(TargetPlayer.Board, "REGULAR");
            UI.Alert("", 10000);
            return "";

        }

        private void GenerateRandomBoard(Player player)
        {
            Dictionary<int, int> availableShips = AvailableShipsList(player);
            var random = new Random();

            while (availableShips.Any(pair => pair.Value > 0)) // Loop til there are ships available
            {
                int sizeKey = availableShips.First(pair => pair.Value > 0).Key;
                var ship = new Battleship(sizeKey);
                var counter = 1;
                while (true)
                {
                    if (++counter > 50000)
                    {
                        UI.Alert("Infinite loop", 5000);
                        return;
                    }
                    Tile start = player.Board.Tiles[random.Next(Rules.BoardRows)][random.Next(Rules.BoardCols)];
                    bool isVertical = random.Next(0, 2) == 0;
                    int directionSign = random.Next(0, 2) == 0 ? -1 : 1;
                    if (isVertical)
                    {
                        try
                        {
                            Tile end = player.Board.Tiles[start.Row + (sizeKey - 1) * directionSign][start.Col];
                            player.Board.AddBattleship((start.Row, start.Col), (end.Row, end.Col), ship);
                            availableShips[sizeKey]--;
                            break;

                        }
                        catch (ArgumentException e)
                        {
                            
                        }
                        catch (IndexOutOfRangeException e)
                        {
                            
                        }
                        
                    }
                    else
                    {
                        try
                        {
                            Tile end = player.Board.Tiles[start.Row][start.Col + (sizeKey - 1) * directionSign];
                            player.Board.AddBattleship((start.Row, start.Col), (end.Row, end.Col), ship);
                            availableShips[sizeKey]--;
                            break;
                        }
                        catch (ArgumentException e)
                        {
                            
                        }
                        catch (IndexOutOfRangeException e)
                        {
                            
                        }
                    }
                }
            }
        }

        public void ResetTargetPlayer(String name)
        {
            TargetPlayer.Board = new Board();
            TargetPlayer.Name = name;
        }
    }
}