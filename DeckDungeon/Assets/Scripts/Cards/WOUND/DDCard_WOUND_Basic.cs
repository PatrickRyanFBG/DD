using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DDCard_WOUND_Basic : DDCard_WOUNDBase
{
    [Header("Basic Wound")]
    [SerializeField]
    private int damage;

    public override IEnumerator DestroyedCard()
    {
        DDGamePlaySingletonHolder.Instance.Dungeon.DoDamage(damage);

        yield return null;
    }
}
