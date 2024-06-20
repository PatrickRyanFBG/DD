using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDPlayer_Match : MonoBehaviour
{
    [SerializeField]
    private int defaultHandSize = 5;
    private int currentHandSize;

    [SerializeField]
    private DDPlayer_MatchDeck deck;
    [SerializeField]
    private DDPlayer_MatchHand hand;
    [SerializeField]
    private DDPlayer_MatchDiscard discard;

    private DDCardInHand selectedCard;
    private List<Target> cardTargets;
    private List<DDSelection> cardSelections;
    private int currentTargetIndex;

    [SerializeField]
    private Transform selectedCardLocation;

    private Coroutine cardResolving;

    [SerializeField]
    private Arrow.ArrowRenderer arrow;

    // This is resource, possible Player_Match will have child classes for different characters?
    private int momentumCounter;
    public int MomentumCounter { get { return momentumCounter; } }

    public UnityEngine.Events.UnityEvent GainedMomentum;

    [Header("Testing")]
    [SerializeField]
    private TMPro.TextMeshProUGUI momentum;

    private void OnEnable()
    {
        SingletonHolder.Instance.PlayerSelector.SomethingSelected.AddListener(SomethingSelected);
    }

    private void OnDisable()
    {
        SingletonHolder.Instance.PlayerSelector.SomethingSelected.RemoveListener(SomethingSelected);
        cardResolving = null;
    }

    public void ClearCards()
    {
        deck.DestroyCards();
        hand.DestroyCards();
        discard.DestroyCards();
    }

    public void ShuffleInDeck()
    {
        deck.ShuffleInCards(SingletonHolder.Instance.Dungeon.PlayerDeck);
    }

    public void SetHandSizeToDefault()
    {
        currentHandSize = defaultHandSize;
    }

    public void AdjustHandSize(int amount)
    {
        currentHandSize += amount;
    }

    private void Update()
    {
        if (selectedCard)
        {
            if (!arrow.gameObject.activeInHierarchy)
            {
                arrow.gameObject.SetActive(true);
            }

            arrow.SetPositions(selectedCardLocation.position + Vector3.up, SingletonHolder.Instance.PlayerSelector.GetMousePos());

            if (Input.GetMouseButtonDown(1))
            {
                selectedCard.CardDeselected();
                SingletonHolder.Instance.PlayerSelector.SetToPlayerCard();
                selectedCard = null;
            }
        }
        else if (arrow.gameObject.activeInHierarchy)
        {
            arrow.gameObject.SetActive(false);
        }

    }

    public void DrawFullHand()
    {
        for (int i = 0; i < currentHandSize; i++)
        {
            DrawACard();
        }
    }

    private void DrawACard()
    {
        bool cardAvail = true;
        if (deck.GetNumberInDeck() == 0)
        {
            if (discard.GetNumberInDiscard() == 0)
            {
                cardAvail = false;
            }
            else
            {
                deck.ShuffleInCards(discard.GetAndClearDiscard());
            }
        }

        if (cardAvail)
        {
            hand.AddCard(deck.GetTopCard());
        }
    }

    public void DiscardHand()
    {
        hand.DiscardHand(discard);
    }

    public void SomethingSelected(DDSelection selection)
    {
        if (cardResolving != null)
        {
            return;
        }

        if (selectedCard == null)
        {
            if (SingletonHolder.Instance.Encounter.CurrentPhase == EEncounterPhase.PlayersTurn)
            {
                DDCardInHand card = selection as DDCardInHand;
                if (card != null)
                {
                    if (card.CardSelected(selectedCardLocation.localPosition))
                    {
                        selectedCard = card;
                        cardTargets = selectedCard.GetCardTarget();
                        cardSelections = new List<DDSelection>(cardTargets.Count);
                        currentTargetIndex = 0;
                        SingletonHolder.Instance.PlayerSelector.SetSelectionLayer(cardTargets[currentTargetIndex].GetTargetTypeLayer());
                    }
                }
            }
        }
        else if (selectedCard != null)
        {
            if (selectedCard.IsSelectionValid(selection, currentTargetIndex))
            {
                cardSelections.Add(selection);
                if (++currentTargetIndex >= cardTargets.Count)
                {
                    cardResolving = StartCoroutine(WaitingForCard());
                    hand.CardRemoved(selectedCard);
                    discard.CardDiscarded(selectedCard);
                    SingletonHolder.Instance.PlayerSelector.SetToPlayerCard();
                    selectedCard = null;
                }
                else
                {
                    SingletonHolder.Instance.PlayerSelector.SetSelectionLayer(cardTargets[currentTargetIndex].GetTargetTypeLayer());
                }
            }
        }
        else
        {
            // here we highlight locations/enemies/effects?
        }
    }

    public void ResetMomentum()
    {
        momentumCounter = 0;
        momentum.text = momentumCounter.ToString();
    }

    public void AddToMomentum(int value)
    {
        momentumCounter += value;

        for (int i = 0; i < value; i++)
        {
            GainedMomentum?.Invoke();
        }

        momentum.text = momentumCounter.ToString();
    }

    public void RemoveFromMomentum(int value)
    {
        momentumCounter -= value;
        momentum.text = momentumCounter.ToString();
    }

    private IEnumerator WaitingForCard()
    {
        yield return selectedCard.ExecuteCard(cardSelections);

        cardResolving = null;
    }
}
