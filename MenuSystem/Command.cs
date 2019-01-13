namespace MenuSystem
{
    public enum Command
    {
        Previous,                //Call Game.Previous()
        BombLocation,            //Call Game.BombLocation()
        None,                    //Just continue
        SaveUnfinishedGame,
        EditShipInRules,
        AddShipToRules,
        DeleteShipInRules,
        EditShipsCanTouchRule,
        EditBoardWidth,
        EditBoardHeight,
        SetRulesetName,
        SetStandardRules,
        GetShipStartTile,
        PlaceShipOnBoard,
        GetShipEndTile,
        GetTileOfDeleteableShip,
        DeleteShipFromBoard,
        ChangePlayersName,
        FillReplayMenu,
        FillLoadMenu,
        LoadGame,
        ReplayGame,
        CantStartGame
    }
}