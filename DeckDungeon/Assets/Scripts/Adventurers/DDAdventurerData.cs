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
    private DDAdventurerCardData cardData;
    public DDAdventurerCardData CardData => cardData;

    [SerializeField]
    private DDCardBase[] startingDeckByScriptable;

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

    [System.NonSerialized]
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

        cardData.Init();
        
        startingDeck = new List<DDCardBase>(startingDeckByScriptable.Length);
        
        string lastStartingDeck = PlayerPrefs.GetString(adventureName + PlayerPrefKeys.StartingDeckPostfix, "");
        if (string.IsNullOrEmpty(lastStartingDeck))
        {
            for (int i = 0; i < startingDeckByScriptable.Length; i++)
            {
                startingDeck.Add(startingDeckByScriptable[i].Clone(true));
            }
        }
        else
        {
            string[] split = lastStartingDeck.Split(',');
            GenerateStartingDeck(split);
        }
    }

    private void GenerateStartingDeck(string[] guids)
    {
        startingDeck = new List<DDCardBase>(10);

        for (int i = 0; i < guids.Length; i++)
        {
            // Now deep cloned to allow for in-game modifications
            startingDeck.Add(cardData.GetCardByGUID(guids[i]).Clone(false));
        }
    }
}
