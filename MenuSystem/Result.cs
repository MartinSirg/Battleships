namespace MenuSystem
{
    public enum Result
    {
        None,
        AllShipsDeleted,
        PlayerReady,
        PlayerNotReady,
        GameStarted,
        FillLoadGame,
        FillReplayGame,
        ChangedMenu,
        QuitToMain,
        NoPreviousMenuFound,
        ReturnToPreviousMenu,
        GameOver,
        SuccessfulBombing,
        SuccessfulBombings,
        ComputerWon,
        NoSuchTile,
        TileAlreadyBombed,
        GameSaved,
        SaveUnfinishedGame
    }
}