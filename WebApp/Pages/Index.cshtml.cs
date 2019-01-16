﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL;
using DAL;
using Domain;
using MenuSystem;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySqlX.XDevAPI.Common;
using Result = MenuSystem.Result;

namespace WebApp.Pages
{
    public class IndexModel : PageModel
    {
        public readonly Game Game;
        private AppDbContext _dbContext;
        public List<int> HiddenMenuItemIds { get; set; }    //Menu items that have functionality(not menu changing item ids)
        public List<(string msg, MsgType type)> Messages { get; set; } = new List<(string msg, MsgType type)>();
        public IndexModel(AppDbContext dbContext, Game game)
        {
            _dbContext = dbContext;
            Game = game;
            //TODO: testing hide replay menu item(id = 35) REMOVE LATER 
            HiddenMenuItemIds = new List<int>{};
        }

        public void OnGet()
        {
            if (Game.Messages != null && Game.Messages.Count > 0)
            {
                Game.Messages.ForEach(msgTuple => Messages.Add(msgTuple));
                Game.Messages.Clear();
            }
        }
        
        public RedirectToPageResult OnPost(string shortCut, string value1 = default, string value2 = default)
        {
            Command command;
            if (shortCut.ToLower().Equals(Menu.ExitString.ToLower()))
            {
                command = Game.CurrentMenu.Previous.GetCommand();
            }
            else
            {
                var menuItem = Game.CurrentMenu.MenuItems.FirstOrDefault(item => item.Shortcut == shortCut);
                if (menuItem == null)
                {
                    Game.Messages.Add( ($"Invalid shortcut {shortCut}", MsgType.Bad));
                    return RedirectToPage();
                }

                command = menuItem.GetCommand();
            }
            
            switch (command)
            {
                case Command.Previous:
                    Game.PreviousMenu();
                    break;
                
                case Command.BombLocation:
                    return BombLocation(value1);
                
                case Command.None:
                    break;
                
                case Command.SaveUnfinishedGame:
                    return SaveUnfinishedGame(value1);
                    
                case Command.EditShipInRules:
                    EditShipInRules(value1, value2);
                    break;
                                    
                case Command.AddShipToRules:
                    AddShipToRules(value1, value2);
                    break;
                                    
                case Command.DeleteShipFromRules:
                    DeleteShipFromRules(value1);
                    break;
                                    
                case Command.EditShipsCanTouchRule:
                    EditShipsCanTouchRule(value1);
                    break;
                                    
                case Command.EditBoardWidth:
                    EditBoardWidth(value1);
                    break;
                                    
                case Command.EditBoardHeight:
                    EditBoardHeight(value1);
                    break;
                                    
                case Command.SetStandardRules:
                    Game.SetStandardRules();
                    Game.Messages.Add( ("Standard rules set!", MsgType.Good));
                    break;
                                    
                case Command.SetShipStartTile:
                    return SetShipStartTile(value1);
                                    
                case Command.PlaceShipOnBoard:
                    return PlaceShipOnBoard();
                                    
                case Command.SetShipEndTile:
                    return SetShipEndTile(value1);
                                    
                case Command.SetTileOfDeleteableShip:
                    return SetTileOfDeletableShip(value1);
                                    
                case Command.DeleteShipFromBoard:
                    var result = Game.DeleteShipFromBoard();
                    if (result == Result.TileNotHighlighted) Game.Messages.Add( ("Tile not highlighted!", MsgType.Bad));
                    else if (result == Result.ShipNotDeleted) Game.Messages.Add( ("Selected tile does not contain a ship!", MsgType.Bad));
                    else Game.Messages.Add( ("Ship deleted from board!", MsgType.Good));
                    break;
                
                case Command.ChangePlayersName:
                    return ChangeName(value1);
                                    
                case Command.FillReplayMenu:
                    Game.FillLoadsMenu(Game.CurrentMenu, true, _dbContext);
                    break;
                
                case Command.FillLoadMenu:
                    Game.FillLoadsMenu(Game.CurrentMenu, false, _dbContext);
                    break;
                
                case Command.LoadGame:
                    Game.LoadGame(_dbContext, int.Parse(shortCut));
                    Game.ChangeMenu(Game.Menus.InGameMenu);
                    break;
                
                case Command.LoadReplay: //TODO:
                    Game.LoadReplayGame(_dbContext, int.Parse(shortCut));
                    RunReplay();
                    break;
                
                case Command.CantStartGame:
                    Game.Messages.Add( ($"Can't start game, All players have to be ready and all ships must have been placed", MsgType.Bad));
                    break;
                
                case Command.GenerateRandomBoard:
                    Game.GenerateRandomBoard(Game.CurrentPlayer);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return RedirectToPage();
        }
        
        public RedirectToPageResult OnPostAlt(int command, string value1 = default, string value2 = default)
        {
            switch ((Command)command)
            {
                case Command.Previous:
                    Game.PreviousMenu();
                    return RedirectToPage("Index");
                
                case Command.BombLocation:
                    return BombLocation(value1);                    
                    
                case Command.None:
                    return RedirectToPage("Index");
                
                case Command.SaveUnfinishedGame:
                    return SaveUnfinishedGame(value1);
                    
                case Command.EditShipInRules:
                    EditShipInRules(value1, value2);
                    break;
                    
               case Command.AddShipToRules:
                    AddShipToRules(value1, value2);
                    break;
                    
                case Command.DeleteShipFromRules:
                    DeleteShipFromRules(value1);
                    break;
                    
                case Command.EditShipsCanTouchRule:
                    EditShipsCanTouchRule(value1);
                    break;
                    
                case Command.EditBoardWidth:
                    EditBoardWidth(value1);
                    break;
                    
                case Command.EditBoardHeight:
                    EditBoardHeight(value1);
                    break;
                    
                case Command.SetStandardRules:
                    Game.SetStandardRules();
                    return RedirectToPage();
                    
                case Command.SetShipStartTile:
                    return SetShipStartTile(value1);
                    
                case Command.PlaceShipOnBoard:
                    return PlaceShipOnBoard();
                    
                case Command.SetShipEndTile:
                    return SetShipEndTile(value1);
                    
                case Command.SetTileOfDeleteableShip:
                    return SetTileOfDeletableShip(value1);
                    
                case Command.DeleteShipFromBoard:
                    var result = Game.DeleteShipFromBoard();
                    if (result == Result.TileNotHighlighted) Game.Messages.Add( ("Tile not highlighted!", MsgType.Bad));
                    else if (result == Result.ShipNotDeleted) Game.Messages.Add( ("Selected tile does not contain a ship!", MsgType.Bad));
                    return RedirectToPage();
                
                case Command.ChangePlayersName:
                    return ChangeName(value1);
                    
                case Command.FillReplayMenu:
                    Game.FillLoadsMenu(Game.CurrentMenu, true, _dbContext);
                    return RedirectToPage();
                    
                case Command.FillLoadMenu:
                    Game.FillLoadsMenu(Game.CurrentMenu, false, _dbContext);
                    return RedirectToPage();
                    

//                !Not applicable!
//
//                    case Command.LoadGame:
//                    Game.LoadGame(_dbContext, int.Parse(shortCut));
//                    Game.ChangeMenu(Game.Menus.InGameMenu);
//                    break;
//                
//                case Command.LoadReplay:
//                    Game.LoadReplayGame(_dbContext, int.Parse(shortCut));
//                    RunReplay();
//                    break;
                
                case Command.CantStartGame:
                    Game.Messages.Add( ($"Can't start game, All players have to be ready and all ships must have been placed", MsgType.Bad));
                    return RedirectToPage();
                
                case Command.GenerateRandomBoard:
                    Game.GenerateRandomBoard(Game.CurrentPlayer);
                    Game.Messages.Add( ("New board generated", MsgType.Good));
                    return RedirectToPage();
                    
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return RedirectToPage();
        }

        public RedirectToPageResult OnPostBoardRules(string width, string height, string canTouch) // canTouch: yes, no
        {
            if (width == null || height == null || canTouch == null)
            {
                throw new Exception("One of the elements is null");
            }
            EditBoardWidth(width);
            EditBoardHeight(height);
            EditShipsCanTouchRule(canTouch);
            return RedirectToPage();
        }

//        -------------------Game methods controlling methods-----------------------
        private void RunReplay()
        {
            throw new NotImplementedException();
        }

        private RedirectToPageResult BombLocation(string locationString)
        {
            if (locationString == null)
            {
                Game.Messages.Add( ("Can't bomb. Location string is null", MsgType.Bad));
                return RedirectToPage();
            }
            
            var result = Game.BombLocation(locationString);
            ParseBombingResult(result, locationString);
            return RedirectToPage();

        }

        private void ParseBombingResult(Result result, string locationString)
        {
            GameMove move;
            switch (result)
            {
                case Result.GameOver:
                    Player p = Game.CurrentPlayer;

                    Game.SaveGame(_dbContext, $"{Game.CurrentPlayer.Name}(WINNER!) vs {Game.TargetPlayer.Name} with {Game.GameMoves.Count} moves", true);
                    Game.Messages.Add( ($"{Game.CurrentPlayer.Name} has won!", MsgType.Good));
                    return;
                
                case Result.OneBombing:
                    move = Game.GameMoves[Game.GameMoves.Count - 1];
                    var sb = new StringBuilder($"{move.Target.Name} is bombed at {move.Tile}.\n");
                    sb.Append(move.Tile.IsEmpty() ? "Its a MISS!" : "Its a HIT!");
                    Game.Messages.Add( (sb.ToString(), MsgType.Good));
                    return;
                
                case Result.TwoBombings:
                    move = Game.GameMoves[Game.GameMoves.Count - 2];
                    sb = new StringBuilder($"{move.Target.Name} is bombed at {move.Tile}.\n");
                    sb.Append(move.Tile.IsEmpty() ? "Its a MISS!" : "Its a HIT!");
                    
                    move = Game.GameMoves[Game.GameMoves.Count - 1];
                    sb.Append($"\n{move.Target.Name} is bombed at {move.Tile}.");
                    sb.Append(move.Tile.IsEmpty() ? "Its a MISS!" : "Its a HIT!");
                    Game.Messages.Add( (sb.ToString(), MsgType.Good));
                    return;
                   
                case Result.ComputerWon:
                    Game.SaveGame(_dbContext, $"{Game.CurrentPlayer.Name}(WINNER!) vs {Game.TargetPlayer.Name} with {Game.GameMoves.Count} moves", true);
                    Game.Messages.Add( ("Computer has won!", MsgType.Good));
                    return;
                
                case Result.NoSuchTile:
                    Game.Messages.Add( ($"{locationString.ToUpper()} is not a valid location!", MsgType.Bad));
                    return;
                
                case Result.TileAlreadyBombed:
                    Game.Messages.Add( ($"{locationString.ToUpper()} is already bombed!", MsgType.Bad));
                    return;
                default:
                    throw new ArgumentOutOfRangeException(nameof(result), result, null);
            }
        }

        private RedirectToPageResult SaveUnfinishedGame(string saveGameName)
        {
            if (saveGameName == null)
            {
                Game.Messages.Add( ("Can't save game. Name is null", MsgType.Bad));
            }
            else
            {
                var result = Game.SaveGame(_dbContext, saveGameName, false);
                if (result == Result.TooLong) Game.Messages.Add( ("Game not saved, name was too long!", MsgType.Bad));
            }
            return RedirectToPage();
        }

        private void EditShipInRules(string sizeString, string quantityString)
        {
            Game.SetCustomRules();
            if (sizeString == null || quantityString == null)
            {
                Game.Messages.Add( ("Can't edit ship, Size or Quantity is null!", MsgType.Bad));
                return;
            }
            
            if (!int.TryParse(sizeString, out var size) || !int.TryParse(quantityString, out var quantity))
            {
                Game.Messages.Add( ("Can't edit ship, Found a non numeric input!", MsgType.Bad));
                return;
            }

            var result = Game.EditShipInRules(size, quantity);
            if (result == Result.InvalidSize) Game.Messages.Add( ($"Can't edit ship, {size} sized ship not found!", MsgType.Bad));
            else if (result == Result.InvalidQuantity) Game.Messages.Add( ($"Can't edit ship, quantity: {quantity} is incorrect", MsgType.Bad));
            else Game.Messages.Add( ($"Size {size} ship quantity is now set to {quantity}", MsgType.Good));
        }

        private void AddShipToRules(string sizeString, string quantityString)
        {
            Game.SetCustomRules();
            if (sizeString == null || quantityString == null)
            {
                Game.Messages.Add( ("Can't add ship, size or quantity is null!", MsgType.Bad));
                return;
            }
            
            if (!int.TryParse(sizeString, out var size) || !int.TryParse(quantityString, out var quantity))
            {
                Game.Messages.Add( ("Cant add ship, Found a non numeric input!", MsgType.Bad));
                return;
            }

            var result = Game.AddShipToRules(size, quantity);
            if (result == Result.InvalidSize) Game.Messages.Add( ($"Can't add ship, size: {size} is incorrect!", MsgType.Bad));
            else if (result == Result.InvalidQuantity) Game.Messages.Add( ($"Can't add ship, Quantity: {quantity} is incorrect", MsgType.Bad));
            else Game.Messages.Add( ($"{quantity} size {size} ships added to rules ", MsgType.Bad));
        }

        private void DeleteShipFromRules(string sizeString)
        {
            Game.SetCustomRules();
            if (sizeString == null)
            {
                Game.Messages.Add( ("Can't delete ship, size string is null", MsgType.Bad));
                return ;
            }
                
            if (!int.TryParse(sizeString, out var size))
            {
                Game.Messages.Add( ("Can't delete ship, found a non numeric input!", MsgType.Bad));
                return;
            }

            var result = Game.DeleteShipFromRules(size);
            Game.Messages.Add( result == Result.InvalidSize 
                ? ($"Can't delete ship, Size {size} ship doesn't exist!", MsgType.Bad) 
                : ($"Size {size} ship deleted!", MsgType.Good));
        }

        private void EditShipsCanTouchRule(string input)    //input has to be "YES" or "NO"
        {
            Game.SetCustomRules();
            if (input == null)
            {
                Game.Messages.Add( ("Ships can touch input is null!", MsgType.Bad));
                return;
            }

            if (input.ToUpper().Equals(Game.Rules.CanShipsTouch == 1 ? "YES" : "NO")) return;
            var result = Game.EditShipsCanTouchRule(input);
            Game.Messages.Add( result == Result.InvalidInput 
                ? ($"Can't change ships can touch rule, {input} is not YES or NO!", MsgType.Bad) 
                : ($"Ships can touch rule is now set to {input}", MsgType.Good));
        }

        private void EditBoardHeight(string sizeString)
        {
            Game.SetCustomRules();
            if (sizeString == null)
            {
                Game.Messages.Add( ("Can't edit board height, input is null!", MsgType.Bad));
                return;
            }
                
            if (!int.TryParse(sizeString, out var size))
            {
                Game.Messages.Add( ("Can't edit board height, found a non numeric input!", MsgType.Bad));
                return;
            }
                
            if (size == Game.Rules.BoardRows) return;
            var result = Game.EditBoardHeight(size);
            
            Game.Messages.Add( result == Result.InvalidInput 
                ? ($"Can't edit board height, input: {sizeString} is not between {Game.MinRows} and {Game.MaxRows} inclusive!",MsgType.Bad) 
                : ($"Board height changed to {size}", MsgType.Good));
        }

        private void EditBoardWidth(string sizeString)
        {
            Game.SetCustomRules();
            if (sizeString == null)
            {
                Game.Messages.Add( ("Can't edit board width, input is null!", MsgType.Bad));
                return;
            }
                
            if (!int.TryParse(sizeString, out var size))
            {
                Game.Messages.Add( ("Can't edit board height, found a non numeric input!", MsgType.Bad));
                return;
            }
            if (size == Game.Rules.BoardCols) return;
            var result = Game.EditBoardWidth(size);
            
            Game.Messages.Add( result == Result.InvalidInput
                ? ($"Can't edit board height, {sizeString} is not between {Game.MinCols} and {Game.MaxCols} inclusive!",MsgType.Bad)
                : ($"Board width changed to {size}", MsgType.Good));
            
        }

        private RedirectToPageResult PlaceShipOnBoard()
        {
            var result = Game.PlaceShipOnBoard();
            if (result == Result.HighlightMissing) 
                Game.Messages.Add( ("Start or end tile not selected", MsgType.Bad));
            else if (result == Result.NoCommonAxis) 
                Game.Messages.Add( ("Start and End tiles don't share a common axis!", MsgType.Bad));
            else if (result == Result.InvalidSize) 
                Game.Messages.Add( ($"Size {Game.GetHighlightedSize()} not found in rules!", MsgType.Bad));
            else if (result == Result.InvalidQuantity) 
                Game.Messages.Add( ($"No more {Game.GetHighlightedSize()} sized ships left in rules", MsgType.Bad));
            else if (result == Result.Overlap) 
                Game.Messages.Add( ("The ship you wish to place would overlap/touch another ship!", MsgType.Bad));
            return RedirectToPage();
        }

        private RedirectToPageResult SetShipStartTile(string location)
        {
            if (location == null)
            {
                Game.Messages.Add( ("Ship start tile is null!", MsgType.Bad));
                return RedirectToPage();
            }
                
            var result = Game.SetShipStartTile(location);
            if (result == Result.NoSuchTile) Game.Messages.Add( ($"{location.ToUpper()} is not a valid tile!", MsgType.Bad));
            else if (result == Result.TileNotHighlighted) Game.Messages.Add( ($"{location.ToUpper()} already contains a ship!", MsgType.Bad));
            return RedirectToPage(); //Successful highlight
        }

        private RedirectToPageResult SetShipEndTile(string location)
        {
            if (location == null)
            {
                Game.Messages.Add( ("Ship end tile is null!", MsgType.Bad));
                return RedirectToPage();
            }
                
            var result = Game.SetShipEndTile(location);
            if (result == Result.NoSuchTile) Game.Messages.Add( ($"{location.ToUpper()} is not a valid tile!", MsgType.Bad));
            else if (result == Result.TileNotHighlighted) Game.Messages.Add( ($"{location.ToUpper()} already contains a ship!", MsgType.Bad));
            return RedirectToPage(); //Successful highlight
        }

        private RedirectToPageResult SetTileOfDeletableShip(string location)
        {
            if (location == null)
            {
                Game.Messages.Add( ("Deletable ship tile is null!", MsgType.Bad));
                return RedirectToPage();
            }
                
            var result = Game.SetTileContainingDeletableShip(location);
            if (result == Result.NoSuchTile) Game.Messages.Add( ($"{location.ToUpper()} is not a valid tile!", MsgType.Bad));
            else if (result == Result.TileNotHighlighted) Game.Messages.Add( ($"{location.ToUpper()} doesn't contain a ship!", MsgType.Bad));
            return RedirectToPage();//Successful highlight
        }

        private RedirectToPageResult ChangeName(string name)
        {
            if (name == null)
            {
                Game.Messages.Add( ("Name parameter is null", MsgType.Bad));
                return RedirectToPage();
            }
            
            var result = Game.ChangePlayersName(name);
            
            if (result == Result.PlayerNameNotChanged) Game.Messages.Add( ($"{name} is not allowed! Try another.", MsgType.Bad));
            else if (result == Result.TooLong) Game.Messages.Add( ($"{name} is too long! Max length is {Player.MaxLength}.", MsgType.Bad));
            else if (result == Result.TooShort) Game.Messages.Add( ($"{name} is too short! Min length is {Player.MinLength}.", MsgType.Bad));
            else
            {    
                Game.CurrentMenu.Title =
                    Game.CurrentMenu.TitleWithName?.Replace("PLAYER_NAME", Game.CurrentPlayer.Name) 
                    ?? Game.CurrentMenu.Title;
            }

            return RedirectToPage();
        }
    }
}