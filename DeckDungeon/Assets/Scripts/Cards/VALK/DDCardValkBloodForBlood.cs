using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDCardValkBloodForBlood : DDCardValkGeneric
{
    [SerializeField] private int selfDamage;

    [SerializeField] private int bleedNumber;
    
    public override List<ETargetType> GetTargets()
    {
        if (targets != null) return targets;

        targets = new List<ETargetType>() { ETargetType.Enemy };
        
        return base.GetTargets();
    }
    
    protected override IEnumerator Execute(List<DDSelection> selections)
    {
        DDGamePlaySingletonHolder.Instance.Dungeon.DoDamage(selfDamage);

        yield return null;
        
        DDEnemyOnBoard eob = selections[0] as DDEnemyOnBoard;
        
        eob.ModifyAffix(EAffixType.Bleed, bleedNumber, false);
        
        yield return base.Execute(selections);
    }
}
