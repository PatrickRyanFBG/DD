using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DDCombatEnemySetup
{
    [SerializeField]
    private int amount;
    public int Amount => amount;

    [SerializeField]
    private DDEnemyBase enemy;
    public DDEnemyBase Enemy => enemy;
}

[System.Serializable]
public class DDDungeonCombatData
{
    [SerializeField]
    private List<DDCombatEnemySetup> enemies;

    public List<DDCombatEnemySetup> Enemies { get => enemies; set => enemies = value; }
}