using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDPlayer_Match : MonoBehaviour
{
    [SerializeField] private DDCardInHand cardInHandPrefab;

    public DDCardInHand CardInHandPrefab => cardInHandPrefab;

    [SerializeField] private int defaultHandSize = 5;
    private int currentHandSize;

    [SerializeField] private DDPlayer_MatchDeck deck;

    public List<DDCardInHand> CurrentDeck => deck.Cards;

    [SerializeField] private DDPlayer_MatchHand hand;
    [SerializeField] private DDPlayer_MatchDiscard discard;

    public List<DDCardInHand> CurrentDiscard => discard.Cards;

    private DDCardInHand selectedCard;
    private List<Target> cardTargets;
    private List<DDSelection> cardSelections;
    private int currentTargetIndex;

    [SerializeField] private Transform selectedCardLocation;

    private Coroutine cardResolving;

    [SerializeField] private Arrow.ArrowRenderer arrow;

    // This is resource, possible Player_Match will have child classes for different characters?
    private int momentumCounter;

    public int MomentumCounter => momentumCounter;

    public UnityEngine.Events.UnityEvent GainedMomentum;

    private int[] laneArmors;
    [SerializeField] private DDPlayerLaneArmorUI[] laneArmorUI;

    private DDAffixManager affixManager = new DDAffixManager();

    [Header("Testing")] [SerializeField] private TMPro.TextMeshProUGUI momentum;

    private void OnEnable()
    {
        DDGamePlaySingletonHolder.Instance.PlayerSelector.SomethingSelected.AddListener(SomethingSelected);
    }

    private void OnDisable()
    {
        DDGamePlaySingletonHolder.Instance.PlayerSelector.SomethingSelected.RemoveListener(SomethingSelected);
        cardResolving = null;
    }

    public void EncounterStarted()
    {
        laneArmors = new int[laneArmorUI.Length];

        for (int i = 0; i < laneArmorUI.Length; i++)
        {
            laneArmorUI[i].SetAmount(0);
        }

        deck.DestroyCards();
        hand.DestroyCards();
        discard.DestroyCards();

        affixManager.AffixAdjusted.AddListener(AffixAdjusted);
    }

    private void AffixAdjusted(EAffixType changedAffix)
    {
        switch (changedAffix)
        {
            default:
                break;
        }
    }

    public void ModifyAffix(EAffixType affixType, int amount, bool shouldSet)
    {
        affixManager.ModifyValueOfAffix(affixType, amount, shouldSet);
    }

    public int GetAffixValue(EAffixType affixType)
    {
        int? value = affixManager.TryGetAffixValue(affixType);
        return value ?? 0;
    }

    public void ShuffleInDeck()
    {
        deck.ShuffleInCards(DDGamePlaySingletonHolder.Instance.Dungeon.PlayerDeck);
    }

    public IEnumerator AddCardToDiscard(DDCardBase card)
    {
        return discard.AddCardOverTime(card);
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

            arrow.SetPositions(selectedCardLocation.position + Vector3.up,
                DDGamePlaySingletonHolder.Instance.PlayerSelector.GetMousePos());

            if (Input.GetMouseButtonDown(1))
            {
                selectedCard.CardDeselected();
                DDGamePlaySingletonHolder.Instance.PlayerSelector.SetToPlayerCard();
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

    public bool IsLaneArmored(int lane)
    {
        return laneArmors[lane] > 0;
    }

    public void AddArmorToLane(int amount, int lane)
    {
        laneArmors[lane] += amount;
        laneArmorUI[lane].SetAmount(laneArmors[lane]);
    }

    public int DealDamageInLane(int damage, int lane)
    {
        int leftOverDamage = laneArmors[lane] - damage;
        laneArmors[lane] = Mathf.Max(leftOverDamage, 0);

        laneArmorUI[lane].SetAmount(laneArmors[lane]);

        if (leftOverDamage >= 0)
        {
            return 0;
        }
        else
        {
            return Mathf.Abs(leftOverDamage);
        }
    }

    public void DrawACard()
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

    public IEnumerator DiscardHand()
    {
        yield return hand.DiscardHand(discard);
    }

    public void DiscardCard(DDCardInHand card)
    {
        hand.DiscardCard(card, discard);
    }

    public void SomethingSelected(DDSelection selection)
    {
        if (cardResolving != null)
        {
            return;
        }

        if (selectedCard == null)
        {
            if (DDGamePlaySingletonHolder.Instance.Encounter.CurrentPhase == EEncounterPhase.PlayersTurn)
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
                        DDGamePlaySingletonHolder.Instance.PlayerSelector.SetSelectionLayer(
                            cardTargets[currentTargetIndex].GetTargetTypeLayer());
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
                    if (selectedCard.AllUsed)
                    {
                        // Show some effect the card being used the final time.
                        Destroy(selectedCard.gameObject);
                    }
                    else
                    {
                        discard.CardDiscarded(selectedCard);
                    }

                    DDGamePlaySingletonHolder.Instance.PlayerSelector.SetToPlayerCard();
                    selectedCard = null;
                }
                else
                {
                    DDGamePlaySingletonHolder.Instance.PlayerSelector.SetSelectionLayer(cardTargets[currentTargetIndex]
                        .GetTargetTypeLayer());
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

    public void DealDamageToEnemy(int damage, ERangeType rangeType, DDEnemyOnBoard enemyOnBoard)
    {
        int totalDamage = damage;
        int? dex = affixManager.TryGetAffixValue(EAffixType.Expertise);
        if (dex != null)
        {
            totalDamage += dex.Value;
        }

        enemyOnBoard.DoDamage(totalDamage, rangeType);
    }
}