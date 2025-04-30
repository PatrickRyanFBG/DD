using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDEnemyGoblinAirRange : DDEnemyBase
{
    [SerializeField] private int damage;

    [SerializeField] private int armorBuff;

    public override List<DDEnemyActionBase> CalculateActions(int number, DDEnemyOnBoard actingEnemy)
    {
        List<DDEnemyActionBase> actions = new List<DDEnemyActionBase>(number);

        DDEnemyActionBase buffArmorAction = null;

        if (actingEnemy.GetAffixValue(EAffixType.Armor) < armorBuff)
        {
            buffArmorAction = new DDEnemyActionModifyAffix(EAffixType.Armor, armorBuff, true);
        }

        if (buffArmorAction != null)
        {
            actions.Add(buffArmorAction);
        }
        else
        {
            actingEnemy.GenericRangeAttackActions(ref actions, new DDEnemyActionAttack(damage, actingEnemy.CurrentEnemy));
        }

        return actions;
    }
}