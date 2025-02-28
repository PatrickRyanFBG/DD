using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DDDungeonEventDataCrypt : DDDungeonEventData
{
    [Header("Crypt")] [SerializeField] private string lootGoldText;

    [SerializeField, Multiline] private string afterLootGoldDescription;

    [SerializeField] private int goldAmount = 200;

    [SerializeField] private string returnCardText;

    [SerializeField, Multiline] private string afterReturnCardDescription;

    public override void DisplayEvent(DDEventArea area)
    {
        base.DisplayEvent(area);

        DDButton buttonOne = area.GenerateButton(lootGoldText);
        buttonOne.Button.onClick.AddListener(() =>
        {
            DDGamePlaySingletonHolder.Instance.Dungeon.AddOrRemoveGold(goldAmount);

            area.Description.text = afterLootGoldDescription;
            area.CleanUpButtons();

            DDButton okayButton = area.GenerateButton("Oops!");
            okayButton.Button.onClick.AddListener(() => { DDGamePlaySingletonHolder.Instance.Dungeon.EndEvent(); });
        });

        DDButton buttonTwo = area.GenerateButton(returnCardText);
        buttonTwo.Button.onClick.AddListener(() =>
        {
            DDGamePlaySingletonHolder.Instance.Dungeon.DisplayDungeonDiscard();
            DDGamePlaySingletonHolder.Instance.ShowDeckArea.DungeonCardSelectedCallback
                .AddListener(DungeonCardSelected);

            area.Description.text = afterReturnCardDescription;
            area.CleanUpButtons();

            DDButton okayButton = area.GenerateButton("Thoughts and Prayers.");
            okayButton.Button.onClick.AddListener(() =>
            {
                DDGamePlaySingletonHolder.Instance.ShowDeckArea.DungeonCardSelectedCallback.RemoveListener(
                    DungeonCardSelected);
                DDGamePlaySingletonHolder.Instance.Dungeon.EndEvent();
            });
        });
    }

    public void DungeonCardSelected(DDDungeonCardBase selectedCard)
    {
        DDGamePlaySingletonHolder.Instance.Dungeon.AddCardToDungeonDeck(selectedCard);
        DDGamePlaySingletonHolder.Instance.Dungeon.RemoveCardFromDungeonDiscard(selectedCard);
        DDGamePlaySingletonHolder.Instance.ShowDeckArea.DungeonCardSelectedCallback.RemoveListener(DungeonCardSelected);
        DDGamePlaySingletonHolder.Instance.Dungeon.DisplayDeckClosed();
    }

    public override bool EventIsValid()
    {
        return DDGamePlaySingletonHolder.Instance.Dungeon.DungeonDiscardCount > 0;
    }
}