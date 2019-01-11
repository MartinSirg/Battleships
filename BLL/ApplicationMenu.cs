using System.Collections.Generic;
using Domain;
using MenuSystem;

namespace BLL
{
    public class ApplicationMenu
    {
        public GameNew Game { get; set; }
        public Menu MainMenu { get; set; }

        public ApplicationMenu(GameNew game)
        {
            Game = game;
            
            
        }

        public Menu GetMain()
        {
            var shipsAndbombings = new Menu
            {
                Title = "CHANGE ME!!!",
                TitleWithName = "PLAYER_NAME's ships",
                DisplayBefore = Display.ShipsAndBombings
            };
            
            
            var inGameMenu = new Menu
            {
                Title = "CHANGE ME!!!",
                Previous = null,
                TitleWithName = "PLAYER_NAME's turn",
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
                        CommandToExecute = s => Game.ChangeMenu(shipsAndbombings)
                    },
                    new MenuItem
                    {
                        Shortcut = "9",
                        Description = "Save and Exit",
                        CommandToExecute = Game.SaveGame
                    },
                    new MenuItem
                    {
                        Shortcut = "8",
                        Description = "Exit without saving"
                        //CommandToExecute added after mainMenu declaration
                    }
                }
            };
            
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
                DisplayBefore = Display.BoardRules,    //TODO: ItemDisplayEnum
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
                        Description = "Set board cols",
                        CommandToExecute = Game.EditBoardWidth
                    },
                    new MenuItem
                    {
                        Shortcut = "3",
                        Description = "Set board rows",
                        CommandToExecute = Game.EditBoardHeight
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
                        CommandToExecute = s => Game.ChangeMenu(customShipsMenu)
                    },
                    new MenuItem
                    {
                        Shortcut = "2",
                        Description = "Board",
                        CommandToExecute = s => Game.ChangeMenu(customBoardMenu)
                    },
                    new MenuItem
                    {
                        Shortcut = "3",
                        Description = "Set name",
                        CommandToExecute = Game.SetRulesetName
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
                        CommandToExecute = s =>
                        {
                            if (Game.Rules.Name.Equals("Standard rules"))
                            {
                                Game.Rules = (Rules) Game.Rules.Clone();
                                Game.Rules.Name = "Custom rules";
                            }
                            return Game.ChangeMenu(customRulesMenu);
                        }
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
                Title = "CHANGE ME!!!",
                TitleWithName = "Add ship to PLAYER_NAME's board",
                DisplayBefore = Display.CurrentAndAvailableShips,
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
                    },
                    new MenuItem
                    {
                        Shortcut = "9",
                        Description = "Generate random board",
                        CommandToExecute = s => Game.GenerateRandomBoard(Game.CurrentPlayer)
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
                        CommandToExecute = Game.GetTileOfDeleteableShip
                    },
                    new MenuItem
                    {
                        Shortcut = "2",
                        Description = "Confirm ship deletion",
                        CommandToExecute = Game.DeleteShipFromBoard
                    },
                    new MenuItem
                    {
                        Shortcut = "9",
                        Description = "Delete all ships",
                        CommandToExecute = s =>
                        {
                            Game.CurrentPlayer.Board = new Board(Game.Rules.BoardRows, Game.Rules.BoardCols,
                                    Game.Rules.CanShipsTouch);
                            return Result.AllShipsDeleted;
                        }
                    }
                }
            };

            var playerMenu = new Menu
            {
                Title = "CHANGE ME!!!!",
                TitleWithName = "PLAYER_NAME's menu",
                MenuItems = new List<MenuItem>
                {
                    new MenuItem
                    {
                        Shortcut = "1",
                        Description = "Add ships menu",
                        CommandToExecute = s =>
                        {
                            Game.ClearCurrentHighlights();
                            addShipsToBoardMenu.Title = $"{Game.CurrentPlayer.Name}'s add ships menu";
                            return Game.ChangeMenu(addShipsToBoardMenu);
                        }
                    },
                    new MenuItem
                    {
                        Shortcut = "2",
                        Description = "Delete ships menu",
                        CommandToExecute = s =>
                        {
                            Game.ClearCurrentHighlights();
                            deleteShipFromBoardMenu.Title = $"{Game.CurrentPlayer.Name}'s delete ship menu";
                            return Game.ChangeMenu(deleteShipFromBoardMenu);
                        }
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
                        CommandToExecute = s =>
                        {
                            Game.SetPlayerReady();
                            //TODO: Change to previous menu, implement stack menu system in GAME
                            return Result.PlayerReady;
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
                        CommandToExecute = s =>
                        {
                            Game.CurrentPlayer = Game.Player1;
                            Game.TargetPlayer = Game.Player2;
                            playerMenu.Title = $"{Game.CurrentPlayer.Name}'s menu";
                            return Game.ChangeMenu(playerMenu);
                        }
                    },
                    new MenuItem
                    {
                        Shortcut = "2",
                        Description = "Player 2 menu",
                        CommandToExecute = s =>
                        {
                            Game.CurrentPlayer = Game.Player2;
                            Game.TargetPlayer = Game.Player1;
                            playerMenu.Title = $"{Game.CurrentPlayer.Name}'s menu";
                            return Game.ChangeMenu(playerMenu);
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
                        CommandToExecute = s => Game.ChangeMenu(rulesMenu)
                    },
                    new MenuItem
                    {
                        Shortcut = "2",
                        Description = "Single player",
                        CommandToExecute = s =>
                        {
                            if (Game.SelectedMode.Equals("MP"))
                            {
                                Game.ResetTargetPlayer("Computer");
                            }
                            playerMenu.Title = $"{Game.CurrentPlayer.Name}'s menu";
                            Game.SetPlayerNotReady();
                            Game.SetSelectedMode("SP");    //TODO: Enumiks teha parameeter
                            Game.GenerateOpponent();
                            return Game.ChangeMenu(playerMenu);
                        }
                    },
                    new MenuItem
                    {
                        Shortcut = "3",
                        Description = "Multi player",
                        CommandToExecute = s =>
                        {
                            if (Game.SelectedMode.Equals("SP"))
                            {
                                Game.ResetTargetPlayer(newTargetName: "Player 2");
                            }
                            Game.SetSelectedMode("MP");
                            Game.SetPlayerNotReady();
                            return Game.ChangeMenu(multiPlayerMenu);                          
                         }
                    },
                    new MenuItem
                    {
                        Shortcut = "4",
                        Description = "Start game",
                        CommandToExecute = s =>
                        {
                            var ready = Game.CheckIfCanStartGame();
                            if (!ready)
                            {
                                return Result.PlayerNotReady;
                            }

                            Game.CurrentPlayer = Game.Player1;
                            Game.TargetPlayer = Game.Player2;
                            
                            inGameMenu.Title = $"{Game.CurrentPlayer.Name}'s turn";
                            Game.ChangeMenu(inGameMenu);  //Game starts and ends here,
                            return Result.GameStarted;
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
                
                MenuItems = new List<MenuItem>
                {
                    new MenuItem
                    {
                        Shortcut = "1",
                        Description = "New game",
                        CommandToExecute = s =>
                        {
                            Game.NewGame();
                            return Game.ChangeMenu(newGameMenu);
                        }
                    },
                    new MenuItem
                    {
                        Shortcut = "2",
                        Description = "Load game",
                        CommandToExecute = s =>
                        {
                            Game.ChangeMenu(loadMenu);
                            return Result.FillLoadGame;
                        }
                    },
                    new MenuItem
                    {
                        Shortcut = "3",
                        Description = "Replay finished game",
                        CommandToExecute = s =>
                        {
                            Game.ChangeMenu(replayMenu);
                            return Result.FillReplayGame;
                        }
                    }
                }
                
            };
            
            inGameMenu.MenuItems
                .Find(item => item.Shortcut.Equals("8"))
                .CommandToExecute = s =>
            {
                Game.ResetAll();
                //TODO: Game.MenuStack empty, push main menu
                Game.CurrentMenu = mainMenu;
                
                return Result.QuitToMain;
            };
            
            return mainMenu;
        }
        
        
    }
}