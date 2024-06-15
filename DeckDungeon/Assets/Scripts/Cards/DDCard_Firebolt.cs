using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DDCard_Firebolt : DDCardBase
{
    [Header("Firebolt")]
    [SerializeField]
    private int damage = 5;

    public override IEnumerator ExecuteCard(List<DDSelection> selections)
    {
        DDEnemyOnBoard enemy = selections[0] as DDEnemyOnBoard;

        if(enemy)
        {
            enemy.DoDamage(damage);
        }

        yield return null;
    }
}