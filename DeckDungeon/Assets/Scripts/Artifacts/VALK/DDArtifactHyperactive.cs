using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DDArtifactHyperactive : DDArtifactBase
{
    [SerializeField] private int damagePerMomentum = 1;

    public override void Equipped()
    {
        DDGamePlaySingletonHolder.Instance.Player.GainedMomentum += GainedMomentum;
    }

    public override void Unequipped()
    {
        DDGamePlaySingletonHolder.Instance.Player.GainedMomentum -= GainedMomentum;
    }
    
    private IEnumerator GainedMomentum(MonoBehaviour sender, EventArgs args)
    {
        List<DDEnemyOnBoard> allEnemies = DDGamePlaySingletonHolder.Instance.Encounter.AllEnemies;

        for (int i = 0; i < allEnemies.Count; i++)
        {
            if (!allEnemies[i].CurrentEnemy.Friendly)
            {
                allEnemies[i].TakeDamage(damagePerMomentum, ERangeType.Pure, false);
            }
            
            yield return null;
        }
    }
}