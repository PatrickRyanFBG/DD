using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DDCardSelection : MonoBehaviour
{
    [SerializeField] private DDCardShown[] playerCards;

    [SerializeField] private LayerMask playerCardLayer;

    [SerializeField] private RawImage extraImage;

    private DDDungeonCardEncounter fromEncounter;

    public void EndCardSelection()
    {
        if (fromEncounter)
        {
            if (fromEncounter.AwardsArtifacts)
            {
                DDGamePlaySingletonHolder.Instance.Dungeon.PromptArtifacts();
            }
            else if (fromEncounter.EventAfterComplete)
            {
                DDGamePlaySingletonHolder.Instance.Dungeon.StartEvent(fromEncounter.EventAfterComplete);
            }
            else
            {
                DDGamePlaySingletonHolder.Instance.Dungeon.PromptDungeonCard();
            }
        }
        else
        {
            DDGamePlaySingletonHolder.Instance.Dungeon.PromptDungeonCard();
        }
    }

    public void DisplayPlayerCards(DDDungeonCardEncounter encounterCard)
    {
        // Generate Three Random Cards
        int amount = 3;
        List<DDCardBase> cards = DDGlobalManager.Instance.SelectedAdventurer.CardData.GenerateCards(amount);
        for (int i = 0; i < amount; i++)
        {
            playerCards[i].SetUpCard(cards[i]);
        }

        DDGamePlaySingletonHolder.Instance.PlayerSelector.SetSelectionLayer(playerCardLayer);

        fromEncounter = encounterCard;

        gameObject.SetActive(true);
    }

    public void DisplayPlayerCards(List<DDCardBase> specificCards)
    {
        // Generate Three Random Cards
        int amount = 3;
        for (int i = 0; i < amount; i++)
        {
            playerCards[i].SetUpCard(specificCards[i]);
        }

        DDGamePlaySingletonHolder.Instance.PlayerSelector.SetSelectionLayer(playerCardLayer);
        extraImage.gameObject.SetActive(false);

        gameObject.SetActive(true);
    }

    // From UI Cards
    public void CardSelected(DDCardShown cardShown)
    {
        DDGamePlaySingletonHolder.Instance.Dungeon.AddCardToDeck(cardShown.CurrentCard, cardShown.transform.position);
        EndCardSelection();
    }
}