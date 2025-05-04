using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDArtifactArmory : DDArtifactBase
{
    [SerializeField] private int armorGained = 5;

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
            for (int i = 1; i < DDGamePlaySingletonHolder.Instance.Board.ColumnsCount; i += 2)
            {
                DDGamePlaySingletonHolder.Instance.Player.ModifyLaneAffix(EAffixType.Armor, armorGained, i, false);
            }
        }

        yield return null;
    }
}