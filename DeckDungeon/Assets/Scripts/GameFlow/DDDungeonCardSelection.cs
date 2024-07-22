using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDDungeonCardSelection : MonoBehaviour
{
    [SerializeField]
    private DDDungeonCardShown[] dungeonCards;

    [SerializeField]
    private LayerMask dungeonCardLayer;

    [Header("Testing")]
    [SerializeField]
    private int testCardOne = -1;
    private List<DDDungeonCardBase> cachedDungeonDeck;

    private void OnEnable()
    {
        SingletonHolder.Instance.PlayerSelector.SomethingSelected.AddListener(SomethingSelected);
    }

    private void OnDisable()
    {
        SingletonHolder.Instance.PlayerSelector.SomethingSelected.RemoveListener(SomethingSelected);
    }

    private void SomethingSelected(DDSelection selection)
    {
        DDDungeonCardShown dungeonCard = selection as DDDungeonCardShown;
        if (dungeonCard != null)
        {
            dungeonCard.DungeonCardSelected();
        }
    }

    public void DungeonCardSelected(DDDungeonCardShown cardShown)
    {
        SingletonHolder.Instance.Dungeon.DungeonCardSelected(cardShown.CurrentDungeonCard);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.M))
        {
            TEST_RedoDungeonCards();
        }
    }

    public void TEST_RedoDungeonCards()
    {
        DisplayDungeonCards(ref cachedDungeonDeck);
    }

    public void DisplayDungeonCards(ref List<DDDungeonCardBase> dungeonDeck)
    {
        cachedDungeonDeck = dungeonDeck;

        int cardOne = testCardOne >= 0 ? testCardOne : Random.Range(0, dungeonDeck.Count);
        dungeonCards[0].SetUpDungeonCard(dungeonDeck[cardOne], cardOne);

        if (dungeonDeck.Count > 1)
        {
            int cardTwo = Random.Range(0, dungeonDeck.Count);
            while (cardTwo == cardOne)
            {
                cardTwo = Random.Range(0, dungeonDeck.Count);
            }

            dungeonCards[1].SetUpDungeonCard(dungeonDeck[cardTwo], cardTwo);

            int cardThree = Random.Range(0, dungeonDeck.Count);
            if (dungeonDeck.Count > 2)
            {
                while (cardThree == cardOne || cardThree == cardTwo)
                {
                    cardThree = Random.Range(0, dungeonDeck.Count);
                }

                dungeonCards[2].SetUpDungeonCard(dungeonDeck[cardThree], cardThree);
            }
            else
            {
                dungeonCards[2].gameObject.SetActive(false);
            }
        }
        else
        {
            dungeonCards[1].gameObject.SetActive(false);
            dungeonCards[2].gameObject.SetActive(false);
        }

        SingletonHolder.Instance.PlayerSelector.SetSelectionLayer(dungeonCardLayer);

        gameObject.SetActive(true);
    }
}
