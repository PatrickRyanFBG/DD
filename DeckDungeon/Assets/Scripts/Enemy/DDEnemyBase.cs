using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class DDEnemyBase : DDEntityBase
{
}

public static class EnemyHelperExtensions
{
    public static void GenericMeleeAttackActions(this DDEnemyOnBoard actingEnemy, ref List<DDEnemyActionBase> actions, int damage)
    {
        bool shouldMove = false;

        if (DDGamePlaySingletonHolder.Instance.Player.IsLaneArmored(actingEnemy.CurrentLocaton.Coord.x))
        {
            // If this lane is armored we have a 90% chance to move, but 10% chance to double attack
            shouldMove = Random.Range(0, 10) < 9;
        }
        else if (actingEnemy.CurrentLocaton.Coord.y == 0)
        {
            // If the lane is not armored and we are at 0 we have a 40% chance of moving.
            shouldMove = Random.Range(0, 10) < 4;
        }
        else
        {
            // If the lane is not armored, and we are not at goal we have a 70% chance to move.
            shouldMove = Random.Range(0, 10) < 7;
        }

        if (shouldMove)
        {
            DDEnemyAction_Move moveAction = DDEnemyAction_Move.CalculateBestMove(actingEnemy, EMoveDirection.Down, true);
            if (moveAction != null)
            {
                actions.Add(moveAction);
            }
        }
        else
        {
            actions.Add(new DDEnemyAction_Attack(damage));
        }

        actions.Add(new DDEnemyAction_Attack(damage));
    }

    public static void GenericRangeAttackActions(this DDEnemyOnBoard actingEnemy, ref List<DDEnemyActionBase> actions, int damage)
    {
        bool shouldMove = false;

        if (DDGamePlaySingletonHolder.Instance.Player.IsLaneArmored(actingEnemy.CurrentLocaton.Coord.x))
        {
            // If this lane is armored we have a 90% chance to move, but 10% chance to double attack
            shouldMove = Random.Range(0, 10) < 9;
        }
        else if (actingEnemy.CurrentLocaton.Coord.y == 0)
        {
            // If the lane is not armored and we are at 0 we have a 40% chance of moving.
            shouldMove = Random.Range(0, 10) < 4;
        }
        else
        {
            // If the lane is not armored, and we are not at goal we have a 70% chance to move.
            shouldMove = Random.Range(0, 10) < 7;
        }

        if (shouldMove)
        {
            DDEnemyAction_Move moveAction = DDEnemyAction_Move.CalculateBestMove(actingEnemy, EMoveDirection.Up, true);
            if (moveAction != null)
            {
                actions.Add(moveAction);
            }
        }
        else
        {
            actions.Add(new DDEnemyAction_Attack(damage));
        }

        actions.Add(new DDEnemyAction_Attack(damage));
    }
}