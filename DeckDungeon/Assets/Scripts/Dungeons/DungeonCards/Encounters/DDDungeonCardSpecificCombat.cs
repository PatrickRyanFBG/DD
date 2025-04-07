using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDDungeonCardSpecificCombat : DDDungeonCardEncounter
{
    [Header("Specific Combat")] [SerializeField]
    private DDCombatEnemySetup enemySetup;

    public override void StartEncounter()
    {
        SpawnEnemies(enemySetup);
    }
}