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

    private EAffixOwner owner;
    public EAffixOwner Owner => owner;

    public DDAffixManager(DDAffixVisualsManager visualsManager, EAffixOwner affixOwner)
    {
        affixVisualsManager = visualsManager;
        owner = affixOwner;
    }

    public int? ModifyValueOfAffix(EAffixType affixType, int value, bool shouldSet, bool useEvent = true)
    {
        int before = 0;
        int? after = null;
        
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
            
            after = affix.Number;

            if(affix.Number > 0 || (affix.Number == 0 && affix.Affix.ExistsAtZero) || (affix.Number < 0 && affix.Affix.ExistsNegative))
            {
            }
            else
            {
                currentAffixes.Remove(affixType);
            }
        }

        DDAffix affixInfo = DDGlobalManager.Instance.AffixLibrary.GetAffixByType(affixType);
        if (value > 0 || (value == 0 && affixInfo.ExistsAtZero) || (value < 0 && affixInfo.ExistsNegative))
        {
            currentAffixes[affixType] = new DDRuntimeAffix { Number = value, Affix = affixInfo };
            affixVisualsManager.AddVisual(affixType, value);
            AffixAdjusted?.Invoke(affixType);
            after = value;
        }

        if (useEvent)
        {
            DDGamePlaySingletonHolder.Instance.Encounter.AffixModified?.Invoke(this, affixType, before, after ?? 0);
        }
        
        return after;
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
