using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class DDDungeonCardBase : DDScriptableObject
{
    [SerializeField]
    private EDungeonCardType type;
    public EDungeonCardType Type { get { return type; } }

    [SerializeField]
    private Texture image;
    public Texture Image { get { return image; } }

    [SerializeField]
    private new string name;
    public string Name { get { return name; } }

    [SerializeField, Multiline]
    private string description;
    public string Description { get { return description; } }

    public virtual bool SelectCard()
    {
        if (CanSelect())
        {
            SingletonHolder.Instance.Dungeon.DungeonCardSelected(this);
            return true;
        }

        return false;
    }

    public virtual bool CanSelect()
    {
        return true;
    }

    public virtual void DisplayInformation(DDDungeonCardShown cardShown)
    {
        cardShown.Image.texture = image;
        cardShown.NameText.text = name;
        cardShown.DescText.text = description;
    }

    // Card Drawn
    // Card Discard
}

[System.Serializable]
public abstract class DDDungeonCardEncounter : DDDungeonCardBase
{
    [SerializeField]
    protected bool testingHasKey;
    public bool TestingHasKey { get => testingHasKey; }

    [SerializeField]
    protected bool testingHasChest;
    public bool TestingHasChest { get => testingHasChest; }

    public abstract void SpawnEnemies();
}

[System.Serializable]
public abstract class DDDungeonCardEvent : DDDungeonCardBase
{
    [Header("Event")]
    [SerializeField]
    private string eventName;

    [SerializeField, Multiline]
    private string eventDescription;

    [SerializeField]
    private Texture eventImage;

    public virtual void DisplayEvent(DDEventArea area)
    {
        area.EventName.text = eventName;
        area.Description.text = eventDescription;
        area.Image.texture = eventImage;
    }
}

[System.Serializable]
public abstract class DDDungeonCardLeisure : DDDungeonCardBase
{
    [Header("Leisure")]
    [SerializeField]
    private string leisureName;

    [SerializeField, Multiline]
    private string leisureDescription;

    [SerializeField]
    private Texture leisureImage;

    public virtual void DisplayLeisure(DDLeisureArea area)
    {
        area.LeisureName.text = leisureName;
        area.Description.text = leisureDescription;
        area.Image.texture = leisureImage;
    }
}