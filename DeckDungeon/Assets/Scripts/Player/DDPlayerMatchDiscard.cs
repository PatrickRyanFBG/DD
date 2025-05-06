using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDPlayerMatchDiscard : MonoBehaviour
{
    [SerializeField]
    private Transform cardSpawnLocation;

    private List<DDCardInHand> cards = new List<DDCardInHand>();
    public List<DDCardInHand> Cards => cards;

    public IEnumerator AddCardOverTime(DDCardBase card, Vector3? position)
    {
        DDCardInHand cardInHand =
            DDGlobalManager.Instance.SpawnNewCardInHand(card, false, transform, position ?? cardSpawnLocation.position);
        cardInHand.transform.DOMove(transform.position, .3f, false);
        
        yield return new WaitForSeconds(.3f);

        cardInHand.SetCanHover(true);
        
        CardDiscarded(cardInHand);
    }

    public void CardDiscarded(DDCardInHand card)
    {
        cards.Add(card);

        // DO ANIMATION OF CARD MOVING TO DISCARD
        card.transform.SetParent(transform);
        card.transform.localPosition = Vector3.zero;
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
