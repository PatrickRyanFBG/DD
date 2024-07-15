using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DDEnemy_GoblinMelee : DDEnemyBase
{
    [SerializeField]
    private int damage;

    [SerializeField]
    private int dexGain;

    public override List<DDEnemyActionBase> CalculateActions(int number, DDEnemyOnBoard actingEnemy)
    {
        List<DDEnemyActionBase> actions = new List<DDEnemyActionBase>(number);

        DDEnemyActionBase buffAction = null;

        // If no dex higher chance to buff
        if ((actingEnemy.Dexterity <= 0 && Random.Range(0, 10) < 5) ||
            (actingEnemy.Dexterity > 0 && Random.Range(0, 10) < 3))
        {
            buffAction = new DDEnemyAction_BuffDexterity(dexGain);
        }

        if (buffAction != null)
        {
            DDEnemyAction_Move moveAction = DDEnemyAction_Move.CalculateBestMove(actingEnemy, EMoveDirection.Down, false);
            if (moveAction != null)
            {
                actions.Add(moveAction);
            }

            actions.Add(buffAction);
        }
        else
        {
            bool shouldMove = false;

            if (SingletonHolder.Instance.Player.IsLaneArmored(actingEnemy.CurrentLocaton.Coord.x))
            {
                // If this lane is armored we have a 90% chance to move, but 10% chance to double attack
                shouldMove = Random.Range(0, 10) < 9;
            }
            else if(actingEnemy.CurrentLocaton.Coord.y == 0)
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

        return actions;
    }
}