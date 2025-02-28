using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDArtifactWeightLifting : DDArtifactBase
{
    [SerializeField] private int vigorGained = 1;

    public override void Equipped()
    {
        DDGamePlaySingletonHolder.Instance.Player.CardLifeTimeChanged.AddListener(CardLifeTimeChanged);
    }
    
    public override void Unequipped()
    {
        DDGamePlaySingletonHolder.Instance.Player.CardLifeTimeChanged.RemoveListener(CardLifeTimeChanged);
    }

    private void CardLifeTimeChanged(DDCardInHand card, EPlayerCardLifeTime lifeTime)
    {
        if (lifeTime == EPlayerCardLifeTime.Played)
        {
            if (card.CurrentCard.CardType == ECardType.Offensive)
            {
                DDGamePlaySingletonHolder.Instance.Player.ModifyAffix(EAffixType.Vigor, vigorGained, false);
            }
        }
    }
}