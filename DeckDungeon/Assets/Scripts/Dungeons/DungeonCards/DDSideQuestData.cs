using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDDungeonSideQuestData : DDScriptableObject
{
    [SerializeField]
    private string sideQuestName;

    [SerializeField, Multiline]
    private List<string> sideQuestDecsription;

    [SerializeField]
    private List<Texture> heroShots;

    [SerializeField]
    private List<DDDungeonCardBase> cards;

    public void SetUpUI(int questStep, DDSideQuestSelection selectionUI)
    {
        selectionUI.SideQuestName.text = sideQuestName;
        selectionUI.SideQuestDescription.text = sideQuestDecsription[questStep] + "/r/n " + (questStep + 1) + " / " + cards.Count;
        selectionUI.HeroShotImage.texture = heroShots[questStep];
    }
}
