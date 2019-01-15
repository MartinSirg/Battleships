using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BLL;
using DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySqlX.XDevAPI.Common;

namespace WebApp.Pages
{
    public class IndexModel : PageModel
    {
        public readonly Game Game;
        private AppDbContext _dbContext;
        public List<int> HiddenMenuItemIds { get; set; }    //Menu items that have functionality(not menu changing item ids)
        public int ErrorId { get; set; }

        public IndexModel(AppDbContext dbContext, Game game)
        {
            _dbContext = dbContext;
            Game = game;
            //TODO: testing hide replay menu item(id = 35) REMOVE LATER 
            HiddenMenuItemIds = new List<int>{35,34};
        }

        public void OnGet(int errorId = -1)
        {
            if (errorId != -1) ErrorId = errorId;
        }
        
        public void OnPost(string shortCut)
        {
        }
        
        public void OnPostChangeMenu(string shortCut)
        {
            var menuItem = Game.CurrentMenu.MenuItems.FirstOrDefault(item => item.Shortcut == shortCut);
//            if (menuItem == null) RedirectToPage("Index", 1);
            ErrorId = 1;
            OnGet(-1);
            // 4)Run and save Command
            // 5)Parse command
        }
    }
}