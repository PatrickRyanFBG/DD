using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DDArtifact_Hyperactive : DDArtifactBase
{
    [SerializeField]
    private int damagePerMomentum = 1;

    public override void Equipped()
    {
        SingletonHolder.Instance.Player.GainedMomentum.AddListener(GainedMomentum);
    }

    private void GainedMomentum()
    {
        List<DDEnemyOnBoard> allEnemies = new List<DDEnemyOnBoard>();
        SingletonHolder.Instance.Board.GetAllEnemies(ref allEnemies);

        for (int i = 0; i < allEnemies.Count; i++)
        {
            allEnemies[i].DoDamage(damagePerMomentum);
        }
    }
}
