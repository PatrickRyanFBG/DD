using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDArtifactFlatFooted : DDArtifactBase
{
    [SerializeField] private int enemyExpertiseReduced = 1;

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
            List<DDEnemyOnBoard> allEnemies = DDGamePlaySingletonHolder.Instance.Encounter.AllEnemies;
            foreach (var e in allEnemies)
            {
                e.ModifyAffix(EAffixType.Expertise, -enemyExpertiseReduced, false);
            }
        }
    }
}