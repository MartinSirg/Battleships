using System;
using System.Text;

namespace UI
{
    public static class StringBuilderExtensions
    {
        public static StringBuilder PrintInColor(this StringBuilder sb, string printable, ConsoleColor color)
        {
            ConsoleColor previousBackground = Console.BackgroundColor, previousForeground = Console.ForegroundColor;
            Console.Write(sb.ToString());
            sb.Clear();
            Console.BackgroundColor = color;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write(printable);
            Console.ForegroundColor = previousForeground;
            Console.BackgroundColor = previousBackground;
            return sb;
        }

        public static StringBuilder AddSpaces(this StringBuilder sb, int amount)
        {
            if (amount < 0) throw new ArgumentException();
            for (int i = 0; i < amount; i++)
            {
                sb.Append(" ");
            }

            return sb;
        }
    }
}