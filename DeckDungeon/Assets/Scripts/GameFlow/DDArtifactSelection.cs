using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DDArtifactSelection : MonoBehaviour
{
    [SerializeField] private DDArtifactUI[] artifactUI;

    private List<DDArtifactBase> grabbedArtifacts;
    
    // Also make a funciton to take specific artifacts maybe from bosses?
    public void DisplayArtifacts()
    {
        grabbedArtifacts = DDGlobalManager.Instance.SelectedAdventurer.GrabArtifacts(3);

        for (int i = 0; i < artifactUI.Length; i++)
        {
            if (i < grabbedArtifacts.Count)
            {
                artifactUI[i].SetUpArtifact(grabbedArtifacts[i]);
                artifactUI[i].gameObject.SetActive(true);
            }
            else
            {
                artifactUI[i].gameObject.SetActive(false);
            }
        }
        
        gameObject.SetActive(true);
    }

    // Called from UI
    public void ArtifactSelected(int index)
    {
        for (int i = 0; i < grabbedArtifacts.Count; i++)
        {
            if (i == index)
            {
                DDGamePlaySingletonHolder.Instance.Dungeon.EquipArtifact(grabbedArtifacts[i]);
            }
            else
            {
                DDGlobalManager.Instance.SelectedAdventurer.ReturnArtifact(grabbedArtifacts[i]);
            }
        }
        
        DDGamePlaySingletonHolder.Instance.Dungeon.PromptDungeonCard();
    }
}
