using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class DDDungeonCardBase : DDScriptableObject
{
    [SerializeField] private EDungeonCardType type;
    public EDungeonCardType Type => type;

    [SerializeField] private Texture image;
    public Texture Image => image;

    [SerializeField] private new string name;
    public string Name => name;

    [SerializeField, Multiline] private string description;
    public string Description => description;

    public virtual bool SelectCard()
    {
        if (CanSelect())
        {
            DDGamePlaySingletonHolder.Instance.Dungeon.DungeonCardSelected(this);
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

    public virtual T Get<T>() where T : DDDungeonCardBase
    {
        return (T)this;
    }
}

[System.Serializable]
public abstract class DDDungeonCardEncounter : DDDungeonCardBase
{
    [SerializeField] private EEncounterType encounterType;
    public EEncounterType EncounterType => encounterType;
    
    [SerializeField] private int goldToGive;
    public int GoldToGive => goldToGive;

    [SerializeField] private List<DDDungeonCardBase> cardsToShuffleInAfter;

    [SerializeField] private DDDungeonCardEvent eventAfterComplete;
    public DDDungeonCardEvent EventAfterComplete => eventAfterComplete;
    
    [SerializeField] private DDDungeonData dungeonAddedUponDefeat;

    public abstract void SpawnEnemies();

    public virtual IEnumerator EncounterCompleted()
    {
        if (cardsToShuffleInAfter != null)
        {
            yield return DDGamePlaySingletonHolder.Instance.Dungeon.AddCardToDungeonDeckOvertime(cardsToShuffleInAfter);
        }

        DDGamePlaySingletonHolder.Instance.Dungeon.AddOrRemoveGold(goldToGive);
    }
}

[System.Serializable]
public abstract class DDDungeonCardEvent : DDDungeonCardBase
{
    public abstract void DisplayEvent(DDEventArea area);
}

[System.Serializable]
public abstract class DDDungeonCardLeisure : DDDungeonCardBase
{
    [Header("Leisure")] [SerializeField] private string leisureName;

    [SerializeField, Multiline] private string leisureDescription;

    [SerializeField] private Texture leisureImage;

    public virtual void DisplayLeisure(DDLeisureArea area)
    {
        area.LeisureName.text = leisureName;
        area.Description.text = leisureDescription;
        area.Image.texture = leisureImage;
    }
}

[System.Serializable]
public abstract class DDDungeonCardShop : DDDungeonCardBase
{
    [Header("Shop")] [SerializeField] private string shopKeepName;

    [SerializeField, Multiline] private string shopDialogue;

    [SerializeField] private Texture shopImage;

    public virtual void DisplayShop(DDShopArea area)
    {
        area.ShopName.text = shopKeepName;
        area.Description.text = shopDialogue;
        area.Image.texture = shopImage;
    }
}