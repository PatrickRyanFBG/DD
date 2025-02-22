using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class DDCard_VALKBase : DDCardGeneric
{
    [Header("Valkyrie")]
    [SerializeField]
    protected int momentumCost;
    public int MomentumCost => momentumCost;

    [SerializeField]
    protected int momentumGain;

    public override bool SelectCard()
    {
        return DDGamePlaySingletonHolder.Instance.Player.MomentumCounter >= momentumCost;
    }

    public override void SetCardInHand(DDCardInHand cardInHand)
    {
        base.SetCardInHand(cardInHand);

        cardInHand.MomentumNumber.text = momentumCost.ToString();
    }

    protected override IEnumerator Execute(List<DDSelection> selections)
    {
        if(momentumGain > 0)
        {
            DDGamePlaySingletonHolder.Instance.Player.AddToMomentum(momentumGain);
        }

        if(momentumCost > 0)
        {
            DDGamePlaySingletonHolder.Instance.Player.RemoveFromMomentum(momentumCost);
        }
        
        yield return base.Execute(selections);
    }
}
