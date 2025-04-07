using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDArtifactAlpha : DDArtifactBase
{
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
        if (phase == EEncounterPhase.PlayersStartTurn)
        {
            DDGamePlaySingletonHolder.Instance.Player.Deck.PeakTopCard().CurrentCard.AddRandomFinish();
        }
    }
}
