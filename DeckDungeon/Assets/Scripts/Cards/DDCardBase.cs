using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class DDCardBase : DDScriptableObject
{
    [Header("base")]
    [SerializeField]
    private Texture image;
    public Texture Image { get { return image; } }

    [SerializeField]
    private new string name;
    public string Name { get { return name; } }

    [SerializeField, Multiline]
    private string description;

    [SerializeField]
    private List<Target> targets;
    public List<Target> Targets { get { return targets; } }

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
    public enum TargetType
    {
        PlayerCard = 6,
        Location = 7,
        Row = 8,
        Column = 9,
        EntireOrEmpty = 10,
        Enemy = 13
    }

    [SerializeField]
    private TargetType targetType;

    public int GetTargetTypeLayer()
    {
        return 1 << (int)targetType;
    }
}