using System;
using System.Collections.Generic;
using System.Linq;
using DAL;
using Domain;
using MenuSystem;

namespace BLL
{
    public class Game
    {
        //Singleton
        private static Game _instance = null;
        public static Game Instance
        {
            get
            {
                if (_instance==null)
                {
                    _instance = new Game();
                }
                return _instance;
            }
        }
        
        public Rules Rules { get; set; }
        public Player Player1 { get; set; }
        public Player Player2 { get; set; }
        public Player CurrentPlayer { get; set; }
        public Player TargetPlayer { get; set; }
        public List<GameMove> GameMoves = new List<GameMove>();
        private readonly LetterNumberSystem _converter = new LetterNumberSystem();
        public string SelectedMode { get; set; } = "";
        public const int MAX_ROWS = 24, MIN_ROWS = 10, MIN_COLS = 10, MAX_COLS = 24;
        
        public Menu CurrentMenu { get; set; }
        public Stack<Menu> MenuStack { get; set; } = new Stack<Menu>();

        private Game()
        {
            Rules = Rules.GetDefaultRules();
            
            var board1 = new Board(Rules.BoardRows, Rules.BoardCols, Rules.CanShipsTouch);
            var board2 = new Board(Rules.BoardRows, Rules.BoardCols, Rules.CanShipsTouch);
            
            Player1 = new Player(board1, "Player1");
            Player2 = new Player(board2, "Player2");
            CurrentPlayer = Player1;
            TargetPlayer = Player2;
            CurrentMenu = new ApplicationMenu(this).GetMain();
        }

        /**
         * Bombs a tile on the Target player board
         * If SelectedMode == "SP" also bombs current players board
         * When CurrentPlayer has bombed all enemies tiles Result.GameOver is returned 
         * When Computer has won Result.ComputerWon is returned
         *
         * @param locationString: a string to be parsed into a Tile object. Format example: "B7"
         * @returns Result: enum Result of the method call 
         */
        public Result BombShip(string locationString)
        {
            Tile targetTile = GetTile(locationString, TargetPlayer.Board);
            if (targetTile == null) return Result.NoSuchTile;
            if(targetTile.IsBombed) return Result.TileAlreadyBombed;

            bool result = TargetPlayer.Board.BombLocation(targetTile.Row, targetTile.Col);
            BombingResult b = result ? BombingResult.Hit : BombingResult.Miss;
            GameMoves.Add(new GameMove(TargetPlayer, targetTile));

            if (TargetPlayer.Board.AnyShipsLeft() == false) return Result.GameOver;
            if (SelectedMode.Equals("MP"))
            {
                SwitchCurrentPlayer();
                return Result.SuccessfulBombing;
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

            if (CurrentPlayer.Board.AnyShipsLeft() == false) return Result.ComputerWon;
            return Result.SuccessfulBombings;
        }

        
        /**
         * Saves current game state to the database (player statuses, their boards, all game moves)
         * Calls PreviousMenu() until there are no previous menus
         * @param isFinished: select between finished and unfinished game save mode
         * @param saveName: save game's name
         * @param dbContext: Database connection class thingamajig
         * @returns Result: enum Result of the method call 
         */
        public Result SaveGame(AppDbContext dbContext, string saveName, bool isFinished)
        {
            
            var saveGame = new SaveGame
            {
                IsFinished = isFinished,
                Rules = Rules,
                Player1 = Player1,
                Player2 = Player2,
                GameMoves = GameMoves,
                Name = saveName
            };
            dbContext.SaveGames.Add(saveGame);
            dbContext.SaveChanges();


            Rules = Rules.GetDefaultRules();
            CurrentPlayer = new Player(new Board(Rules.BoardRows,Rules.BoardCols, Rules.CanShipsTouch ), "Player 1" );
            TargetPlayer = new Player(new Board(Rules.BoardRows,Rules.BoardCols, Rules.CanShipsTouch ), "Player 2" );
            Player1 = CurrentPlayer;
            Player2 = TargetPlayer;
            GameMoves = new List<GameMove>();
            
            while (PreviousMenu() != Result.NoPreviousMenuFound)
            {
                //Calls previousMenu until reaches bottom of the stack i.e, main menu
            }

            return Result.GameSaved;
        }

        /**
         * Adds a ship to current rules
         * @param size: ship size to be added (can't be existing size, less than 1 or more than 10)
         * @param quantity: quantity of the ship to be added (can't be less than 1 or more than 5)
         * @returns Result: enum Result of the method call
         */
        public Result AddShipToRules(int size, int quantity)
        {
            List<int> currentSizes = new List<int>();
            Rules.BoatRules.ForEach(rule => currentSizes.Add(rule.Size));
            if (size < 1 || size > 10 || currentSizes.Contains(size)) return Result.InvalidSize;
            if (quantity < 1 || quantity > 5) return Result.InvalidQuantity;
            Rules.BoatRules.Add(new BoatRule(size, quantity));
            Rules.BoatRules.Sort((rule, boatRule) =>
            {   //This is a comparable thingamajig
                if(rule.Size > boatRule.Size) return 1;
                if(rule.Size < boatRule.Size) return -1;
                return 0;
            });
            return Result.ShipRuleAdded;

        }

        /**
         * Edits existing ship size in current rules.
         * Also resets player's boards, due to possible conflicts
         *
         * @param size: ship size to be edited (has to exist in current rules)
         * @param quantity: new quantity of the editable ship (can't be less than 1 or more than 5)
         * @returns Result: enum Result of the method call
         */
        public Result EditShipInRules(int size, int quantity)
        {
            if (Rules.BoatRules.All(rule => rule.Size != size)) return Result.InvalidSize;
            if (quantity < 1 || quantity > 5) return Result.InvalidQuantity;
            
            Rules.BoatRules.Find(rule => rule.Size == size).Quantity = quantity;
     
            CurrentPlayer.Board = new Board(Rules.BoardRows, Rules.BoardCols, Rules.CanShipsTouch);
            TargetPlayer.Board = new Board(Rules.BoardRows, Rules.BoardCols, Rules.CanShipsTouch);
            return Result.ShipRuleEdited;
        }

        /**
         * Removes existing ship size from current rules.
         * Also resets player's boards, due to possible conflicts
         *
         * @param shipSize: ship size to be deleted (has to exist in current rules)
         * @returns Result: enum Result of the method call
         */
        public Result DeleteShipInRules(int shipSize)
        {
            if (Rules.BoatRules.All(rule => rule.Size != shipSize)) return Result.InvalidSize;
            
            Rules.BoatRules.Remove(Rules.BoatRules.Find(rule => rule.Size == shipSize));
            CurrentPlayer.Board = new Board(Rules.BoardRows, Rules.BoardCols, Rules.CanShipsTouch);
            TargetPlayer.Board = new Board(Rules.BoardRows, Rules.BoardCols, Rules.CanShipsTouch);

            return Result.ShipRuleDeleted;
        }

        /**
         * Changes the Ships can touch rule.
         * If user input is different from current rule, then player's boards are reset.
         *
         * @param userInput: has to be either "YES" or "NO". Not case sensitive.
         * @returns Result: enum Result of the method call
         */
        public Result EditShipsCanTouchRule(string userInput)
        {
            userInput = userInput.ToUpper();
            if (!userInput.Equals("YES") && !userInput.Equals("NO")) return Result.InvalidInput;
            
            if (userInput.Equals("YES") && Rules.CanShipsTouch == 0 || //current rule and new rule are different
                userInput.Equals("NO") && Rules.CanShipsTouch == 1)
            {
                Rules.CanShipsTouch = userInput.Equals("YES") ? 1 : 0;
                CurrentPlayer.Board = new Board(Rules.BoardRows,Rules.BoardCols, Rules.CanShipsTouch);
                TargetPlayer.Board = new Board(Rules.BoardRows,Rules.BoardCols, Rules.CanShipsTouch);
                return Result.ShipsTouchRuleChanged;
            }

            return Result.None;
        }

        
        /**
         * Changes the board height(rows) rule.
         * If user input is different from current rule, then player's boards are reset.
         *
         * @param newHeight: new height of the board
         * @returns Result: enum Result of the method call
         */
        public Result EditBoardHeight(int newHeight)
        {
            if (newHeight < MIN_ROWS || newHeight > MAX_ROWS) return Result.InvalidInput;
            if (Rules.BoardRows == newHeight) return Result.None;     //current rule == new rule. Do nothing
            
            CurrentPlayer.Board = new Board(newHeight, Rules.BoardCols, Rules.CanShipsTouch);
            TargetPlayer.Board = new Board(newHeight, Rules.BoardCols, Rules.CanShipsTouch);
            Rules.BoardRows = newHeight;
            return Result.BoardHeightChnaged;
            
        }

        /**
         * Changes the board width(columns) rule.
         * If user input is different from current rule, then player's boards are reset.
         *
         * @param newHeight: new width of the board
         * @returns Result: enum Result of the method call
         */
        public Result EditBoardWidth(int newWidth)
        {
            if (newWidth < MIN_COLS || newWidth > MAX_COLS) return Result.InvalidInput;
            if (Rules.BoardCols == newWidth) return Result.None;        //current rule == new rule. Do nothing
            
            CurrentPlayer.Board = new Board(Rules.BoardRows, newWidth, Rules.CanShipsTouch);
            TargetPlayer.Board = new Board(Rules.BoardRows, newWidth, Rules.CanShipsTouch);
            Rules.BoardCols = newWidth;
            return Result.BoardWidthChnaged;
        }

        /**
         * Changes current custom rule's name.
         *
         * @param newRulesetName: new name for the rules. Can't be "standard rules" (not case sensitive)
         * @returns Result: enum Result of the method call
         */
        public Result SetRulesetName(string newRulesetName)
        {
            if (newRulesetName.ToUpper().Equals("STANDARD RULES")) return Result.InvalidInput;
            Rules.Name = newRulesetName;
            return Result.RulesNameChanged;
        }

        public Result SetStandardRules(string userInput)
        {
            Rules = Rules.GetDefaultRules();
            UI.Alert("Standard rules set", 500);
            return "";
        }

        public Result PlaceShipOnBoard(string userInput)
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

        public Result DeleteShipFromBoard(string userInput)
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

        public Result ChangePlayersName(string userInput)
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

        public Result GetTileOfDeleteableShip(string userInput)
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


        public Result GetShipStartTile(string userInput)
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
                int row = _converter.GetNumberFromLetters(stringRow) - 1, col = int.Parse(stringCol) - 1;
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

        public Result GetShipEndTile(string userInput)
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

        public List<SaveGame> GetSaveGames(AppDbContext ctx, bool isFinished)
        {
            return ctx.SaveGames
                .Where(game => game.IsFinished == isFinished)
                .OrderBy(game => game.SaveGameId)
                .ToList();
        }

        public Result LoadGame(AppDbContext dbContext)
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
                UI.Alert($"{move.Target.Name} is bombed at {_converter.GetLetter(move.Tile.Row + 1)}:{move.Tile.Col + 1} and it's a {(b == BombingResult.Hit ? "Hit" : "Miss")}", 0);
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

        public Result GenerateRandomBoard(Player player)
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

        public void ResetTargetPlayer(String newTargetName)
        {
            TargetPlayer.Board = new Board(Rules.BoardRows,Rules.BoardCols,Rules.CanShipsTouch);
            TargetPlayer.Name = newTargetName;
            TargetPlayer.IsReady = false;
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

        /**
         * Rules, Players, And GameMoves are reset to their default values.
         */
        public void ResetAll()
        {
            Rules = Rules.GetDefaultRules();
            CurrentPlayer = new Player(new Board(Rules.BoardRows,Rules.BoardCols, Rules.CanShipsTouch ), "Player 1" );
            TargetPlayer = new Player(new Board(Rules.BoardRows,Rules.BoardCols, Rules.CanShipsTouch ), "Player 2" );
            Player1 = CurrentPlayer;
            Player2 = TargetPlayer;
            GameMoves = new List<GameMove>();
        }

        /**
         * Current menu is pushed to the MenuStack and then new menu is marked as current menu 
         */
        public Result ChangeMenu(Menu menu)
        {
            MenuStack.Push(CurrentMenu);
            CurrentMenu = menu;
            return Result.ChangedMenu;
        }
        
        /**
         * Changes current menu to the previous one.
         * Last menu is kept in MenuStack object on the top of the stack
         */
        public Result PreviousMenu()
        {
            if (MenuStack.Count == 0)
            {
                return Result.NoPreviousMenuFound;
            }

            CurrentMenu = MenuStack.Pop();
            return Result.ReturnToPreviousMenu;
        }

        public Result PopulateLoadsMenu(Menu menu, bool isFinished, AppDbContext dbContext)
        {
            menu.MenuItems.Clear();
            List<SaveGame> saveGames = dbContext.SaveGames
                .Where(game => game.IsFinished == isFinished)
                .OrderBy(game => game.SaveGameId)
                .ToList();

            foreach (var saveGame in saveGames)
            {
                menu.MenuItems.Add(new MenuItem
                {
                    Shortcut = saveGame.SaveGameId.ToString(),
                    Description = saveGame.Name,
                    CommandToExecute = () => isFinished? Command.ReplayGame : Command.LoadGame
                });
            }
        }
    }
}