using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDEnemyLibrary : MonoBehaviour
{
    public Enemies EnemyDictionary;
    public SharedActionIcons SharedActionIconDictionary;
}

[System.Serializable]
public class Enemies
{
    public DDEnemy_GoblinMelee GoblinMelee;
}

[System.Serializable]
public class SharedActionIcons
{
    public Texture Move_Up;
    public Texture Move_Right;
    public Texture Move_Down;
    public Texture Move_Left;

    public Texture Attack_Melee;
    public Texture Attack_Explode;
    public Texture Attack_Heal;

    public Texture Attack_GainDexterity;
}
