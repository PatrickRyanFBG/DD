using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDArtifactAlpha : DDArtifactBase
{
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
        if (phaseArgs.Phase == EEncounterPhase.PlayersStartTurn)
        {
            DDGamePlaySingletonHolder.Instance.Player.Deck.PeakTopCard().CurrentCard.AddRandomFinish();
        }

        yield return null;
    }
}
