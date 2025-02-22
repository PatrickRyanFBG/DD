using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class DDArtifactWingedBoots : DDArtifactBase
{
    [SerializeField]
    private int momentumGain = 1;
    
    public override void Equipped()
    {
        DDGamePlaySingletonHolder.Instance.Encounter.PhaseChanged.AddListener(EncounterPhaseChanged);
    }

    private void EncounterPhaseChanged(EEncounterPhase phase)
    {
        if(phase == EEncounterPhase.PlayersTurn)
        {
            DDGamePlaySingletonHolder.Instance.Player.AddToMomentum(momentumGain);
        }
    }
}
