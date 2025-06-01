using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;

public class DDPlayerMatch : MonoBehaviour
{
    [SerializeField] private DDCardInHand cardInHandPrefab;

    public DDCardInHand CardInHandPrefab => cardInHandPrefab;

    [SerializeField] private int defaultHandSize = 5;
    private int currentHandSize;

    [SerializeField] private Transform cardSpawnLocation;

    [SerializeField] private DDPlayerMatchDeck deck;
    public DDPlayerMatchDeck Deck => deck;

    public List<DDCardInHand> CurrentDeck => deck.Cards;

    [SerializeField] private DDPlayerMatchHand hand;
    [SerializeField] private DDPlayerMatchDiscard discard;

    private List<DDCardBase> cardsExecutedThisTurn = new List<DDCardBase>();
    public List<DDCardBase> CardsExecutedThisTurn => cardsExecutedThisTurn;
    private List<DDCardBase> cardsExecutedThisEncounter = new List<DDCardBase>();

    public List<DDCardInHand> CurrentDiscard => discard.Cards;

    private DDCardInHand selectedCard;
    private List<DDCardTargetInfo> cardTargets;
    private List<DDSelection> cardSelections;
    private int currentTargetIndex;

    [SerializeField] private Transform selectedCardLocation;

    private Coroutine cardResolving;

    [SerializeField] private Arrow.ArrowRenderer arrow;

    [SerializeField] private Transform arrowStart;

    // This is resource, possible Player_Match will have child classes for different characters?
    private int momentumCounter;

    public int MomentumCounter => momentumCounter;

    public IEnumeratorHelper.EventHandler GainedMomentum;

    private DDAffixManager[] laneAffixes;
    [SerializeField] private DDAffixVisualsManager[] lanesAffixVisualsManager;

    [SerializeField] private DDAffixVisualsManager playersAffixVisualsManager;

    private DDAffixManager affixManager;

    public UnityEngine.Events.UnityEvent<DDCardInHand, EPlayerCardLifeTime> CardLifeTimeChanged;

    [Header("Health")] [SerializeField] private TMPro.TextMeshProUGUI healthText;

    [SerializeField] private Image healthBar;

    [Header("Testing")] [SerializeField] private TMPro.TextMeshProUGUI momentum;

    private void OnEnable()
    {
        DDGamePlaySingletonHolder.Instance.PlayerSelector.SomethingSelected.AddListener(SomethingSelected);
        DDGamePlaySingletonHolder.Instance.Dungeon.OnHealthChange.AddListener(HealthChanged);
        DDGamePlaySingletonHolder.Instance.Encounter.PhaseChanged += EncounterPhaseChanged;

        HealthChanged(DDGamePlaySingletonHolder.Instance.Dungeon.CurrentHealth,
            DDGamePlaySingletonHolder.Instance.Dungeon.MaxHealth);
    }

    private void OnDisable()
    {
        DDGamePlaySingletonHolder.Instance.PlayerSelector.SomethingSelected.RemoveListener(SomethingSelected);
        DDGamePlaySingletonHolder.Instance.Dungeon.OnHealthChange.RemoveListener(HealthChanged);
        DDGamePlaySingletonHolder.Instance.Encounter.PhaseChanged -= EncounterPhaseChanged;

        cardResolving = null;
    }

    public void HealthChanged(int current, int max)
    {
        healthText.text = $"{current} / {max}";
        healthBar.fillAmount = (float)current / max;
    }

    private IEnumerator EncounterPhaseChanged(MonoBehaviour sender, System.EventArgs args)
    {
        DDEncounter.DDPhaseChangeEventArgs phaseArgs = args as DDEncounter.DDPhaseChangeEventArgs;
        if (phaseArgs.Phase == EEncounterPhase.PlayersStartTurn)
        {
            cardsExecutedThisTurn.Clear();
        }

        yield return null;
    }

    public void EncounterStarted()
    {
        playersAffixVisualsManager.ClearVisuals();
        affixManager = new DDAffixManager(playersAffixVisualsManager, EAffixOwner.Player);

        laneAffixes = new DDAffixManager[lanesAffixVisualsManager.Length];

        for (int i = 0; i < lanesAffixVisualsManager.Length; i++)
        {
            lanesAffixVisualsManager[i].ClearVisuals();
            laneAffixes[i] = new DDAffixManager(lanesAffixVisualsManager[i], EAffixOwner.Lane);
        }

        deck.DestroyCards();
        hand.DestroyCards();
        discard.DestroyCards();

        affixManager.AffixAdjusted.AddListener(AffixAdjusted);

        cardsExecutedThisTurn.Clear();
        cardsExecutedThisEncounter.Clear();
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

    public IEnumerator AddCardTo(DDCardBase card, Vector3? spawnPosition, ECardLocation location, bool cloned)
    {
        switch (location)
        {
            case ECardLocation.Deck:
                break;
            case ECardLocation.Hand:
                DDCardInHand cardInHand =
                    DDGlobalManager.Instance.SpawnNewCardInHand(card, false, transform,
                        spawnPosition ?? cardSpawnLocation.position, cloned);
                yield return hand.AddCard(cardInHand, false);
                break;
            case ECardLocation.Discard:
                yield return discard.AddCardOverTime(card, spawnPosition ?? cardSpawnLocation.position, cloned);
                break;
        }
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
        if (selectedCard && cardResolving == null)
        {
            if (!arrow.gameObject.activeInHierarchy)
            {
                arrow.gameObject.SetActive(true);
            }
            
            arrow.SetPositions(arrowStart.position,
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

    public IEnumerator DrawFullHand()
    {
        for (int i = 0; i < currentHandSize; i++)
        {
            yield return DrawACard();
        }
    }

    public bool IsLaneArmored(int lane)
    {
        return (laneAffixes[lane].TryGetAffixValue(EAffixType.Armor) ?? 0) > 0;
    }

    public void ModifyLaneAffix(EAffixType affix, int amount, int lane, bool shouldSet)
    {
        laneAffixes[lane].ModifyValueOfAffix(affix, amount, shouldSet);
    }

    public int? GetLaneAffix(EAffixType affixType, int lane)
    {
        return laneAffixes[lane].TryGetAffixValue(affixType);
    }

    public void DealDamageInLane(int damage, int lane)
    {
        // This means it was already negative
        if (damage < 0)
        {
            return;
        }

        int leftOverDamage = (laneAffixes[lane].ModifyValueOfAffix(EAffixType.Armor, -damage, false) ?? -damage);
        laneAffixes[lane].ModifyValueOfAffix(EAffixType.Armor, Mathf.Max(leftOverDamage, 0), true);

        if (leftOverDamage < 0)
        {
            DDGamePlaySingletonHolder.Instance.Dungeon.DoDamage(Mathf.Abs(leftOverDamage));
        }
    }

    public IEnumerator DrawACard()
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
                yield return deck.ShuffleInCardsOverTime(discard.GetAndClearDiscard());
            }
        }

        if (cardAvail)
        {
            yield return hand.AddCard(deck.GetTopCard(), true);
        }
    }

    public IEnumerator EndOfTurn()
    {
        yield return hand.EndOfTurn();
    }

    public IEnumerator DiscardHand()
    {
        yield return hand.DiscardHand(discard);
    }

    public IEnumerator DiscardCard(DDCardInHand card)
    {
        yield return hand.DiscardCard(card, discard);
    }

    public void SomethingSelected(DDSelection selection)
    {
        if (cardResolving != null)
        {
            return;
        }

        if (!selectedCard)
        {
            if (DDGamePlaySingletonHolder.Instance.Encounter.CurrentPhase == EEncounterPhase.PlayersTurn)
            {
                DDCardInHand card = selection as DDCardInHand;
                if (card)
                {
                    if (card.CardSelected(selectedCardLocation.position))
                    {
                        selectedCard = card;
                        cardTargets = selectedCard.GetCardTarget();
                        cardSelections = new List<DDSelection>(cardTargets.Count);
                        currentTargetIndex = 0;
                        DDGamePlaySingletonHolder.Instance.PlayerSelector.SetSelectionLayer(
                            cardTargets[currentTargetIndex].TargetType.GetLayer());
                    }
                }
            }
        }
        else if (selectedCard)
        {
            // Safety check here for targettype because cards/player are now UI based and can be selected
            if (selection.TargetType == cardTargets[currentTargetIndex].TargetType &&
                selectedCard.IsSelectionValid(cardSelections, selection, currentTargetIndex))
            {
                DDGlobalManager.Instance.ClipLibrary.SelectTarget.PlayNow();

                cardSelections.Add(selection);
                if (++currentTargetIndex >= cardTargets.Count)
                {
                    cardResolving = StartCoroutine(WaitingForCardExecution());
                }
                else if (selectedCard.ShouldExecuteEarly(cardSelections))
                {
                    // if no valid selections left, execute card
                    // This is sort of only possible if a card specifically targets 2+ enemies
                    // Or if a card targets two other cards, but actually this should be up to the card?
                    // Like we wouldn't want something that says Discard 2 cards and Deal 50 damage, but then allow it if there is only one other card in hand
                    // But we should allow something to says "Add 2 Finish to 2 Cards", be only select 1 if 1 other card is valid?
                    // I don't want to use "Up To".
                    cardResolving = StartCoroutine(WaitingForCardExecution());
                }
                else
                {
                    DDGamePlaySingletonHolder.Instance.PlayerSelector.SetSelectionLayer(cardTargets[currentTargetIndex]
                        .TargetType.GetLayer());
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

    public IEnumerator AddToMomentum(int value)
    {
        momentumCounter += value;

        yield return null;

        for (int i = 0; i < value; i++)
        {
            yield return GainedMomentum.Occured(this);
        }

        yield return DDGamePlaySingletonHolder.Instance.Encounter.CheckDestroyedEnemies();

        momentum.text = momentumCounter.ToString();
    }

    public void RemoveFromMomentum(int value)
    {
        momentumCounter -= value;
        momentum.text = momentumCounter.ToString();
    }

    private IEnumerator WaitingForCardExecution()
    {
        yield return hand.CardRemoved(selectedCard);

        DDGamePlaySingletonHolder.Instance.PlayerSelector.SetToPlayerCard();

        DDGamePlaySingletonHolder.Instance.Player.CardLifeTimeChanged?.Invoke(selectedCard,
            EPlayerCardLifeTime.PrePlayed);

        yield return selectedCard.ExecuteFinishes(EPlayerCardLifeTime.PrePlayed);

        yield return selectedCard.ExecuteCard(cardSelections);

        DDGamePlaySingletonHolder.Instance.Player.CardLifeTimeChanged?.Invoke(selectedCard,
            EPlayerCardLifeTime.PostPlayed);

        cardsExecutedThisTurn.Add(selectedCard.CurrentCard);
        cardsExecutedThisEncounter.Add(selectedCard.CurrentCard);

        yield return selectedCard.ExecuteFinishes(EPlayerCardLifeTime.PostPlayed);

        if (selectedCard)
        {
            // Check if this works with fleeting cards?
            yield return discard.CardDiscarded(selectedCard);
        }

        selectedCard = null;

        yield return DDGamePlaySingletonHolder.Instance.Encounter.CheckDestroyedEnemies();

        cardResolving = null;
    }

    public void DealDamageToEnemy(int damage, ERangeType rangeType, DDEnemyOnBoard enemyOnBoard, bool useExpertise)
    {
        int totalDamage = damage;

        if (useExpertise)
        {
            int? dex = affixManager.TryGetAffixValue(EAffixType.Expertise);
            if (dex != null)
            {
                totalDamage += dex.Value;
            }

            totalDamage += GetFinishCountByType(EPlayerCardFinish.Sharp);
        }

        enemyOnBoard.TakeDamage(totalDamage, rangeType, false);
    }

    public int GetFinishCountByType(EPlayerCardFinish finishType)
    {
        return hand.GetFinishCountByType(finishType);
    }
}