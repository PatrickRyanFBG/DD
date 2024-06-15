using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DDDungeonCardShown : DDSelection
{
    [SerializeField]
    private RawImage image;
    public RawImage Image { get => image; }

    [SerializeField]
    private TMPro.TextMeshProUGUI nameText;
    public TextMeshProUGUI NameText { get => nameText; }

    [SerializeField]
    private TMPro.TextMeshProUGUI descText;
    public TextMeshProUGUI DescText { get => descText; }

    [SerializeField]
    private GameObject locked;

    private DDDungeonCardBase currentDungeonCard;

    private int index;
    public int Index { get { return index; } }

    public void SetUpDungeonCard(DDDungeonCardBase dungeonCard, int cardIndex)
    {
        currentDungeonCard = dungeonCard;
        currentDungeonCard.DisplayInformation(this);

        locked.SetActive(!currentDungeonCard.CanSelect());

        index = cardIndex;

        descText.enabled = false;

        gameObject.SetActive(true);
    }

    public bool CardSelected()
    {
        return currentDungeonCard.SelectCard();
    }

    public override void Hovered()
    {
        descText.enabled = true;
    }

    public override void Unhovered()
    {
        descText.enabled = false;
    }
}
