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
    MonsterSpawn,
    MonsterForecast,
    PlayersStartTurn,
    PlayersTurn,
    PlayersEndTurn,
    MonstersStartTurn,
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
    Pure,
    None
}

public enum ECardLocation
{ 
    Deck,
    Hand,
    Discard
}

public enum EPlayerCardFinish
{
    // Space here in case we want some special finishes?
    // Positive
    Energized = 100,
    Serrated,
    Sharp,
    Weighty,
    Explosive,
    Replicating,
    // Neutral
    Fleeting = 1100, 
    Sticky,
    // Negative
    Fragile = 2100,
    Siphon,
}

public enum EPlayerCardFinishImpact
{
    None,
    Positive,
    Neutral,
    Negative,
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
    PrePlayed,
    PostPlayed,
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

public enum EEnemyState
{
    PreSpawn,
    Spawning,
    Alive,
    Dead,
    Dying
}