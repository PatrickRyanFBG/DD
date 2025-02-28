using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DDDungeonEventDataIllicitDealer : DDDungeonEventData
{
    [Header("Illicit Dealer")] [SerializeField] private DDArtifactBase alphaArtifact;
    
    [SerializeField] private DDArtifactBase omegaArtifact;

    [SerializeField] private string alphaText;

    [SerializeField] private string omegaText;

    [SerializeField, Multiline] private string afterSelectionText;

    public override void DisplayEvent(DDEventArea area)
    {
        base.DisplayEvent(area);

        DDButton buttonOne = area.GenerateButton(alphaText);
        buttonOne.Button.onClick.AddListener(() =>
        {
            DDGamePlaySingletonHolder.Instance.Dungeon.EquipArtifact(alphaArtifact);
            AfterSelection(area);
        });
        
        buttonOne.OnPointerEnter.AddListener((BaseEventData) =>
        {
            area.Description.text = alphaArtifact.Description;
        });

        DDButton buttonTwo = area.GenerateButton(omegaText);
        buttonTwo.Button.onClick.AddListener(() =>
        {
            DDGamePlaySingletonHolder.Instance.Dungeon.EquipArtifact(omegaArtifact);
            AfterSelection(area);
        });
        
        buttonTwo.OnPointerEnter.AddListener((BaseEventData) =>
        {
            area.Description.text = omegaArtifact.Description;
        });

        DDButton buttonThree = area.GenerateButton("Leave");
        buttonOne.Button.onClick.AddListener(() =>
        {
            DDGamePlaySingletonHolder.Instance.Dungeon.EndEvent();
        });
    }

    private void AfterSelection(DDEventArea area)
    {
        area.CleanUpButtons();
        area.Description.text = afterSelectionText;
        
        DDButton buttonOne = area.GenerateButton("Leave");
        buttonOne.Button.onClick.AddListener(() =>
        {
            DDGamePlaySingletonHolder.Instance.Dungeon.EndEvent();
        });
    }
}