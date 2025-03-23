using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DDPlayerDeckBuilder : MonoBehaviour
{
    /*
    [SerializeField]
    private DDAdventurerSelection selectionPrefab;

    [SerializeField]
    private List<Transform> spots;
    */

    [SerializeField]
    private DDAdventurerSelection[] selections;

    [SerializeField]
    private GameObject selectionParent;

    [SerializeField]
    private GameObject deckBuilderParent;

    [SerializeField]
    private DDShowDeckArea showDeckArea;

    [Header("Testing")]
    [SerializeField]
    private bool testing;

    [SerializeField]
    private DDAdventurerData testingData;

    private void OnEnable()
    {
        if(testing)
        {
            testingData.SetUpUI(selections[0]);
            selections[0].SelectionButton.onClick.AddListener(() =>
            {
                // TODO GOTTA FIX THIS SHIT!~
                MadeSelection(0);
            });
            return;
        }

        if(DDGlobalManager.Instance.SelectedAdventurer == null)
        {
            var data = DDGlobalManager.Instance.AdventurerDataLibrary.AdventureDatas;
            for (int i = 0; i < DDGlobalManager.Instance.AdventurerDataLibrary.AdventureDatas.Count; i++)
            {
                data[i].SetUpUI(selections[i]);
                selections[i].SelectionButton.onClick.AddListener(() => 
                {
                    // TODO GOTTA FIX THIS SHIT!~
                    MadeSelection(0);
                });
            }
        }
        else
        {
            ToggleToBuilder();
        }
    }

    private void OnDisable()
    {
        for (int i = 0; i < selections.Length; i++)
        {
            selections[i].SelectionButton.onClick.RemoveAllListeners();
        }
    }

    public void MadeSelection(int index)
    {
        Debug.Log("Made a selection: " + index);
        DDGlobalManager.Instance.SetAdventurer(testingData);
        ToggleToBuilder();
    }

    private void ToggleToBuilder()
    {
        selectionParent.SetActive(false);
        showDeckArea.ShowPlayerDeck(DDGlobalManager.Instance.SelectedAdventurer.StartingDeck);
        showDeckArea.gameObject.SetActive(true);
        deckBuilderParent.SetActive(true);
    }

    public void GoToDungeonBuilder()
    {
        SceneManager.LoadScene(2);
    }
}
