using System;
using System.Collections.Generic;
using System.Text;


namespace MenuSystem
{
    public class Menu
    {
        public string NameInTitle = "";
        public string Title { get; set; }
        public Action DisplayBefore;
        public List<MenuItem> MenuItems { get; set; }
        public MenuItem Previous { get; set; } = new MenuItem()
        {
            Shortcut = "X",
            Description = "Previous menu"
        };

        public void Print()
        {
            StringBuilder sb = new StringBuilder();
            if (NameInTitle.Length > 0)
            {
                sb.Append($"-------{NameInTitle + Title}-------\n");
            }
            else
            {
                sb.Append($"-------{Title}-------\n");
            }
            foreach (var menuItem in MenuItems)
            {
                sb.Append($"{menuItem.Shortcut}) {menuItem.Description}\n");
            }

            sb.Append("----------------------------\n");
            sb.Append($"{Previous.Shortcut}) {Previous.Description}");
            Console.WriteLine(sb.ToString());
        }
    }
}