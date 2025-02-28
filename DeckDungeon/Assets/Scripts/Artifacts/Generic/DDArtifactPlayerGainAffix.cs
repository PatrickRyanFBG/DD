using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDArtifactPlayerGainAffix : DDArtifactBase
{
    [SerializeField] private EAffixType affixType;

    [SerializeField] private int amount;

    [SerializeField] private EDungeonPhase onPhase;
    
    public override void Equipped()
    {
        DDGamePlaySingletonHolder.Instance.Dungeon.PhaseChanged.AddListener(PhaseChanged);
    }

    public override void Unequipped()
    {
        DDGamePlaySingletonHolder.Instance.Dungeon.PhaseChanged.RemoveListener(PhaseChanged);
    }
    
    private void PhaseChanged(EDungeonPhase phase)
    {
        if (onPhase == phase)
        {
            DDGamePlaySingletonHolder.Instance.Player.ModifyAffix(affixType, amount, false);
        }
    }
}
