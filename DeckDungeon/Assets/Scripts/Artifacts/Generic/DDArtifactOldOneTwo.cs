using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDArtifactOldOneTwo : DDArtifactBase
{
    [SerializeField] private int retaliateGained = 2;

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
        if (phaseArgs.Phase == EEncounterPhase.EncounterStart)
        {
            for (int i = 0; i < DDGamePlaySingletonHolder.Instance.Board.ColumnsCount; i += 2)
            {
                DDGamePlaySingletonHolder.Instance.Player.ModifyLaneAffix(EAffixType.Retaliate, retaliateGained, i,
                    false);
            }
        }
        
        yield return null;
    }
}