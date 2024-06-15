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
        SingletonHolder.Instance.Player.RemoveFromMomentum(momentumCost);

        yield return null;

        SingletonHolder.Instance.Dungeon.HealDamage(healAmount);

        yield return null;
    }
}
