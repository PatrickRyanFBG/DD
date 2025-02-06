using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDEnemy_TurretWound : DDEnemyBase
{
    [SerializeField]
    private int damage;

    [SerializeField]
    private int amountOfWounds;

    [SerializeField]
    private DDCardBase woundCard;

    public override List<DDEnemyActionBase> CalculateActions(int number, DDEnemyOnBoard actingEnemy)
    {
        List<DDEnemyActionBase> actions = new List<DDEnemyActionBase>(number);

        actions.Add(new DDEnemyAction_Attack(damage));
        actions.Add(new DDEnemyAction_AddCardTo(amountOfWounds, woundCard, ECardLocation.Discard));
        
        return actions;
    }
}
