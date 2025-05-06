using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDDungeonCardGenericCombat : DDDungeonCardEncounter
{
    [Header("Generic Combat")] [SerializeField] private DDDungeonCombatData data;

    // I need to put this data somewhere else to avoid playing against the same combat atleast twice in a row
    // But it cant be here because this isn't shared between DIFFERENT (camp and campKey) cards.
    public override void StartEncounter(ref HashSet<string> usedSetups)
    {
        DDCombatEnemySetup enemySetup = data.GetRandomEnemySetup(DDGamePlaySingletonHolder.Instance.Dungeon.DungeonStats.CombatTier, usedSetups);
        
        usedSetups.Add(enemySetup.GUID);
        
        SpawnEnemies(enemySetup);
    }
}