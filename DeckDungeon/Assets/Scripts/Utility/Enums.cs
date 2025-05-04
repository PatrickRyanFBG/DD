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

public enum EEncounterType
{
    Normal,
    Elite,
    Unique,
    Boss
}

public enum EDungeonCardExtra
{
    Key,
}

public enum EEncounterPhase
{
    EncounterStart,
    MonsterForecast,
    PlayersStartTurn,
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
    ArtifactSelection,
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

public enum ECardRarity
{
    Common,
    Uncommon,
    Rare,
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

public enum EPlayerCardFinish
{
    None,
    // Positive
    Energized,
    Serrated,
    Sharp,
    Weighty,
    Explosive,
    Sticky,
    END_OF_POSITIVE,
    // Neutral
    Fleeting = 100, 
    // Negative
    Fragile = 200,
    Siphon,
}

public enum EPlayerCardFinishPriority
{
    First,
    None,
    Last
}

public enum EDungeonCardFinish
{
    None,
}

public enum EAffixType
{
    Expertise,
    Armor,
    Vigor,
    Retaliate,
    Bleed,
    Immobile,
}

public enum EAffixOwner
{
    Player,
    Lane,
    Enemy
}

public enum ETargetType
{
    PlayerCard = 6,
    Location = 7,
    Row = 8,
    Column = 9,
    Entire = 10,
    Enemy = 13,
    Player
}

public enum EPlayerCardLifeTime
{
    None,
    Drawn,
    Played,
    EndOfRound,
    Discarded,
}

public enum EDungeonCardExecutionTime
{
    None,
    Drawn,
    Selected,
    ShuffledIn,
    Completed
}

public enum ECombatTier
{
    Intro,
    One,
    Two,
    Three,
}