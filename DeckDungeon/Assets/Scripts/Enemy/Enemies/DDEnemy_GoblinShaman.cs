using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDEnemy_GoblinShaman : DDEnemyBase
{
    [SerializeField]
    private int damage;

    [SerializeField]
    private int healAmount;

    public override List<DDEnemyActionBase> CalculateActions(int number, DDEnemyOnBoard actingEnemy)
    {
        List<DDEnemyActionBase> actions = new List<DDEnemyActionBase>(number);

        List<DDEnemyOnBoard> allEnemies = new List<DDEnemyOnBoard>();
        SingletonHolder.Instance.Board.GetAllEnemies(ref allEnemies);

        DDEnemyActionBase healAction = null;

        for (int i = 0; i < allEnemies.Count; i++)
        {
            DDEnemyOnBoard eob = allEnemies[i];
            if (eob != null && eob.IsDamaged())
            {
                healAction = new DDEnemyAction_HealAlly(healAmount, eob.CurrentLocaton.Coord);
            }
        }

        if (healAction != null)
        {
            actions.Add(healAction);
        }
        else
        {
            EMoveDirection randomDirection = (EMoveDirection)Random.Range(0, 4);
            actions.Add(new DDEnemyAction_Move(randomDirection));
            actions.Add(new DDEnemyAction_Attack(damage));
        }

        return actions;
    }
}
