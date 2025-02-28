using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDArtifactOmega : DDArtifactBase
{
    private DDCardInHand lastCardPlayed;
    
    public override void Equipped()
    {
        DDGamePlaySingletonHolder.Instance.Encounter.PhaseChanged.AddListener(EncounterPhaseChanged);
        DDGamePlaySingletonHolder.Instance.Player.CardLifeTimeChanged.AddListener(CardLifeTimeChanged);
    }
    
    public override void Unequipped()
    {
        DDGamePlaySingletonHolder.Instance.Encounter.PhaseChanged.RemoveListener(EncounterPhaseChanged);
        DDGamePlaySingletonHolder.Instance.Player.CardLifeTimeChanged.RemoveListener(CardLifeTimeChanged);
    }

    private void CardLifeTimeChanged(DDCardInHand card, EPlayerCardLifeTime lifeTime)
    {
        if(lifeTime == EPlayerCardLifeTime.Played)
        {
            lastCardPlayed = card;
        }
    }

    private void EncounterPhaseChanged(EEncounterPhase phase)
    {
        if (phase == EEncounterPhase.PlayersEndTurn)
        {
            if (lastCardPlayed != null)
            {
                lastCardPlayed.CurrentCard.AddRandomFinish();
            }
            lastCardPlayed = null;
        }
    }
}
