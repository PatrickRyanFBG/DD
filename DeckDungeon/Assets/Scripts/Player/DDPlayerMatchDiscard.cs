using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDPlayerMatchDiscard : MonoBehaviour
{
    private List<DDCardInHand> cards = new List<DDCardInHand>();
    public List<DDCardInHand> Cards => cards;

    public IEnumerator AddCardOverTime(DDCardBase card, Vector3 position, bool cloned)
    {
        DDCardInHand cardInHand =
            DDGlobalManager.Instance.SpawnNewCardInHand(card, false, transform, position, cloned);
        
        yield return CardDiscarded(cardInHand);
        
        cardInHand.SetCanHover(true);
    }

    public IEnumerator CardDiscarded(DDCardInHand card)
    {
        cards.Add(card);

        // DO ANIMATION OF CARD MOVING TO DISCARD
        card.transform.SetParent(transform);
        card.SetCanHover(false);
        yield return card.transform.DOLocalMove(Vector3.zero, .1f).WaitForCompletion();
        card.SetCanHover(true);
        card.gameObject.SetActive(false);
    }

    public List<DDCardInHand> GetAndClearDiscard()
    {
        List<DDCardInHand> outgoing = cards;
        cards = new List<DDCardInHand>();
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
    }
}
