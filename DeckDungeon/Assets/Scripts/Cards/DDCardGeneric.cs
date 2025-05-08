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

            if (effect.TargetInfo.DifferentTarget || (effect.TargetInfo.TargetType != selections[selectionIndex].TargetType && !effect.TargetInfo.UseLastTarget))
            {
                selectionIndex++;
            }
            
            // This is the safety catch for if a card specifically selects multiple enemies, but there just isn't enough legal targets
            // So card will execute early with less than required amounts and need to just stop when it runs out of selections
            if (selectionIndex >= selections.Count)
            {
                yield break;
            }
            
            yield return effect.ExecuteEffect(selections[selectionIndex], this);
            
            effectIndex++;
            
            yield return null;
        }
    }

    public override List<DDCardTargetInfo> GetTargets()
    {
        if (targets != null) return targets;
        
        targets = new List<DDCardTargetInfo>();

        for (int i = 0; i < effects.Length; i++)
        {
            DDCardEffectBase effect = effects[i];
            if (i == 0 || effect.TargetInfo.DifferentTarget || (effect.TargetInfo.TargetType != targets[i - 1].TargetType && !effect.TargetInfo.UseLastTarget))
            {
                targets.Add(effect.TargetInfo);
            }
        }

        return targets;
    }
    
    public override bool IsSelectionValid(List<DDSelection> selections, DDSelection selection, int targetIndex)
    {
        if (targetIndex == 0)
        {
            return true;
        }

        // This is for the case where we want to target two of the same types (like two different Enemies)
        // but we do not want to be able to select the same target twice
        if (targets[targetIndex].DifferentTarget)
        {
            if (selections[^1] == selection)
            {
                return false;
            }
        }
        
        return true;
    }
    
    // We only check this if we are looking for more targets
    public override bool ShouldExecuteEarly(List<DDSelection> selections)
    {
        bool isAllEnemyTargets = true;

        foreach (DDCardTargetInfo target in targets)
        {
            if (target.TargetType != ETargetType.Enemy)
            {
                isAllEnemyTargets = false;
                break;
            }
        }

        if (isAllEnemyTargets)
        {
            // We've clicked on all available enemies execute
            if (selections.Count == DDGamePlaySingletonHolder.Instance.Encounter.AllEnemies.Count)
            {
                return true;
            }
        }
        
        return false;
    }
}