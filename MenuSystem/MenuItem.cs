using System;
using DAL;

namespace MenuSystem
{
    public class MenuItem
    {
        public int Id { get; set; } = 0; // default is 0
        public string Shortcut { get; set; }
        public string Description { get; set; }
        public Func<Command> GetCommand { get; set; }
//        public Func<AppDbContext, Result> DatabaseCommand { get; set; }
    }
}