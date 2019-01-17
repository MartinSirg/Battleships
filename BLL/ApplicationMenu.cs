using System.Collections.Generic;
using Domain;
using MenuSystem;

namespace BLL
{
    public class ApplicationMenu
    {
        public Game Game { get; set; }
        public Menu MainMenu { get; set; }
        public Menu InGameMenu { get; set; }

        public Menu ReplayMenu { get; set; }


        public ApplicationMenu(Game game)
        {
            Game = game;
            MainMenu = GetMain();
        }

        private Menu GetMain()
        {
            var shipsAndBombings = new Menu
            {
                Title = "CHANGE ME!!!",
                TitleWithName = "PLAYER_NAME's ships",
                DisplayBefore = Display.ShipsAndBombings,
                MenuItems = new List<MenuItem>()
            };
            
            var inGameMenu = new Menu
            {
                Title = "CHANGE ME!!!",
                Previous = null,
                TitleWithName = "PLAYER_NAME's turn",
                DisplayBefore = Display.Bombings,
                MenuItems = new List<MenuItem>
                {
                    new MenuItem
                    {
                        Id = 1,
                        Shortcut = "1", 
                        Description = "Bomb location",
                        GetCommand = () => Command.BombLocation
                    },
                    new MenuItem
                    {
                        Id = 2,
                        Shortcut = "2", 
                        Description = "Show my ships and bombings",
                        GetCommand = () =>
                        {
                            Game.ChangeMenu(shipsAndBombings);
                            return Command.None;
                        }
                    },
                    new MenuItem
                    {
                        Id = 3,
                        Shortcut = "9",
                        Description = "Save and Exit",
                        GetCommand = () => Command.SaveUnfinishedGame
                    }
//                    new MenuItem
//                    {
//                        Id = 4,
//                        Shortcut = "8",
//                        Description = "Exit without saving"
//                        //CommandToExecute added after mainMenu declaration
//                    }
                }
            };
            InGameMenu = inGameMenu;
            //NO previous menu item here
            
            var customShipsMenu = new Menu
            {
                Title = "Customize your ships",
                DisplayBefore = Display.ShipRules,
                MenuItems = new List<MenuItem>
                
                {
                    new MenuItem
                    {
                        Id = 5,
                        Shortcut = "1",
                        Description = "Add ships to rules",
                        GetCommand = () => Command.AddShipToRules
                    },
                    new MenuItem
                    {
                        Id = 6,
                        Shortcut = "2",
                        Description = "Edit ship",
                        GetCommand = () => Command.EditShipInRules
                    },
                    new MenuItem
                    {
                        Id = 7,
                        Shortcut = "3",
                        Description = "Delete ship",
                        GetCommand = () => Command.DeleteShipFromRules
                    }
                    
                }
            };

            var customBoardMenu = new Menu
            {
                Title = "Customize the game board",
                DisplayBefore = Display.BoardRules,
                MenuItems = new List<MenuItem>
                {
                    new MenuItem
                    {
                        Id = 8,
                        Shortcut = "1",
                        Description = "Set ships can touch rule.",
                        GetCommand = () => Command.EditShipsCanTouchRule
                    },
                    new MenuItem
                    {
                        Id = 9,
                        Shortcut = "2",
                        Description = "Set board cols",
                        GetCommand = () => Command.EditBoardWidth
                    },
                    new MenuItem
                    {
                        Id = 10,
                        Shortcut = "3",
                        Description = "Set board rows",
                        GetCommand = () => Command.EditBoardHeight
                    }
                }
            };
            
            var customRulesMenu = new Menu
            {
                Title = "Custom rules",
                DisplayBefore = Display.CurrentRules,
                MenuItems = new List<MenuItem>
                {
                    new MenuItem
                    {
                        Id = 11,
                        Shortcut = "1",
                        Description = "Ships",
                        GetCommand = () =>
                        {
                            Game.ChangeMenu(customShipsMenu);
                            return Command.None;
                        }
                    },
                    new MenuItem
                    {
                        Id = 12,
                        Shortcut = "2",
                        Description = "Board",
                        GetCommand = () =>
                        {
                            Game.ChangeMenu(customBoardMenu);
                            return Command.None;
                        }
                    },
//                    new MenuItem
//                    {
//                        Id = 13,
//                        Shortcut = "3",
//                        Description = "Set name",
//                        GetCommand = () => Command.SetRulesetName
//                    }
                }
            };
            
            var rulesMenu = new Menu
            {
                Title ="Rules",
                DisplayBefore = Display.CurrentRules,
                MenuItems = new List<MenuItem>
                {
                    new MenuItem
                    {
                        Id = 14,
                        Shortcut = "1",
                        Description = "Set custom rules",
                        GetCommand = () =>
                        {
                            Game.SetCustomRules();
                            Game.ChangeMenu(customRulesMenu);
                            return Command.None;
                        }
                    },
                    new MenuItem
                    {
                        Id = 15,
                        Shortcut = "2",
                        Description = "Set standard Rules",
                        GetCommand = () => Command.SetStandardRules
                    }
                }
            };
            
            var addShipsToBoardMenu = new Menu
            {
                Title = "CHANGE ME!!!",
                TitleWithName = "Add ship to PLAYER_NAME's board",
                DisplayBefore = Display.CurrentShipsAdding,
                MenuItems = new List<MenuItem>
                {
                    new MenuItem
                    {
                        Id = 16,
                        Shortcut = "1",
                        Description = "Select ship start point(example: D6)",
                        GetCommand = () => Command.SetShipStartTile
                    },
                    new MenuItem
                    {
                        Id = 17,
                        Shortcut = "2",
                        Description = "Select ship end point(example: D7)",
                        GetCommand = () => Command.SetShipEndTile
                    },
                    new MenuItem
                    {
                        Id = 18,
                        Shortcut = "3",
                        Description = "Confirm ship placement",
                        GetCommand = () => Command.PlaceShipOnBoard
                    },
                    new MenuItem
                    {
                        Id = 19,
                        Shortcut = "9",
                        Description = "Generate random board",
                        GetCommand = () =>
                        {
                            Game.GenerateRandomBoard(Game.CurrentPlayer);
                            return Command.GenerateRandomBoard;
                        }
                    }
                    
                }
            };
            
            var deleteShipFromBoardMenu = new Menu
            {
                Title = "Delete ship",
                DisplayBefore = Display.CurrentShipsDeleting,
                MenuItems = new List<MenuItem>
                {
                    new MenuItem
                    {
                        Id = 20,
                        Shortcut = "1",
                        Description = "Select occupying tile(example: D6)",
                        GetCommand = () => Command.SetTileOfDeleteableShip
                    },
                    new MenuItem
                    {
                        Id = 21,
                        Shortcut = "2",
                        Description = "Confirm ship deletion",
                        GetCommand = () => Command.DeleteShipFromBoard
                    },
                    new MenuItem
                    {
                        Id = 22,
                        Shortcut = "9",
                        Description = "Delete all ships",
                        GetCommand = ()=>
                        {
                            Game.CurrentPlayer.Board = new Board(Game.Rules.BoardRows, Game.Rules.BoardCols,
                                    Game.Rules.CanShipsTouch);
                            return Command.Previous;
                        }
                    }
                }
            };

            var playerMenu = new Menu
            {
                Title = "CHANGE ME!!!!",
                TitleWithName = "PLAYER_NAME's menu",
                DisplayBefore = Display.CurrentShips,
                MenuItems = new List<MenuItem>
                {
                    new MenuItem
                    {
                        Id = 23,
                        Shortcut = "1",
                        Description = "Add ships menu",
                        GetCommand = () =>
                        {
                            Game.ClearCurrentHighlights();
                            addShipsToBoardMenu.Title = $"{Game.CurrentPlayer.Name}'s add ships menu";
                            Game.ChangeMenu(addShipsToBoardMenu);
                            return Command.None;
                        }
                    },
                    new MenuItem
                    {
                        Id = 24,
                        Shortcut = "2",
                        Description = "Delete ships menu",
                        GetCommand = () =>
                        {
                            Game.ClearCurrentHighlights();
                            deleteShipFromBoardMenu.Title = $"{Game.CurrentPlayer.Name}'s delete ship menu";
                            Game.ChangeMenu(deleteShipFromBoardMenu);
                            return Command.None;
                        }
                    },
                    new MenuItem
                    {
                        Id = 25,
                        Shortcut = "3",
                        Description = "Change your name",
                        GetCommand = () => Command.ChangePlayersName
                    },
                    new MenuItem
                    {
                        Id = 26,
                        Shortcut = "4",
                        Description = "Ready Up!",
                        GetCommand = () =>
                        {
                            Game.SetCurrentPlayerReady();
                            return Command.Previous;
                        }
                    }
                }
            };

            var multiPlayerMenu = new Menu
            {
                Title = "Multiplayer Menu",
                MenuItems = new List<MenuItem>
                {
                    new MenuItem
                    {
                        Id = 27,
                        Shortcut = "1",
                        Description = "Player 1 menu",
                        GetCommand = () =>
                        {
                            Game.CurrentPlayer = Game.Player1;
                            Game.TargetPlayer = Game.Player2;
                            playerMenu.Title = $"{Game.CurrentPlayer.Name}'s menu";
                            Game.ChangeMenu(playerMenu);
                            return Command.None;
                        }
                    },
                    new MenuItem
                    {
                        Id = 28,
                        Shortcut = "2",
                        Description = "Player 2 menu",
                        GetCommand = () =>
                        {
                            Game.CurrentPlayer = Game.Player2;
                            Game.TargetPlayer = Game.Player1;
                            playerMenu.Title = $"{Game.CurrentPlayer.Name}'s menu";
                            Game.ChangeMenu(playerMenu);
                            return Command.None;
                        }
                    }
                },
                Previous = new MenuItem
                {
                    Shortcut = Menu.ExitString,
                    Description = "Previous menu",
                    GetCommand = () =>
                    {
                        Game.CurrentPlayer = Game.Player1;
                        Game.TargetPlayer = Game.Player2;
                        return Command.Previous;
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
                        Id = 29,
                        Shortcut = "1",
                        Description = "Set game rules",
                        GetCommand = () =>
                        {
                            Game.ChangeMenu(rulesMenu);
                            return Command.None;
                        }
                    },
                    new MenuItem
                    {
                        Id = 30,
                        Shortcut = "2",
                        Description = "Single player",
                        GetCommand = () =>
                        {
                            if (Game.SelectedMode.Equals("MP"))
                            {
                                Game.ResetTargetPlayer("Computer");
                            }
                            playerMenu.Title = $"{Game.CurrentPlayer.Name}'s menu";
                            Game.SetCurrentPlayerNotReady();
                            Game.SetSelectedMode("SP");
                            Game.GenerateOpponent();
                            Game.ChangeMenu(playerMenu);
                            return Command.None;
                        }
                    },
                    new MenuItem
                    {
                        Id = 31,
                        Shortcut = "3",
                        Description = "Multi player",
                        GetCommand = () =>
                        {
                            if (Game.SelectedMode.Equals("SP"))
                            {
                                Game.ResetTargetPlayer(newTargetName: "Player 2");
                            }
                            Game.SetSelectedMode("MP");
                            Game.SetCurrentPlayerNotReady();
                            Game.ChangeMenu(multiPlayerMenu);
                            return Command.None;
                        }
                    },
                    new MenuItem
                    {
                        Id = 32,
                        Shortcut = "4",
                        Description = "Start game",
                        GetCommand = () =>
                        {
                            var ready = Game.CheckIfCanStartGame();
                            if (!ready)
                            {
                                return Command.CantStartGame;
                            }

                            Game.CurrentPlayer = Game.Player1;
                            Game.TargetPlayer = Game.Player2;
                            
                            inGameMenu.Title = $"{Game.CurrentPlayer.Name}'s turn";
                            Game.ChangeMenu(inGameMenu);  //Game starts and ends here,
                            return Command.None;
                        }
                    }
                }
            };
            
            var loadMenu = new Menu
            {
                Title = "Load game menu",
                DisplayBefore = Display.UnfinishedGames,
                //This list is filled after selecting it from the main menu
                MenuItems = new List<MenuItem>()
            };
            
            var replayMenu= new Menu
            {
                Title = "Replay finished game menu",
                DisplayBefore = Display.FinishedGames,
                //This list is filled after selecting it from the main menu
                MenuItems = new List<MenuItem>()
            };

            var mainMenu = new Menu
            {
                Title = "Main menu",
                Previous = null,
                MenuItems = new List<MenuItem>
                {
                    new MenuItem
                    {
                        Id = 33,
                        Shortcut = "1",
                        Description = "New game",
                        GetCommand = () =>
                        {
                            Game.NewGame();
                            Game.ChangeMenu(newGameMenu);
                            return Command.None;
                        }
                    },
                    new MenuItem
                    {
                        Id = 34,
                        Shortcut = "2",
                        Description = "Load game",
                        GetCommand = () =>
                        {
                            Game.ChangeMenu(loadMenu);
                            return Command.FillLoadMenu;
                        }
                    },
                    new MenuItem
                    {
                        Id = 35,
                        Shortcut = "3",
                        Description = "Replay finished game",
                        GetCommand = () =>
                        {
                            Game.ChangeMenu(replayMenu);
                            return Command.FillReplayMenu;
                        }
                    }
                }
            };

            inGameMenu.Previous = new MenuItem
            {
                Shortcut = Menu.ExitString,
                Description = "Exit to main menu without saving",
                GetCommand = () =>
                {
                    Game.ResetAll();
                    Game.MenuStack.Clear();
                    Game.CurrentMenu = mainMenu;

                    return Command.None;
                }
            };
                
            ReplayMenu =  new Menu //Applicable in web UI
            {
                Title = $"{Game.Player1.Name} vs {Game.Player2.Name}",
                DisplayBefore = Display.Replay,
                Previous = new MenuItem
                {
                    Shortcut = Menu.ExitString,
                    Description = "Exit to main menu",
                    GetCommand = () =>
                    {
                        Game.ResetAll();
                        Game.MenuStack.Clear();
                        Game.CurrentMenu = mainMenu;

                        return Command.None;
                    }
                },
                MenuItems = new List<MenuItem>()
            };
            
            return mainMenu;
        }

    }
}