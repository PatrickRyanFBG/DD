using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDEnemyCreatureMelee : DDEnemyBase
{
    [SerializeField] private int damage;

    [SerializeField] private int armorBuff;

    public override List<DDEnemyActionBase> CalculateActions(int number, DDEnemyOnBoard actingEnemy)
    {
        List<DDEnemyActionBase> actions = new List<DDEnemyActionBase>(number);

        List<DDEnemyOnBoard> allEnemies = new List<DDEnemyOnBoard>();
        DDGamePlaySingletonHolder.Instance.Board.GetAllEnemies(ref allEnemies);

        DDEnemyActionBase buffArmorAction = null;

        // Does one attempt to at buffing a random ally, excluding self
        if (allEnemies.Count > 1)
        {
            DDEnemyOnBoard eob = allEnemies[Random.Range(0, allEnemies.Count)];
            if (eob != null && eob != actingEnemy && eob.GetAffixValue(EAffixType.Armor) < armorBuff)
            {
                buffArmorAction =
                    new DDEnemyActionModifyAffix(EAffixType.Armor, armorBuff, true, eob.CurrentLocaton.Coord);
            }
        }

        if (buffArmorAction != null)
        {
            actions.Add(buffArmorAction);
        }
        else
        {
            actingEnemy.GenericMeleeAttackActions(ref actions, new DDEnemyActionAttack(damage));
        }

        return actions;
    }
}