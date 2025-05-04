using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDArtifactFlatFooted : DDArtifactBase
{
    [SerializeField] private int enemyExpertiseReduced = 1;

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
            List<DDEnemyOnBoard> allEnemies = DDGamePlaySingletonHolder.Instance.Encounter.AllEnemies;
            foreach (var e in allEnemies)
            {
                e.ModifyAffix(EAffixType.Expertise, -enemyExpertiseReduced, false);
            }
        }
        
        yield return null;
    }
}