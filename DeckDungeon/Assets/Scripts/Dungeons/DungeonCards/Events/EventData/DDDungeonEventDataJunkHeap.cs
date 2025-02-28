using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDDungeonEventDataJunkHeap : DDDungeonEventData
{
    [Header("Junk Heap")] [SerializeField] private string rummageText;

    [SerializeField, Multiline] private string afterRummageDescription;

    [SerializeField] private string removeCardText;

    [SerializeField, Multiline] private string removeCardTextDescription;
    
    public override void DisplayEvent(DDEventArea area)
    {
        base.DisplayEvent(area);

        DDButton buttonOne = area.GenerateButton(rummageText);
        buttonOne.Button.onClick.AddListener(() =>
        {
            // Generate Random Card
            List<DDCardBase> card = DDGlobalManager.Instance.SelectedAdventurer.CardData.GenerateCards(1);
            DDGamePlaySingletonHolder.Instance.Dungeon.AddCardToDeck(card[0], DDGamePlaySingletonHolder.Instance.Dungeon.DungeonCardStartPosition);
            
            area.Description.text = afterRummageDescription;
            area.CleanUpButtons();

            DDButton okayButton = area.GenerateButton("Leave.");
            okayButton.Button.onClick.AddListener(() => { DDGamePlaySingletonHolder.Instance.Dungeon.EndEvent(); });
        });

        DDButton buttonTwo = area.GenerateButton(removeCardText);
        buttonTwo.Button.onClick.AddListener(() =>
        {
            DDGamePlaySingletonHolder.Instance.Dungeon.DisplayPlayerDeck();
            DDGamePlaySingletonHolder.Instance.ShowDeckArea.CardSelectedCallback
                .AddListener(PlayerCardSelected);

            area.Description.text = removeCardTextDescription;
            area.CleanUpButtons();

            DDButton okayButton = area.GenerateButton("Leave.");
            okayButton.Button.onClick.AddListener(() =>
            {
                DDGamePlaySingletonHolder.Instance.ShowDeckArea.CardSelectedCallback.RemoveListener(
                    PlayerCardSelected);
                DDGamePlaySingletonHolder.Instance.Dungeon.EndEvent();
            });
        });
    }

    private void PlayerCardSelected(DDCardBase selectedCard)
    {
        DDGamePlaySingletonHolder.Instance.Dungeon.RemoveCardFromDeck(selectedCard);
        DDGamePlaySingletonHolder.Instance.ShowDeckArea.CardSelectedCallback.RemoveListener(PlayerCardSelected);
        DDGamePlaySingletonHolder.Instance.Dungeon.DisplayDeckClosed();
    }
}
