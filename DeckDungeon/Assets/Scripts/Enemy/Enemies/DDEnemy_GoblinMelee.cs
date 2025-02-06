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

        int dexValue = actingEnemy.GetAffixValue(EAffixType.Expertise);

        // If no dex higher chance to buff
        if ((dexValue <= 0 && Random.Range(0, 10) < 5) ||
            (dexValue > 0 && Random.Range(0, 10) < 3))
        {
            buffAction = new DDEnemyAction_ModifyAffix(EAffixType.Expertise, dexGain, false);
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
            actingEnemy.GenericMeleeAttackActions(ref actions, damage);
        }

        return actions;
    }
}