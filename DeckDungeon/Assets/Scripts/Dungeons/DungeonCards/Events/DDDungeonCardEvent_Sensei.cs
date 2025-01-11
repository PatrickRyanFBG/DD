using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class DDDungeonCardEvent_Sensei : DDDungeonCardEvent
{
    [Header("Sensei")]
    [SerializeField]
    private List<DDCardBase> availableCards;

    [SerializeField]
    private string trainText;

    [SerializeField]
    private string secondOptionText;

    public override void DisplayEvent(DDEventArea area)
    {
        base.DisplayEvent(area);

        DDButton buttonOne = area.GenerateButton();
        buttonOne.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = trainText;
        buttonOne.Button.onClick.AddListener(() =>
        {
            DDGamePlaySingletonHolder.Instance.Dungeon.PromptPlayerCard(availableCards);
        });

        DDButton buttonTwo = area.GenerateButton();
        buttonTwo.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = secondOptionText;
    }
}
