using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class DDDungeonEventDataSensei : DDDungeonEventData
{
    [Header("Sensei")] [SerializeField] private DDArtifactBase wayOfFistArtifact;
    
    [SerializeField] private DDArtifactBase wayOfSwordArtifact;

    [SerializeField] private string wayOfFistText;

    [SerializeField] private string wayOfSwordText;

    [SerializeField, Multiline] private string afterSelectionText;

    public override void DisplayEvent(DDEventArea area)
    {
        base.DisplayEvent(area);

        DDButton buttonOne = area.GenerateButton(wayOfFistText);
        buttonOne.Button.onClick.AddListener(() =>
        {
            DDGamePlaySingletonHolder.Instance.Dungeon.EquipArtifact(wayOfFistArtifact);
            AfterSelection(area);
        });

        DDButton buttonTwo = area.GenerateButton(wayOfSwordText);
        buttonTwo.Button.onClick.AddListener(() =>
        {
            DDGamePlaySingletonHolder.Instance.Dungeon.EquipArtifact(wayOfSwordArtifact);
            AfterSelection(area);
        });
    }

    private void AfterSelection(DDEventArea area)
    {
        area.CleanUpButtons();
        area.Description.text = afterSelectionText;
        
        DDButton buttonOne = area.GenerateButton("Bow and Leave");
        buttonOne.Button.onClick.AddListener(() =>
        {
            DDGamePlaySingletonHolder.Instance.Dungeon.EndEvent();
        });
    }
}