using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using BLL;
using DAL;
using Domain;
using MenuSystem;
using Microsoft.EntityFrameworkCore;

namespace UI
{
    public class ConsoleUi
    {
        enum PrintBoard
        {
            Regular,
            Adding,
            Deleting
        }
        private LetterNumberSystem Letters { get; } = new LetterNumberSystem();
        private string LeftPad { get; } = "            ";
        private ConsoleColor Background { get; }
        private ConsoleColor Foreground { get; }
        private readonly Game _game = Game.Instance;
        private AppDbContext _dbContext = new AppDbContext();
        public ConsoleUi(ConsoleColor background = ConsoleColor.White, ConsoleColor foreground = ConsoleColor.Black)
        {
            Background = background;
            Foreground = foreground;
            Console.ForegroundColor = foreground;
            Console.BackgroundColor = background;
            Console.Clear();
            Console.Title = "Battleships";
            Console.WindowHeight = 35;
        }

        public void Loop()
        {
            outerloop:
            while (true)
            {
                Console.Clear();
                
                // 1)Parse DisplayBefore
                ParseDisplayBefore(_game.CurrentMenu.DisplayBefore);
                
                // 2)Print menu options + print previous + menu title
                DisplayMenu(_game.CurrentMenu);
                
                // 3)Get Menu shortcut (Console.ReadLine())
                var input = GetMenuShortcut(_game.CurrentMenu);
                if (input.Equals("")) continue;
                if (input.Equals("X"))
                {
                    _game.PreviousMenu();
                    continue;
                }
                
                // 4)Run and save Command
                var command = _game.CurrentMenu.MenuItems
                    .Find(item => item.Shortcut.ToLower().Equals(input.ToLower())).GetCommand();

                // 5)Parse command
                ParseCommand(command);
            }
        }

        private void ParseDisplayBefore(Display display)
        {
            switch (display)
            {
                case Display.ShipsAndBombings:
                    PrintBombedLocationsAndFriendlyShips(_game.TargetPlayer.Board, _game.CurrentPlayer.Board);
                    break;
                case Display.ShipRules:
                    DisplayShipRules(_game.Rules);
                    break;
                case Display.BoardRules:
                    DisplayBoardRules(_game.Rules);
                    break;
                case Display.CurrentRules:
                    DisplayCurrentRules(_game.Rules);
                    break;
                case Display.CurrentAndAvailableShips:
                    PrintFriendlyShips(_game.CurrentPlayer.Board);
                    DisplayAvailableShips(_game.AvailableShips(_game.CurrentPlayer));
                    break;
                case Display.CurrentShipsDeleting:
                    PrintFriendlyShipsDeleting(_game.CurrentPlayer.Board);
                    break;
                case Display.FinishedGames:
                    //No printing needed
                    break;
                case Display.UnfinishedGames:
                    //No printing needed
                    break;
                case Display.Bombings:
                    PrintBombedLocations(_game.TargetPlayer.Board, _game.CurrentPlayer);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(display), display, null);
            }
        }

        private void ParseCommand(Command command)
        {
            switch (command)
            {
                case Command.Previous:
                    _game.PreviousMenu();
                    break;
                
                case Command.BombLocation:
                    var input = GetTargetLocation();
                    if (input.ToUpper().Equals("X")) return;
                    var result = _game.BombLocation(input);
                    ParseResult(result, input);
                    break;
                
                case Command.None:
                    break;
                
                case Command.SaveUnfinishedGame:
                    _game.SaveGame(_dbContext, GetSaveGameName(), false);
                    break;
                
                case Command.EditShipInRules:
                    break;
                
                case Command.AddShipToRules:
                    break;
                
                case Command.DeleteShipInRules:
                    break;
                
                case Command.EditShipsCanTouchRule:
                    break;
                    
                case Command.EditBoardWidth:
                    break;
                    
                case Command.EditBoardHeight:
                    break;
                    
                case Command.SetRulesetName:
                    break;
                    
                case Command.SetStandardRules:
                    break;
                    
                case Command.GetShipStartTile:
                    break;
                    
                case Command.PlaceShipOnBoard:
                    break;
                    
                case Command.GetShipEndTile:
                    break;
                    
                case Command.GetTileOfDeleteableShip:
                    break;
                    
                case Command.DeleteShipFromBoard:
                    break;
                    
                case Command.ChangePlayersName:
                    break;
                    
                case Command.FillReplayMenu:
                    break;
                    
                case Command.FillLoadMenu:
                    break;
                    
                case Command.LoadGame:
                    break;
                    
                case Command.ReplayGame:
                    break;
                    
                case Command.CantStartGame:
                    break;
                    
                default:
                    throw new ArgumentOutOfRangeException(nameof(command), command, null);
            }
        }

        private void ParseResult<T>(Result result, T var1 = default, T var2 = default)
        {
            GameMove move;
            switch (result)
            {
                case Result.None:            //No further action needed
                    break;
                
                case Result.ChangedMenu:     //No further action needed
                    break;
                
                case Result.NoPreviousMenuFound:
                    Alert("No previous menu found!", ConsoleColor.Red);
                    break;
                
                case Result.ReturnToPreviousMenu: //No further action needed
                    break;
                
                case Result.GameOver:
                    Alert($"{_game.CurrentPlayer.Name} has won!", ConsoleColor.Green);
                    _game.SaveGame(_dbContext, GetSaveGameName(), true);
                    break;
                
                case Result.OneBombing:
                    move = _game.GameMoves[_game.GameMoves.Count - 1];
                    Console.WriteLine($"{move.Target.Name} is bombed at {move.Tile}.");
                    if (move.Tile.IsEmpty()) Alert("Its a MISS!", ConsoleColor.DarkRed);
                    else Alert("Its a HIT!", ConsoleColor.DarkGreen);
                    break;
                
                case Result.TwoBombings:
                    move = _game.GameMoves[_game.GameMoves.Count - 2];
                    Console.WriteLine($"{move.Target.Name} is bombed at {move.Tile}.");
                    if (move.Tile.IsEmpty()) Alert("Its a MISS!", ConsoleColor.DarkRed);
                    else Alert("Its a HIT!", ConsoleColor.DarkGreen);
                    
                    move = _game.GameMoves[_game.GameMoves.Count - 1];
                    Console.WriteLine($"{move.Target.Name} is bombed at {move.Tile}.");
                    if (move.Tile.IsEmpty()) Alert("Its a MISS!", ConsoleColor.DarkGreen);
                    else Alert("Its a HIT!", ConsoleColor.DarkRed);
                    break;
                   
                case Result.ComputerWon:
                    Alert("Computer has won!", ConsoleColor.Green);
                    _game.SaveGame(_dbContext, GetSaveGameName(), true);
                    break;
                
                case Result.NoSuchTile:
                    Alert($"{var1} is not a valid location!", ConsoleColor.Red);
                    break;
                
                case Result.TileAlreadyBombed:
                    Alert($"{var1} is already bombed!", ConsoleColor.Red);
                    break;
                
                case Result.InvalidSize:
                    break;
                case Result.InvalidQuantity:
                    break;
                case Result.InvalidInput:
                    break;
                case Result.RulesChanged:
                    break;
                case Result.ShipPlaced:
                    break;
                case Result.ShipNotPlaced:
                    break;
                case Result.ShipNotDeleted:
                    break;
                case Result.ShipDeleted:
                    break;
                case Result.PlayerNameChanged:
                    break;
                case Result.TileNotHighlighted:
                    break;
                case Result.TileHighlighted:
                    break;
                case Result.NoSuchSaveGameId:
                    break;
                case Result.PlayerNameNotChanged:
                    break;
                case Result.GameParametersLoaded:
                    break;
                case Result.ReplayReadySaveLoaded:
                    break;
                case Result.SuccessfulReplayBombing:
                    break;
                case Result.CouldntPlaceAllShips:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(result), result, null);
            }
        }

       //--------------------------OLD METHODS-----------------------------------------
        private void PrintBombedLocations(Board enemyBoard, Player current)
        {
            var sb = new StringBuilder();
            var line = GetTableLine(enemyBoard.Tiles[0].Count);
            sb.Append(LeftPad).Append("   ").Append($"{current.Name} bombings:\n");
            sb.Append(LeftPad).Append("   ").Append(GetTopNumbersLine(enemyBoard.Tiles[0].Count)).Append("\n");
            sb.Append(LeftPad).Append("   ").Append(line).Append("\n");
            for (var index = 0; index < enemyBoard.Tiles.Count; index++)
            {
                var row = enemyBoard.Tiles[index];
                sb.Append(LeftPad).Append(Letters.GetLetter(index + 1).PadLeft(2)).Append(" |");
                foreach (var tile in row)
                {
                    if (tile.IsBombed == false) sb.Append("   ");
                    else if (tile.IsBombed && tile.IsEmpty() == false) sb.PrintInColor(" \u00D7 ", ConsoleColor.Green);
                    else sb.PrintInColor(" o ", ConsoleColor.Red);

                    sb.Append("|");
                }

                sb.Append("\n").Append(LeftPad).Append("   ").Append(line).Append("\n");
            }
            
            sb.Append("\n").Append(LeftPad).Append(" ");
            sb.PrintInColor(" \u00D7 ", ConsoleColor.Green).Append(" : Bombing hit\n").Append(LeftPad).Append(" ");
            sb.PrintInColor(" o ", ConsoleColor.Red).Append(" : Bombing miss\n");

            Console.WriteLine(sb.ToString());
        }

        private void PrintBombedLocationsAndFriendlyShips(Board enemyBoard, Board playerBoard)
        {
            var sb = new StringBuilder();
            var line = GetTableLine(enemyBoard.Tiles[0].Count);
            sb.Append(LeftPad).Append("   ").Append("Your bombings:").AddSpaces(line.Length - 7).Append("Your ships:\n");
            sb.Append(LeftPad).Append("   ").Append(GetTopNumbersLine(enemyBoard.Tiles[0].Count)).Append("    ");
            sb.Append("    ").Append(GetTopNumbersLine(playerBoard.Tiles[0].Count)).Append("\n");
            
            sb.Append(LeftPad).Append("   ").Append(line).Append("    ");
            sb.Append("   ").Append(line).Append("\n");

            for (int index = 0; index < enemyBoard.Tiles.Count; index++)
            {
                sb.Append(LeftPad).Append(Letters.GetLetter(index + 1).PadLeft(2)).Append(" |");
                var enemyRow = enemyBoard.Tiles[index];
                var playerRow = playerBoard.Tiles[index];
                foreach (var tile in enemyRow)
                {
                    if (tile.IsBombed == false) sb.Append("   ");
                    else if (tile.IsBombed && tile.IsEmpty() == false) sb.PrintInColor(" \u00D7 ", ConsoleColor.Green); // multiplication sign
                    else sb.PrintInColor(" o ", ConsoleColor.Red); // dot

                    sb.Append("|");
                }

                sb.Append("    ").Append(Letters.GetLetter(index + 1).PadLeft(2)).Append(" |");
                
                foreach (var tile in playerRow)
                {
                    if (tile.IsEmpty()) sb.Append("   ");
                    else if (tile.IsBombed) sb.PrintInColor(" - ", ConsoleColor.Red);
                    else sb.PrintInColor(" + ", ConsoleColor.Green);

                    sb.Append("|");
                }

                sb.Append("\n").Append(LeftPad).Append("   ").Append(line).Append("       ").Append(line).Append("\n");
            }
            
            sb.Append("\n");
            sb.Append(LeftPad).Append(" ").PrintInColor(" \u00D7 ", ConsoleColor.Green).Append(" : Bombing hit");
            sb.AddSpaces(line.Length - 10).PrintInColor(" + ", ConsoleColor.Green).Append(" : Ship tile alive\n");
            sb.Append(LeftPad).Append(" ").PrintInColor(" o ", ConsoleColor.Red).Append(" : Bombing miss");
            sb.AddSpaces(line.Length - 11).PrintInColor(" - ", ConsoleColor.Red).Append(" : Ship tile sunk\n");
            
            
            Console.Write(sb.ToString());
        }

        private void PrintFriendlyShipsAdding(Board currentBoard)
        {
            var sb = new StringBuilder();
            var line = GetTableLine(currentBoard.Tiles[0].Count);
            sb.Append(LeftPad).Append("   ").Append("Your ships:\n");
            sb.Append(LeftPad).Append("   ").Append(GetTopNumbersLine(currentBoard.Tiles[0].Count)).Append("\n");
            sb.Append(LeftPad).Append("   ").Append(line).Append("\n");
            for (var index = 0; index < currentBoard.Tiles.Count; index++)
            {
                var row = currentBoard.Tiles[index];
                sb.Append(LeftPad).Append(Letters.GetLetter(index + 1).PadLeft(2)).Append(" |");
                foreach (var tile in row)
                {
                    if (tile.IsHighlightedStart) sb.PrintInColor(" - ", ConsoleColor.DarkCyan);
                    else if (tile.IsHighlightedEnd) sb.PrintInColor(" + ", ConsoleColor.DarkCyan);
                    else if (tile.IsEmpty()) sb.Append("   ");
                    else if (tile.IsBombed) sb.PrintInColor(" - ", ConsoleColor.Red);
                    else sb.PrintInColor(" + ", ConsoleColor.Green);

                    sb.Append("|");
                }

                sb.Append("\n").Append(LeftPad).Append("   ").Append(line).Append("\n");
            }
            
            sb.Append("\n").Append(LeftPad).Append(" ");
            sb.PrintInColor(" + ", ConsoleColor.Green).Append(" : Placed ship tile\n").Append(LeftPad).Append(" ");
            sb.PrintInColor(" - ", ConsoleColor.DarkCyan).Append(" : Selected ship start\n").Append(LeftPad).Append(" ");
            sb.PrintInColor(" + ", ConsoleColor.DarkCyan).Append(" : Selected ship end\n");
//            sb.PrintInColor(" - ", ConsoleColor.Red).Append(" : Sunk ship tile\n");

            Console.WriteLine(sb.ToString());
        }

        public int GetInteger(string message)
        {
            Console.Write(message);
            int result;
            while (int.TryParse(Console.ReadLine(), out result) == false)
            {
                Console.Write($"Not a number. {message}");
            }

            return result;
        }

        public string GetString(string message)
        {
            Console.Write($"{message}: ");
            return Console.ReadLine();
        }

        public string GetTargetLocation()
        {
            Console.Write("Select target(or \"x\"): ");
            return Console.ReadLine().ToUpper();
        }

        public void DisplayShipsAndBombings(Board enemy, Board current)
        {
            Console.Clear();
            PrintBombedLocationsAndFriendlyShips(enemy, current);
            Console.WriteLine("\nPress enter to go back.");
            Console.ReadLine();
        }

        public void DisplayBombingResult(BombingResult bombingResult, Board targetBoard, Player currentPlayer)
        {
            Console.Clear();
            PrintBombedLocations(targetBoard, currentPlayer);
        }

        public bool AskExitConfirmation()
        {
            Console.Write("Are you sure you want to save and exit?(y/n):");
            var input = Console.ReadLine().ToUpper();
            while (!input.Equals("Y") && !input.Equals("N"))
            {
                Console.Write("Enter \"y\" or \"n\": ");
                input = Console.ReadLine().ToUpper();
            }

            return input.Equals("Y");
        }

        public string GetShipSize(Rules rules)
        {
            Console.Clear();
            Console.WriteLine("Add ship");
            rules.BoatRules.ForEach(rule => Console.WriteLine($"Size: {rule.Size} - {rule.Quantity}"));
            Console.WriteLine("Ship size cant be more than 10 and less than 0");
            Console.WriteLine("Entered size cant already exist");
            Console.Write("Enter new ship's size: ");
            return Console.ReadLine();
        }

        public string GetShipQuantity(int size)
        {
            Console.Clear();
            Console.Write($"Enter size {size} ship's quantity(can't be less than 1 and more than 5): ");
            return Console.ReadLine();
        }

        private void DisplayShipRules(Rules rules)
        {
            rules.BoatRules.ForEach(rule => Console.WriteLine($"Size: {rule.Size} - {rule.Quantity}"));
        }

        private void DisplayBoardRules(Rules rules)
        {
            Console.WriteLine($"Number of rows: {rules.BoardRows}");
            Console.WriteLine($"Number of cols: {rules.BoardCols}");
            Console.WriteLine($"Ships can touch: {rules.CanShipsTouch}");
        }

        private void DisplayCurrentRules(Rules rules)
        {
            Console.WriteLine($"Ruleset name: {rules.Name}");
            Console.WriteLine($"Number of rows: {rules.BoardRows}");
            Console.WriteLine($"Number of cols: {rules.BoardCols}");
            Console.WriteLine($"Ships can touch: {rules.CanShipsTouch}");
            rules.BoatRules.ForEach(rule => Console.WriteLine($"Size: {rule.Size} - {rule.Quantity}"));
        }

        private void DisplayAvailableShips(Dictionary<int, int> availableShips)
        {
            availableShips
                .ToList()
                .OrderBy(pair => pair.Key)
                .ToList()
                .ForEach(pair => Console.WriteLine($"Size: {pair.Key} - {pair.Value} available"));
        }

        private void PrintFriendlyShipsDeleting(Board currentBoard)
        {
            var sb = new StringBuilder();
            var line = GetTableLine(currentBoard.Tiles[0].Count);
            sb.Append(LeftPad).Append("   ").Append("Your ships:\n");
            sb.Append(LeftPad).Append("   ").Append(GetTopNumbersLine(currentBoard.Tiles[0].Count)).Append("\n");
            sb.Append(LeftPad).Append("   ").Append(line).Append("\n");
            for (var index = 0; index < currentBoard.Tiles.Count; index++)
            {
                var row = currentBoard.Tiles[index];
                sb.Append(LeftPad).Append(Letters.GetLetter(index + 1).PadLeft(2)).Append(" |");
                foreach (var tile in row)
                {
                    if (tile.IsHighlightedStart) sb.PrintInColor(" - ", ConsoleColor.Red);
                    else if (tile.IsEmpty()) sb.Append("   ");
                    else sb.PrintInColor(" + ", ConsoleColor.Green);

                    sb.Append("|");
                }

                sb.Append("\n").Append(LeftPad).Append("   ").Append(line).Append("\n");
            }
            
            sb.Append("\n").Append(LeftPad).Append(" ");
            sb.PrintInColor(" + ", ConsoleColor.Green).Append(" : Placed ship tile\n").Append(LeftPad).Append(" ");
            sb.PrintInColor(" - ", ConsoleColor.Red).Append(" : Selected tile of deletable ship \n");

            Console.WriteLine(sb.ToString());
        }

        private void PrintFriendlyShips(Board currentBoard)
        {
            var sb = new StringBuilder();
            var line = GetTableLine(currentBoard.Tiles[0].Count);
            sb.Append(LeftPad).Append("   ").Append("Your ships:\n");
            sb.Append(LeftPad).Append("   ").Append(GetTopNumbersLine(currentBoard.Tiles[0].Count)).Append("\n");
            sb.Append(LeftPad).Append("   ").Append(line).Append("\n");
            for (var index = 0; index < currentBoard.Tiles.Count; index++)
            {
                var row = currentBoard.Tiles[index];
                sb.Append(LeftPad).Append(Letters.GetLetter(index + 1).PadLeft(2)).Append(" |");
                foreach (var tile in row)
                {
                    if (tile.IsEmpty()) sb.Append("   ");
                    else if (tile.IsBombed) sb.PrintInColor(" - ", ConsoleColor.Red);
                    else sb.PrintInColor(" + ", ConsoleColor.Green);

                    sb.Append("|");
                }

                sb.Append("\n").Append(LeftPad).Append("   ").Append(line).Append("\n");
            }
            
            sb.Append("\n").Append(LeftPad).Append(" ");
            sb.PrintInColor(" + ", ConsoleColor.Green).Append(" : Placed ship tile\n").Append(LeftPad).Append(" ");

            Console.WriteLine(sb.ToString());
        }

        public string GetShipStartPoint()
        {
            Console.Clear();
            Console.Write("Enter ship start tile(or \"x\"): ");
            return Console.ReadLine().ToUpper();
        }

        public string GetShipEndPoint()
        {
            Console.Clear();
            Console.Write("Enter ship end tile(or \"x\"): ");
            return Console.ReadLine().ToUpper();
        }

        private void Alert(string alert, ConsoleColor color)
        {
            var temp = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(alert);
            Console.ForegroundColor = temp;
            Continue();
        }

        private string GetMenuShortcut(Menu menu)
        {
            Console.Write("Enter command: ");
            var input = Console.ReadLine();

            if (input == null) return "";
            if (menu.Previous?.Shortcut.ToLower().Equals(input.ToLower()) ?? false) return "X";
            if (menu.MenuItems.All(item => item.Shortcut.ToUpper() != input.ToUpper()))
            {
                Alert("No such shortcut!", ConsoleColor.Red);
                return "";
            }

            return input;
        }

        private void DisplayMenu(Menu menu)
        {
            Console.WriteLine(menu.Title);
            for (int i = 0; i < menu.Title.Length; i++) Console.Write("-");
            Console.WriteLine();
            menu.MenuItems.ForEach(item => Console.WriteLine($"{item.Shortcut}) {item.Description}"));
            if (menu.Previous != null) Console.WriteLine($"{menu.Previous.Shortcut}) {menu.Previous.Description}");
        }

        public void WaitForUser()
        {
            Thread.Sleep(1000);
        }

        private string GetSaveGameName()
        {
            Console.Write("Enter a name for the save game: ");
            var input = Console.ReadLine();
            while (input == null || input.Equals(""))
            {
                Console.Write("Enter a name for the save game: ");
                input = Console.ReadLine();
            }
            return input;
        }

        public void DisplaySavedGames(List<SaveGame> saves)
        {
            Console.Clear();
            saves.ForEach(s => Console.WriteLine($"{s.SaveGameId}) {s.Name}"));
        }

        public string GetDeletableShipTile()
        {
            Console.Write("Enter a tile occupying a ship you wish to delete(or \"x\"): ");
            return Console.ReadLine().ToUpper();
        }

        public string GetRulesetName()
        {
            Console.Clear();
            Console.Write("Enter a name for your custom rules: ");
            return Console.ReadLine();

        }

        public string ConfirmBoatsOverride()
        {
            Console.Clear();
            Console.Write("Are you sure you want to change this setting? \n" +
                          "Changes may reset ships on boards if necessary (YES/NO):");
            return Console.ReadLine();
        }

        public string GetShipsCanTouch(int current)
        {
            Console.Clear();
            Console.WriteLine($"Current ships can touch: {current == 1}");
            Console.Write("Enter whether ships can touch(YES/NO or x): ");
            return Console.ReadLine();
        }

        public string GetNewBoardHeight(int currentRows)
        {
            Console.Clear();
            Console.WriteLine($"Current board rows(height): {currentRows}");
            Console.Write("Enter amount of rows(or x): ");
            return Console.ReadLine();
        }

        public string GetNewBoardWidth(int currentCols)
        {
            Console.Clear();
            Console.WriteLine($"Current board columns(width): {currentCols}");
            Console.Write("Enter amount of columns(or x): ");
            return Console.ReadLine();
            
        }

        public string GetExistingShipSize(Rules rules)
        {
            Console.Clear();
            rules.BoatRules.ForEach(rule => Console.WriteLine($"Size: {rule.Size} - {rule.Quantity}"));
            Console.Write("Enter an existing ship's size: ");
            return Console.ReadLine();
        }

        private void Continue()
        {
            Console.WriteLine("Press enter to continue");
            Console.ReadLine();
        }

        private static string GetTableLine(int length)
        {
            var sb = new StringBuilder();
            sb.Append("+");
            for (int i = 0; i < length; i++)
            {
                sb.Append("---+");
            }

            return sb.ToString();
        }
        
        private static string GetTopNumbersLine(int length)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                sb.Append(" ");
                sb.Append($"{i + 1}".PadLeft(2).PadRight(3));
            }

            return sb.ToString();
        }
    }
}