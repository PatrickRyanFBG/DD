using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDCardGeneric : DDCardBase
{
    [SerializeReferenceDropdown, SerializeReference] protected DDCardEffectBase[] effects;
    
    protected override IEnumerator Execute(List<DDSelection> selections)
    {
        int effectIndex = 0;
        int selectionIndex = 0;
        
        while (effectIndex < effects.Length)
        {
            DDCardEffectBase effect = effects[effectIndex];

            if (effect.DifferentTarget || (effect.TargetType != selections[selectionIndex].TargetType && !effect.UseLastTarget))
            {
                selectionIndex++;
            }
            
            yield return effect.ExecuteEffect(selections[selectionIndex], this);
            
            effectIndex++;
            
            yield return null;
        }
    }

    public override List<ETargetType> GetTargets()
    {
        if (targets != null) return targets;
        
        targets = new List<ETargetType>();

        for (int i = 0; i < effects.Length; i++)
        {
            DDCardEffectBase effect = effects[i];
            if (i == 0 || effect.DifferentTarget || (effect.TargetType != targets[i - 1] && !effect.UseLastTarget))
            {
                targets.Add(effect.TargetType);
            }
        }

        return targets;
    }
}