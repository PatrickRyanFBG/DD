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
        base.DisplayEvent(area);

        artifactOne = SingletonHolder.Instance.Dungeon.GrabArtifact();
        artifactTwo = SingletonHolder.Instance.Dungeon.GrabArtifact();

        DDButton buttonOne = area.GenerateButton();
        buttonOne.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = artifactOne.ArtifactName;
        buttonOne.Button.onClick.AddListener(() =>
        {
            SingletonHolder.Instance.Dungeon.EquipArtifact(artifactOne);
            if(artifactTwo != null)
            {
                SingletonHolder.Instance.Dungeon.ReturnArtifact(artifactTwo);
            }
            SingletonHolder.Instance.Dungeon.PromptDungeonCard();
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
                SingletonHolder.Instance.Dungeon.EquipArtifact(artifactTwo);
                SingletonHolder.Instance.Dungeon.ReturnArtifact(artifactOne);
                SingletonHolder.Instance.Dungeon.PromptDungeonCard();
            });

            buttonTwo.OnPointerEnter.AddListener((UnityEngine.EventSystems.BaseEventData data) =>
            {
                area.Description.text = artifactTwo.Description;
            });
        }
    }
}
