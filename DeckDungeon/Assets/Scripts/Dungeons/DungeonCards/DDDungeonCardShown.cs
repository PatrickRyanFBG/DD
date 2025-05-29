using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DDDungeonCardShown : DDSelection
{
    [SerializeField] private RawImage image;
    public RawImage Image => image;

    [SerializeField] private TMPro.TextMeshProUGUI nameText;
    public TextMeshProUGUI NameText => nameText;

    [SerializeField] private TMPro.TextMeshProUGUI descText;
    public TextMeshProUGUI DescText => descText;

    [SerializeField] private GameObject descParent;

    [SerializeField] private GameObject locked;

    private DDDungeonCardBase currentDungeonCard;
    public DDDungeonCardBase CurrentDungeonCard => currentDungeonCard;

    private int index;
    public int Index => index;

    public UnityEngine.Events.UnityEvent<DDDungeonCardShown> OnCardSelected;

    [SerializeField] private RectTransform rectTransform;
    public RectTransform RectTransform => rectTransform;

    public void DungeonCardSelected()
    {
        if (currentDungeonCard.CanSelect())
        {
            OnCardSelected.Invoke(this);
        }
    }

    public void SetUpDungeonCard(DDDungeonCardBase dungeonCard, int cardIndex, bool interactable = true)
    {
        gameObject.name = "DCS: " + dungeonCard.Name;

        currentDungeonCard = dungeonCard;
        currentDungeonCard.DisplayInformation(this);

        locked.SetActive(!currentDungeonCard.CanSelect());

        index = cardIndex;

        descParent.SetActive(false);

        gameObject.SetActive(true);
    }

    public override void Hovered(bool fromAnotherSelection = false)
    {
        DDGlobalManager.Instance.ClipLibrary.HoverCard.PlayNow();
        
        descParent.SetActive(true);
    }

    public override void Unhovered(bool fromAnotherSelection = false)
    {
        descParent.SetActive(false);
    }
}