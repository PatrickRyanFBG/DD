using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDGlobalManager : MonoBehaviour
{
    private static DDGlobalManager instance;
    public static DDGlobalManager Instance 
    { 
        get 
        { 
            if(instance == null)
            {
                Instantiate(Resources.Load<GameObject>("Prefabs/GlobalManager"));
            }
            return instance; 
        } 
    }

    [Header("Global Singletons")]
    [SerializeField]
    private DDAdventurerDataLibrary adventurerDataLibrary;
    public DDAdventurerDataLibrary AdventurerDataLibrary { get => adventurerDataLibrary; }

    private DDAdventurerData selectedAdventurer;
    public DDAdventurerData SelectedAdventurer { get => selectedAdventurer; }

    [SerializeField]
    private DDDungeonDataLibrary dungeonDataLibrary;
    public DDDungeonDataLibrary DungeonDataLibrary { get => dungeonDataLibrary; }

    private DDDungeonMetaData selectedDungeon;
    public DDDungeonMetaData SelectedDungeon { get => selectedDungeon; }

    private List<DDDungeonSideQuestData> sideQuests = new List<DDDungeonSideQuestData>();

    private List<DDDungeonCardBase> startingDungeonDeck = new List<DDDungeonCardBase>();

    [Header("Testing")]
    public bool FromMainMenu;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetAdventurer(DDAdventurerData data)
    {
        selectedAdventurer = data;
        selectedAdventurer.RuntimeInit();
    }

    public void SetDungeon(DDDungeonMetaData data)
    {
        selectedDungeon = data;
    }

    public void AddRemoveSideQuest(DDDungeonSideQuestData data, bool adding)
    {
        if(adding)
        {
            sideQuests.Add(data);
        }
        else
        {
            sideQuests.Remove(data);
        }
    }
}
