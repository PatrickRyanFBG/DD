using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DDCard_VALK_DealDamage : DDCard_VALKBase
{
    [Header("Deal Damage")]
    [SerializeField]
    private int damage;

    protected override IEnumerator Execute(List<DDSelection> selections)
    {
        yield return base.Execute(selections);

        List<DDEnemyOnBoard> allEnemies = new List<DDEnemyOnBoard>();

        for (int i = 0; i < selections.Count; i++)
        {
            selections[i].FillEnemyList(ref allEnemies);
        }

        for (int i = 0; i < allEnemies.Count; i++)
        {
            DDEnemyOnBoard enemy = allEnemies[i];

            if (enemy)
            {
                DDGamePlaySingletonHolder.Instance.Player.DealDamageToEnemy(damage, RangeType, enemy);
            }
        }

        yield return null;
    }
}
