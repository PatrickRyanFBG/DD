using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class DDCardBase : DDScriptableObject
{
    [Header("Base")]
    [SerializeField]
    private ECardType cardType;
    public ECardType CardType { get { return cardType; } }

    [SerializeField]
    private Texture image;
    public Texture Image { get { return image; } }

    [SerializeField]
    private new string name;
    public string Name { get { return name; } }

    [SerializeField, Multiline]
    private string description;

    [SerializeField]
    private List<ECardFinishing> finishes;
    public List<ECardFinishing> Finishes { get => finishes; }

    [SerializeField]
    protected List<Target> targets;
    public List<Target> Targets { get { return targets; } }

    // Need to make a base between this and player cards
    [SerializeField]
    private int uses = 0;
    public int Uses { get { return uses; } }

    [SerializeField]
    private Vector2 price = new Vector2(100, 200);
    public int Price { get { return (int)Random.Range(price.x, price.y); } }

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
    public enum ETargetType
    {
        PlayerCard = 6,
        Location = 7,
        Row = 8,
        Column = 9,
        EntireOrEmpty = 10,
        Enemy = 13
    }

    [SerializeField]
    private ETargetType targetType;
    public ETargetType TargetType { get { return targetType; } }

    public int GetTargetTypeLayer()
    {
        return 1 << (int)targetType;
    }
}