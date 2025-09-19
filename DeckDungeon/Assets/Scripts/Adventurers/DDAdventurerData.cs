using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDAdventurerData : DDScriptableObject
{
    [SerializeField] private string adventureName;
    public string AdventureName => adventureName;

    [SerializeField, Multiline] private string adventureDecsription;
    public string AdventureDecsription => adventureDecsription;

    [SerializeField] private Texture heroShot;

    [SerializeField] private Texture gamePlayVisuals;

    [SerializeField] private int startingHealth;
    public int StartingHealth => startingHealth;

    [SerializeField] private int startingGold;
    public int StartingGold => startingGold;

    // Add Starting Artifacts here
    [SerializeField] private List<DDArtifactBase> artifacts;
    [System.NonSerialized] private List<DDArtifactBase> runtimeArtifacts;
    
    [SerializeField] private DDAdventurerCardData cardData;
    public DDAdventurerCardData CardData => cardData;

    [SerializeField] private DDCardBase[] startingDeckByScriptable;

    [Header("Testing")] [SerializeField] private bool comingSoon;

    [System.NonSerialized] private bool didInit;

    public void SetUpUI(DDAdventurerSelection selectionUI)
    {
        selectionUI.AdventurerName.text = adventureName;
        selectionUI.AdventurerDescription.text = adventureDecsription;
        selectionUI.HeroShotImage.texture = heroShot;
    }

    [System.NonSerialized] private List<DDCardBase> startingDeck;
    public List<DDCardBase> StartingDeck => startingDeck;

    private List<DDCardBase> ownedCards;

    public void RuntimeInit()
    {
        if (didInit)
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
        
        runtimeArtifacts = new List<DDArtifactBase>(artifacts);
    }

    private void GenerateStartingDeck(string[] guids)
    {
        startingDeck = new List<DDCardBase>(10);

        for (int i = 0; i < guids.Length; i++)
        {
            // Now deep cloned to allow for in-game modifications
            startingDeck.Add(cardData.GetCardByGuid(guids[i]).Clone(false));
        }
    }

    public DDArtifactBase GrabArtifact()
    {
        if (runtimeArtifacts.Count == 0)
        {
            return null;
        }

        int index = Random.Range(0, runtimeArtifacts.Count);
        DDArtifactBase artifact = runtimeArtifacts[index];
        runtimeArtifacts.RemoveAt(index);
        return artifact;
    }

    public List<DDArtifactBase> GrabArtifacts(int amount)
    {
        if (runtimeArtifacts.Count == 0)
        {
            return null;
        }

        List<DDArtifactBase> grabbedArtifacts = new List<DDArtifactBase>(amount);

        for (int i = 0; i < amount; i++)
        {
            DDArtifactBase artifact = GrabArtifact();
            if (artifact)
            {
                grabbedArtifacts.Add(artifact);
            }
            else
            {
                break;
            }
        }

        return grabbedArtifacts;
    }

    public void ReturnArtifact(DDArtifactBase artifact)
    {
        runtimeArtifacts.Add(artifact);
    }
}