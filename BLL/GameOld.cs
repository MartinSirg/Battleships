using System;
using System.Collections.Generic;
using System.Linq;
using DAL;
using Domain;
using MenuSystem;
using Microsoft.EntityFrameworkCore;
using WebApp;

namespace BLL
{
    public class GameOld
    {
        public Rules Rules { get; set; }
        public Player Player1 { get; set; }
        public Player Player2 { get; set; }
        public Player CurrentPlayer { get; set; }
        public Player TargetPlayer { get; set; }
        public List<GameMove> GameMoves = new List<GameMove>();
        private LetterNumberSystem Converter = new LetterNumberSystem();
        private IUserInterface UI;
        public AppDbContext Ctx;
        public string SelectedMode { get; set; } = "";
        public const int MAX_ROWS = 24, MIN_ROWS = 10, MIN_COLS = 10, MAX_COLS = 24;

        public GameOld(IUserInterface ui, AppDbContext dbContext)
        {
            Rules = Rules.GetDefaultRules();
            UI = ui;
            Ctx = dbContext;
            var board1 = new Board(Rules.BoardRows, Rules.BoardCols, Rules.CanShipsTouch);
            var board2 = new Board(Rules.BoardRows, Rules.BoardCols, Rules.CanShipsTouch);
            
            Player1 = new Player(board1, "Player1");
            Player2 = new Player(board2, "Player2");
            CurrentPlayer = Player1;
            TargetPlayer = Player2;
        }
        
        public string BombShip()
        {
            Tile targetTile;
            while (true)
            {
                string userInput = UI.GetTargetLocation(TargetPlayer.Board, CurrentPlayer).ToUpper();
                if (userInput.Equals("X")) return "";
                targetTile = GetTile(userInput, TargetPlayer.Board);
                if (targetTile == null) UI.Alert("Invalid location", 1000);
                else if(targetTile.IsBombed) UI.Alert("Target already bombed", 1000);
                else break;
            }
            bool result = TargetPlayer.Board.BombLocation(targetTile.Row, targetTile.Col);
            BombingResult b = result ? BombingResult.Hit : BombingResult.Miss;
            GameMoves.Add(new GameMove(TargetPlayer, targetTile));
            UI.DisplayBombingResult(b, TargetPlayer.Board, CurrentPlayer);
            UI.Continue();

            if (TargetPlayer.Board.AnyShipsLeft() == false)
            {
                UI.Alert($"Game over. {CurrentPlayer.Name} has won!", 0);
                UI.Continue();
                SaveFinishedGame();
                return "Q";
            }
            if (SelectedMode.Equals("MP"))
            {
                Player temp = TargetPlayer;
                TargetPlayer = CurrentPlayer;
                CurrentPlayer = temp;
                return "CHANGE NAME";
            }
            
            //Generating random bombing location for computer
            List<Tile> tiles = new List<Tile>();                
            CurrentPlayer.Board.Tiles.ForEach(row => row.ForEach(tile =>
            {
                if (!tile.IsBombed) tiles.Add(tile);
            })); //Adds all not bombed tiles to list
            
            
            Tile computerTarget = tiles[new Random().Next(0, tiles.Count - 1)];
            bool computerResultbool = CurrentPlayer.Board.BombLocation(computerTarget.Row, computerTarget.Col);
            BombingResult computerResult = computerResultbool ? BombingResult.Hit : BombingResult.Miss;
            GameMoves.Add(new GameMove(CurrentPlayer, computerTarget));
            UI.DisplayBombingResult(computerResult, CurrentPlayer.Board, TargetPlayer);
            UI.Continue();
            
            if (CurrentPlayer.Board.AnyShipsLeft() == false)
            {
                UI.Alert($"Game over. {TargetPlayer.Name} has won!", 0);
                UI.Continue();
                SaveFinishedGame();
                return "Q";
            }
            
            return "";
        }

        private void SaveFinishedGame()
        {
            string name = UI.GetSaveGameName();
            while (Ctx.SaveGames.ToList().Any(game => game.Name.Equals(name)))
            {
                UI.Alert($"Enter a different name {name} is already taken.", 0);
                name = UI.GetSaveGameName();
            }

            var saveGame = new SaveGame
            {
                IsFinished = true,
                Rules = Rules,
                Player1 = Player1,
                Player2 = Player2,
                GameMoves = GameMoves,
                Name = name
            };
            Ctx.SaveGames.Add(saveGame);
            Ctx.SaveChanges();


            Rules = Rules.GetDefaultRules();
            CurrentPlayer = new Player(new Board(Rules.BoardRows, Rules.BoardCols, Rules.CanShipsTouch), "Player 1" );
            TargetPlayer = new Player(new Board(Rules.BoardRows, Rules.BoardCols, Rules.CanShipsTouch), "Player 2" );
            Player1 = CurrentPlayer;
            Player2 = TargetPlayer;
            GameMoves = new List<GameMove>();
        }

        public string RunMenu(Menu menu)
        {
            bool done = false;
            do
            {
                //Check menu title name
                UI.DisplayNewMenu(menu);
                var chosenShortcut = UI.GetMenuShortcut().ToUpper();
                if (chosenShortcut == menu.Previous?.Shortcut) return menu.Previous.Shortcut;
                if (chosenShortcut == "Q" && !menu.Title.Equals("Main menu") && !menu.Title.EndsWith("'s turn"))
                {
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
            
            while (Ctx.SaveGames.ToList().Any(game => game.Name.Equals(name)))
            {
                UI.Alert($"Enter a different name {name} is already taken.", 0);
                name = UI.GetSaveGameName();
                if (name.ToUpper().Equals("X")) return "";
            }

            var saveGame = new SaveGame
            {
                IsFinished = false,
                Rules = Rules,
                Player1 = Player1,
                Player2 = Player2,
                GameMoves = GameMoves,
                Name = name
            };
            Ctx.SaveGames.Add(saveGame);
            Ctx.SaveChanges();


            Rules = Rules.GetDefaultRules();
            CurrentPlayer = new Player(new Board(Rules.BoardRows,Rules.BoardCols, Rules.CanShipsTouch ), "Player 1" );
            TargetPlayer = new Player(new Board(Rules.BoardRows,Rules.BoardCols, Rules.CanShipsTouch ), "Player 2" );
            Player1 = CurrentPlayer;
            Player2 = TargetPlayer;
            GameMoves = new List<GameMove>();
            return "Q";
        }

        public string AddShipToRules()
        {
            while (true)
            {
                string userInputSize = UI.GetShipSize(Rules);
                if (userInputSize.ToUpper().Equals("X")) return "";
                List<int> currentSizes = new List<int>();
                Rules.BoatRules.ForEach(rule => currentSizes.Add(rule.Size));
                if (!int.TryParse(userInputSize, out var size) || size < 1 || size > 10 || currentSizes.Contains(size))
                {
                    UI.Alert("Invalid input", 500);
                    continue;
                }

                int quantity;
                while (true)
                {
                    string userInputQuantity = UI.GetShipQuantity(size);
                    if (userInputQuantity.ToUpper().Equals("X")) return "";
                    if (!int.TryParse(userInputQuantity, out quantity) || quantity < 1 || quantity > 5)
                    {
                        UI.Alert("Invalid input", 500);
                        continue;
                    }
                    break;
                }
                Rules.BoatRules.Add(new BoatRule(size, quantity));
                Rules.BoatRules.Sort((rule, boatRule) =>
                {
                    if(rule.Size > boatRule.Size) return 1;
                    if(rule.Size < boatRule.Size) return -1;
                    return 0;
                });
                return "";
            }
        }

        public string EditShipInRules()
        {
            while (true)
            {
                string userInputSize = UI.GetExistingShipSize(Rules);
                if (userInputSize.ToUpper().Equals("X")) return "";
                if (!int.TryParse(userInputSize, out var size) || Rules.BoatRules.All(rule => rule.Size != size))
                {
                    UI.Alert("Invalid input", 500);
                    continue;
                }

                int quantity;
                while (true)
                {
                    string userInputQuantity = UI.GetShipQuantity(size);
                    if (userInputQuantity.ToUpper().Equals("X")) return "";
                    if (!int.TryParse(userInputQuantity, out quantity) || quantity < 1 || quantity > 5)
                    {
                        UI.Alert("Invalid input", 500);
                        continue;
                    }
                    break;
                }

                int index = 0;
                Rules.BoatRules.Find(rule => rule.Size == size).Quantity = quantity;
     
                CurrentPlayer.Board = new Board(Rules.BoardRows, Rules.BoardCols, Rules.CanShipsTouch);
                TargetPlayer.Board = new Board(Rules.BoardRows, Rules.BoardCols, Rules.CanShipsTouch);
                return "";
            }
        }

        public string DeleteShipInRules()
        {
            while (true)
            {
                string userInputSize = UI.GetExistingShipSize(Rules);
                if (userInputSize.ToUpper().Equals("X")) return "";
                if (!int.TryParse(userInputSize, out var size) || Rules.BoatRules.All(rule => rule.Size != size))
                {
                    UI.Alert("Invalid input", 500);
                    continue;
                }
                Rules.BoatRules.Remove(Rules.BoatRules.Find(rule => rule.Size == size));
                CurrentPlayer.Board = new Board(Rules.BoardRows, Rules.BoardCols, Rules.CanShipsTouch);
                TargetPlayer.Board = new Board(Rules.BoardRows, Rules.BoardCols, Rules.CanShipsTouch);
                return "";
            }
        }

        public string EditShipsCanTouchRule()
        {
            while (true)
            {
                string userInput = UI.GetShipsCanTouch(Rules.CanShipsTouch).ToUpper();
                if (userInput.Equals("X")) return "";
                if (!userInput.Equals("YES") && !userInput.Equals("NO"))
                {
                    UI.Alert("Invalid input", 1000);
                    continue;
                }

                if (userInput.Equals("YES") && Rules.CanShipsTouch == 0 || //User input and current are different
                    userInput.Equals("NO") && Rules.CanShipsTouch == 1)
                {
                    Rules.CanShipsTouch = userInput.Equals("YES") ? 1 : 0;
                    CurrentPlayer.Board = new Board(Rules.BoardRows,Rules.BoardCols, Rules.CanShipsTouch);
                    TargetPlayer.Board = new Board(Rules.BoardRows,Rules.BoardCols, Rules.CanShipsTouch);
                }

                return "";
            }
        }

        public string EditBoardHeight()
        {
            while (true)
            {
                string userInput = UI.GetNewBoardHeight(Rules.BoardRows);
                if (userInput.ToUpper().Equals("X")) return "";
                if (!int.TryParse(userInput, out var rows) || rows < MIN_ROWS || rows > MAX_ROWS)
                {
                    UI.Alert("Invalid input, enter a number or X", 2000);
                    continue;
                }
                if (Rules.BoardRows == rows) return "";        //current is new, else reset boards
                CurrentPlayer.Board = new Board(rows, Rules.BoardCols, Rules.CanShipsTouch);
                TargetPlayer.Board = new Board(rows, Rules.BoardCols, Rules.CanShipsTouch);
                Rules.BoardRows = rows;
                return "";
            }
        }

        public string EditBoardWidth()
        {
            while (true)
            {
                string userInput = UI.GetNewBoardWidth(Rules.BoardCols);
                if (userInput.ToUpper().Equals("X")) return "";
                if (!int.TryParse(userInput, out var cols) || cols < MIN_COLS || cols > MAX_COLS)
                {
                    UI.Alert("Invalid input, enter a number or X", 2000);
                    continue;
                }
                if (Rules.BoardCols == cols) return "";        //current is new, else reset boards
                
                CurrentPlayer.Board = new Board(Rules.BoardRows, cols, Rules.CanShipsTouch);
                TargetPlayer.Board = new Board(Rules.BoardRows, cols, Rules.CanShipsTouch);
                Rules.BoardCols = cols;
                return "";
            }
        }

        public string SetRulesetName()
        {
            while (true)
            {
                string userInput = UI.GetRulesetName();
                if (userInput.ToUpper().Equals("X")) return "";
                if (userInput.ToUpper().Equals("STANDARD RULES"))
                {
                    UI.Alert("Name can't be Standard rules", 2000);
                    continue;
                }
                Rules.Name = userInput;
                return "";
            }
        }

        public string SetStandardRules()
        {
            Rules = Rules.GetDefaultRules();
            UI.Alert("Standard rules set", 500);
            return "";
        }

        public string PlaceShipOnBoard()
        {
            //Key size, Value Quantity;
            Dictionary<int, int>  boats = new Dictionary<int, int>();
            Rules.BoatRules.ForEach(rule => boats.Add(rule.Size, rule.Quantity));
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
            
            Dictionary<int, int>  ships = new Dictionary<int, int>();
            Rules.BoatRules.ForEach(rule => ships.Add(rule.Size, rule.Quantity));
            CurrentPlayer.Board.Battleships.ForEach(battleship => ships[battleship.Size]--);
            if (ships.Any(pair => pair.Value > 0))
            {
                UI.Alert($"All {CurrentPlayer.Name}'s ships have not been placed", 1000);
                return false;
            }
            
            ships = new Dictionary<int, int>();
            Rules.BoatRules.ForEach(rule => ships.Add(rule.Size, rule.Quantity));
            TargetPlayer.Board.Battleships.ForEach(battleship => ships[battleship.Size]--);
            if (ships.Any(pair => pair.Value > 0))
            {
                UI.Alert($"All {TargetPlayer.Name}'s ships have not been placed", 1000);
                return false;
            }

            if (SelectedMode.Equals("SP")) return CurrentPlayer.IsReady;
            return CurrentPlayer.IsReady && TargetPlayer.IsReady;
        }

        public string DisplayShipsAndBombings()
        {
            UI.DisplayShipsAndBombings(TargetPlayer.Board, CurrentPlayer.Board);
            return "";
        }

        public void DisplayRulesShips()
        {
            UI.DisplayRulesShips(Rules);
        }

        public void DisplayBoardRules()
        {
            UI.DisplayBoardRules(Rules);
        }

        public void DisplayCurrentShipsRegular()
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

        public void DisplayCurrentRuleset()
        {
            UI.DisplayCurrentRules(Rules);
        }

        public void DisplayCurrentAndAvailableShips()
        {
            UI.DisplayCurrentShips(CurrentPlayer.Board, "ADDING");
            
            Dictionary<int, int>  ships = new Dictionary<int, int>();
            Rules.BoatRules.ForEach(rule => ships.Add(rule.Size, rule.Quantity));
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
                DisplayCurrentAndAvailableShips();
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
            Rules.BoatRules.ForEach(rule => availableShips.Add(rule.Size, rule.Quantity));
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
            
            UI.DisplaySavedGames(Ctx.SaveGames.Where(game => game.IsFinished == false).OrderBy(game => game.SaveGameId).ToList());
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
                
                bool isIdPresent = Ctx.SaveGames
                    .Where(game => game.IsFinished == false).ToList()
                    .Any(game => game.SaveGameId == num);

                if (int.Parse(saveGameNumber) <= 0 || isIdPresent == false)
                {
                    UI.Alert("No such save game number", 0);
                    continue;
                }
                break;
            }


            SaveGame s = Ctx.SaveGames
                .Include(saveGame => saveGame.Rules)
                .ThenInclude(rules => rules.BoatRules)
                .Include(saveGame => saveGame.Player1)
                .ThenInclude(player => player.Board)
                .Include(saveGame => saveGame.Player2)
                .ThenInclude(player => player.Board)
                .Include(saveGame => saveGame.GameMoves)
                .First(saveGame => saveGame.SaveGameId == num);
            

            var tempRules = s.Rules;
            var tempPlayer1 = s.Player1;
            var tempPlayer2 = s.Player2;
            Rules = (Rules) tempRules.Clone();
            RestorePlayerBoardTiles(tempPlayer1, s);
            RestorePlayerBoardTiles(tempPlayer2, s);

            var tempGameMoves = s.GameMoves;
            Player1 = (Player) tempPlayer1.Clone();
            Player2 = (Player) tempPlayer2.Clone();
            
            GameMoves = new List<GameMove>();
            tempGameMoves.ForEach(move =>
            {
                var targetPlayer = Player1.Name == move.Target.Name ? Player1 : Player2; //TODO: use better solution than name comparison
                var targetTile = targetPlayer.Board.Tiles[move.Tile.Row][move.Tile.Col];
                GameMoves.Add(new GameMove(targetPlayer, targetTile));
            });
            
            

            if (GameMoves.Count == 0)
            {
                TargetPlayer = Player2;
                CurrentPlayer = Player1;
            }
            else
            {
                CurrentPlayer = GameMoves[GameMoves.Count - 1].Target;
                TargetPlayer = CurrentPlayer == Player1 ? Player2 : Player1;
            }
            
            
            
            

//            (string name, int gameMoves, int player1, int player2, int rules) totalGame =
//                _appDbContext.TotalGameOld[num - 1];
//            Player1 = _appDbContext.PlayersOld[totalGame.player1];
//            Player2 = _appDbContext.PlayersOld[totalGame.player2];
//            Rules = _appDbContext.RulesOld[totalGame.rules];
//            GameMoves = _appDbContext.GameMovesOld[totalGame.gameMoves];
//            if (GameMoves.Count == 0)
//            {
//                TargetPlayer = Player2;
//                CurrentPlayer = Player1;
//            }
//            else
//            {
//                CurrentPlayer = GameMoves[GameMoves.Count - 1].Target;
//                TargetPlayer = CurrentPlayer == Player1 ? Player2 : Player1;
//            }

            return "";
        }

        public string ReplayGame()
        {
            UI.DisplaySavedGames(Ctx.SaveGames.Where(game => game.IsFinished).OrderBy(game => game.SaveGameId).ToList());

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

                bool isIdPresent = Ctx.SaveGames
                    .Where(game => game.IsFinished).ToList()
                    .Any(game => game.SaveGameId == num);
                
                if (int.Parse(saveGameNumber) <= 0 || isIdPresent == false)
                {
                    UI.Alert("No such save game number", 0);
                    continue;
                }
                break;
            }
            
            SaveGame s = Ctx.SaveGames
                .Include(saveGame => saveGame.Rules)
                .ThenInclude(rules => rules.BoatRules)
                .Include(saveGame => saveGame.Player1)
                .ThenInclude(player => player.Board)
                .Include(saveGame => saveGame.Player2)
                .ThenInclude(player => player.Board)
                .Include(saveGame => saveGame.GameMoves)
                .First(saveGame => saveGame.SaveGameId == num);
            

            var tempRules = s.Rules;
            var tempPlayer1 = s.Player1;
            var tempPlayer2 = s.Player2;
            Rules = (Rules) tempRules.Clone();
            RestorePlayerBoardTiles(tempPlayer1, s);
            RestorePlayerBoardTiles(tempPlayer2, s);

            var tempGameMoves = s.GameMoves;
            Player1 = (Player) tempPlayer1.Clone();
            Player2 = (Player) tempPlayer2.Clone();
            
            GameMoves = new List<GameMove>();
            tempGameMoves.ForEach(move =>
            {
                var targetPlayer = Player1.Name == move.Target.Name ? Player1 : Player2; //TODO: use better solution than name comparison
                var targetTile = targetPlayer.Board.Tiles[move.Tile.Row][move.Tile.Col];
                GameMoves.Add(new GameMove(targetPlayer, targetTile));
            });
            
            GameMoves.ForEach(move => move.Target.Board.UnBomb(move.Tile));
            Player lastMoveBy = Player1;
            GameMoves.ForEach(move =>
            {
                Tile targetTile = move.Tile;
                bool result = move.Target.Board.BombLocation(targetTile.Row, targetTile.Col);
                BombingResult b = result ? BombingResult.Hit : BombingResult.Miss;
                UI.DisplayBombingResult(b, move.Target.Board, Player1 == move.Target ? Player2 : Player1);
                UI.Alert($"{move.Target.Name} is bombed at {Converter.GetLetter(move.Tile.Row + 1)}:{move.Tile.Col + 1} and it's a {(b == BombingResult.Hit ? "Hit" : "Miss")}", 0);
                UI.Continue();
                lastMoveBy = move.Target == Player1 ? Player2 : Player1;
            });
            UI.Alert($"{lastMoveBy.Name} has won the game!", 0);
            UI.Continue();
            
            ResetAll();
            return "";
        }

//        public void ChangeMenuTitle(MenuEnum menuEnum)
//        {
//            throw new NotImplementedException();
//        }
        public void NewGame()
        {
            Board board1 = new Board(Rules.BoardRows, Rules.BoardCols, Rules.CanShipsTouch),
                  board2 = new Board(Rules.BoardRows, Rules.BoardCols, Rules.CanShipsTouch);
            Player1 = new Player(board1, "Player 1");
            Player2 = new Player(board2, "Player 2");
            CurrentPlayer = Player1;
            TargetPlayer = Player2;
        }

        public void DisplayCurrentShipsDeleting()
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
            return "";

        }

        public string GenerateRandomBoard(Player player)
        {
            player.Board = new Board(Rules.BoardRows, Rules.BoardCols, Rules.CanShipsTouch);
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
                        return "";
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

            return "";
        }

        public void ResetTargetPlayer(String name)
        {
            TargetPlayer.Board = new Board(Rules.BoardRows,Rules.BoardCols,Rules.CanShipsTouch);
            TargetPlayer.Name = name;
        }

        public void RestorePlayerBoardTiles(Player player, SaveGame s)
        {
            
            List<List<Tile>> boardTiles = new List<List<Tile>>(s.Rules.BoardRows);
            List<Tile> tilesFromDb = Ctx.Tiles
                .Include(tile => tile.Board)
                .Include(tile => tile.Battleship)
                .Where(tile => tile.Board == player.Board).ToList();
            
            Console.WriteLine($"DbTilesCount: {tilesFromDb.Count}");
            for (int i = 0; i < s.Rules.BoardRows; i++)
            {
                boardTiles.Add(new List<Tile>(s.Rules.BoardCols));
                
                for (int j = 0; j < s.Rules.BoardCols; j++)
                {
                    if (tilesFromDb.Any(tile => tile.Row == i && tile.Col == j))
                        boardTiles[i].Add(tilesFromDb.Find(tile => tile.Row == i && tile.Col == j));
                    else
                        boardTiles[i].Add(new Tile(i, j, player.Board));
                }
            }
            player.Board.Tiles = boardTiles;
        }

        public void ResetAll()
        {
            Ctx = new AppDbContext();
            Rules = Rules.GetDefaultRules();
            CurrentPlayer = new Player(new Board(Rules.BoardRows,Rules.BoardCols, Rules.CanShipsTouch ), "Player 1" );
            TargetPlayer = new Player(new Board(Rules.BoardRows,Rules.BoardCols, Rules.CanShipsTouch ), "Player 2" );
            Player1 = CurrentPlayer;
            Player2 = TargetPlayer;
            GameMoves = new List<GameMove>();
        }
    }
}