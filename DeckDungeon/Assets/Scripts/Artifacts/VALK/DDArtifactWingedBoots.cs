using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class DDArtifactWingedBoots : DDArtifactBase
{
    [SerializeField] private int momentumGain = 1;

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
        if (phaseArgs.Phase == EEncounterPhase.PlayersTurn)
        {
            yield return DDGamePlaySingletonHolder.Instance.Player.AddToMomentum(momentumGain);
        }
    }
}