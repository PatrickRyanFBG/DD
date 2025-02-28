using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDEnemyGoblinShaman : DDEnemyBase
{
    [SerializeField] private int damage;

    [SerializeField] private int healAmount;

    public override List<DDEnemyActionBase> CalculateActions(int number, DDEnemyOnBoard actingEnemy)
    {
        List<DDEnemyActionBase> actions = new List<DDEnemyActionBase>(number);

        List<DDEnemyOnBoard> allEnemies = new List<DDEnemyOnBoard>();
        DDGamePlaySingletonHolder.Instance.Board.GetAllEnemies(ref allEnemies);
        allEnemies.Shuffle();

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
            DDEnemyAction_Move moveAction = DDEnemyAction_Move.CalculateBestMove(actingEnemy, EMoveDirection.Up, true);
            if (moveAction != null)
            {
                actions.Add(moveAction);
            }

            actions.Add(new DDEnemyAction_Attack(damage));
        }

        return actions;
    }
}