using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDDungeonCardSpecificCombat : DDDungeonCardEncounter
{
    [Header("Specific Combat")] [SerializeField]
    private DDCombatEnemySetup enemySetup;

    [SerializeField] private DDEnemyBase combatEndsWhenThisDies;

    public override void StartEncounter(ref HashSet<string> usedSetups)
    {
        SpawnEnemies(enemySetup);
    }

    public override bool ShouldEndEarly()
    {
        if (!combatEndsWhenThisDies)
        {
            return false;
        }

        bool enemyFound = false;

        List<DDEnemyOnBoard> allEnemies = DDGamePlaySingletonHolder.Instance.Encounter.AllEnemies;

        foreach (DDEnemyOnBoard eob in allEnemies)
        {
            if (eob.CurrentEnemy == combatEndsWhenThisDies)
            {
                enemyFound = true;
                break;
            }
        }

        return !enemyFound;
    }
}