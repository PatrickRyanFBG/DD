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
            if (!instance)
            {
                Instantiate(Resources.Load<GameObject>("Prefabs/GlobalManager"));
            }

            return instance;
        }
    }

    [Header("Global Singletons")] [SerializeField]
    private DDAdventurerDataLibrary adventurerDataLibrary;

    public DDAdventurerDataLibrary AdventurerDataLibrary => adventurerDataLibrary;

    private DDAdventurerData selectedAdventurer;

    public DDAdventurerData SelectedAdventurer
    {
        get
        {
            if (FromMainMenu)
            {
                return selectedAdventurer;
            }
            else
            {
                debugAdventurer.RuntimeInit();
                return debugAdventurer;
            }
        }
    }

    [SerializeField] private DDDungeonDataLibrary dungeonDataLibrary;
    public DDDungeonDataLibrary DungeonDataLibrary => dungeonDataLibrary;

    private DDDungeonMetaData selectedDungeon;

    public DDDungeonMetaData SelectedDungeon
    {
        get
        {
            if (FromMainMenu)
            {
                return selectedDungeon;
            }
            else
            {
                return debugDungeon;
            }
        }
    }

    private List<DDDungeonSideQuestData> sideQuests = new List<DDDungeonSideQuestData>();

    public List<DDDungeonSideQuestData> SideQuests
    {
        get
        {
            if (FromMainMenu)
            {
                return sideQuests;
            }
            else
            {
                return debugSideQuests;
            }
        }
    }

    [SerializeField] private DDEventDataLibrary eventDataLibrary;
    public DDEventDataLibrary EventDataLibrary => eventDataLibrary;
    
    [SerializeField] private DDAffixLibrary affixLibrary;
    public DDAffixLibrary AffixLibrary => affixLibrary;

    [SerializeField] private DDCardFinishLibrary cardFinishLibrary;
    public DDCardFinishLibrary CardFinishLibrary => cardFinishLibrary;
    
    [Header("Testing")] public bool FromMainMenu;

    [SerializeField] private DDAdventurerData debugAdventurer;

    [SerializeField] private DDDungeonMetaData debugDungeon;

    [SerializeField] private List<DDDungeonSideQuestData> debugSideQuests;

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
        if (adding)
        {
            sideQuests.Add(data);
        }
        else
        {
            sideQuests.Remove(data);
        }
    }
}