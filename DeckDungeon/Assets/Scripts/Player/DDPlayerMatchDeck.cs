using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class DDPlayerMatchDeck : MonoBehaviour
{
    private List<DDCardInHand> cards = new List<DDCardInHand>();
    public List<DDCardInHand> Cards => cards;

    public void ShuffleInCard(DDCardInHand card, bool waitToShuffle = false)
    {
        // DO ANIMATION HERE
        card.transform.SetParent(transform);
        card.transform.localPosition = Vector3.zero;
        card.gameObject.SetActive(false);        
        
        cards.Add(card);
        
        if(!waitToShuffle)
        {
            ShuffleDeck();
        }
    }
    
    public IEnumerator ShuffleInCardOverTime(DDCardInHand card, bool waitToShuffle = false)
    {
        DDGlobalManager.Instance.ClipLibrary.HoverCard.PlayNow();
        
        // DO ANIMATION HERE
        card.transform.SetParent(transform);
        
        card.gameObject.SetActive(true);
        card.SetCanHover(false);
        
        yield return card.transform.DOLocalMove(Vector3.zero, 0.1f).WaitForCompletion();
        
        card.gameObject.SetActive(false);
        card.SetCanHover(true);
        
        cards.Add(card);
        
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
                DDGlobalManager.Instance.SpawnNewCardInHand(otherCards[i], true, transform, transform.position, false);
            ShuffleInCard(cardInHand, true);
        }

        ShuffleDeck();
    }

    public IEnumerator ShuffleInCardsOverTime(List<DDCardInHand> otherCards)
    {
        for (int i = 0; i < otherCards.Count; i++)
        {
            StartCoroutine(ShuffleInCardOverTime(otherCards[i], true));

            yield return new WaitForSeconds(Random.Range(0.04f, 0.06f));
        }

        yield return new WaitForSeconds(0.1f);
        
        ShuffleDeck();
    }

    private void ShuffleDeck()
    {
        cards.Shuffle();
    }

    public DDCardInHand PeakTopCard()
    {
        return cards[^1];
    }
    
    public DDCardInHand GetTopCard()
    {
        DDCardInHand outGoingCard = cards[^1];
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
