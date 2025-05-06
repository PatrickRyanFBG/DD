using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class DDEnemyLibrary : MonoBehaviour
{
    public Enemies EnemyDictionary;
    public SharedActionIcons SharedActionIconDictionary;
}

[System.Serializable]
public class Enemies
{
    public DDEnemyGoblinMelee GoblinMelee;
    public DDEnemyBush Bush;
}

[System.Serializable]
public class SharedActionIcons
{
    public Texture MoveUp;
    public Texture MoveRight;
    public Texture MoveDown;
    public Texture MoveLeft;

    public Texture AttackMelee;
    public Texture AttackRanged;
    public Texture AttackPure;
    public Texture AttackExplode;
    public Texture AttackArmorBased;

    public Texture ActionHeal;
    public Texture ActionGainDexterity;
    public Texture ActionGainArmor;
    public Texture ActionAddCard;
    public Texture ActionLockCard;
    public Texture ActionArmorAbsorb;
    public Texture ActionHealPerBleed;

    public Texture ActionHide;
    public Texture ActionReveal;
}