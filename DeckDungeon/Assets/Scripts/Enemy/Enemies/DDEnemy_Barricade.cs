using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DDEnemy_Barricade : DDEnemyBase
{
    public override List<DDEnemyActionBase> CalculateActions(int number, DDEnemyOnBoard actingEnemy)
    {
        List<DDEnemyActionBase> actions = new List<DDEnemyActionBase>(number);
        return actions;
    }
}
