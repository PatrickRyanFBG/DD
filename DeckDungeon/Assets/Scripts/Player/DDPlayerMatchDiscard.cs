using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDPlayerMatchDiscard : MonoBehaviour
{
    [SerializeField]
    private Transform cardSpawnLocation;

    [Header("Testing")]
    [SerializeField]
    private TMPro.TextMeshProUGUI numberText;

    private List<DDCardInHand> cards = new List<DDCardInHand>();
    public List<DDCardInHand> Cards => cards;

    public IEnumerator AddCardOverTime(DDCardBase card)
    {
        DDCardInHand cardInHand = Instantiate(DDGamePlaySingletonHolder.Instance.Player.CardInHandPrefab, cardSpawnLocation.position, Quaternion.identity, transform);
        cardInHand.transform.DOMove(transform.position, .3f, false);
        cardInHand.SetUpCard(card, false);
        
        yield return new WaitForSeconds(.3f);

        cardInHand.SetCanHover(true);
        
        CardDiscarded(cardInHand);
    }

    public void CardDiscarded(DDCardInHand card)
    {
        cards.Add(card);

        // DO ANIMATION OF CARD MOVING TO DISCARD
        card.transform.parent = transform;
        card.transform.position = transform.position;
        card.gameObject.SetActive(false);

        numberText.text = cards.Count.ToString();
    }

    public List<DDCardInHand> GetAndClearDiscard()
    {
        List<DDCardInHand> outgoing = cards;
        cards = new List<DDCardInHand>();
        numberText.text = "0";
        return outgoing;
    }

    public int GetNumberInDiscard()
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
