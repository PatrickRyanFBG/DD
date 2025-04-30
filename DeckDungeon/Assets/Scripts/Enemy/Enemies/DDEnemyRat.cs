using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DDEnemyRat : DDEnemyBase
{
    [SerializeField] private int damage;

    public override List<DDEnemyActionBase> CalculateActions(int number, DDEnemyOnBoard actingEnemy)
    {
        List<DDEnemyActionBase> actions = new List<DDEnemyActionBase>(number);

        EMoveDirection randomDirection = (EMoveDirection)Random.Range(0, 4);

        actions.Add(new DDEnemyActionMove(randomDirection));
        actions.Add(new DDEnemyActionAttack(damage, actingEnemy.CurrentEnemy));

        return actions;
    }
}