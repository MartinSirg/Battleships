namespace MenuSystem
{
    public enum Command : int
    {
        Previous,                //Call Game.Previous()
        BombLocation,            //Call Game.BombLocation()
        None,                    //Just continue
        SaveUnfinishedGame,
        EditShipInRules,
        AddShipToRules,
        DeleteShipFromRules,
        EditShipsCanTouchRule,
        EditBoardWidth,
        EditBoardHeight,
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
        CantStartGame,
        GenerateRandomBoard,
        LoadReplay
    }
}