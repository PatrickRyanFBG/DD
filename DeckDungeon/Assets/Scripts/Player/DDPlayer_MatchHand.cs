using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDPlayer_MatchHand : MonoBehaviour
{
    private List<DDCardInHand> cards = new List<DDCardInHand>();

    public void AddCard(DDCardInHand card)
    {
        cards.Add(card);
        card.UpdateDisplayInformation();
        card.gameObject.SetActive(true);
        card.transform.parent = transform;

        UpdateCardsPosition();
    }

    public void CardRemoved(DDCardInHand card)
    {
        if(cards.Remove(card))
        {
            UpdateCardsPosition();
        }
    }

    public IEnumerator DiscardHand(DDPlayer_MatchDiscard discard)
    {
        for (int i = cards.Count - 1; i >= 0; i--)
        {
            // TODO :: Change this to bit wise enum
            if(cards[i].CurrentCard.Finishes.Contains(ECardFinishing.Fleeting))
            {
                yield return cards[i].CurrentCard.DestroyedCard();
                Destroy(cards[i].gameObject);
            }
            else
            {
                yield return cards[i].CurrentCard.DiscardCard();
                discard.CardDiscarded(cards[i]);
            }

            cards.RemoveAt(i);
        }
    }

    public void DiscardCard(DDCardInHand card, DDPlayer_MatchDiscard discard)
    {
        discard.CardDiscarded(card);
        cards.Remove(card);
    }

    private void UpdateCardsPosition()
    {
        if(cards.Count == 1)
        {
            cards[0].transform.position = transform.position;
        }
        else
        {
            float minMax = cards.Count - 1f;
            for (float i = 0; i < cards.Count; i++)
            {
                float percent = i / ((float)cards.Count - 1);
                
                Vector3 pos = transform.localPosition;
                pos.x = Mathf.Lerp(-minMax, minMax, percent);
                cards[(int)i].transform.position = pos; 
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
}
