using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class DDCardBase : DDScriptableObject
{
    [Header("Base")]
    [SerializeField]
    private ECardType cardType;
    public ECardType CardType => cardType;

    [SerializeField]
    private Texture image;
    public Texture Image => image;

    [SerializeField]
    private new string name;
    public string Name => name;

    [SerializeField, Multiline]
    private string description;

    [SerializeField]
    protected ERangeType rangeType = ERangeType.None;

    [SerializeField]
    private List<ECardFinishing> finishes;
    public List<ECardFinishing> Finishes => finishes;

    [SerializeField]
    protected List<Target> targets;
    public List<Target> Targets => targets;

    // Need to make a base between this and player cards
    [SerializeField]
    private int uses = 0;
    public int Uses => uses;

    [SerializeField]
    private Vector2 price = new Vector2(100, 200);
    public int Price => (int)Random.Range(price.x, price.y);

    public virtual bool SelectCard()
    {
        return true;
    }

    public virtual void UnselectCard()
    {

    }

    public virtual IEnumerator DrawCard()
    {
        yield return null;
    }

    public abstract IEnumerator ExecuteCard(List<DDSelection> selections);

    public virtual IEnumerator DiscardCard()
    {
        yield return null;
    }

    public virtual IEnumerator DestroyedCard()
    {
        yield return null;
    }

    public virtual void DisplayInformation(DDCardInHand cardInHand)
    {
        cardInHand.Image.texture = image;
        cardInHand.CardTypeText.text = cardType.ToString();
        cardInHand.NameText.text = name;
        cardInHand.DescText.text = description;
        if (uses > 0)
        {
            cardInHand.DescText.text += "\r\nUses: " + (uses - cardInHand.AmountUsed).ToString();
        }
    }

    public virtual bool IsSelectionValid(DDSelection selection, int targetIndex)
    {
        return true;
    }
}

// Selection
// Type
// Complete
[System.Serializable]
public class Target
{
    [SerializeField]
    private ETargetType targetType;
    public ETargetType TargetType => targetType;

    public int GetTargetTypeLayer()
    {
        return 1 << (int)targetType;
    }
}