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
        DDGamePlaySingletonHolder.Instance.Encounter.PhaseChanged += EncounterPhaseChanged;
    }
    
    public override void Unequipped()
    {
        DDGamePlaySingletonHolder.Instance.Encounter.PhaseChanged -= EncounterPhaseChanged;
    }

    private IEnumerator EncounterPhaseChanged(MonoBehaviour sender, System.EventArgs args)
    {
        DDEncounter.DDPhaseChangeEventArgs phaseArgs = args as DDEncounter.DDPhaseChangeEventArgs;
        if (phaseArgs.Phase == encounterPhase)
        {
            DDGamePlaySingletonHolder.Instance.Player.ModifyAffix(affixType, amount, false);
        }
        
        yield return null;
    }
}
