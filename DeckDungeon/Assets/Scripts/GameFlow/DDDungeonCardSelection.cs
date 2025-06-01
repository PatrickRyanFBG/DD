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
        DDGamePlaySingletonHolder.Instance.PlayerSelector.SomethingSelected.AddListener(SomethingSelected);
    }

    private void OnDisable()
    {
        DDGamePlaySingletonHolder.Instance.PlayerSelector.SomethingSelected.RemoveListener(SomethingSelected);
    }

    private void SomethingSelected(DDSelection selection)
    {
        DDDungeonCardShown dungeonCard = selection as DDDungeonCardShown;
        if (dungeonCard)
        {
            dungeonCard.DungeonCardSelected();
        }
    }

    public void DungeonCardSelected(DDDungeonCardShown cardShown)
    {
        DDGamePlaySingletonHolder.Instance.Dungeon.DungeonCardSelected(cardShown.CurrentDungeonCard);
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
        
        List<int> indexes = new List<int>();

        if (dungeonDeck.Count < 4)
        {
            for (int i = 0; i < dungeonDeck.Count; i++)
            {
                indexes.Add(i);
            }
        }
        else
        {
            bool debugging = testCardOne >= 0;
            if (debugging)
            {
                indexes.Add(testCardOne);
            }
            else
            {
                // Lets try to find a normal encounter to always show atleast one
                for (int i = 0; i < dungeonDeck.Count; i++)
                {
                    if (dungeonDeck[i].Type == EDungeonCardType.Encounter)
                    {
                        if(dungeonDeck[i].Get<DDDungeonCardEncounter>().EncounterType == EEncounterType.Normal)
                        {
                            indexes.Add(i);
                            break;
                        }
                    }
                }
            }

            for (int i = indexes.Count; i < 3; i++)
            {
                int randomIndex = Random.Range(0, dungeonDeck.Count);
                while (indexes.Contains(randomIndex))
                {
                    randomIndex = Random.Range(0, dungeonDeck.Count);
                }
                indexes.Add(randomIndex);
            }
        }

        indexes.Shuffle();
        
        for (int i = 0; i < dungeonCards.Length; i++)
        {
            if (i >= indexes.Count)
            {
                dungeonCards[i].gameObject.SetActive(false);
            }
            else
            {
                dungeonCards[i].SetUpDungeonCard(dungeonDeck[indexes[i]], indexes[i]);
            }
        }
        
        DDGamePlaySingletonHolder.Instance.PlayerSelector.SetSelectionLayer(dungeonCardLayer);

        gameObject.SetActive(true);
    }
}
