using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DDDungeonCardEvent_Chest : DDDungeonCardEvent
{
    private DDArtifactBase artifactOne;
    private DDArtifactBase artifactTwo;

    public override void DisplayEvent(DDEventArea area)
    {
        artifactOne = DDGlobalManager.Instance.SelectedAdventurer.GrabArtifact();
        artifactTwo = DDGlobalManager.Instance.SelectedAdventurer.GrabArtifact();

        DDButton buttonOne = area.GenerateButton(artifactOne.ArtifactName);
        buttonOne.Button.onClick.AddListener(() =>
        {
            DDGamePlaySingletonHolder.Instance.Dungeon.EquipArtifact(artifactOne);
            if(artifactTwo)
            {
                DDGlobalManager.Instance.SelectedAdventurer.ReturnArtifact(artifactTwo);
            }
            DDGamePlaySingletonHolder.Instance.Dungeon.PromptDungeonCard();
        });

        buttonOne.OnPointerEnter.AddListener((UnityEngine.EventSystems.BaseEventData data) => 
        {
            area.Description.text = artifactOne.Description;
        });

        if(artifactTwo)
        {
            DDButton buttonTwo = area.GenerateButton(artifactTwo.ArtifactName);
            buttonTwo.Button.onClick.AddListener(() =>
            {
                DDGamePlaySingletonHolder.Instance.Dungeon.EquipArtifact(artifactTwo);
                DDGlobalManager.Instance.SelectedAdventurer.ReturnArtifact(artifactOne);
                DDGamePlaySingletonHolder.Instance.Dungeon.PromptDungeonCard();
            });

            buttonTwo.OnPointerEnter.AddListener((UnityEngine.EventSystems.BaseEventData data) =>
            {
                area.Description.text = artifactTwo.Description;
            });
        }
    }
}
