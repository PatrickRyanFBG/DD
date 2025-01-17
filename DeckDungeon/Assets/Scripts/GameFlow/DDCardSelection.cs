using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DDCardSelection : MonoBehaviour
{
    [SerializeField]
    private DDCardShown[] playerCards;

    [SerializeField]
    private LayerMask playerCardLayer;

    [SerializeField]
    private RawImage extraImage;

    private DDDungeonCardEncounter fromEncounter;

    [SerializeField]
    private Texture keyImage;

    [SerializeField]
    private Texture chestImage;

    [SerializeField]
    private DDDungeonCardEvent chestEvent;

    private void OnEnable()
    {
        DDGamePlaySingletonHolder.Instance.PlayerSelector.SomethingSelected.AddListener(SomethingSelected);
    }

    private void OnDisable()
    {
        DDGamePlaySingletonHolder.Instance.PlayerSelector.SomethingSelected.RemoveListener(SomethingSelected);
    }

    public void EndCardSelection()
    {
        if (fromEncounter != null)
        {
            if (fromEncounter.TestingHasChest)
            {
                DDGamePlaySingletonHolder.Instance.Dungeon.StartEvent(chestEvent);
                DDGamePlaySingletonHolder.Instance.Dungeon.HasKey = false;
            }
            else
            {
                if (fromEncounter.TestingHasKey)
                {
                    DDGamePlaySingletonHolder.Instance.Dungeon.HasKey = true;
                }

                DDGamePlaySingletonHolder.Instance.Dungeon.PromptDungeonCard();
            }
        }
        else
        {
            DDGamePlaySingletonHolder.Instance.Dungeon.PromptDungeonCard();
        }
    }

    private void SomethingSelected(DDSelection selection)
    {
        DDCardShown playerCard = selection as DDCardShown;
        if (playerCard != null)
        {
            playerCard.CardSelected();
            EndCardSelection();
        }
    }

    public void DisplayPlayerCards(DDDungeonCardEncounter encounterCard)
    {
        // Generate Three Random Cards
        int amount = 3;
        List<DDCardBase> cards = DDGlobalManager.Instance.SelectedAdventurer.GenerateCards(amount);
        for (int i = 0; i < amount; i++)
        {
            playerCards[i].SetUpCard(cards[i]);
        }

        DDGamePlaySingletonHolder.Instance.PlayerSelector.SetSelectionLayer(playerCardLayer);

        fromEncounter = encounterCard;

        if (fromEncounter.TestingHasKey)
        {
            extraImage.gameObject.SetActive(true);
            extraImage.texture = keyImage;
        }
        else if (fromEncounter.TestingHasChest)
        {
            extraImage.gameObject.SetActive(true);
            extraImage.texture = chestImage;
        }
        else
        {
            extraImage.gameObject.SetActive(false);
        }

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

    public void CardSelected(DDCardShown cardShown)
    {
        DDGamePlaySingletonHolder.Instance.Dungeon.AddCardToDeck(cardShown.CurrentCard, cardShown.transform.position);
    }
}
