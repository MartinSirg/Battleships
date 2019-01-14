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

        
        public ApplicationMenu(Game game)
        {
            Game = game;
            MainMenu = GetMain();
        }

        private Menu GetMain()
        {
            var shipsAndbombings = new Menu
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
                        Shortcut = "1", 
                        Description = "Bomb location",
                        GetCommand = () => Command.BombLocation
                    },
                    new MenuItem
                    {
                        Shortcut = "2", 
                        Description = "Show my ships and bombings",
                        GetCommand = () =>
                        {
                            Game.ChangeMenu(shipsAndbombings);
                            return Command.None;
                        }
                    },
                    new MenuItem
                    {
                        Shortcut = "9",
                        Description = "Save and Exit",
                        GetCommand = () => Command.SaveUnfinishedGame
                    },
                    new MenuItem
                    {
                        Shortcut = "8",
                        Description = "Exit without saving"
                        //CommandToExecute added after mainMenu declaration
                    }
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
                        Shortcut = "1",
                        Description = "Add ships to rules",
                        GetCommand = () => Command.AddShipToRules
                    },
                    new MenuItem
                    {
                        Shortcut = "2",
                        Description = "Edit ship",
                        GetCommand = () => Command.EditShipInRules
                    },
                    new MenuItem
                    {
                        Shortcut = "3",
                        Description = "Delete ship",
                        GetCommand = () => Command.DeleteShipInRules
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
                        Shortcut = "1",
                        Description = "Set ships can touch rule.",
                        GetCommand = () => Command.EditShipsCanTouchRule
                    },
                    new MenuItem
                    {
                        Shortcut = "2",
                        Description = "Set board cols",
                        GetCommand = () => Command.EditBoardWidth
                    },
                    new MenuItem
                    {
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
                        Shortcut = "2",
                        Description = "Board",
                        GetCommand = () =>
                        {
                            Game.ChangeMenu(customBoardMenu);
                            return Command.None;
                        }
                    },
                    new MenuItem
                    {
                        Shortcut = "3",
                        Description = "Set name",
                        GetCommand = () => Command.SetRulesetName
                    }
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
                        Shortcut = "1",
                        Description = "Set custom rules",
                        GetCommand = () =>
                        {
                            if (Game.Rules.Name.Equals("Standard rules"))
                            {
                                Game.Rules = (Rules) Game.Rules.Clone();
                                Game.Rules.Name = "Custom rules";
                            }
                            Game.ChangeMenu(customRulesMenu);
                            return Command.None;
                        }
                    },
                    new MenuItem
                    {
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
                        Shortcut = "1",
                        Description = "Select ship start point(example: D6)",
                        GetCommand = () => Command.SetShipStartTile
                    },
                    new MenuItem
                    {
                        Shortcut = "2",
                        Description = "Select ship end point(example: D7)",
                        GetCommand = () => Command.SetShipEndTile
                    },
                    new MenuItem
                    {
                        Shortcut = "3",
                        Description = "Confirm ship placement",
                        GetCommand = () => Command.PlaceShipOnBoard
                    },
                    new MenuItem
                    {
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
                        Shortcut = "1",
                        Description = "Select occupying tile(example: D6)",
                        GetCommand = () => Command.SetTileOfDeleteableShip
                    },
                    new MenuItem
                    {
                        Shortcut = "2",
                        Description = "Confirm ship deletion",
                        GetCommand = () => Command.DeleteShipFromBoard
                    },
                    new MenuItem
                    {
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
                        Shortcut = "3",
                        Description = "Change your name",
                        GetCommand = () => Command.ChangePlayersName
                    },
                    new MenuItem
                    {
                        Shortcut = "4",
                        Description = "Finished!",
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
                }
            };
            multiPlayerMenu.Previous = new MenuItem
            {
                Shortcut = Menu.ExitString,
                Description = "Previous menu",
                GetCommand = () =>
                {
                    Game.CurrentPlayer = Game.Player1;
                    Game.TargetPlayer = Game.Player2;
                    return Command.Previous;
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
                        GetCommand = () =>
                        {
                            Game.ChangeMenu(rulesMenu);
                            return Command.None;
                        }
                    },
                    new MenuItem
                    {
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
                            Game.SetSelectedMode("SP");    //TODO: Enumiks teha parameeter
                            Game.GenerateOpponent();
                            Game.ChangeMenu(playerMenu);
                            return Command.None;
                        }
                    },
                    new MenuItem
                    {
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
            
            inGameMenu.MenuItems
                .Find(item => item.Shortcut.Equals("8"))
                .GetCommand = () =>
            {
                Game.ResetAll();
                Game.MenuStack.Clear();
                Game.CurrentMenu = mainMenu;
                
                return Command.None;
            };
            
            
            return mainMenu;
        }

    }
}