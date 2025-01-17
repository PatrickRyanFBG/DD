public enum EMoveDirection
{
    Up,
    Right,
    Down,
    Left
}

public enum EDungeonCardType
{
    Encounter,
    Event,
    Leisure,
    Shop
}

public enum EDungeonCardExtra
{
    Key,
}

public enum EEncounterPhase
{
    EncounterStart,
    MonsterForecast,
    PlayersTurn,
    PlayersEndTurn,
    MonstersAct,
    EncounterEnd,
}

public enum EDungeonPhase
{
    DungeonStart,
    DungeonCardSelection,
    Event,
    Leisure,
    Encounter,
    Shop,
    PlayerCardSelection,
    DungeonLost,
    DungeonWon,
}

public enum ECardType
{
    Utility,
    Action,
    Offensive,
    Defensive,
    Wound,
    Curses
}

public enum ECardLocation
{ 
    Deck,
    Hard,
    Discard
}

public enum ECardFinishing
{
    None,
    Fleeting
}
