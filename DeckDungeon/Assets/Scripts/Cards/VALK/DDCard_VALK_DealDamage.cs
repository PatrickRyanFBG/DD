using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DDCard_VALK_DealDamage : DDCard_VALKBase
{
    [Header("Deal Damage")]
    [SerializeField]
    private int damage;

    public override IEnumerator ExecuteCard(List<DDSelection> selections)
    {
        yield return base.ExecuteCard(selections);

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
                DDGamePlaySingletonHolder.Instance.Player.DealDamageToEnemy(damage, rangeType, enemy);
            }
        }

        yield return null;
    }
}
