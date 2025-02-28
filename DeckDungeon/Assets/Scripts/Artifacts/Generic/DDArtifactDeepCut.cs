using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDArtifactDeepCut : DDArtifactBase
{
    [SerializeField] private int additionalBleed = 1;
    
    public override void Equipped()
    {
        DDGamePlaySingletonHolder.Instance.Encounter.AffixModified.AddListener(AffixModified);
    }

    public override void Unequipped()
    { 
        DDGamePlaySingletonHolder.Instance.Encounter.AffixModified.RemoveListener(AffixModified);
    }
    
    private void AffixModified(DDAffixManager manager, EAffixType affixType, int before, int? after)
    {
        if (manager.Owner == EAffixOwner.Enemy)
        {
            if (affixType == EAffixType.Bleed)
            {
                if (before < (after ?? 0))
                {
                    manager.ModifyValueOfAffix(EAffixType.Bleed, additionalBleed, false, false);
                }
            }
        }
    }
}