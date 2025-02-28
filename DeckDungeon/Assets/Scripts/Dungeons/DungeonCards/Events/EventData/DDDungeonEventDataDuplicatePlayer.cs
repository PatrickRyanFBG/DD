using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDDungeonEventDataDuplicatePlayer : DDDungeonEventData
{
    [Header("Duplicate")] [SerializeField] private string duplicateText;

    [SerializeField, Multiline] private string afterDuplicateDescription;

    public override void DisplayEvent(DDEventArea area)
    {
        base.DisplayEvent(area);

        DDButton buttonOne = area.GenerateButton(duplicateText);
        buttonOne.Button.onClick.AddListener(() =>
        {
            DDGamePlaySingletonHolder.Instance.Dungeon.DisplayPlayerDeck();
            DDGamePlaySingletonHolder.Instance.ShowDeckArea.CardSelectedCallback
                .AddListener(PlayerCardSelected);

            area.Description.text = afterDuplicateDescription;
            area.CleanUpButtons();

            DDButton okayButton = area.GenerateButton("Leave.");
            okayButton.Button.onClick.AddListener(() =>
            {
                DDGamePlaySingletonHolder.Instance.ShowDeckArea.CardSelectedCallback.RemoveListener(
                    PlayerCardSelected);
                DDGamePlaySingletonHolder.Instance.Dungeon.EndEvent();
            });
        });

        DDButton buttonTwo = area.GenerateButton("Leave");
        buttonTwo.Button.onClick.AddListener(() =>
        {
            DDGamePlaySingletonHolder.Instance.Dungeon.EndEvent();
        });
    }

    private void PlayerCardSelected(DDCardBase selectedCard)
    {
        DDCardBase copy = selectedCard.Clone();
        DDGamePlaySingletonHolder.Instance.Dungeon.AddCardToDeck(copy, DDGamePlaySingletonHolder.Instance.Dungeon.DungeonCardStartPosition);
        DDGamePlaySingletonHolder.Instance.ShowDeckArea.CardSelectedCallback.RemoveListener(PlayerCardSelected);
        DDGamePlaySingletonHolder.Instance.Dungeon.DisplayDeckClosed();
    }
}
