using System;
using DAL;

namespace MenuSystem
{
    public class MenuItem
    {
        public string Shortcut { get; set; }
        public string Description { get; set; }
        public Func<Command> CommandToExecute { get; set; }
//        public Func<AppDbContext, Result> DatabaseCommand { get; set; }
    }
}