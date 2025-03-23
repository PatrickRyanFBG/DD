using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDEnemyBush : DDEnemyBase
{
    public override List<DDEnemyActionBase> CalculateActions(int number, DDEnemyOnBoard actingEnemy)
    {
        List<DDEnemyActionBase> actions = new List<DDEnemyActionBase>();
        DDEnemyActionEmpty empty = new DDEnemyActionEmpty();
        actions.Add(empty);
        return actions;
    }

    public override IEnumerator OnDeath()
    {
        // Spawn
        yield return null;
        
    }
}
