using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DDDungeonCardShown : DDSelection
{
    [SerializeField]
    private RawImage image;
    public RawImage Image => image;

    [SerializeField]
    private TMPro.TextMeshProUGUI nameText;
    public TextMeshProUGUI NameText => nameText;

    [SerializeField]
    private TMPro.TextMeshProUGUI descText;
    public TextMeshProUGUI DescText => descText;

    [SerializeField]
    private GameObject locked;

    private DDDungeonCardBase currentDungeonCard;
    public DDDungeonCardBase CurrentDungeonCard => currentDungeonCard;

    private int index;
    public int Index => index;

    [SerializeField]
    private Collider col;

    public UnityEngine.Events.UnityEvent<DDDungeonCardShown> OnCardSelected;

    public void DungeonCardSelected()
    {
        if(currentDungeonCard.CanSelect())
        {
            OnCardSelected.Invoke(this);
        }
    }

    public void SetUpDungeonCard(DDDungeonCardBase dungeonCard, int cardIndex, bool interactable = true)
    {
        currentDungeonCard = dungeonCard;
        currentDungeonCard.DisplayInformation(this);

        locked.SetActive(!currentDungeonCard.CanSelect());

        index = cardIndex;

        descText.enabled = false;

        gameObject.SetActive(true);

        col.enabled = interactable;
    }

    public override bool Hovered()
    {
        descText.enabled = true;
        return true;
    }

    public override void Unhovered()
    {
        descText.enabled = false;
    }
}
