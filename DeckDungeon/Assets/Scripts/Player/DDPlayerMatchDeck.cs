using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDPlayerMatchDeck : MonoBehaviour
{
    private List<DDCardInHand> cards = new List<DDCardInHand>();
    public List<DDCardInHand> Cards => cards;

    public void ShuffleInCard(DDCardInHand card, bool waitToShuffle = false)
    {
        cards.Add(card);
        // DO ANIMATION HERE
        card.transform.SetParent(transform);
        card.transform.position = transform.position;
        card.gameObject.SetActive(false);

        if(!waitToShuffle)
        {
            ShuffleDeck();
        }
    }

    public void ShuffleInCards(List<DDCardBase> otherCards)
    {
        for (int i = 0; i < otherCards.Count; i++)
        {
            DDCardInHand cardInHand =
                DDGlobalManager.Instance.SpawnNewCardInHand(otherCards[i], true, transform, transform.position);
            ShuffleInCard(cardInHand, true);
        }

        ShuffleDeck();
    }

    public void ShuffleInCards(List<DDCardInHand> otherCards)
    {
        for (int i = 0; i < otherCards.Count; i++)
        {
            ShuffleInCard(otherCards[i], true);
        }

        ShuffleDeck();
    }

    private void ShuffleDeck()
    {
        cards.Shuffle();
    }

    public DDCardInHand PeakTopCard()
    {
        return cards[cards.Count - 1];
    }
    
    public DDCardInHand GetTopCard()
    {
        DDCardInHand outGoingCard = cards[cards.Count - 1];
        cards.RemoveAt(cards.Count - 1);
        return outGoingCard;
    }

    public int GetNumberInDeck()
    {
        return cards.Count;
    }

    public void DestroyCards()
    {
        for (int i = 0; i < cards.Count; i++)
        {
            Destroy(cards[i].gameObject);
        }

        cards.Clear();
    }
}
