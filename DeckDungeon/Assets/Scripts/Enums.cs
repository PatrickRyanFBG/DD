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

public enum ERangeType
{
    Melee,
    Ranged,
    None
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

public enum EAffixType
{
    Expertise,
    Armor,
    Vigor,
    Retaliate,
    Bleed
}

public enum ETargetType
{
    PlayerCard = 6,
    Location = 7,
    Row = 8,
    Column = 9,
    EntireOrEmpty = 10,
    Enemy = 13
}