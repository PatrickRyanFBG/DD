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

    [SerializeField] private bool awardsArtifacts;
    public bool AwardsArtifacts => awardsArtifacts;
    
    [SerializeField] private DDDungeonCardEvent eventAfterComplete;
    public DDDungeonCardEvent EventAfterComplete => eventAfterComplete;

    [SerializeField] private DDDungeonData dungeonAddedUponDefeat;

    public abstract void StartEncounter(ref HashSet<string> usedSetups);

    public virtual void SpawnEnemies(DDCombatEnemySetup enemySetup)
    {
        DDBoard board = DDGamePlaySingletonHolder.Instance.Board;

        List<Vector2Int> locs = new List<Vector2Int>();
        for (int i = 0; i < enemySetup.Enemies.Count; i++)
        {
            int count = enemySetup.Enemies[i].Amount;
            for (int j = 0; j < count; j++)
            {
                int xMin = enemySetup.Enemies[i].Enemy.XSpawnPreference ? enemySetup.Enemies[i].Enemy.XMinMax.x : 0;
                int xMax = enemySetup.Enemies[i].Enemy.XSpawnPreference
                    ? (enemySetup.Enemies[i].Enemy.XMinMax.y + 1)
                    : board.ColumnsCount;
                int x = Random.Range(xMin, xMax);

                int yMin = enemySetup.Enemies[i].Enemy.YSpawnPreference ? enemySetup.Enemies[i].Enemy.YMinMax.x : 0;
                int yMax = enemySetup.Enemies[i].Enemy.YSpawnPreference
                    ? (enemySetup.Enemies[i].Enemy.YMinMax.y + 1)
                    : board.RowCount;
                int y = Random.Range(yMin, yMax);

                Vector2Int nextPos = new Vector2Int(x, y);

                // TODO:: put some safety in to check if we've tried to spawn in all positions and can't find a spot
                // in the case of woodland boss the entire board is full sooooooooooooooo?
                while (locs.Contains(nextPos))
                {
                    x = Random.Range(xMin, xMax);
                    y = Random.Range(yMin, yMax);

                    nextPos = new Vector2Int(x, y);
                }

                locs.Add(nextPos);
                board.SpawnEnemy(x, y, enemySetup.Enemies[i].Enemy);
            }
        }
    }

    public virtual bool ShouldEndEarly()
    {
        return false;
    }

    public virtual IEnumerator EncounterCompleted()
    {
        if (cardsToShuffleInAfter.Count > 0)
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