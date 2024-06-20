using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DDArtifact_Preparations : DDArtifactBase
{
    [SerializeField]
    private int extraDrawCount = 1;

    public override void Equipped()
    {
        SingletonHolder.Instance.Encounter.PhaseChanged.AddListener(EncounterPhaseChanged);
    }

    private void EncounterPhaseChanged(EEncounterPhase phase)
    {
        if (phase == EEncounterPhase.EncounterStart)
        {
            SingletonHolder.Instance.Player.AdjustHandSize(extraDrawCount);
        }
    }
}
