using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class DDDungeonCardEvent_RatCatcher : DDDungeonCardEvent
{
    [Header("Rat Catcher")]
    [SerializeField]
    private int numberOfRatCards;

    [SerializeField]
    private DDDungeonCardBase ratCard;

    [SerializeField]
    private string releaseRatsText;

    [SerializeField]
    private string secondOptionText;

    [SerializeField, Multiline]
    private string afterReleaseDescription;

    public override void DisplayEvent(DDEventArea area)
    {
        base.DisplayEvent(area);

        Button buttonOne = area.GenerateButton();
        buttonOne.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = releaseRatsText;
        buttonOne.onClick.AddListener(() =>
        {
            for (int i = 0; i < numberOfRatCards; i++)
            {
                SingletonHolder.Instance.Dungeon.AddCardToDungeonDeck(ratCard);
            }

            area.Description.text = afterReleaseDescription;
            area.CleanUpButtons();

            Button okayButton = area.GenerateButton();
            okayButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Oh no!";
            okayButton.onClick.AddListener(() =>
            {
                SingletonHolder.Instance.Dungeon.PromptDungeonCard();
            });
        });

        Button buttonTwo = area.GenerateButton();
        buttonTwo.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = secondOptionText;
        buttonTwo.onClick.AddListener(() =>
        {
            for (int i = 0; i < numberOfRatCards; i++)
            {
                SingletonHolder.Instance.Dungeon.AddCardToDungeonDeck(ratCard);
            }

            area.Description.text = afterReleaseDescription;
            area.CleanUpButtons();

            Button okayButton = area.GenerateButton();
            okayButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Oh no!";
            okayButton.onClick.AddListener(() =>
            {
                SingletonHolder.Instance.Dungeon.PromptDungeonCard();
            });
        });
    }
}
