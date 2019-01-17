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
        private static string _exitString = Menu.ExitString.ToLower();

        public void Loop()
        {
            while (true)
            {
                Console.Clear();
                Command command;
                // 1)Parse DisplayBefore
                ParseDisplayBefore(_game.CurrentMenu.DisplayBefore);
                
                // 2)Print menu options + print previous + menu title
                DisplayMenu(_game.CurrentMenu);
                
                // 3)Get Menu shortcut (Console.ReadLine())
                var shortcut = GetMenuShortcut(_game.CurrentMenu);
                if (shortcut.Equals("")) continue; // invalid shortcut, continue loop to try again
                if (shortcut.ToLower().Equals(_exitString.ToLower()))
                {
                    command = _game.CurrentMenu.Previous.GetCommand();
                }
                else
                {
                    // 4)Run and save Command
                    command = _game.CurrentMenu.MenuItems
                        .Find(item => item.Shortcut.ToLower().Equals(shortcut.ToLower())).GetCommand();
                }
                

                // 5)Parse command
                ParseCommand(command, shortcut);
            }
        }
        
        private void ParseDisplayBefore(Display display)
        {
            switch (display)
            {
                case Display.CurrentShips:
                    PrintFriendlyShips(_game.CurrentPlayer.Board);
                    break;
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
                case Display.Nothing: //Does nothing
                    break;
                case Display.CurrentShipsAdding:
                    PrintFriendlyShipsAdding(_game.CurrentPlayer.Board);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(display), display, null);
            }
        }

        //--------------------------Command enum derived methods(found in menu items as commands)-----------------------
        
        private void ParseCommand(Command command, string shortCut)
        {
            string sizeString, quantityString;
            int size , quantity ;
            Result result;
            switch (command)
            {
                case Command.GenerateRandomBoard:
                    while (true)
                    {
                        _game.GenerateRandomBoard(_game.CurrentPlayer);
                        Console.Clear();
                        PrintFriendlyShips(_game.CurrentPlayer.Board);
                        Console.WriteLine("Random ships generated!");
                        Console.Write("If you want generate again press \"y\".");
                        var input = Console.ReadLine();
                        if (input.ToLower().Equals("y")) continue;
                        _game.PreviousMenu();
                        break;
                    }
                    break;
                
                case Command.Previous:
                    _game.PreviousMenu();
                    break;
                
                case Command.BombLocation:
                    BombLocation();
                    break;
                
                case Command.None:
                    break;
                
                case Command.SaveUnfinishedGame:
                    _game.SaveGame(_dbContext, GetSaveGameName(), false);
                    break;
                
                case Command.EditShipInRules:
                    EditShipInRules();
                    break;
                
                case Command.AddShipToRules:
                    AddShipToRules();
                    break;
                
                case Command.DeleteShipFromRules:
                    DeleteShipFromRules();
                    break;
                
                case Command.EditShipsCanTouchRule:
                    EditShipsCanTouchRule();
                    break;
                    
                case Command.EditBoardWidth:
                    EditBoardWidth();
                    break;
                    
                case Command.EditBoardHeight:
                    EditBoardHeight();
                    break;
                    
                case Command.SetStandardRules:
                    Console.Clear();
                    _game.SetStandardRules();
                    Alert("Standard rules set!", ConsoleColor.Green);
                    break;
                    
                case Command.SetShipStartTile:
                    SetShipStartTile();
                    break;

                case Command.SetShipEndTile:
                    SetShipEndTile();
                    break;
                
                case Command.PlaceShipOnBoard:
                    result = _game.PlaceShipOnBoard();
                    if (result == Result.HighlightMissing) 
                        Alert("Start or End tiles not highlighted!", ConsoleColor.Red);
                    else if (result == Result.NoCommonAxis) 
                        Alert("Start and End tiles don't share a common axis!", ConsoleColor.Red);
                    else if (result == Result.InvalidSize) 
                        Alert($"Size {_game.GetHighlightedSize()} not found in rules!", ConsoleColor.Red);
                    else if (result == Result.InvalidQuantity) 
                        Alert($"No more {_game.GetHighlightedSize()} sized ships left in rules", ConsoleColor.Red);
                    else if (result == Result.Overlap) 
                        Alert("The ship you wish to place would overlap/touch another ship!", ConsoleColor.Red);
                    break;
                
                case Command.SetTileOfDeleteableShip:
                    SetTileContainingDeletableShip();
                    break;
                    
                case Command.DeleteShipFromBoard:
                    result = _game.DeleteShipFromBoard();
                    if (result == Result.TileNotHighlighted)
                        Alert("Tile not highlighted!", ConsoleColor.Red);
                    else if (result == Result.ShipNotDeleted)
                        Alert("Selected tile does not contain a ship!", ConsoleColor.Red);
                    break;
                    
                case Command.ChangePlayersName:
                    ChangePlayerName();
                    break;
                    
                case Command.FillReplayMenu:
                    _game.FillLoadsMenu(_game.CurrentMenu, true, _dbContext);
                    break;
                    
                case Command.FillLoadMenu:
                    _game.FillLoadsMenu(_game.CurrentMenu, false, _dbContext);
                    break;
                                        
                case Command.LoadReplay:
                    _game.LoadReplayGame(_dbContext, int.Parse(shortCut));
                    RunReplay();
                    break;
                
                case Command.LoadGame:
                    _game.LoadGame(_dbContext, int.Parse(shortCut));
                    _game.ChangeMenu(_game.Menus.InGameMenu);
                    break;
                
                case Command.CantStartGame:
                    Alert("Both players have to be ready and all ships have to be placed!", ConsoleColor.Red);
                    break;
                    
                default:
                    throw new ArgumentOutOfRangeException(nameof(command), command, null);
            }
        }

        private void BombLocation()
        {
            while (true)
            {
                Console.Clear();
                PrintBombedLocations(_game.TargetPlayer.Board, _game.CurrentPlayer);
                
                var input = GetTargetLocation();
                if (input != null && input.ToLower().Equals(_exitString.ToLower())) break;
                
                var result = _game.BombLocation(input);
                var parseResult = ParseBombingResult(result, input);
                if (parseResult == Result.Continue) continue;
                break;
            }
        }

        private Result ParseBombingResult(Result result, string location)
        {
            GameMove move;
            switch (result)
            {
                case Result.GameAlreadyOver:
                    Alert("Game already over!", ConsoleColor.Red);
                    _game.SaveGame(_dbContext, GetSaveGameName(), true);
                    return Result.Continue;
                
                case Result.GameOver:
                    Alert($"{_game.CurrentPlayer.Name} has won!", ConsoleColor.Green);
                    
                    _game.SaveGame(_dbContext, GetSaveGameName(), true);
                    return Result.Break;
                
                case Result.OneBombing:
                    Console.Clear();
                    PrintBombedLocations(_game.CurrentPlayer.Board, _game.TargetPlayer);
                    move = _game.GameMoves[_game.GameMoves.Count - 1];
                    Console.WriteLine($"{move.Target.Name} is bombed at {move.Tile}.");
                    if (move.Tile.IsEmpty()) Alert("Its a MISS!", ConsoleColor.DarkRed);
                    else Alert("Its a HIT!", ConsoleColor.DarkGreen);
                    return Result.Break;
                
                case Result.TwoBombings:
                    Console.Clear();
                    PrintBombedLocations(_game.TargetPlayer.Board, _game.CurrentPlayer);
                    move = _game.GameMoves[_game.GameMoves.Count - 2];
                    Console.WriteLine($"{move.Target.Name} is bombed at {move.Tile}.");
                    if (move.Tile.IsEmpty()) Alert("Its a MISS!", ConsoleColor.DarkRed);
                    else Alert("Its a HIT!", ConsoleColor.DarkGreen);
                    
                    Console.Clear();
                    PrintBombedLocations(_game.CurrentPlayer.Board, _game.TargetPlayer);
                    move = _game.GameMoves[_game.GameMoves.Count - 1];
                    Console.WriteLine($"{move.Target.Name} is bombed at {move.Tile}.");
                    if (move.Tile.IsEmpty()) Alert("Its a MISS!", ConsoleColor.DarkGreen);
                    else Alert("Its a HIT!", ConsoleColor.DarkRed);
                    return Result.Break;
                   
                case Result.ComputerWon:
                    Alert("Computer has won!", ConsoleColor.Green);
                    _game.SaveGame(_dbContext, GetSaveGameName(), true);
                    return Result.Break;
                
                case Result.NoSuchTile:
                    Alert($"{location.ToUpper()} is not a valid location!", ConsoleColor.Red);
                    return Result.Continue;
                
                case Result.TileAlreadyBombed:
                    Alert($"{location.ToUpper()} is already bombed!", ConsoleColor.Red);
                    return Result.Continue;
                default:
                    throw new ArgumentOutOfRangeException(nameof(result), result, null);
            }
        }

        private void RunReplay()
        {
            Player lastMoveBy = _game.Player1;
            _game.GameMoves.ForEach(move =>
            {
                Console.Clear();
                Tile targetTile = move.Tile;
                Player bombingPlayer = _game.Player1 == move.Target ? _game.Player2 : _game.Player1;
                _game.BombShipReplay(targetTile);
                PrintBombedLocations(move.Target.Board, bombingPlayer);
                Console.WriteLine();
                Console.WriteLine($"{move.Target.Name} is bombed at {move.Tile}.");
                if (move.Tile.IsEmpty()) Alert("Its a MISS!", ConsoleColor.DarkRed);
                else Alert("Its a HIT!", ConsoleColor.DarkGreen);
                lastMoveBy = bombingPlayer;
            });
            Alert($"{lastMoveBy.Name} has won the game!", ConsoleColor.Red);
            _game.ResetAll();
            _game.ReturnToRootMenu();
        }

        private void AddShipToRules()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Adding ships:\n");
                DisplayShipRules(_game.Rules);
                Console.Write($"Enter the size you want to add between 1 and 10 ({_exitString} to exit): ");
                var sizeString = Console.ReadLine();
                if (sizeString != null && sizeString.ToLower().Equals(_exitString)) break;
                Console.Write("Enter the amount(between 1 and 5 inclusive): ");
                var quantityString = Console.ReadLine();
                if (!int.TryParse(sizeString, out var size) || !int.TryParse(quantityString, out var quantity))
                {
                    Alert("Found a non numeric input!", ConsoleColor.Red);
                    continue;
                }

                var result = _game.AddShipToRules(size, quantity);
                if (result == Result.InvalidSize) Alert($"Size: {size} is incorrect!", ConsoleColor.Red);
                else if (result == Result.InvalidQuantity) Alert($"Quantity: {quantity} is incorrect",ConsoleColor.Red);
                else if (result == Result.TooManyShipTiles) Alert($"Too many ship tiles. Can't be more than {_game.Rules.MaxShipTiles()} tiles!", ConsoleColor.Red);
                else break; //Successful add
            }
        }

        private void EditShipInRules()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Editing ships:\n");
                DisplayShipRules(_game.Rules);
                Console.Write($"Enter the ship(size) you want to edit ({_exitString} to exit): ");
                var sizeString = Console.ReadLine();
                if (sizeString != null && sizeString.ToLower().Equals(_exitString)) break;
                Console.Write("Enter the new amount(between 1 and 5 inclusive): ");
                var quantityString = Console.ReadLine();
                if (!int.TryParse(sizeString, out var size) || !int.TryParse(quantityString, out var quantity))
                {
                    Alert("Found a non numeric input!", ConsoleColor.Red);
                    continue;
                }

                var result = _game.EditShipInRules(size, quantity);
                if (result == Result.InvalidSize) Alert($"{size} sized ship not found!", ConsoleColor.Red);
                else if (result == Result.InvalidQuantity) Alert($"Quantity: {quantity} is incorrect",ConsoleColor.Red);
                else if (result == Result.TooManyShipTiles) Alert($"Too many ship tiles. Can't be more than {_game.Rules.MaxShipTiles()} tiles!", ConsoleColor.Red);
                else break; //Successful edit
            }
        }

        private void DeleteShipFromRules()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Deleting ships: \n");
                DisplayShipRules(_game.Rules);
                Console.Write($"Enter the size of the ship you want to delete ({_exitString} to exit): ");
                var sizeString = Console.ReadLine();
                if (sizeString != null && sizeString.ToLower().Equals(_exitString)) break;
                
                if (!int.TryParse(sizeString, out var size))
                {
                    Alert("Found a non numeric input!", ConsoleColor.Red);
                    continue;
                }

                var result = _game.DeleteShipFromRules(size);
                if (result == Result.InvalidSize) Alert($"Size {size} doesn't exist!", ConsoleColor.Red);
                else break; //Successful add
            }
        }

        private void EditShipsCanTouchRule()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Editing ships can touch rule: \n");
                Console.WriteLine("Current setting: Ships can touch - " + (_game.Rules.CanShipsTouch == 1 ? "true" : "false"));
                Console.Write($"Do you want ships to be able to touch each other? YES/NO ({_exitString} to exit): ");
                var input = Console.ReadLine();
                if (input != null && input.ToLower().Equals(_exitString)) break;
                
                
                var result = _game.EditShipsCanTouchRule(input);
                if (result == Result.InvalidInput) Alert($"{input} is not YES or NO!", ConsoleColor.Red);
                else break; //Successful change
            }
        }

        private void EditBoardWidth()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Editing board width:\n");
                DisplayBoardRules(_game.Rules);
                Console.Write($"Enter the desired board width between {Game.MinCols} and {Game.MaxCols} inclusive. ({_exitString} to exit): ");
                var input = Console.ReadLine();
                if (input != null && input.ToLower().Equals(_exitString)) break;
                
                if (!int.TryParse(input, out var size))
                {
                    Alert("Found a non numeric input!", ConsoleColor.Red);
                    continue;
                }
                
                var result = _game.EditBoardWidth(size);
                if (result == Result.InvalidInput) Alert($"{input} is not between {Game.MinCols} and {Game.MaxCols} inclusive!", ConsoleColor.Red);
                else break; //Successful change
            }
        }

        private void EditBoardHeight()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Editing board height:\n");
                DisplayBoardRules(_game.Rules);
                Console.Write($"Enter the desired board height between {Game.MinRows} and {Game.MaxRows} inclusive. ({_exitString} to exit): ");
                var input = Console.ReadLine();
                if (input != null && input.ToLower().Equals(_exitString)) break;
                
                if (!int.TryParse(input, out var size))
                {
                    Alert("Found a non numeric input!", ConsoleColor.Red);
                    continue;
                }
                
                var result = _game.EditBoardHeight(size);
                if (result == Result.InvalidInput) Alert($"{input} is not between {Game.MinRows} and {Game.MaxRows} inclusive!", ConsoleColor.Red);
                else break; //Successful change
            }
        }

        private void SetShipStartTile()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Selecting ship start tile:\n");
                Console.WriteLine("Currently placed ships:\n");
                PrintFriendlyShipsAdding(_game.CurrentPlayer.Board);
                DisplayAvailableShips(_game.AvailableShips(_game.CurrentPlayer));
                
                Console.Write($"Enter new ship's start tile. ({_exitString} to exit): ");
                var input = Console.ReadLine();
                if (input != null && input.ToLower().Equals(_exitString)) break;
                
                var result = _game.SetShipStartTile(input);
                if (result == Result.NoSuchTile) Alert($"{input.ToUpper()} is not a valid tile!", ConsoleColor.Red);
                else if (result == Result.TileNotHighlighted) Alert($"{input.ToUpper()} already contains a ship!", ConsoleColor.Red);
                else break; //Successful highlight
            }
        }

        private void SetShipEndTile()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Selecting ship end tile:\n");
                Console.WriteLine("Currently placed ships:\n");
                PrintFriendlyShipsAdding(_game.CurrentPlayer.Board);
                DisplayAvailableShips(_game.AvailableShips(_game.CurrentPlayer));
                
                Console.Write($"Enter new ship's end tile. ({_exitString} to exit): ");
                var input = Console.ReadLine();
                if (input != null && input.ToLower().Equals(_exitString)) break;
                
                var result = _game.SetShipEndTile(input);
                if (result == Result.NoSuchTile) Alert($"{input.ToUpper()} is not a valid tile!", ConsoleColor.Red);
                else if (result == Result.TileNotHighlighted) Alert($"{input.ToUpper()} already contains a ship!", ConsoleColor.Red);
                else break; //Successful highlight
            }
        }

        private void SetTileContainingDeletableShip()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Deleting ship:\n");
                Console.WriteLine("Currently placed ships:\n");
                PrintFriendlyShipsDeleting(_game.CurrentPlayer.Board);
                
                Console.Write($"Enter a tile occupying a ship you wish to delete. ({_exitString} to exit): ");
                var input = Console.ReadLine();
                if (input != null && input.ToLower().Equals(_exitString)) break;
                
                var result = _game.SetTileContainingDeletableShip(input);
                if (result == Result.NoSuchTile) Alert($"{input.ToUpper()} is not a valid tile!", ConsoleColor.Red);
                else if (result == Result.TileNotHighlighted) Alert($"{input.ToUpper()} doesn't contain a ship!", ConsoleColor.Red);
                else break; //Successful highlight
            }
        }

        private void ChangePlayerName()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Changing player name: ");
                Console.WriteLine($"Current name: {_game.CurrentPlayer.Name}\n");
                
                Console.Write($"Enter a new name. ({_exitString} to exit): ");
                var input = Console.ReadLine();
                if (input != null && input.ToLower().Equals(_exitString)) break;
                
                var result = _game.ChangePlayersName(input);
                if (result == Result.PlayerNameNotChanged)
                {
                    Alert($"{input} is not allowed! Try another.", ConsoleColor.Red);
                    continue;
                }
                
                if (result == Result.TooLong)
                {
                    Alert($"{input} is too long! Max length is {Player.MaxLength}.", ConsoleColor.Red);
                    continue;
                }
                
                if (result == Result.TooShort)
                {
                    Alert($"{input} is too short! Min length is {Player.MinLength}.", ConsoleColor.Red);
                    continue;
                }

                _game.CurrentMenu.Title =
                    _game.CurrentMenu.TitleWithName.Replace("PLAYER_NAME", _game.CurrentPlayer.Name);
                break; //Successful change
            }
        }


        //--------------------------Displaying helper and UserInput methods-----------------------------------------

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
                    if (tile.IsEmpty() && tile.IsBombed) sb.PrintInColor("   ", ConsoleColor.Cyan);
                    else if (tile.IsEmpty()) sb.Append("   ");
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

        private string GetTargetLocation()
        {
            Console.Write($"Select target({_exitString} to exit): ");
            return Console.ReadLine();
        }

        private void DisplayShipRules(Rules rules)
        {
            Console.WriteLine("Current ship rules:");
            rules.BoatRules.ForEach(rule => Console.WriteLine($"Size: {rule.Size} - {rule.Quantity}"));
            Console.WriteLine();
        }

        private void DisplayBoardRules(Rules rules)
        {
            string shipsCanTouch = rules.CanShipsTouch == 1 ? "YES" : "NO";
            Console.WriteLine("Current board rules:\n");
            Console.WriteLine($"Number of rows: {rules.BoardRows}");
            Console.WriteLine($"Number of cols: {rules.BoardCols}");
            Console.WriteLine($"Ships can touch: {shipsCanTouch}");
            Console.WriteLine();
        }

        private void DisplayCurrentRules(Rules rules)
        {
            Console.WriteLine("Current rules:");
            Console.WriteLine();
            Console.WriteLine($"Ruleset name: {rules.Name}");
            Console.WriteLine($"Number of rows: {rules.BoardRows}");
            Console.WriteLine($"Number of cols: {rules.BoardCols}");
            Console.WriteLine($"Ships can touch: {rules.CanShipsTouch}");
            rules.BoatRules.ForEach(rule => Console.WriteLine($"Size: {rule.Size} - {rule.Quantity}"));
            Console.WriteLine();
        }

        private void DisplayAvailableShips(Dictionary<int, int> availableShips) 
        {
            Console.WriteLine();
            availableShips
                .ToList()
                .OrderBy(pair => pair.Key)
                .ToList()
                .ForEach(pair => Console.WriteLine($"Size: {pair.Key} - {pair.Value} available"));
            Console.WriteLine();
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
            if (menu.Previous?.Shortcut.ToLower().Equals(input.ToLower()) ?? false)
            {
                return input;
            }
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
            menu.MenuItems?.ForEach(item => Console.WriteLine($"{item.Shortcut}) {item.Description}"));
            if (menu.Previous != null) Console.WriteLine($"{menu.Previous.Shortcut}) {menu.Previous.Description}");
        }

        private string GetSaveGameName()
        {
            Console.Write($"Enter a name for the save game.(max length {SaveGame.MaxLength}):  ");
            var input = Console.ReadLine();
            while (input == null || input.Equals("") || input.Length > SaveGame.MaxLength)
            {
                Console.Write($"Enter a name for the save game. (max length {SaveGame.MaxLength}):  ");
                input = Console.ReadLine();
            }
            return input;
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