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
        SingletonHolder.Instance.PlayerSelector.SomethingSelected.AddListener(SomethingSelected);
    }

    private void OnDisable()
    {
        SingletonHolder.Instance.PlayerSelector.SomethingSelected.RemoveListener(SomethingSelected);
    }

    public void EndCardSelection()
    {
        if (fromEncounter != null)
        {
            if (fromEncounter.TestingHasChest)
            {
                SingletonHolder.Instance.Dungeon.StartEvent(chestEvent);
                SingletonHolder.Instance.Dungeon.HasKey = false;
            }
            else
            {
                if (fromEncounter.TestingHasKey)
                {
                    SingletonHolder.Instance.Dungeon.HasKey = true;
                }

                SingletonHolder.Instance.Dungeon.PromptDungeonCard();
            }
        }
        else
        {
            SingletonHolder.Instance.Dungeon.PromptDungeonCard();
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
        List<DDCardBase> cards = SingletonHolder.Instance.CardLibrary.GenerateValkyrieCards(amount);
        for (int i = 0; i < amount; i++)
        {
            playerCards[i].SetUpCard(cards[i]);
        }

        SingletonHolder.Instance.PlayerSelector.SetSelectionLayer(playerCardLayer);

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

        SingletonHolder.Instance.PlayerSelector.SetSelectionLayer(playerCardLayer);
        extraImage.gameObject.SetActive(false);

        gameObject.SetActive(true);
    }
}
