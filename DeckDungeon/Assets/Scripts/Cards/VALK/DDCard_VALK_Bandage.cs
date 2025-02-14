using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DDCard_VALK_Bandage : DDCard_VALKBase
{
    [Header("Bandage")]
    [SerializeField]
    private int healAmount;

    protected override IEnumerator Execute(List<DDSelection> selections)
    {
        yield return base.Execute(selections);

        DDGamePlaySingletonHolder.Instance.Dungeon.HealDamage(healAmount);

        yield return null;
    }
}
