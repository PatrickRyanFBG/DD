using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDPlayer_MatchDeck : MonoBehaviour
{
    [Header("Testing")]
    [SerializeField]
    private TMPro.TextMeshProUGUI numberText;

    private List<DDCardInHand> cards = new List<DDCardInHand>();
    public List<DDCardInHand> Cards => cards;

    public void ShuffleInCard(DDCardInHand card, bool waitToShuffle = false)
    {
        cards.Add(card);
        // DO ANIMATION HERE
        card.transform.parent = transform;
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
            // Do another deep copy here to allow for in-match modifications?
            DDCardInHand cardInHand = Instantiate(DDGamePlaySingletonHolder.Instance.Player.CardInHandPrefab, transform);
            cardInHand.SetUpCard(otherCards[i].Clone());
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
        numberText.text = cards.Count.ToString();
        cards.Shuffle();
    }

    public DDCardInHand GetTopCard()
    {
        DDCardInHand outGoingCard = cards[cards.Count - 1];
        cards.RemoveAt(cards.Count - 1);
        numberText.text = cards.Count.ToString();
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
        numberText.text = "0";
    }
}
