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

        DDButton buttonOne = area.GenerateButton();
        buttonOne.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = releaseRatsText;
        buttonOne.Button.onClick.AddListener(() =>
        {
            List<DDDungeonCardBase> cards = new List<DDDungeonCardBase>();
            for (int i = 0; i < numberOfRatCards; i++)
            {
                cards.Add(ratCard);
            }
            SingletonHolder.Instance.Dungeon.AddCardToDungeonDeck(cards);

            area.Description.text = afterReleaseDescription;
            area.CleanUpButtons();

            DDButton okayButton = area.GenerateButton();
            okayButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Oh no!";
            okayButton.Button.onClick.AddListener(() =>
            {
                SingletonHolder.Instance.Dungeon.PromptDungeonCard();
            });
        });

        DDButton buttonTwo = area.GenerateButton();
        buttonTwo.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = secondOptionText;
        buttonTwo.Button.onClick.AddListener(() =>
        {
            for (int i = 0; i < numberOfRatCards; i++)
            {
                SingletonHolder.Instance.Dungeon.AddCardToDungeonDeck(ratCard);
            }

            area.Description.text = afterReleaseDescription;
            area.CleanUpButtons();

            DDButton okayButton = area.GenerateButton();
            okayButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Oh no!";
            okayButton.Button.onClick.AddListener(() =>
            {
                SingletonHolder.Instance.Dungeon.PromptDungeonCard();
            });
        });
    }
}
