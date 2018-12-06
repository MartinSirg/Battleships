using System;

namespace MenuSystem
{
    public class MenuItem
    {
        public string Shortcut { get; set; }
        public string Description { get; set; }

        public Func<string> CommandToExecute { get; set; }
    }
}