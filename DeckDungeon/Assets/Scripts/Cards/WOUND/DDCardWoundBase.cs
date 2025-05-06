using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DDCardWoundBase : DDCardBase
{
    protected override IEnumerator Execute(List<DDSelection> selections)
    {
        yield return null;
    }

    public override bool SelectCard()
    {
        return false;
    }
    
    public override void SetCardInHand(DDCardInHand cardInHand)
    {
        base.SetCardInHand(cardInHand);

        cardInHand.MomentumNumber.transform.parent.gameObject.SetActive(false);
    }
}
