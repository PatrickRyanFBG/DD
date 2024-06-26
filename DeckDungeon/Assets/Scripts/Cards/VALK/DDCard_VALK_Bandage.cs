using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDCard_VALK_Bandage : DDCard_VALKBase
{
    [Header("Bandage")]
    [SerializeField]
    private int healAmount;

    public override IEnumerator ExecuteCard(List<DDSelection> selections)
    {
        yield return base.ExecuteCard(selections);

        SingletonHolder.Instance.Dungeon.HealDamage(healAmount);

        yield return null;
    }
}
