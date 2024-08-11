using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class DDCardBase : DDScriptableObject
{
    [Header("base")]
    [SerializeField]
    protected Texture image;
    public Texture Image { get { return image; } }

    [SerializeField]
    protected new string name;
    public string Name { get { return name; } }

    [SerializeField, Multiline]
    protected string description;

    [SerializeField]
    protected List<Target> targets;
    public List<Target> Targets { get { return targets; } }

    [SerializeField]
    protected int uses = 0;
    public int Uses { get { return uses; } }

    public virtual bool SelectCard()
    {
        return true;
    }

    public virtual void UnselectCard()
    {

    }

    public abstract IEnumerator ExecuteCard(List<DDSelection> selections);

    public virtual void DisplayInformation(DDCardInHand cardInHand)
    {
        cardInHand.Image.texture = image;
        cardInHand.NameText.text = name;
        cardInHand.DescText.text = description;
        if(uses > 0)
        {
            cardInHand.DescText.text += "\r\nUses: " + (uses - cardInHand.AmountUsed).ToString();
        }
    }

    public virtual bool IsSelectionValid(DDSelection selection, int targetIndex)
    {
        return true;
    }

    // Card Drawn
    // Card Discard
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