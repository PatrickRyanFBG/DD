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
        //base.DisplayEvent(area);

        artifactOne = DDGamePlaySingletonHolder.Instance.Dungeon.GrabArtifact();
        artifactTwo = DDGamePlaySingletonHolder.Instance.Dungeon.GrabArtifact();

        DDButton buttonOne = area.GenerateButton();
        buttonOne.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = artifactOne.ArtifactName;
        buttonOne.Button.onClick.AddListener(() =>
        {
            DDGamePlaySingletonHolder.Instance.Dungeon.EquipArtifact(artifactOne);
            if(artifactTwo != null)
            {
                DDGamePlaySingletonHolder.Instance.Dungeon.ReturnArtifact(artifactTwo);
            }
            DDGamePlaySingletonHolder.Instance.Dungeon.PromptDungeonCard();
        });

        buttonOne.OnPointerEnter.AddListener((UnityEngine.EventSystems.BaseEventData data) => 
        {
            area.Description.text = artifactOne.Description;
        });

        if(artifactTwo != null)
        {
            DDButton buttonTwo = area.GenerateButton();
            buttonTwo.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = artifactTwo.ArtifactName;
            buttonTwo.Button.onClick.AddListener(() =>
            {
                DDGamePlaySingletonHolder.Instance.Dungeon.EquipArtifact(artifactTwo);
                DDGamePlaySingletonHolder.Instance.Dungeon.ReturnArtifact(artifactOne);
                DDGamePlaySingletonHolder.Instance.Dungeon.PromptDungeonCard();
            });

            buttonTwo.OnPointerEnter.AddListener((UnityEngine.EventSystems.BaseEventData data) =>
            {
                area.Description.text = artifactTwo.Description;
            });
        }
    }
}
