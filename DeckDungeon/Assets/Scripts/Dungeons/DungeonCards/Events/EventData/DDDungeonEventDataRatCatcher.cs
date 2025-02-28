using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class DDDungeonEventDataRatCatcher : DDDungeonEventData
{
    [Header("Rat Catcher")]
    [SerializeField]
    private int numberOfRatCards;

    [SerializeField]
    private DDDungeonCardBase ratCard;

    [SerializeField]
    private string releaseRatsText;

    [SerializeField, Multiline]
    private string afterReleaseDescription;

    [SerializeField]
    private string payGoldOptionText;

    [SerializeField, Multiline]
    private string afterPayDescription;

    public override void DisplayEvent(DDEventArea area)
    {
        base.DisplayEvent(area);

        DDButton buttonOne = area.GenerateButton(releaseRatsText);
        buttonOne.Button.onClick.AddListener(() =>
        {
            List<DDDungeonCardBase> cards = new List<DDDungeonCardBase>();
            for (int i = 0; i < numberOfRatCards; i++)
            {
                cards.Add(ratCard);
            }
            DDGamePlaySingletonHolder.Instance.Dungeon.AddCardToDungeonDeck(cards);

            area.Description.text = afterReleaseDescription;
            area.CleanUpButtons();

            DDButton okayButton = area.GenerateButton("Oh no!");
            okayButton.Button.onClick.AddListener(() =>
            {
                DDGamePlaySingletonHolder.Instance.Dungeon.EndEvent();
            });
        });

        DDButton buttonTwo = area.GenerateButton(payGoldOptionText);
        buttonTwo.Button.onClick.AddListener(() =>
        {
            if(DDGamePlaySingletonHolder.Instance.Dungeon.HasEnoughGold(100))
            {
                DDGamePlaySingletonHolder.Instance.Dungeon.AddOrRemoveGold(-100);

                area.Description.text = afterPayDescription;
                area.CleanUpButtons();

                DDButton okayButton = area.GenerateButton("Awkward.");
                okayButton.Button.onClick.AddListener(() =>
                {
                    DDGamePlaySingletonHolder.Instance.Dungeon.EndEvent();
                });
            }
            else
            {
                List<DDDungeonCardBase> cards = new List<DDDungeonCardBase>();
                for (int i = 0; i < numberOfRatCards; i++)
                {
                    cards.Add(ratCard);
                }
                DDGamePlaySingletonHolder.Instance.Dungeon.AddCardToDungeonDeck(cards);

                area.Description.text = afterReleaseDescription;
                area.CleanUpButtons();

                DDButton okayButton = area.GenerateButton("Oh no!");
                okayButton.Button.onClick.AddListener(() =>
                {
                    DDGamePlaySingletonHolder.Instance.Dungeon.EndEvent();
                });
            }
        });
    }
}
