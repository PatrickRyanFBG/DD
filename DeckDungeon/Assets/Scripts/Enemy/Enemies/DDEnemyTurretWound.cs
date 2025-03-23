using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDEnemyTurretWound : DDEnemyBase
{
    [SerializeField] private int damage;

    [SerializeField] private int amountOfWounds;

    [SerializeField] private DDCardBase woundCard;

    public override List<DDEnemyActionBase> CalculateActions(int number, DDEnemyOnBoard actingEnemy)
    {
        List<DDEnemyActionBase> actions = new List<DDEnemyActionBase>(number);

        actions.Add(new DDEnemyActionAttack(damage));
        actions.Add(new DDEnemyActionAddCardTo(amountOfWounds, woundCard, ECardLocation.Discard));

        return actions;
    }
}