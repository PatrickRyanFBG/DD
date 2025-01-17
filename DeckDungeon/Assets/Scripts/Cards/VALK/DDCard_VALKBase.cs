using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
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
        return DDGamePlaySingletonHolder.Instance.Player.MomentumCounter >= momentumCost;
    }

    public override void DisplayInformation(DDCardInHand cardInHand)
    {
        base.DisplayInformation(cardInHand);

        cardInHand.MomentumNumber.text = momentumCost.ToString();
    }

    public override IEnumerator ExecuteCard(List<DDSelection> selections)
    {
        if(momentumGain > 0)
        {
            DDGamePlaySingletonHolder.Instance.Player.AddToMomentum(momentumGain);
        }

        if(momentumCost > 0)
        {
            DDGamePlaySingletonHolder.Instance.Player.RemoveFromMomentum(momentumCost);
        }

        yield return null;
    }
}
