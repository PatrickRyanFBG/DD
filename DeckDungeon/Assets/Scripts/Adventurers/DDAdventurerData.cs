using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDAdventurerData : DDScriptableObject
{
    [SerializeField]
    private string adventureName;
    public string AdventureName => adventureName;

    [SerializeField, Multiline]
    private string adventureDecsription;
    public string AdventureDecsription => adventureDecsription;

    [SerializeField]
    private Texture heroShot;

    [SerializeField]
    private Texture gamePlayVisuals;

    [SerializeField]
    private int startingHealth;
    public int StartingHealth => startingHealth;

    [SerializeField]
    private int startingStrength;
    public int StartingStrength { get => startingStrength; set => startingStrength = value; }

    [SerializeField]
    private int startingDexterity;
    public int StartingDexterity { get => startingDexterity; set => startingDexterity = value; }

    [SerializeField]
    private int startingIntelligence;
    public int StartingIntelligence { get => startingIntelligence; set => startingIntelligence = value; }

    [SerializeField]
    private int startingGold;
    public int StartingGold { get => startingGold; set => startingGold = value; }

    [SerializeField]
    private List<DDArtifactBase> artifacts;
    public List<DDArtifactBase> Artifacts { get => artifacts; set => artifacts = value; }

    [SerializeField]
    private List<DDCardBase> cards;
    public List<DDCardBase> Cards => cards;

    [SerializeField]
    private int[] startingDeckByIndex;

    [Header("Testing")]
    [SerializeField]
    private bool comingSoon;

    [System.NonSerialized] 
    private bool didInit;

    public void SetUpUI(DDAdventurerSelection selectionUI)
    {
        selectionUI.AdventurerName.text = adventureName;
        selectionUI.AdventurerDescription.text = adventureDecsription;
        selectionUI.HeroShotImage.texture = heroShot;
    }

    private List<DDCardBase> startingDeck;
    public List<DDCardBase> StartingDeck => startingDeck;

    private List<DDCardBase> ownedCards;

    public void RuntimeInit()
    {
        if(didInit)
        {
            return;
        }

        didInit = true;

        string lastStartingDeck = PlayerPrefs.GetString(adventureName + PlayerPrefKeys.StartingDeckPostfix, "");
        if (string.IsNullOrEmpty(lastStartingDeck))
        {
            GenerateStartingDeck(startingDeckByIndex);
        }
        else
        {
            string[] split = lastStartingDeck.Split(',');
            int[] savedStartingDeckIndexes = new int[split.Length];

            for (int i = 0; i < split.Length; i++)
            {
                savedStartingDeckIndexes[i] = int.Parse(split[i]);
            }

            GenerateStartingDeck(savedStartingDeckIndexes);
        }
    }

    private void GenerateStartingDeck(int[] indexes)
    {
        startingDeck = new List<DDCardBase>(10);

        for (int i = 0; i < indexes.Length; i++)
        {
            // Now deep cloned to allow for in-game modifications
            startingDeck.Add(cards[indexes[i]].Clone());
        }
    }

    public List<DDCardBase> GenerateCards(int amount)
    {
        List<DDCardBase> cards = new List<DDCardBase>(amount);

        for (int i = 0; i < amount; i++)
        {
            int randNum = Random.Range(0, cards.Count);

            while (cards.Contains(cards[randNum]))
            {
                randNum = Random.Range(0, cards.Count);
            }

            cards.Add(cards[randNum]);
        }

        return cards;
    }
}
