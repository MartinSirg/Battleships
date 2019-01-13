using System;
using System.Collections.Generic;
using System.Linq;
using BLL;
using DAL;
using Domain;
using MenuSystem;
using Microsoft.EntityFrameworkCore;
using UI;


namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
//            IUserInterface ui = new ConsoleUI();
            
            AppDbContext ctx = new AppDbContext();
            
            
//            GameOld gameOld = new GameOld(ui, ctx);
//
//            ApplicationMenu applicationMenu = new ApplicationMenu(gameOld);
//            Menu main = applicationMenu.GetMain();
//            gameOld.RunMenu(menu: main);


        }
    }
}