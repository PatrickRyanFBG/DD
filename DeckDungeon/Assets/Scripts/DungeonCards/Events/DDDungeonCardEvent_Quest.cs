using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DDDungeonCardEvent_Quest : DDDungeonCardEvent
{
    [Header("Quest")]
    [SerializeField, Multiline]
    private string secondDescription;

    public override void DisplayEvent(DDEventArea area)
    {
        base.DisplayEvent(area);

        Button buttonOne = area.GenerateButton();
        buttonOne.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Follow the mysterious figure.";
        buttonOne.onClick.AddListener(() =>
        {
            area.Description.text = secondDescription;
            area.CleanUpButtons();

            Button okayButton = area.GenerateButton();
            okayButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Wow that is cool!";
            okayButton.onClick.AddListener(() =>
            {
                SingletonHolder.Instance.Dungeon.PromptDungeonCard();
            });
        });

        Button buttonTwo = area.GenerateButton();
        buttonTwo.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Loot their camp. (100 gold)";
        buttonTwo.onClick.AddListener(() =>
        {
            area.Description.text = secondDescription;//"Well gold isn't added into this demo, so that stinks, but secret is the other button didn't do anything either";
            area.CleanUpButtons();

            Button okayButton = area.GenerateButton();
            okayButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Wow that is cool!";
            okayButton.onClick.AddListener(() =>
            {
                SingletonHolder.Instance.Dungeon.PromptDungeonCard();
            });
        });
    }
}
