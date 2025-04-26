using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDArtifactGainAffixAtTime : DDArtifactBase
{
    [SerializeField] private EAffixType affixType;

    [SerializeField] private int amount;
    
    [SerializeField] private EEncounterPhase encounterPhase;
    
    public override void Equipped()
    {
        DDGamePlaySingletonHolder.Instance.Encounter.PhaseChanged.AddListener(EncounterPhaseChanged);
    }
    
    public override void Unequipped()
    {
        DDGamePlaySingletonHolder.Instance.Encounter.PhaseChanged.RemoveListener(EncounterPhaseChanged);
    }

    private void EncounterPhaseChanged(EEncounterPhase phase)
    {
        if (phase == encounterPhase)
        {
            DDGamePlaySingletonHolder.Instance.Player.ModifyAffix(affixType, amount, false);
        }
    }
}
