using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DDArtifactHyperactive : DDArtifactBase
{
    [SerializeField] private int damagePerMomentum = 1;

    public override void Equipped()
    {
        DDGamePlaySingletonHolder.Instance.Player.GainedMomentum.AddListener(GainedMomentum);
    }

    public override void Unequipped()
    {
        DDGamePlaySingletonHolder.Instance.Player.GainedMomentum.RemoveListener(GainedMomentum);
    }
    
    private void GainedMomentum()
    {
        List<DDEnemyOnBoard> allEnemies = DDGamePlaySingletonHolder.Instance.Encounter.AllEnemies;

        for (int i = 0; i < allEnemies.Count; i++)
        {
            allEnemies[i].TakeDamage(damagePerMomentum, ERangeType.None, false);
        }
    }
}