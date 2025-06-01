using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class DDPlayerMatchHand : MonoBehaviour
{
    [SerializeField] private float cardSpreadDistance = 160;

    private List<DDCardInHand> cards = new List<DDCardInHand>();

    public IEnumerator AddCard(DDCardInHand card, bool drawn)
    {
        DDGlobalManager.Instance.ClipLibrary.DrawCard.PlayNow();

        cards.Add(card);
        card.SetCanHover(false);
        card.UpdateDisplayInformation();
        card.gameObject.SetActive(true);
        card.transform.SetParent(transform);
        card.transform.SetAsFirstSibling();

        if (drawn)
        {
            yield return card.DrawCard();
        }

        yield return UpdateCardsPosition();
        
        card.SetCanHover(true);
    }

    public IEnumerator CardRemoved(DDCardInHand card)
    {
        if (cards.Remove(card))
        {
            yield return UpdateCardsPosition();
        }
    }

    public IEnumerator EndOfTurn()
    {
        for (int i = cards.Count - 1; i >= 0; i--)
        {
            yield return cards[i].EndOfTurn();
        }
    }

    public IEnumerator DiscardHand(DDPlayerMatchDiscard discard)
    {
        for (int i = 0; i < cards.Count; i++)
        {
            if (cards[i].CurrentCard.AllCardFinishes.ContainsKey(EPlayerCardFinish.Sticky))
            {
                // play sticky animation
                yield return null;
                continue;
            }

            DDCardInHand card = cards[i];
            cards.RemoveAt(i--);

            yield return MoveCardToDiscard(card, discard);
        }

        yield return UpdateCardsPosition();
    }

    public IEnumerator DiscardCard(DDCardInHand card, DDPlayerMatchDiscard discard)
    {
        if (cards.Remove(card))
        {
            yield return MoveCardToDiscard(card, discard);
        }
    }

    private IEnumerator MoveCardToDiscard(DDCardInHand card, DDPlayerMatchDiscard discard)
    {
        DDGlobalManager.Instance.ClipLibrary.DiscardCard.PlayNow();

        yield return card.DiscardCard(false);
        if (card)
        {
            yield return discard.CardDiscarded(card);
        }
    }

    private IEnumerator UpdateCardsPosition()
    {
        if (cards.Count == 1)
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
                pos.x = Mathf.Lerp(minMax, -minMax, percent);
                cards[(int)i].SetCanHover(false);
                cards[(int)i].transform.DOLocalMove(pos, .1f);
            }
        }

        yield return new WaitForSeconds(.1125f);

        foreach (var card in cards)
        {
            card.SetCanHover(true);
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