using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DDCard_VALKBase : DDCardBase
{
    [Header("Valkyrie")]
    [SerializeField]
    protected int momentumCost;
    public int MomentumCost { get { return momentumCost; } }

    [SerializeField]
    protected int momentumGain;

    public override bool SelectCard()
    {
        return SingletonHolder.Instance.Player.MomentumCounter >= momentumCost;
    }

    public override void DisplayInformation(DDCardInHand cardInHand)
    {
        base.DisplayInformation(cardInHand);

        cardInHand.MomentumNumber.text = momentumCost.ToString();
    }
}
