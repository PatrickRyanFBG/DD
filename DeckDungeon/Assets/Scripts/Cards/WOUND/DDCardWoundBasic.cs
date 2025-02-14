using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DDCardWoundBasic : DDCardWoundBase
{
    [Header("Basic Wound")]
    [SerializeField]
    private int damage;

    public override IEnumerator EndOfTurn()
    {
        yield return base.EndOfTurn();
        
        DDGamePlaySingletonHolder.Instance.Dungeon.DoDamage(damage);
        
        yield return null;
    }
}
