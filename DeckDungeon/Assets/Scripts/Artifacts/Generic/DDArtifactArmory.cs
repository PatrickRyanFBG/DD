using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDArtifactArmory : DDArtifactBase
{
    [SerializeField]
    private int armorGained = 5;
    
    public override void Equipped()
    {
        DDGamePlaySingletonHolder.Instance.Encounter.PhaseChanged.AddListener(EncounterPhaseChanged);
    }

    private void EncounterPhaseChanged(EEncounterPhase phase)
    {
        if(phase == EEncounterPhase.EncounterStart)
        {
            for (int i = 1; i < DDGamePlaySingletonHolder.Instance.Board.ColumnsCount; i += 2)
            {
                DDGamePlaySingletonHolder.Instance.Player.ModifyLaneAffix(EAffixType.Armor, armorGained,  i, false);
            }
        }
    }
}