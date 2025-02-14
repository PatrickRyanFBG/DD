using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDCardValkGeneric : DDCardGeneric
{
    [Header("VALK")][SerializeField] private int momentumCost;
    public int MomentumCost => momentumCost;
    
    [SerializeField] private int momentumGain;
    public int MomentumGain => momentumGain;
    
    public override bool SelectCard()
    {
        return true;//DDGamePlaySingletonHolder.Instance.Player.MomentumCounter >= momentumCost;
    }

    public override void RuntimeInit(DDCardInHand cardInHand)
    {
        base.RuntimeInit(cardInHand);

        cardInHand.MomentumNumber.text = momentumCost.ToString();
    }

    protected override IEnumerator PreExecute(List<DDSelection> selections)
    {
        if(momentumCost > 0)
        {
            DDGamePlaySingletonHolder.Instance.Player.RemoveFromMomentum(momentumCost);
        }
        
        return base.PreExecute(selections);
    }

    protected override IEnumerator Execute(List<DDSelection> selections)
    {
        yield return base.Execute(selections);
    }

    protected override IEnumerator PostExecute(List<DDSelection> selections)
    {
        if(momentumGain > 0)
        {
            DDGamePlaySingletonHolder.Instance.Player.AddToMomentum(momentumGain);
        }
        
        return base.PostExecute(selections);
    }
}
