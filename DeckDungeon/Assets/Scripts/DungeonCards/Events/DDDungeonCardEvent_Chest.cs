using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DDDungeonCardEvent_Chest : DDDungeonCardEvent
{
    [Header("Chest")]
    [SerializeField]
    private DDArtifactBase artifact;

    public override void DisplayEvent(DDEventArea area)
    {
        base.DisplayEvent(area);

        Button buttonOne = area.GenerateButton();
        buttonOne.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = artifact.ArtifactName;
        
        buttonOne.onClick.AddListener(() =>
        {
            SingletonHolder.Instance.Dungeon.EquipArtifact(artifact);
            SingletonHolder.Instance.Dungeon.PromptDungeonCard();
        });

        Button buttonTwo = area.GenerateButton();
        buttonTwo.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Insert More Artifacts";
        buttonTwo.onClick.AddListener(() =>
        {
            
        });
    }
}
