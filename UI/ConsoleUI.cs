using System;
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
        public Player CurrentPlayer { get; set; }
        public Player TargetPlayer { get; set; }
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

        public void PrintBombedLocations(Board enemyBoard)
        {
            var sb = new StringBuilder();
            var line = GetTableLine(enemyBoard.Tiles[0].Count);
            sb.Append(LeftPad).Append("   ").Append("Your bombings:\n");
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

        public void PrintFriendlyShips(Board enemyBoard)
        {
            var sb = new StringBuilder();
            var line = GetTableLine(enemyBoard.Tiles[0].Count);
            sb.Append(LeftPad).Append("   ").Append("Your ships:\n");
            sb.Append(LeftPad).Append("   ").Append(GetTopNumbersLine(enemyBoard.Tiles[0].Count)).Append("\n");
            sb.Append(LeftPad).Append("   ").Append(line).Append("\n");
            for (var index = 0; index < enemyBoard.Tiles.Count; index++)
            {
                var row = enemyBoard.Tiles[index];
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
            sb.PrintInColor(" + ", ConsoleColor.Green).Append(" : Alive ship tile\n").Append(LeftPad).Append(" ");
            sb.PrintInColor(" - ", ConsoleColor.Red).Append(" : Sunk ship tile\n");

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
            Console.Write(message);
            return Console.ReadLine();
        }

        public string GetTargetLocation(Board enemyBoard)
        {
            Console.Clear();
            PrintBombedLocations(enemyBoard);
            Console.Write("Select target: ");
            return Console.ReadLine();
        }

        public void ShowShipsAndBombings(Board enemy, Board current)
        {
            Console.Clear();
            PrintBombedLocationsAndFriendlyShips(enemy, current);
            Console.WriteLine("\nPress enter to go back.");
            Console.ReadLine();
        }

        public void ShowBombingResult(BombingResult bombingResult, Board targetBoard)
        {
            Console.Clear();
            PrintBombedLocations(targetBoard);
            WaitForUser();
            Console.Clear();
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

        public int GetShipSize()
        {
            throw new NotImplementedException();
        }

        public int GetShipQuantity()
        {
            throw new NotImplementedException();
        }

        public void DisplayRulesShips(IUserInterface ui)
        {
            
        }

        public void ShowBoardRules(IUserInterface obj)
        {
            throw new NotImplementedException();
        }

        public void ShowCurrentRules(IUserInterface obj)
        {
            throw new NotImplementedException();
        }

        public void DisplayCurrentRules(IUserInterface obj)
        {
            throw new NotImplementedException();
        }

        public void DisplayAvailableShips(Board currentPlayerBoard, Rules gameRules)
        {
            throw new NotImplementedException();
        }

        public void DisplayCurrentShips(Board currentPlayerBoard)
        {
            throw new NotImplementedException();
        }

        public string GetShipStartPoint(IUserInterface obj)
        {
            throw new NotImplementedException();
        }

        public string GetShipEndPoint(IUserInterface obj)
        {
            throw new NotImplementedException();
        }

        public void Alert(string pleasePlaceAllShipsOnTheBoard)
        {
            throw new NotImplementedException();
        }

        public string GetMenuShortcut()
        {
            throw new NotImplementedException();
        }

        public void DisplayNewMenu(Menu menu)
        {
            throw new NotImplementedException();
        }

        void IUserInterface.WaitForUser()
        {
            WaitForUser();
        }

        private static void WaitForUser()
        {
            Thread.Sleep(2000);
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