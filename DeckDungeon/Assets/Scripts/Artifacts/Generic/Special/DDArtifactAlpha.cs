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
        // Monster Forecast happens right before PlayerTurn
        // So we are just cheating here a little bit
        // Maybe need a "BeforePlayerTurn" to catch this before draw
        if (phase == EEncounterPhase.MonsterForecast)
        {
            DDGamePlaySingletonHolder.Instance.Player.Deck.PeakTopCard().CurrentCard.AddRandomFinish();
        }
    }
}
