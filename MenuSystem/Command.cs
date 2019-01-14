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
        SetShipStartTile,
        PlaceShipOnBoard,
        SetShipEndTile,
        SetTileOfDeleteableShip,
        DeleteShipFromBoard,
        ChangePlayersName,
        FillReplayMenu,
        FillLoadMenu,
        LoadGame,
        ShowGameReplay,
        CantStartGame,
        GenerateRandomBoard
    }
}