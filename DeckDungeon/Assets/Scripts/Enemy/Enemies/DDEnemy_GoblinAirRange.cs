using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDEnemy_GoblinAirRange : DDEnemyBase
{
    [SerializeField]
    private int damage;

    [SerializeField]
    private int armorBuff;

    public override List<DDEnemyActionBase> CalculateActions(int number, DDEnemyOnBoard actingEnemy)
    {
        List<DDEnemyActionBase> actions = new List<DDEnemyActionBase>(number);

        DDEnemyActionBase buffArmorAction = null;

        if (actingEnemy.GetAffixValue(EAffixType.Armor) < armorBuff)
        {
            buffArmorAction = new DDEnemyAction_ModifyAffix(EAffixType.Armor, armorBuff, true);
        }

        if (buffArmorAction != null)
        {
            actions.Add(buffArmorAction);
        }
        else
        {
            actingEnemy.GenericRangeAttackActions(ref actions, damage);
        }

        return actions;
    }
}
