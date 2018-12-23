using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using BLL;
using Domain;
using MenuSystem;

namespace UI
{
    public class ConsoleUI : IUserInterface
    {
        private LetterNumberSystem Letters { get; } = new LetterNumberSystem();
//        public Player CurrentPlayer { get; set; }
//        public Player TargetPlayer { get; set; }
        public string ShipStartPoint { get; set; }
        public string ShipEndPoint { get; set; }
        public int MaxWidth { get; set; }
        public int MaxHeight { get; set; }
        private string LeftPad { get; } = "            ";
        private ConsoleColor DefaultBackground { get; }
        private ConsoleColor DefaultForeground { get; }

        public ConsoleUI(ConsoleColor defaultBackground = ConsoleColor.White,
                         ConsoleColor defaultForeground = ConsoleColor.Black)
        {
            DefaultBackground = defaultBackground;
            DefaultForeground = defaultForeground;
            Console.ForegroundColor = defaultForeground;
            Console.BackgroundColor = defaultBackground;
            Console.Clear();
            Console.Title = "Battleships";
            MaxWidth = (Console.LargestWindowWidth - LeftPad.Length * 2 - 12) / 8;
//            Console.WindowWidth = width;
            Console.WindowHeight = 35;
        }

        public void PrintBombedLocations(Board enemyBoard, Player current)
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

        public void PrintBombedLocationsAndFriendlyShips(Board enemyBoard, Board playerBoard)
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

        public void PrintFriendlyShipsAdding(Board currentBoard)
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

        public string GetTargetLocation(Board enemyBoard, Player currentPlayer)
        {
            Console.Clear();
            PrintBombedLocations(enemyBoard, currentPlayer);
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

        public void DisplayRulesShips(Rules rules)
        {
            rules.BoatRules.ForEach(rule => Console.WriteLine($"Size: {rule.Size} - {rule.Quantity}"));
        }

        public void DisplayBoardRules(Rules rules)
        {
            Console.WriteLine($"Number of rows: {rules.BoardRows}");
            Console.WriteLine($"Number of cols: {rules.BoardCols}");
            Console.WriteLine($"Ships can touch: {rules.CanShipsTouch}");
        }

        public void DisplayCurrentRules(Rules rules)
        {
            Console.WriteLine($"Ruleset name: {rules.Name}");
            Console.WriteLine($"Number of rows: {rules.BoardRows}");
            Console.WriteLine($"Number of cols: {rules.BoardCols}");
            Console.WriteLine($"Ships can touch: {rules.CanShipsTouch}");
            rules.BoatRules.ForEach(rule => Console.WriteLine($"Size: {rule.Size} - {rule.Quantity}"));
        }


        public void DisplayAvailableShips(List<KeyValuePair<int,int>> availableShips)
        {
            availableShips.ForEach(pair => Console.WriteLine($"Size: {pair.Key} - {pair.Value} available"));
        }

        public void DisplayCurrentShips(Board currentPlayerBoard, string type)
        {
            switch (type)
            {
                    case "REGULAR":
                        PrintFriendlyShips(currentPlayerBoard);
                        break;
                    case "ADDING":
                        PrintFriendlyShipsAdding(currentPlayerBoard);
                        break;
                    case "DELETING":
                        PrintFriendlyShipsDeleting(currentPlayerBoard);
                        break;
                    default:
                        Alert("UNKNOWN ERROR in DisplayCurrentShips in ConsoleUI", 5000);
                        break;
            }
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

        public string GetShipStartPoint(Board currentPlayerBoard, List<KeyValuePair<int,int>> availableShips)
        {
            Console.Clear();
            DisplayCurrentShips(currentPlayerBoard, "ADDING");
            DisplayAvailableShips(availableShips);
            Console.Write("Enter ship start tile(or \"x\"): ");
            return Console.ReadLine().ToUpper();
        }

        public string GetShipEndPoint(Board currentPlayerBoard, List<KeyValuePair<int,int>> availableShips)
        {
            Console.Clear();
            DisplayCurrentShips(currentPlayerBoard, "ADDING");
            DisplayAvailableShips(availableShips);
            Console.Write("Enter ship end tile(or \"x\"): ");
            return Console.ReadLine().ToUpper();
        }

        public void Alert(string alert, int waitTime)
        {
            Console.WriteLine(alert);
            Thread.Sleep(waitTime);
        }

        public string GetMenuShortcut()
        {
            Console.Write("Enter command: ");
            return Console.ReadLine().ToUpper();
        }

        public void DisplayNewMenu(Menu menu)
        {
            Console.Clear();
            Console.WriteLine(menu.Title);
            menu.DisplayBefore?.Invoke();
            for (int i = 0; i < menu.Title.Length; i++) Console.Write("-");
            Console.WriteLine();
            menu.MenuItems.ForEach(item => Console.WriteLine($"{item.Shortcut}) {item.Description}"));
            if (menu.Previous != null) Console.WriteLine($"{menu.Previous.Shortcut}) {menu.Previous.Description}");
        }

        public void WaitForUser()
        {
            Thread.Sleep(1000);
        }

        public string GetSaveGameName()
        {
            Console.Write("Enter a name for the save game(or \"x\"): ");
            return Console.ReadLine();
        }

        public void DisplaySavedGames(List<string> names)
        {
            Console.Clear();
            int counter = 1;
            names.ForEach(s => Console.WriteLine($"{counter++}. {s}"));
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

        public void Continue()
        {
            Console.WriteLine("Press any key to continue");
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