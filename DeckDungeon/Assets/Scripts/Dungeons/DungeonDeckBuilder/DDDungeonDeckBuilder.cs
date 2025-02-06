using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class DDDungeonDeckBuilder : MonoBehaviour
{
    // Gotta make this a list and prefab and shit
    [SerializeField]
    private DDDungeonSelection[] dungeonSelections;

    [SerializeField]
    private DDSideQuestSelection[] sideQuestSelections;

    [SerializeField]
    private GameObject selectionParent;

    [SerializeField]
    private GameObject sideQuestParent;

    [Header("Testing")]
    [SerializeField]
    private bool testing;

    [SerializeField]
    private DDDungeonMetaData testingData;

    [SerializeField]
    private DDDungeonSideQuestData testingSideQuestData;

    private void OnEnable()
    {
        if (testing)
        {
            testingData.SetUpUI(dungeonSelections[0]);

            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((data) => 
            {
                MadeDungeonSelection(0);
            });
            dungeonSelections[0].Events.triggers.Add(entry);

            testingSideQuestData.SetUpUI(0, sideQuestSelections[0]);
            sideQuestSelections[0].SelectionButton.onValueChanged.AddListener((selected) =>
            {
                MadeSideQuestSelection(0, selected);
            });

            return;
        }

        if (DDGlobalManager.Instance.SelectedDungeon == null)
        {
            var data = DDGlobalManager.Instance.DungeonDataLibrary.MetaDatas;
            for (int i = 0; i < data.Count; i++)
            {
                data[i].SetUpUI(dungeonSelections[i]);
                int index = i;

                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerClick;
                entry.callback.AddListener((data) =>
                {
                    MadeDungeonSelection(index);
                });
                dungeonSelections[i].Events.triggers.Add(entry);
            }
        }
    }

    public void MadeDungeonSelection(int index)
    {
        DDGlobalManager.Instance.SetDungeon(testingData);
        ToggleToSideQuests();
    }

    private void ToggleToSideQuests()
    {
        selectionParent.SetActive(false);
        sideQuestParent.SetActive(true);
    }

    public void MadeSideQuestSelection(int index, bool toggle)
    {
        DDGlobalManager.Instance.AddRemoveSideQuest(DDGlobalManager.Instance.DungeonDataLibrary.SideQuestDatas[index], toggle);
    }

    public void StartGame()
    {
        SceneManager.LoadScene(3);
    }
}
