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
            EMoveDirection randomDirection = (EMoveDirection)Random.Range(0, 4);
            actions.Add(new DDEnemyAction_Move(randomDirection));

            actions.Add(buffAction);
        }
        else
        {
            if (Random.Range(0, 4) <= 2)
            {
                EMoveDirection randomDirection = (EMoveDirection)Random.Range(0, 4);
                actions.Add(new DDEnemyAction_Move(randomDirection));
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