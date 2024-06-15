using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DDEnemy_Rat : DDEnemyBase
{
    [SerializeField]
    private int damage;

    public override List<DDEnemyActionBase> CalculateActions(int number, DDEnemyOnBoard actingEnemy)
    {
        List<DDEnemyActionBase> actions = new List<DDEnemyActionBase>(number);

        EMoveDirection randomDirection = (EMoveDirection)Random.Range(0, 4);

        actions.Add(new DDEnemyAction_Move(randomDirection));
        actions.Add(new DDEnemyAction_Attack(damage));

        return actions;
    }
}
