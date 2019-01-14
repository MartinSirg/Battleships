using System;
using System.Collections.Generic;
using System.Linq;
using DAL;
using Domain;
using MenuSystem;
using Microsoft.EntityFrameworkCore;

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
        public ApplicationMenu Menus;
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
            Menus = new ApplicationMenu(this);
            CurrentMenu = Menus.MainMenu;
        }

        /**
         * Bombs a tile on the Target player board
         * If SelectedMode == "SP" also bombs current players board
         * When CurrentPlayer has bombed all enemies tiles Result.GameOver is returned 
         * When Computer has won Result.ComputerWon is returned
         *
         * @param locationString: a string to be parsed into a Tile object. Format example: "B7"
         *                      If tile can't be pares then Result.NoSuchTile is returned
         * @returns Result: enum Result of the method call (NoSuchTile, TileAlreadyBombed, GameOver, ComputerWon, TwoBombings, OneBombing)
         */
        public Result BombLocation(string locationString)
        {
            Tile targetTile = GetTile(locationString, TargetPlayer.Board);
            if (targetTile == null) return Result.NoSuchTile;
            if (targetTile.IsBombed) return Result.TileAlreadyBombed;

            var isHit = TargetPlayer.Board.BombLocation(targetTile.Row, targetTile.Col);
            GameMoves.Add(new GameMove(TargetPlayer, targetTile));

            if (TargetPlayer.Board.AnyShipsLeft() == false) return Result.GameOver;
            if (SelectedMode.Equals("MP"))
            {
                SwitchCurrentPlayer();
                return Result.OneBombing;
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
            return Result.TwoBombings;
        }

        
        /**
         * Saves current game state to the database (player statuses, their boards, all game moves)
         * Calls PreviousMenu() until there are no previous menus (Goes back to main)
         * 
         * @param isFinished: select between finished and unfinished game save mode
         * @param saveName: save game's name
         * @param dbContext: Database connection class thingamajig
         * @returns Result: enum Result of the method call 
         */
        public Result SaveGame(AppDbContext dbContext, string saveName, bool isFinished)
        {
            if (saveName.Length > Domain.SaveGame.MaxLength) return Result.TooLong;
            var saveGame = new SaveGame
            {
                IsFinished = isFinished,
                Rules = Rules,
                Player1 = Player1,
                Player2 = Player2,
                GameMoves = GameMoves,
                Name = saveName,
                Mode = SelectedMode
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

            return Result.None;
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
            return Result.RulesChanged;

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
            return Result.RulesChanged;
        }

        /**
         * Removes existing ship size from current rules.
         * Also resets player's boards, due to possible conflicts
         *
         * @param shipSize: ship size to be deleted (has to exist in current rules)
         * @returns Result: enum Result of the method call (InvalidSize, RulesChanged)
         */
        public Result DeleteShipFromRules(int shipSize)
        {
            if (Rules.BoatRules.All(rule => rule.Size != shipSize)) return Result.InvalidSize;
            
            Rules.BoatRules.Remove(Rules.BoatRules.Find(rule => rule.Size == shipSize));
            CurrentPlayer.Board = new Board(Rules.BoardRows, Rules.BoardCols, Rules.CanShipsTouch);
            TargetPlayer.Board = new Board(Rules.BoardRows, Rules.BoardCols, Rules.CanShipsTouch);

            return Result.None;
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
                return Result.RulesChanged;
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
            return Result.RulesChanged;
            
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
            return Result.RulesChanged;
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
            return Result.RulesChanged;
        }

        /**
         * Changes current ruleset to Default rules (obtained from a static Rules class method).
         *
         * @returns Result: enum Result of the method call
         */
        public Result SetStandardRules()
        {
            Rules = Rules.GetDefaultRules();
            return Result.RulesChanged;
        }

        /**
         * Places ship on current player's board, using highlighted tiles assigned previously.
         * Will not place if:
         * 1) both end and start tiles are not highlighted
         * 2) end and start tiles do not share a common axis
         * 3) ship size does not exist in rules
         * 4) all determined sized ships have been placed
         * 5) overlaps other ships/ violates can touch rules (this is checked in Board.AddBattleship() method)
         *
         * @returns Result: enum Result of the method call (HighlightMissing, NoCommonAxis, InvalidSize, Invalid Quantity, Overlap)
         */
        public Result PlaceShipOnBoard()
        {
            //Key size, Value Quantity;
            Dictionary<int, int>  boats = new Dictionary<int, int>();
            Rules.BoatRules.ForEach(rule => boats.Add(rule.Size, rule.Quantity));              //Add rules to dict
            CurrentPlayer.Board.Battleships.ForEach(battleship => boats[battleship.Size]--);   //Reduce count with placed ships 
            Tile start = CurrentPlayer.Board.HighlightedStart, end = CurrentPlayer.Board.HighLightedEnd;
            
            if (start == null || end == null) return Result.HighlightMissing;
            bool hasCommonAxis = start.Row - end.Row == 0 || start.Col - end.Col == 0;
            if (!hasCommonAxis) return Result.NoCommonAxis;

            var length = GetHighlightedSize();

            if (!boats.ContainsKey(length)) return Result.InvalidSize;         // Invalid size
            if (boats[length] <= 0)         return Result.InvalidQuantity;     // No ships left

            try
            {
                CurrentPlayer.Board.AddBattleship((start.Row, start.Col), (end.Row, end.Col), new Battleship(length));
                ResetHighlightedTiles();
                return Result.None;
            }
            catch (Exception e)
            {
                return Result.Overlap;
            }
        }
        
        /**
         * Deletes a ship from the board using Highlighted start tile
         *
         * @returns Result: enum Result of the method call  
         */
        public Result DeleteShipFromBoard()
        {
            Tile deletable = CurrentPlayer.Board.HighlightedStart;
            
            if (deletable == null) return Result.TileNotHighlighted;
            if (deletable.IsEmpty()) return Result.ShipNotDeleted;
            
            CurrentPlayer.Board.DeleteShipFromBoard(deletable);
            ResetHighlightedTiles();
            return Result.ShipDeleted;
        }

        /**
         * Changes current players name
         * Has to be case sensitively different from other player's name
         * Can't be "Computer"
         * 
         * @param name: new name for the player
         * @returns Result: enum Result of the method call  
         */
        public Result ChangePlayersName(string name)
        {
            if (TargetPlayer.Name.Equals(name)) return Result.PlayerNameNotChanged;
            if (name.Equals("Computer")) return Result.PlayerNameNotChanged;
            if (name.Length > Player.MaxLength) return Result.TooLong;
            if (name.Length < Player.MinLength) return Result.TooShort;
            
            CurrentPlayer.Name = name;
            return Result.PlayerNameChanged;
        }

        /**
         * Switches current player  
         */
        public void SwitchCurrentPlayer()
        {
            Player temp = CurrentPlayer;
            CurrentPlayer = TargetPlayer;
            TargetPlayer = temp;
        }

        /**
         * Checks whether all requirements are met to start the game
         * 1) both players have placed all their ships
         * 2.1) Multiplayer: both player's property Player.IsReady is true
         * 2.2) Singleplayer: current player's Player.IsReady property is true
         * 
         *
         * @returns bool: true if all conditions are met, 
         */
        public bool CheckIfCanStartGame()
        {
            if (SelectedMode == null) return false;
            
            Dictionary<int,int> ships = AvailableShips(CurrentPlayer);
            if (ships.Any(pair => pair.Value > 0)) return false; // all current player's ships haven't been placed
            
            ships = AvailableShips(TargetPlayer);
            if (ships.Any(pair => pair.Value > 0)) return false; // all target player's ships haven't been placed

            if (SelectedMode.Equals("SP")) return CurrentPlayer.IsReady; // SP: true if current player is ready
            return CurrentPlayer.IsReady && TargetPlayer.IsReady;        // MP: true if both players are read
        }

        /**
         * Highlights a tile, where a desired deletable ship is located
         * Has to contain a ship
         *
         * @param locationString: a string to be parsed into a Tile object. Format example: "B7"
         *                      If tile can't be pares then Result.NoSuchTile is returned
         * @returns Result: enum Result of the method call 
         */
        public Result SetTileContainingDeletableShip(string locationString)
        {
            if (CurrentPlayer.Board.HighlightedStart != null) // resets current highlight if there is one
                CurrentPlayer.Board.HighlightedStart.IsHighlightedStart = false;
            
            Tile startTile = GetTile(locationString, CurrentPlayer.Board);
            
            if (startTile == null)   return Result.NoSuchTile;
            if (startTile.IsEmpty()) return Result.TileNotHighlighted;
        
            startTile.IsHighlightedStart = true;
            CurrentPlayer.Board.HighlightedStart = startTile;
            return Result.None;
        }

        /**
         * Highlights a tile, where a new battleships start point should reside.
         * Tile has to be empty
         *
         * @param locationString: a string to be parsed into a Tile object. Format example: "B7"
         *                      If tile can't be pares then Result.NoSuchTile is returned
         * @returns Result: enum Result of the method call (NoSuchTile, TileNotHighlighted, None)
         */
        public Result SetShipStartTile(string locationString)
        {
            if (CurrentPlayer.Board.HighlightedStart != null)    // resets current highlight if there is one
                CurrentPlayer.Board.HighlightedStart.IsHighlightedStart = false;
            Tile startTile = GetTile(locationString, CurrentPlayer.Board);
            
            if (startTile == null)            return Result.NoSuchTile;
            if (startTile.IsEmpty() == false) return Result.TileNotHighlighted;
            
            startTile.IsHighlightedStart = true;
            CurrentPlayer.Board.HighlightedStart = startTile;
            return Result.None;
        }
        
        /**
         * Highlights a tile on current players board, where a new battleships end point should reside.
         * Tile has to be empty
         *
         * @param locationString: a string to be parsed into a Tile object. Format example: "B7"
         *                      If tile can't be pares then Result.NoSuchTile is returned
         * @returns Result: enum Result of the method call 
         */
        public Result SetShipEndTile(string locationString)
        {
            if (CurrentPlayer.Board.HighLightedEnd != null)    // resets current highlight if there is one
                CurrentPlayer.Board.HighLightedEnd.IsHighlightedEnd = false;
            Tile endTile = GetTile(locationString, CurrentPlayer.Board);
            
            if (endTile == null)            return Result.NoSuchTile;
            if (endTile.IsEmpty() == false) return Result.TileNotHighlighted;
            
            endTile.IsHighlightedEnd = true;
            CurrentPlayer.Board.HighLightedEnd = endTile;
            return Result.None;
        }

        /**
         * Returns a dictionary of a specified player's available ships
         * Key: ship size (int)
         * Value: available ships quantity(int)
         * @param player: Desired players Player object
         */
        public Dictionary<int, int> AvailableShips(Player player)
        {
            Dictionary<int, int>  availableShips = new Dictionary<int, int>();
            Rules.BoatRules.ForEach(rule => availableShips.Add(rule.Size, rule.Quantity));
            player.Board.Battleships.ForEach(battleship => availableShips[battleship.Size]--);
            return availableShips;
        }

        /**
         * Pares a locationString to get a Tile from the specified board
         *
         * @param location: string to be parsed. Format example: "B7"
         * @param board: Board object where the tile will be searched from
         * @returns Tile: if string is successfully parsed returns a Tile object, otherwise returns a null
         */
        private Tile GetTile(string location, Board board)
        {
            string stringRow = "", stringCol = "";
            foreach (var c in location)    //separates column and row parts in location
            {
                if (stringCol.Length == 0 && Char.IsLetter(c)) stringRow += c;
                else if (stringRow.Length > 0 && char.IsDigit(c)) stringCol += c;
                else
                {
                    return null;
                }
            }

            try // tries to convert row int from row string(i.e "B"). Returns tile if successful
            {
                int row = _converter.GetNumberFromLetters(stringRow.ToUpper()) - 1, col = int.Parse(stringCol) - 1;
                return board.Tiles[row]?[col];
            }
            catch (IndexOutOfRangeException e) {return null;}
            catch (ArgumentException e)        {return null;}
            catch (FormatException e)          {return null;}
        }

        /**
         * Current player's IsReady parameter is set to false
         */
        public void SetCurrentPlayerNotReady()
        {
            CurrentPlayer.IsReady = false;
        }

        /**
         * Current player's IsReady parameter is set to true
         */
        public void SetCurrentPlayerReady()
        {
            CurrentPlayer.IsReady = true;
        }
        
        /**
         * Sets gameMode
         * @param modeName: a string which has to be either "SP"(singleplayer) or "MP"(multiplayer)
         */
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
                        throw new Exception("Unknown modeName: " + modeName);
            }
        }
        
        /**
         * Returns a list of saved games
         *
         * @param dbContext: AppDbContext object for database connection and data
         * @param isFinished: changes whether returned save games are in finished state or not
         * @returns List<SaveGame>: list of SaveGame objects
         */
        public List<SaveGame> GetSaveGames(AppDbContext dbContext, bool isFinished)
        {
            return dbContext.SaveGames
                .Where(game => game.IsFinished == isFinished)
                .OrderBy(game => game.SaveGameId)
                .ToList();
        }

        /**
         * Changes current Game properties to align with the saved games properties (gamestate)
         *
         * @param dbContext: AppDbContext object for database connection and data
         * @param saveGameId: loadable save game's id (can be received from selecting one from GetSaveGames() method)
         * @returns Result: enum Result of the method call (NoSuchSaveGameId, GameParametersLoaded) 
         */
        private void LoadGame(AppDbContext dbContext, int saveGameId)
        {
            bool isIdPresent = dbContext.SaveGames
                .Where(game => game.IsFinished == false).ToList()
                .Any(game => game.SaveGameId == saveGameId);
            if (isIdPresent == false) throw new Exception("Invalid saveGameId: " + saveGameId);

            SaveGame save = dbContext.SaveGames
                .Include(saveGame => saveGame.Rules)
                    .ThenInclude(rules => rules.BoatRules)
                .Include(saveGame => saveGame.Player1)
                    .ThenInclude(player => player.Board)
                .Include(saveGame => saveGame.Player2)
                    .ThenInclude(player => player.Board)
                .Include(saveGame => saveGame.GameMoves)
                .First(saveGame => saveGame.SaveGameId == saveGameId);
            
            //cloning is used to preserve database save entries
            var tempRules = save.Rules;
            var tempPlayer1 = save.Player1;
            var tempPlayer2 = save.Player2;
            Rules = (Rules) tempRules.Clone();
            RestorePlayerBoardTiles(tempPlayer1, save, dbContext); // clones tiles into player's boards
            SelectedMode = new string(save.Mode.ToCharArray());
            RestorePlayerBoardTiles(tempPlayer2, save, dbContext);

            List<GameMove> tempGameMoves = save.GameMoves.OrderBy(move => move.GameMoveId).ToList();
            Player1 = (Player) tempPlayer1.Clone();
            Player2 = (Player) tempPlayer2.Clone();
            
            GameMoves = new List<GameMove>();
            //clones gameMoves parameter
            tempGameMoves.ForEach(move =>
            {
                var targetPlayer = Player1.Name == move.Target.Name ? Player1 : Player2;
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

        }
        
        /**
         * Changes current Game properties to align with the saved games properties (gamestate)
         * Bombed ships and tiles are reset to their state before the game
         * IMPORTANT! When looping through GameMoves, BombLocationReplay should be used!
         *
         * @param dbContext: AppDbContext object for database connection and data
         * @param saveGameId: loadable save game's id (can be received from selecting one from GetSaveGames() method)
         * @returns Result: enum Result of the method call (NoSuchSaveGameId, GameParametersLoaded) 
         */
        private void LoadReplayGame(AppDbContext dbContext, int saveGameId)
        {
            
            bool isIdPresent = dbContext.SaveGames
                .Where(game => game.IsFinished).ToList()
                .Any(game => game.SaveGameId == saveGameId);

            if (isIdPresent == false) throw new Exception("Invalid saveGameId: " + saveGameId);
            
            SaveGame s = dbContext.SaveGames
                .Include(saveGame => saveGame.Rules)
                    .ThenInclude(rules => rules.BoatRules)
                .Include(saveGame => saveGame.Player1)
                    .ThenInclude(player => player.Board)
                .Include(saveGame => saveGame.Player2)
                    .ThenInclude(player => player.Board)
                .Include(saveGame => saveGame.GameMoves)
                .First(saveGame => saveGame.SaveGameId == saveGameId);
            
            //cloning is used to preserve database save entries
            var tempRules = s.Rules;
            var tempPlayer1 = s.Player1;
            var tempPlayer2 = s.Player2;
            Rules = (Rules) tempRules.Clone();
            RestorePlayerBoardTiles(tempPlayer1, s, dbContext); // clones tiles into player's boards
            RestorePlayerBoardTiles(tempPlayer2, s, dbContext);

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
            
            GameMoves.ForEach(move => move.Target.Board.UnBomb(move.Tile)); // removes bombed tag and restore lives for ship
            CurrentPlayer = Player1;                                        // Game moves are preserved
            TargetPlayer = Player2;
            
        }
        
        /**
         * Bombs a ship location doesn't update GameMoves list.
         * Upon bombing either Result.GameOver is returned
         * Or CurrentPlayer is switched and returns SuccessfulReplayBombing
         * @param locationString: locationString to be parsed into a Tile on a board (Example "B7")
         */
        public Result BombShipReplay(Tile targetTile)
        {
            if (targetTile == null) return Result.NoSuchTile;
            if (targetTile.IsBombed) return Result.TileAlreadyBombed;
            
            TargetPlayer.Board.BombLocation(targetTile.Row, targetTile.Col);
            if (TargetPlayer.Board.AnyShipsLeft() == false) return Result.GameOver;
            
            SwitchCurrentPlayer();
            return Result.SuccessfulReplayBombing;
        }

        /**
         * Game properties are reset
         */
        public void NewGame()
        {
            Board board1 = new Board(Rules.BoardRows, Rules.BoardCols, Rules.CanShipsTouch),
                  board2 = new Board(Rules.BoardRows, Rules.BoardCols, Rules.CanShipsTouch);
            Player1 = new Player(board1, "Player 1");
            Player2 = new Player(board2, "Player 2");
            CurrentPlayer = Player1;
            TargetPlayer = Player2;
            GameMoves = new List<GameMove>();
        }

        /**
         * Highlighted tiles that are specified in Game properties are reset (only two)
         * Doesn't unhighlight tiles that are not specified in Game properties
         */
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

        /**
         * TargetPlayer is set to generated computer player
         */
        public void GenerateOpponent()
        {
            TargetPlayer.Name = "Computer";
            GenerateRandomBoard(TargetPlayer);
        }

        /**
         * Generates a random board for the specified player
         * Deletes previous ship placements
         */
        public Result GenerateRandomBoard(Player player)
        {
            player.Board = new Board(Rules.BoardRows, Rules.BoardCols, Rules.CanShipsTouch);
            Dictionary<int, int> availableShips = AvailableShips(player);
            var random = new Random();

            while (availableShips.Any(pair => pair.Value > 0)) // Loop til there are ships available
            {
                int sizeKey = availableShips.First(pair => pair.Value > 0).Key;
                var ship = new Battleship(sizeKey);
                var counter = 1;
                while (true)
                {
                    if (++counter > 50000) return Result.CouldntPlaceAllShips; 
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
                        catch (ArgumentException e){}
                        catch (IndexOutOfRangeException e){}
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
                        catch (ArgumentException e){}
                        catch (IndexOutOfRangeException e){}
                    }
                }
            }

            return Result.None;
        }

        /**
         * Resets target players board, name and IsReady parameter
         * @param newTargetName: new name for the player
         */
        public void ResetTargetPlayer(String newTargetName)
        {
            TargetPlayer.Board = new Board(Rules.BoardRows,Rules.BoardCols,Rules.CanShipsTouch);
            TargetPlayer.Name = newTargetName;
            TargetPlayer.IsReady = false;
        }

        /**
         * Restores player's board tiles
         * Needed because database only stores tiles with changed values
         * @param player: Player object, whose board will be restored
         * @param save: Save object. Needed for loading
         * @param dbContext: database connection object 
         */
        private void RestorePlayerBoardTiles(Player player, SaveGame save, AppDbContext dbContext)
        {
            List<List<Tile>> boardTiles = new List<List<Tile>>(save.Rules.BoardRows);
            List<Tile> tilesFromDb = dbContext.Tiles
                .Include(tile => tile.Board)
                .Include(tile => tile.Battleship)
                .Where(tile => tile.Board == player.Board).ToList();
            
            Console.WriteLine($"DbTilesCount: {tilesFromDb.Count}");
            for (int i = 0; i < save.Rules.BoardRows; i++)
            {
                boardTiles.Add(new List<Tile>(save.Rules.BoardCols));
                
                for (int j = 0; j < save.Rules.BoardCols; j++)
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
        public void ChangeMenu(Menu menu)
        {
            MenuStack.Push(CurrentMenu);
            CurrentMenu = menu;
            if (CurrentMenu.TitleWithName != null)
            {
                CurrentMenu.Title = CurrentMenu.TitleWithName.Replace("PLAYER_NAME", CurrentPlayer.Name);
            }
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

        /**
         * Fills the menu's menuItems with id-s and names
         */
        public Result FillLoadsMenu(Menu menu, bool isFinished, AppDbContext dbContext)
        {
            menu.MenuItems.Clear();
            List<SaveGame> saveGames = dbContext.SaveGames
                .Where(game => game.IsFinished == isFinished)
                .Include(game => game.Player1)
                .Include(game => game.Player2)
                .Include(game => game.GameMoves)
                .OrderBy(game => game.SaveGameId)
                .ToList();
            int longestPad = saveGames.Max(game => game.Name.Length);
            
            foreach (var saveGame in saveGames)
            {
                menu.MenuItems.Add(new MenuItem
                {
                    Shortcut = saveGame.SaveGameId.ToString(),
                    Description = $"{saveGame.Name.PadRight(longestPad + 4)}{saveGame.Player1.Name} vs {saveGame.Player2.Name} with {saveGame.GameMoves.Count} moves",
                    GetCommand = () =>
                    {
                        if (isFinished) LoadReplayGame(dbContext, saveGame.SaveGameId);
                        else
                        {
                            LoadGame(dbContext, saveGame.SaveGameId);
                            ChangeMenu(Menus.InGameMenu);
                        }
                        return isFinished ? Command.ShowGameReplay : Command.None;
                    }
                });
            }

            return Result.None;
        }

        /**
         * Removes end and start highlights in board and max two tiles.
         */
        private void ResetHighlightedTiles()
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

        public int GetHighlightedSize()
        {
            Tile start = CurrentPlayer.Board.HighlightedStart, end = CurrentPlayer.Board.HighLightedEnd;
            if (start == null || end == null)
            {
                return -1;
            }
            return start.Row - end.Row == 0 ? Math.Abs(start.Col - end.Col) + 1: Math.Abs(start.Row - end.Row) + 1;
        }

        public void ReturnToRootMenu()
        {
            while (PreviousMenu() != Result.NoPreviousMenuFound)
            {
                //Calls previousMenu until reaches bottom of the stack i.e, main menu
            }
        }
    }
}