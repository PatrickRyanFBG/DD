using System.Collections.Generic;

public class DDRuntimeAffix
{
    public int Number;
    public DDAffix Affix;
}

public class DDAffixManager
{
    private Dictionary<EAffixType, DDRuntimeAffix> currentAffixes = new Dictionary<EAffixType, DDRuntimeAffix>();

    public UnityEngine.Events.UnityEvent<EAffixType> AffixAdjusted = new UnityEngine.Events.UnityEvent<EAffixType>();

    private DDAffixVisualsManager affixVisualsManager;
    
    public DDAffixManager(DDAffixVisualsManager visualsManager)
    {
        affixVisualsManager = visualsManager;
    }
    
    public int? ModifyValueOfAffix(EAffixType affixType, int value, bool shouldSet)
    {
        // Does exist
        if (currentAffixes.TryGetValue(affixType, out DDRuntimeAffix affix))
        {
            int total = affix.Number + value;
            if(shouldSet)
            {
                total = value;
            }

            affix.Number = total;
            AffixAdjusted?.Invoke(affixType);
            affixVisualsManager.ModifyVisual(affixType, affix.Number);

            if(affix.Number > 0 || (affix.Number == 0 && affix.Affix.ExistsAtZero) || (affix.Number < 0 && affix.Affix.ExistsNegative))
            {
                return affix.Number;
            }
            else
            {
                currentAffixes.Remove(affixType);
                return null;
            }
        }

        DDAffix affixInfo = DDGlobalManager.Instance.AffixLibrary.GetAffixByType(affixType);
        if (value > 0 || (value == 0 && affixInfo.ExistsAtZero) || (value < 0 && affixInfo.ExistsNegative))
        {
            currentAffixes[affixType] = new DDRuntimeAffix { Number = value, Affix = affixInfo };
            affixVisualsManager.AddVisual(affixType, value);
            AffixAdjusted?.Invoke(affixType);
            return value;
        }

        return null;
    }

    public int? TryGetAffixValue(EAffixType affixType)
    {
        if (currentAffixes.TryGetValue(affixType, out DDRuntimeAffix affix))
        {
            return affix.Number;
        }

        return null;
    }
}
