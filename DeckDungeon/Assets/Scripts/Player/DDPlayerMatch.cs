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

    [SerializeField] private DDPlayerMatchDeck deck;
    public DDPlayerMatchDeck Deck => deck;
    
    public List<DDCardInHand> CurrentDeck => deck.Cards;

    [SerializeField] private DDPlayerMatchHand hand;
    [SerializeField] private DDPlayerMatchDiscard discard;

    public List<DDCardInHand> CurrentDiscard => discard.Cards;

    private DDCardInHand selectedCard;
    private List<ETargetType> cardTargets;
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
        HealthChanged(DDGamePlaySingletonHolder.Instance.Dungeon.CurrentHealth, DDGamePlaySingletonHolder.Instance.Dungeon.MaxHealth);
    }

    private void OnDisable()
    {
        DDGamePlaySingletonHolder.Instance.PlayerSelector.SomethingSelected.RemoveListener(SomethingSelected);
        DDGamePlaySingletonHolder.Instance.Dungeon.OnHealthChange.RemoveListener(HealthChanged);
        cardResolving = null;
    }

    public void HealthChanged(int current, int max)
    {
        healthText.text = $"{current} / {max}";
        healthBar.fillAmount = (float)current/max;
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

    public IEnumerator AddCardToDiscard(DDCardBase card, Vector3? spawnPosition)
    {
        return discard.AddCardOverTime(card, spawnPosition);
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
                deck.ShuffleInCards(discard.GetAndClearDiscard());
                
                // Do some shuffle animation
                yield return new WaitForSeconds(.25f);
            }
        }

        if (cardAvail)
        {
            yield return hand.AddCard(deck.GetTopCard());
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
                    if (card.CardSelected(selectedCardLocation.localPosition))
                    {
                        selectedCard = card;
                        cardTargets = selectedCard.GetCardTarget();
                        cardSelections = new List<DDSelection>(cardTargets.Count);
                        currentTargetIndex = 0;
                        DDGamePlaySingletonHolder.Instance.PlayerSelector.SetSelectionLayer(cardTargets[currentTargetIndex].GetLayer());
                    }
                }
            }
        }
        else if (selectedCard)
        {
            // Safety check here for targettype because cards/player are now UI based and can be selected
            if (selection.TargetType == cardTargets[currentTargetIndex] && selectedCard.IsSelectionValid(selection, currentTargetIndex))
            {
                cardSelections.Add(selection);
                if (++currentTargetIndex >= cardTargets.Count)
                {
                    cardResolving = StartCoroutine(WaitingForCardExecution());
                    hand.CardRemoved(selectedCard);
                    // Check if this works with fleeting cards?
                    discard.CardDiscarded(selectedCard);

                    DDGamePlaySingletonHolder.Instance.PlayerSelector.SetToPlayerCard();
                    selectedCard = null;
                }
                else
                {
                    DDGamePlaySingletonHolder.Instance.PlayerSelector.SetSelectionLayer(cardTargets[currentTargetIndex].GetLayer());
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
        yield return selectedCard.ExecuteCard(cardSelections);

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