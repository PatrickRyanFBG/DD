using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DDCardShown : DDCardInHand
{
    public void CardSelected()
    {
        SingletonHolder.Instance.Dungeon.PlayerCardSelect(currentCard);
    }

    public override void Hovered()
    {
    }

    public override void Unhovered()
    {
    }
}

