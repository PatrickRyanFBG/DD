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

    public override void Hovered(bool fromAnotherSelection = false)
    {
        DDGlobalManager.Instance.ClipLibrary.HoverCard.PlayNow();
    }

    public override void Unhovered(bool fromAnotherSelection = false)
    {
    }

    public void CardSelected()
    {
        OnCardSelected.Invoke(this);
    }
    
    protected override void OnDisable()
    {
        base.OnDisable();
        
        foreach (var finish in finishIcons)
        {
            Destroy(finish.Value.gameObject);
        }
        
        finishIcons.Clear();
    }
}