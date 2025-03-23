using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDEnemySpinningAxe : DDEnemyBase
{
    [Header("Spinning Axe")]
    [SerializeField] private int bleedAmount = 2;
    
    public override List<DDEnemyActionBase> CalculateActions(int number, DDEnemyOnBoard actingEnemy)
    {
        List<DDEnemyActionBase> actions = new List<DDEnemyActionBase>();

        actions.Add(new DDEnemyActionModifyAffix(EAffixType.Bleed, bleedAmount, false));
        
        return actions;
    }
}
