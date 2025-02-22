using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DDCardShown : DDCardInHand
{
    public UnityEngine.Events.UnityEvent<DDCardShown> OnCardSelected;

    public void CardSelected()
    {
        OnCardSelected.Invoke(this);
    }

    public override bool Hovered()
    {
        return false;
    }

    public override void Unhovered()
    {
    }
}

