using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDAdventurerData : DDScriptableObject
{
    [SerializeField]
    private string adventureName;
    public string AdventureName { get => adventureName; }

    [SerializeField, Multiline]
    private string adventureDecsription;
    public string AdventureDecsription { get => adventureDecsription; }

    [SerializeField]
    private Texture heroShot;

    [SerializeField]
    private Texture gamePlayVisuals;

    [SerializeField]
    private int startingStrength;

    [SerializeField]
    private int startingDexterity;

    [SerializeField]
    private int startingIntelligence;

    [SerializeField]
    private List<DDArtifactBase> artifacts;

    [SerializeField]
    private List<DDCardBase> cards;

    [SerializeField]
    private int[] startingDeckByIndex;

    [Header("Testing")]
    [SerializeField]
    private bool comingSoon;

    public void SetUpUI(DDAdventurerSelection selectionUI)
    {
        selectionUI.AdventurerName.text = adventureName;
        selectionUI.AdventurerDescription.text = adventureDecsription;
        selectionUI.HeroShotImage.texture = heroShot;
    }

    private List<DDCardBase> startingDeck;
    public List<DDCardBase> StartingDeck { get => startingDeck; }

    private List<DDCardBase> ownedCards;

    public void RuntimeInit()
    {
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
            startingDeck.Add(cards[indexes[i]]);
        }
    }
}
