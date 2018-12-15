using System;
using System.Collections.Generic;
using System.Text;


namespace MenuSystem
{
    public class Menu
    {
        public string Title { get; set; }
        public string TitleWithName;
        public Action DisplayBefore;
        public List<MenuItem> MenuItems { get; set; }
        public MenuItem Previous { get; set; } = new MenuItem()
        {
            Shortcut = "X",
            Description = "Previous menu"
        };
    }
}