namespace MenuSystem
{
    /**
     * class Game methods can return this enum.
     * It is up to a user interface class to decide what to do after these result have been returned
     */
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
        SaveUnfinishedGame,
        InvalidSize,
        //TODO: maybe just add one for all rule changes i.e. RulesChanged
        ShipRuleAdded,
        InvalidQuantity,
        ShipRuleEdited,
        ShipRuleDeleted,
        InvalidInput,
        ShipsTouchRuleChanged,
        BoardWidthChnaged,
        BoardHeightChnaged,
        RulesNameChanged
    }
}