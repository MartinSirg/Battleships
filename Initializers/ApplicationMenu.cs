
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
                        CommandToExecute = ui => ui.RunMenu(customShipsMenu)
                    },
                    new MenuItem
                    {
                        Shortcut = "2",
                        Description = "Board",
                        CommandToExecute = ui => ui.RunMenu(customBoardMenu)
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
                DisplayBefore = UI.DisplayCurrentRules,
                MenuItems = new List<MenuItem>
                {
                    new MenuItem
                    {
                        Shortcut = "1",
                        Description = "Set custom rules",
                        CommandToExecute = ui => ui.RunMenu(customRulesMenu)
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
                DisplayBefore = ui =>
                {
                    ui.DisplayCurrentShips(Game.CurrentPlayer.Board);
                    ui.DisplayAvailableShips(Game.CurrentPlayer.Board, Game.Rules);
                },
                MenuItems = new List<MenuItem>
                {
                    new MenuItem
                    {
                        Shortcut = "1",
                        Description = "Select ship start point(example: D6)",
                        CommandToExecute = UI.GetShipStartPoint
                    },
                    new MenuItem
                    {
                        Shortcut = "2",
                        Description = "Select ship end point(example: D7)",
                        CommandToExecute = UI.GetShipEndPoint
                    },
                    new MenuItem
                    {
                        Shortcut = "3",
                        Description = "Confirm ship placement",
                        CommandToExecute = Game.DeleteShipFromBoard
                    }
                }
            };
            
            var deleteShipFromBoardMenu = new Menu
            {
                Title = "Delete ship",
                DisplayBefore = ui =>
                {
                    ui.DisplayCurrentShips(Game.CurrentPlayer.Board);
                },
                MenuItems = new List<MenuItem>
                {
                    new MenuItem
                    {
                        Shortcut = "1",
                        Description = "Select occupying tile(example: D6)",
                        CommandToExecute = UI.GetShipStartPoint
                    },
                    new MenuItem
                    {
                        Shortcut = "2",
                        Description = "Confirm ship deletion(example: D7)",
                        CommandToExecute = Game.PlaceShipOnBoard
                    }
                }
            };
            
            var shipsMenu = new Menu
            {
                Title = "Ships",
                MenuItems = new List<MenuItem>
                {
                    new MenuItem
                    {
                        Shortcut = "1",
                        Description = "Add ships menu",
                        CommandToExecute = ui => ui.RunMenu(addShipsToBoardMenu)
                    },
                    new MenuItem
                    {
                        Shortcut = "2",
                        Description = "Delete ships menu",
                        CommandToExecute = ui => ui.RunMenu(deleteShipFromBoardMenu)
                    }
                }
            };
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
                        CommandToExecute = ui => ui.RunMenu(addShipsToBoardMenu)
                    },
                    new MenuItem
                    {
                        Shortcut = "2",
                        Description = "Delete ships menu",
                        CommandToExecute = ui => ui.RunMenu(deleteShipFromBoardMenu)
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
                        CommandToExecute = ui =>
                        {
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
                        CommandToExecute = ui => ui.RunMenu(rulesMenu)
                    },
                    new MenuItem
                    {
                        Shortcut = "2",
                        Description = "Single player",
                        CommandToExecute = ui =>
                        {
                            SinglePlayerSelected = true;
                            MultiPlayerSelected = false;
                            if (PlayersReady > 0) PlayersReady = 0;
                            ui.RunMenu(playerMenu);
                            return "";
                        }
                    },
                    new MenuItem
                    {
                        Shortcut = "3",
                        Description = "Multi player",
                        CommandToExecute = ui =>
                        {
                            SinglePlayerSelected = false;
                            MultiPlayerSelected = true;
                            if (PlayersReady > 0) PlayersReady = 0;
                            ui.RunMenu(playerMenu);
                            Game.SwitchCurrentPlayer();
                            ui.RunMenu(playerMenu);
                            return "";
                        }
                    },
                    new MenuItem
                    {
                        Shortcut = "4",
                        Description = "Start game",
                        CommandToExecute = ui =>
                        {
                            var ready = Game.CheckIfCanStartGame();
                            if (!ready) ui.Alert("Please place all ships on the board");
                            else
                            {
                                PlayersReady = 0;
                                ui.RunMenu(inGameMenu);
                            }
                            return "";
                        }
                    }
                }
            };


            var mainMenu = new Menu
            {
                Title = "Main menu",
                MenuItems = new List<MenuItem>()
                
            };
            
            return rulesMenu;
        }
        
        
    }
}