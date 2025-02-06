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

    public int ModifyValueOfAffix(EAffixType affixType, int value, bool shouldSet)
    {
        if (currentAffixes.TryGetValue(affixType, out DDRuntimeAffix affix))
        {
            int total = affix.Number + value;
            if(shouldSet)
            {
                total = value;
            }

            if(affix.Affix.ExistsAtOrBelowZero)
            {
                affix.Number = total;
            }
            else
            {
                currentAffixes.Remove(affixType);
            }
            AffixAdjusted?.Invoke(affixType);
            return total;
        }

        DDAffix affixInfo = DDGlobalManager.Instance.AffixLibrary.GetAffixByType(affixType);
        if (value > 0 || affixInfo.ExistsAtOrBelowZero)
        {
            currentAffixes[affixType] = new DDRuntimeAffix { Number = value, Affix = affixInfo };
        }
        AffixAdjusted?.Invoke(affixType);
        return value;
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
