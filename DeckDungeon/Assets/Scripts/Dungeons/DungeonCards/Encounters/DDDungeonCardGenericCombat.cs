using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDDungeonCardGenericCombat : DDDungeonCardEncounter
{
    [Header("Generic Combat")] [SerializeField] private DDDungeonCombatData data;

    public override void StartEncounter()
    {
        DDCombatEnemySetup enemySetup = null;

        if (DDGamePlaySingletonHolder.Instance.Dungeon.DungeonStats.EncountersCompleted < 3)
        {
            enemySetup = data.GetRandomEnemySetup(ECombatTier.Intro);
        }
        else
        {
            enemySetup = data.GetRandomEnemySetup(ECombatTier.One);
        }
        
        SpawnEnemies(enemySetup);
    }
}