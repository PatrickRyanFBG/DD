using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDArtifactOmega : DDArtifactBase
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
        if (phaseArgs.Phase == EEncounterPhase.PlayersEndTurn)
        {
            if (DDGamePlaySingletonHolder.Instance.Player.CardsExecutedThisTurn.Count > 0)
            {
                DDGamePlaySingletonHolder.Instance.Player.CardsExecutedThisTurn[^1].AddRandomFinishByImpact(EPlayerCardFinishImpact.Positive);
            }
        }
        
        yield return null;
    }
}
