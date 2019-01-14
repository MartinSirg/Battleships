using System;
using System.Collections.Generic;
using System.Text;


namespace MenuSystem
{
    public class Menu
    {
        public string Title { get; set; }
        public string TitleWithName;
        public Display DisplayBefore = Display.Nothing;
        public List<MenuItem> MenuItems { get; set; }
        public MenuItem Previous { get; set; } = new MenuItem
        {
            Shortcut = ExitString,
            Description = "Previous menu",
            GetCommand = () => Command.Previous
        };

        public static string ExitString { get; set; } = "X";
    }
}