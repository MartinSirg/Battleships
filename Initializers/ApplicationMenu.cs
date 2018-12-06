
using System;
using System.Collections;
using System.Collections.Generic;
using BLL;
using Domain;
using MenuSystem;

namespace ConsoleApp
{
    public class ApplicationMenu
    {
        public Game Game { get; set; }
        public Menu MainMenu { get; set; }
        public bool SinglePlayerSelected { get; set; } = false;
        public bool MultiPlayerSelected { get; set; } = false;
        public int PlayersReady { get; set; } = 0;

        public ApplicationMenu(Game game)
        {
            Game = game;
            
            
        }

        public Menu GetMain()
        {
            var inGameMenu = new Menu
            {
                NameInTitle = Game.CurrentPlayer.Name,
                Title = "'s turn",
                MenuItems = new List<MenuItem>
                {
                    new MenuItem
                    {
                        Shortcut = "1", 
                        Description = "Bomb location",
                        CommandToExecute = Game.BombShip
                    },
                    new MenuItem
                    {
                        Shortcut = "2", 
                        Description = "Show my ships and bombings",
                        CommandToExecute = Game.ShowShipsAndBombings
                    },
                    new MenuItem
                    {
                        Shortcut = "9",
                        Description = "Save and Exit",
                        CommandToExecute = Game.SaveGame
                    }
                }
            };
            
            var customShipsMenu =new Menu
            {
                Title = "Customize your ships",
                DisplayBefore = Game.DisplayRulesShips,
                MenuItems = new List<MenuItem>
                
                {
                    new MenuItem
                    {
                        Shortcut = "1",
                        Description = "Add ships to rules",
                        CommandToExecute = Game.AddShipToRules
                    },
                    new MenuItem
                    {
                        Shortcut = "2",
                        Description = "Edit ship",
                        CommandToExecute = Game.EditShipInRules
                    },
                    new MenuItem
                    {
                        Shortcut = "3",
                        Description = "Delete ship",
                        CommandToExecute = Game.DeleteShipInRules
                    }
                    
                }
            };


            var customBoardMenu = new Menu
            {
                Title = "Customize the game board",
                DisplayBefore = Game.ShowBoardRules,
                MenuItems = new List<MenuItem>
                {
                    new MenuItem
                    {
                        Shortcut = "1",
                        Description = "Set ships can touch rule.",
                        CommandToExecute = Game.EditShipsCanTouchRule
                    },
                    new MenuItem
                    {
                        Shortcut = "2",
                        Description = "Set board width",
                        CommandToExecute = Game.EditBoardWidth
                    },
                    new MenuItem
                    {
                        Shortcut = "3",
                        Description = "Set board height",
                        CommandToExecute = Game.EditBoardHeight
                    }
                }
            };
            var customRulesMenu = new Menu
            {
                Title = "Custom rules",
                MenuItems = new List<MenuItem>
                {
                    new MenuItem
                    {
                        Shortcut = "1",
                        Description = "Ships",
                        CommandToExecute = () => Game.RunMenu(customShipsMenu)
                    },
                    new MenuItem
                    {
                        Shortcut = "2",
                        Description = "Board",
                        CommandToExecute = () => Game.RunMenu(customBoardMenu)
                    },
                    new MenuItem
                    {
                        Shortcut = "3",
                        Description = "Set name",
                        CommandToExecute = Game.SetRulesName
                    },
                    new MenuItem
                    {
                        Shortcut = "4",
                        Description = "Load custom rules preset",
                        CommandToExecute = Game.LoadCustomRulesPreset
                    }
                }
            };
            var rulesMenu = new Menu
            {
                Title ="Rules",
                DisplayBefore = Game.ShowCurrentRuleset,
                MenuItems = new List<MenuItem>
                {
                    new MenuItem
                    {
                        Shortcut = "1",
                        Description = "Set custom rules",
                        CommandToExecute = () => Game.RunMenu(customRulesMenu)
                    },
                    new MenuItem
                    {
                        Shortcut = "2",
                        Description = "Set standard Rules",
                        CommandToExecute = Game.SetStandardRules
                    }
                }
            };
            
            var addShipsToBoardMenu = new Menu
            {
                Title = "Add ship to board",
                DisplayBefore = Game.ShowCurrentAndAvailableShips,
                MenuItems = new List<MenuItem>
                {
                    new MenuItem
                    {
                        Shortcut = "1",
                        Description = "Select ship start point(example: D6)",
                        CommandToExecute = Game.GetShipStartTile
                    },
                    new MenuItem
                    {
                        Shortcut = "2",
                        Description = "Select ship end point(example: D7)",
                        CommandToExecute = Game.GetShipEndTile
                    },
                    new MenuItem
                    {
                        Shortcut = "3",
                        Description = "Confirm ship placement",
                        CommandToExecute = Game.PlaceShipOnBoard
                    }
                }
            };
            
            var deleteShipFromBoardMenu = new Menu
            {
                Title = "Delete ship",
                DisplayBefore = Game.ShowCurrentShips,
                MenuItems = new List<MenuItem>
                {
                    new MenuItem
                    {
                        Shortcut = "1",
                        Description = "Select occupying tile(example: D6)",
                        CommandToExecute = Game.GetTileOfDeleteableShip
                    },
                    new MenuItem
                    {
                        Shortcut = "2",
                        Description = "Confirm ship deletion(example: D7)",
                        CommandToExecute = Game.DeleteShipFromBoard
                    }
                }
            };
//            
//            var shipsMenu = new Menu
//            {
//                Title = "Ships",
//                MenuItems = new List<MenuItem>
//                {
//                    new MenuItem
//                    {
//                        Shortcut = "1",
//                        Description = "Add ships menu",
//                        CommandToExecute = ui => ui.RunMenu(addShipsToBoardMenu)
//                    },
//                    new MenuItem
//                    {
//                        Shortcut = "2",
//                        Description = "Delete ships menu",
//                        CommandToExecute = ui => ui.RunMenu(deleteShipFromBoardMenu)
//                    }
//                }
//            };
            var playerMenu = new Menu
            {
                Title = "' menu",
                NameInTitle = Game.CurrentPlayer.Name,
                MenuItems = new List<MenuItem>
                {
                    new MenuItem
                    {
                        Shortcut = "1",
                        Description = "Add ships menu",
                        CommandToExecute = () => Game.RunMenu(addShipsToBoardMenu)
                    },
                    new MenuItem
                    {
                        Shortcut = "2",
                        Description = "Delete ships menu",
                        CommandToExecute = () => Game.RunMenu(deleteShipFromBoardMenu)
                    },
                    new MenuItem
                    {
                        Shortcut = "3",
                        Description = "Change your name",
                        CommandToExecute = Game.ChangePlayersName
                    },
                    new MenuItem
                    {
                        Shortcut = "4",
                        Description = "Finished!",
                        CommandToExecute = () =>
                        {
                            Game.SetPlayerReady();
                            PlayersReady++;
                            return "FINISHED";
                        }
                    }
                }
            };
            
            var newGameMenu = new Menu
            {
                Title = "New game",
                MenuItems = new List<MenuItem>
                {
                    new MenuItem
                    {
                        Shortcut = "1",
                        Description = "Set game rules",
                        CommandToExecute = () => Game.RunMenu(rulesMenu)
                    },
                    new MenuItem
                    {
                        Shortcut = "2",
                        Description = "Single player",
                        CommandToExecute = () =>
                        {
                            Game.SetPlayerNotReady();
                            Game.SetSelectedMode("SP");    //TODO: Enumiks teha parameeter
                            return Game.RunMenu(playerMenu);
                        }
                    },
                    new MenuItem
                    {
                        Shortcut = "3",
                        Description = "Multi player",
                        CommandToExecute = () =>
                        {
                            Game.SetSelectedMode("MP");
                            
                            Game.SetPlayerNotReady();
                            var selected = Game.RunMenu(playerMenu);
                            
                            Game.SwitchCurrentPlayer();
                            Game.SetPlayerNotReady();
                            var selected2 = Game.RunMenu(playerMenu);
                            return "";    //TODO: maybe do sth with selected values
                        }
                    },
                    new MenuItem
                    {
                        Shortcut = "4",
                        Description = "Start game",
                        CommandToExecute = () =>
                        {
                            var ready = Game.CheckIfCanStartGame();
                            if (!ready)
                            {
                                Game.Alert("Please place all ships on the board");
                                return "";
                            }
                            else
                            {
                                Game.RunMenu(inGameMenu);  //Game starts and ends here, TODO: reset all objects maybe
                                return "RETURN TO MAIN";
                            }
                        }
                    }
                }
            };


            var mainMenu = new Menu
            {
                Title = "Main menu",
                MenuItems = new List<MenuItem>
                {
                    new MenuItem
                    {
                        Shortcut = "1",
                        Description = "New game",
                        CommandToExecute = () => Game.RunMenu(newGameMenu)
                    },
                    new MenuItem
                    {
                        Shortcut = "2",
                        Description = "Load game",
                        CommandToExecute = () =>
                        {
                            Game.LoadGame();
                            return Game.RunMenu(inGameMenu);
                        }
                    },
                    new MenuItem
                    {
                        Shortcut = "3",
                        Description = "Replay finished game",
                        CommandToExecute = Game.ReaplayGame
                    }
                }
                
            };
            
            return mainMenu;
        }
        
        
    }
}