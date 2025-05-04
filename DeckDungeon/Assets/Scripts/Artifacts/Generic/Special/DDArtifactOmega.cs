using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDArtifactOmega : DDArtifactBase
{
    private DDCardInHand lastCardPlayed;
    
    public override void Equipped()
    {
        DDGamePlaySingletonHolder.Instance.Encounter.PhaseChanged += EncounterPhaseChanged;

        DDGamePlaySingletonHolder.Instance.Player.CardLifeTimeChanged.AddListener(CardLifeTimeChanged);
    }
    
    public override void Unequipped()
    {
        DDGamePlaySingletonHolder.Instance.Encounter.PhaseChanged -= EncounterPhaseChanged;
        DDGamePlaySingletonHolder.Instance.Player.CardLifeTimeChanged.RemoveListener(CardLifeTimeChanged);
    }

    private void CardLifeTimeChanged(DDCardInHand card, EPlayerCardLifeTime lifeTime)
    {
        if(lifeTime == EPlayerCardLifeTime.Played)
        {
            lastCardPlayed = card;
        }
    }

    private IEnumerator EncounterPhaseChanged(MonoBehaviour sender, System.EventArgs args)
    {
        DDEncounter.DDPhaseChangeEventArgs phaseArgs = args as DDEncounter.DDPhaseChangeEventArgs;
        if (phaseArgs.Phase == EEncounterPhase.PlayersEndTurn)
        {
            if (lastCardPlayed)
            {
                lastCardPlayed.CurrentCard.AddRandomFinish();
            }
            lastCardPlayed = null;
        }
        
        yield return null;
    }
}
