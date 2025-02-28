using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDArtifactOldOneTwo : DDArtifactBase
{
    [SerializeField] private int retaliateGained = 2;

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
        if (phase == EEncounterPhase.EncounterStart)
        {
            for (int i = 0; i < DDGamePlaySingletonHolder.Instance.Board.ColumnsCount; i += 2)
            {
                DDGamePlaySingletonHolder.Instance.Player.ModifyLaneAffix(EAffixType.Retaliate, retaliateGained, i,
                    false);
            }
        }
    }
}