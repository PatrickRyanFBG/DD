using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDPlayerMatchHand : MonoBehaviour
{
    [SerializeField] private float cardSpreadDistance = 160;
    
    private List<DDCardInHand> cards = new List<DDCardInHand>();

    public void AddCard(DDCardInHand card)
    {
        cards.Add(card);
        card.UpdateDisplayInformation();
        card.gameObject.SetActive(true);
        card.transform.SetParent(transform);

        UpdateCardsPosition();
    }

    public void CardRemoved(DDCardInHand card)
    {
        if(cards.Remove(card))
        {
            UpdateCardsPosition();
        }
    }

    public IEnumerator EndOfTurn()
    {
        for (int i = cards.Count - 1; i >= 0; i--)
        {
            yield return cards[i].CurrentCard.EndOfTurn();
        }
    }
    
    public IEnumerator DiscardHand(DDPlayerMatchDiscard discard)
    {
        for (int i = cards.Count - 1; i >= 0; i--)
        {
            yield return cards[i].CurrentCard.DiscardCard(true);
            if (cards[i])
            {
                discard.CardDiscarded(cards[i]);
            }

            cards.RemoveAt(i);
        }
    }

    public IEnumerator DiscardCard(DDCardInHand card, DDPlayerMatchDiscard discard)
    {
        cards.Remove(card);
        yield return card.CurrentCard.DiscardCard(false);
        if (card)
        {
            discard.CardDiscarded(card);
        }
    }

    private void UpdateCardsPosition()
    {
        if(cards.Count == 1)
        {
            cards[0].transform.position = transform.position;
        }
        else
        {
            float minMax = (cards.Count - 1f) * .5f * cardSpreadDistance;
            
            for (float i = 0; i < cards.Count; i++)
            {
                float percent = i / (cards.Count - 1);
                
                Vector3 pos = Vector3.zero;
                pos.x = Mathf.Lerp(-minMax, minMax, percent);
                cards[(int)i].transform.localPosition = pos; 
            }
        }
    }

    public void DestroyCards()
    {
        for (int i = 0; i < cards.Count; i++)
        {
            Destroy(cards[i].gameObject);
        }

        cards.Clear();
    }

    public int GetFinishCountByType(EPlayerCardFinish finishType)
    {
        int count = 0;

        for (int i = 0; i < cards.Count; i++)
        {
            if (cards[i].CurrentCard.AllCardFinishes.ContainsKey(finishType))
            {
                ++count;
            }
        }
        
        return count;
    }
}
