using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DDAffixLibrary : DDScriptableObject
{
    [SerializeField] private List<DDAffix> allAffixes = new List<DDAffix>();

    [System.NonSerialized] Dictionary<EAffixType, DDAffix> affixDictionary = null;

    public DDAffix GetAffixByType(EAffixType targetAffix)
    {
        if (affixDictionary == null)
        {
            affixDictionary = new Dictionary<EAffixType, DDAffix>(allAffixes.Count);

            foreach (DDAffix affix in allAffixes)
            {
                affixDictionary[affix.AffixType] = affix;
            }
        }

        affixDictionary.TryGetValue(targetAffix, out DDAffix outAffix);
        return outAffix;
    }
}