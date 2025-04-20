using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DDCardShown : DDCardInHand
{
    public UnityEngine.Events.UnityEvent<DDCardShown> OnCardSelected;

    [SerializeField] private RectTransform rectTransform;
    public RectTransform RectTransform => rectTransform;

    public override void SetUpCard(DDCardBase cardBase, bool hover = true)
    {
        base.SetUpCard(cardBase, hover);

        transform.name = "CS: " + currentCard.CardName;
    }

    // Called from UI
    public override void UI_Clicked()
    {
        CardSelected();
    }

    public override void Hovered()
    {
    }

    public override void Unhovered()
    {
    }

    public void CardSelected()
    {
        OnCardSelected.Invoke(this);
    }
}