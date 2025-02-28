using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DDDungeonStats
{
    public int EventsCompleted;
    public int EncountersCompleted;
}

public class DDDungeon : MonoBehaviour
{
    // The current card, so we don't add it to discard before it is finished.
    private DDDungeonCardBase currentDungeonCard = null;

    // Dungeon Deck
    private List<DDDungeonCardBase> dungeonDeck = new List<DDDungeonCardBase>();

    // Dungeon Discard
    private List<DDDungeonCardBase> dungeonDiscard = new List<DDDungeonCardBase>();
    public int DungeonDiscardCount => dungeonDiscard.Count;

    private DDDungeonStats dungeonStats;
    public DDDungeonStats DungeonStats => dungeonStats;

    [Header("Player")]
    // Players Deck
    private List<DDCardBase> playerDeck = new List<DDCardBase>();

    public List<DDCardBase> PlayerDeck => playerDeck;

    // Players Health
    [SerializeField] private int startingHealth;

    private int maxHealth;
    public int MaxHealth => maxHealth;

    private int currentHealth;

    private EDungeonPhase currentDungeonPhase = EDungeonPhase.DungeonStart;

    [SerializeField] private List<DDArtifactBase> artifacts = new List<DDArtifactBase>();

    [Header("Areas")] [SerializeField] private DDEncounter encounter;
    [SerializeField] private DDDungeonCardSelection dungeonCardSelection;
    [SerializeField] private DDCardSelection playerCardSelection;
    [SerializeField] private DDEventArea eventArea;
    [SerializeField] private DDLeisureArea leisureArea;
    [SerializeField] private DDShowDeckArea showDeckArea;
    [SerializeField] private DDShopArea shopArea;

    public UnityEngine.Events.UnityEvent<EDungeonPhase> PhaseChanged;

    // Gold
    [Header("Gold")] private int goldAmount;
    public int GoldAmount => goldAmount;

    public UnityEngine.Events.UnityEvent<int> GoldAmountChanged;

    // Artifacts/Equipment
    [Header("Testing")] [SerializeField] private bool forceInternalData;

    [SerializeField] private TMPro.TextMeshProUGUI healthText;
    [SerializeField] private TMPro.TextMeshProUGUI goldText;

    [SerializeField] private TMPro.TextMeshProUGUI dungeonDeckCount;
    [SerializeField] private TMPro.TextMeshProUGUI dungeonDiscardCount;

    [SerializeField] private TMPro.TextMeshProUGUI playerDeckCount;
    [SerializeField] private TMPro.TextMeshProUGUI playerDiscardCount;

    [SerializeField] private List<DDDungeonCardBase> startingDungeonDeck;
    [SerializeField] private List<DDCardBase> startingPlayerDeck;
    [SerializeField] private List<DDArtifactBase> startingArtifacts;

    [SerializeField] private DDCardShown cardPrefabForAdded;
    [SerializeField] private Transform cardAddedEnd;

    [SerializeField] private DDDungeonCardShown dungeonCardPrefabForAdded;
    [SerializeField] private Transform dungeonCardAddedStart;
    public Vector3 DungeonCardStartPosition => dungeonCardAddedStart.position;
    [SerializeField] private Transform dungeonCardAddedEnd;

    [SerializeField] private Transform artifactUIParent;

    [SerializeField] private DDArtifactUI artifactUIPrefab;

    [SerializeField] private int startingGold = 500;

    [SerializeField] private TMPro.TextMeshProUGUI toolTip;

    private void Awake()
    {
        if (!forceInternalData)
        {
            DDAdventurerData adventurerData = DDGlobalManager.Instance.SelectedAdventurer;
            maxHealth = adventurerData.StartingHealth;
            currentHealth = maxHealth;

            goldAmount = adventurerData.StartingGold;

            playerDeck.AddRange(adventurerData.StartingDeck);

            dungeonDeck.AddRange(DDGlobalManager.Instance.SelectedDungeon.DungeonOrder[0].Cards);
            // Figure out side quests here
        }
        else
        {
            maxHealth = startingHealth;
            currentHealth = maxHealth;

            dungeonDeck.AddRange(startingDungeonDeck);

            playerDeck.AddRange(startingPlayerDeck);

            goldAmount = startingGold;
        }

        UpdateHealthText();

        dungeonStats = new DDDungeonStats();

        playerDeckCount.text = playerDeck.Count.ToString();
        // Should have a global player discard for cards that will start in an encounter in the discard pile
        playerDiscardCount.text = "0";
        dungeonDeckCount.text = dungeonDeck.Count.ToString();
        goldText.text = goldAmount.ToString();

        PromptDungeonCard();

        DDGamePlaySingletonHolder.Instance.ShowDeckArea.OnClose.AddListener(DisplayDeckClosed);
    }

    public void SetToolTip(string value)
    {
        toolTip.text = value;
    }
    
    private void TurnOffAreas()
    {
        if (eventArea.gameObject.activeInHierarchy)
        {
            eventArea.gameObject.SetActive(false);
        }

        if (leisureArea.gameObject.activeInHierarchy)
        {
            leisureArea.gameObject.SetActive(false);
        }

        if (playerCardSelection.gameObject.activeInHierarchy)
        {
            playerCardSelection.gameObject.SetActive(false);
        }

        if (dungeonCardSelection.gameObject.activeInHierarchy)
        {
            dungeonCardSelection.gameObject.SetActive(false);
        }

        if (shopArea.gameObject.activeInHierarchy)
        {
            shopArea.gameObject.SetActive(false);
        }
    }

    #region Dungeon Deck Related

    public void RemoveCardFromDungeonDiscard(DDDungeonCardBase card)
    {
        dungeonDiscard.Remove(card);
        dungeonDiscardCount.text = dungeonDiscard.Count.ToString();
    }

    public void AddCardToDungeonDeck(DDDungeonCardBase card)
    {
        dungeonDeck.Add(card);

        DDDungeonCardShown shown = GameObject.Instantiate(dungeonCardPrefabForAdded, dungeonCardAddedStart.position,
            Quaternion.identity);
        shown.SetUpDungeonCard(card, 0, false);
        shown.transform.DOMove(dungeonCardAddedEnd.position, .6f, false);
        Destroy(shown.gameObject, .61f);

        dungeonDeckCount.text = dungeonDeck.Count.ToString();
    }

    public void AddCardToDungeonDeck(List<DDDungeonCardBase> cards)
    {
        StartCoroutine(AddCardToDungeonDeckOvertime(cards));
    }

    public IEnumerator AddCardToDungeonDeckOvertime(List<DDDungeonCardBase> cards)
    {
        for (int i = 0; i < cards.Count; i++)
        {
            DDDungeonCardBase card = cards[i];

            DDDungeonCardShown shown = GameObject.Instantiate(dungeonCardPrefabForAdded, dungeonCardAddedStart.position,
                Quaternion.identity);
            shown.SetUpDungeonCard(card, 0, false);
            shown.transform.DOMove(dungeonCardAddedEnd.position, .3f, false);
            Destroy(shown.gameObject, .35f);
            dungeonDeck.Add(card);
            dungeonDeckCount.text = dungeonDeck.Count.ToString();

            yield return new WaitForSeconds(.25f);
        }
    }

    public void PromptDungeonCard()
    {
        if (currentDungeonCard != null)
        {
            dungeonDiscard.Add(currentDungeonCard);
            dungeonDiscardCount.text = dungeonDiscard.Count.ToString();
            currentDungeonCard = null;
        }

        TurnOffAreas();

        // Do something based on where it comes from?
        dungeonCardSelection.DisplayDungeonCards(ref dungeonDeck);
        ChangeDungeonPhase(EDungeonPhase.DungeonCardSelection);
    }

    public void DungeonCardSelected(DDDungeonCardBase card)
    {
        // Do something here maybe that modifies the card selected
        dungeonDeck.Remove(card);
        dungeonDeckCount.text = dungeonDeck.Count.ToString();

        currentDungeonCard = card;

        switch (card.Type)
        {
            case EDungeonCardType.Encounter:
                DDDungeonCardEncounter encounterCard = card as DDDungeonCardEncounter;
                if (encounterCard != null)
                {
                    StartEncounter(encounterCard);
                }

                break;
            case EDungeonCardType.Event:
                DDDungeonCardEvent eventCard = card as DDDungeonCardEvent;
                if (eventCard != null)
                {
                    StartEvent(eventCard);
                }

                break;
            case EDungeonCardType.Leisure:
                DDDungeonCardLeisure leisureCard = card as DDDungeonCardLeisure;
                if (leisureCard != null)
                {
                    StartLeisure(leisureCard);
                }

                break;
            case EDungeonCardType.Shop:
                DDDungeonCardShop shopCard = card as DDDungeonCardShop;
                if (shopCard != null)
                {
                    StartShop(shopCard);
                }

                break;
        }
    }

    #endregion

    #region Player Card Related

    public void EncounterCompleted(DDDungeonCardEncounter encounterCard)
    {
        TurnOffAreas();

        ++dungeonStats.EncountersCompleted;

        playerDeckCount.text = playerDeck.Count.ToString();
        // Should have a global player discard for cards that will start in an encounter in the discard pile
        playerDiscardCount.text = "0";

        StartCoroutine(EncounterCompletedOvertime(encounterCard));
    }

    public IEnumerator EncounterCompletedOvertime(DDDungeonCardEncounter encounterCard)
    {
        yield return StartCoroutine(encounterCard.EncounterCompleted());
        PromptPlayerCard(encounterCard);
    }

    public void PromptPlayerCard(DDDungeonCardEncounter encounterCard)
    {
        playerCardSelection.DisplayPlayerCards(encounterCard);
        ChangeDungeonPhase(EDungeonPhase.PlayerCardSelection);
    }

    public void PromptPlayerCard(List<DDCardBase> specificCards)
    {
        TurnOffAreas();
        playerCardSelection.DisplayPlayerCards(specificCards);
        ChangeDungeonPhase(EDungeonPhase.PlayerCardSelection);
    }

    public void AddCardToDeck(DDCardBase card, Vector3 startPos)
    {
        StartCoroutine(AddCardToDeckOvertime(card, startPos));
    }

    public IEnumerator AddCardToDeckOvertime(DDCardBase card, Vector3 startPos)
    {
        DDCardShown shown = GameObject.Instantiate(cardPrefabForAdded, startPos, Quaternion.identity);
        shown.SetUpCard(card);
        shown.transform.DOMove(cardAddedEnd.position, .3f, false);
        Destroy(shown.gameObject, .35f);
        playerDeck.Add(card);
        playerDeckCount.text = playerDeck.Count.ToString();

        yield return new WaitForSeconds(.25f);
    }

    // TODO Probably remove by index?
    public void RemoveCardFromDeck(DDCardBase card)
    {
        playerDeck.Remove(card);
        playerDeckCount.text = playerDeck.Count.ToString();
    }

    #endregion

    #region Artifacts

    public void EquipArtifact(DDArtifactBase artifact)
    {
        artifacts.Add(artifact);
        artifact.Equipped();

        DDArtifactUI artifactUI = Instantiate(artifactUIPrefab, artifactUIParent);
        artifactUI.SetUpArtifact(artifact);
    }

    #endregion

    public void StartEvent(DDDungeonCardEvent eventCard)
    {
        TurnOffAreas();
        eventArea.DisplayEvent(eventCard);
        ChangeDungeonPhase(EDungeonPhase.Event);
    }

    public void EndEvent()
    {
        ++dungeonStats.EventsCompleted;

        DDGamePlaySingletonHolder.Instance.Dungeon.PromptDungeonCard();
    }

    public void StartLeisure(DDDungeonCardLeisure leisureCard)
    {
        TurnOffAreas();
        leisureArea.DisplayLeisure(leisureCard);
        ChangeDungeonPhase(EDungeonPhase.Leisure);
    }

    public void StartShop(DDDungeonCardShop shopCard)
    {
        TurnOffAreas();
        shopArea.DisplayShop(shopCard);
        ChangeDungeonPhase(EDungeonPhase.Shop);
    }

    public void StartEncounter(DDDungeonCardEncounter encounterCard)
    {
        TurnOffAreas();
        encounter.SetUpEncounter(encounterCard);
        ChangeDungeonPhase(EDungeonPhase.Encounter);
    }

    private void ChangeDungeonPhase(EDungeonPhase toPhase)
    {
        currentDungeonPhase = toPhase;
        PhaseChanged.Invoke(currentDungeonPhase);
    }

    private void UpdateHealthText()
    {
        healthText.text = currentHealth.ToString() + " / " + maxHealth.ToString();
    }

    public void DoDamage(int value)
    {
        currentHealth -= value;
        UpdateHealthText();

        if (currentHealth <= 0)
        {
            // Game over
        }
    }

    public void HealDamage(int value)
    {
        currentHealth = Mathf.Min(currentHealth + value, maxHealth);
        UpdateHealthText();
    }

    public DDArtifactBase GrabArtifact()
    {
        if (startingArtifacts.Count == 0)
        {
            return null;
        }

        int index = Random.Range(0, startingArtifacts.Count);
        DDArtifactBase artifact = startingArtifacts[index];
        startingArtifacts.RemoveAt(index);
        return artifact;
    }

    public void ReturnArtifact(DDArtifactBase artifact)
    {
        startingArtifacts.Add(artifact);
    }

    public void DisplayDungeonDeck()
    {
        TurnOffAreas();
        showDeckArea.gameObject.SetActive(true);
        showDeckArea.ShowDungeonDeck(dungeonDeck);
    }

    public void DisplayDungeonDiscard()
    {
        TurnOffAreas();
        showDeckArea.gameObject.SetActive(true);
        showDeckArea.ShowDungeonDeck(dungeonDiscard);
    }

    public void DisplayPlayerDeck()
    {
        TurnOffAreas();
        showDeckArea.gameObject.SetActive(true);

        if (currentDungeonPhase == EDungeonPhase.Encounter)
        {
            showDeckArea.ShowPlayerDeck(DDGamePlaySingletonHolder.Instance.Player.CurrentDeck);
        }
        else
        {
            showDeckArea.ShowPlayerDeck(playerDeck);
        }
    }

    public void DisplayPlayerDiscard()
    {
        if (currentDungeonPhase == EDungeonPhase.Encounter)
        {
            TurnOffAreas();
            showDeckArea.gameObject.SetActive(true);
            showDeckArea.ShowPlayerDeck(DDGamePlaySingletonHolder.Instance.Player.CurrentDiscard);
        }
        else
        {
            // Show global discard?
            //showDeckArea.ShowPlayerDeck(playerDeck);
        }
    }

    public void DisplayDeckClosed()
    {
        showDeckArea.gameObject.SetActive(false);

        switch (currentDungeonPhase)
        {
            case EDungeonPhase.DungeonStart:
                break;
            case EDungeonPhase.DungeonCardSelection:
                dungeonCardSelection.gameObject.SetActive(true);
                break;
            case EDungeonPhase.Event:
                eventArea.gameObject.SetActive(true);
                break;
            case EDungeonPhase.Leisure:
                leisureArea.gameObject.SetActive(true);
                break;
            case EDungeonPhase.Encounter:
                encounter.gameObject.SetActive(true);
                break;
            case EDungeonPhase.PlayerCardSelection:
                playerCardSelection.gameObject.SetActive(true);
                break;
            case EDungeonPhase.DungeonLost:
                break;
            case EDungeonPhase.DungeonWon:
                break;
            default:
                break;
        }
    }

    public void AddOrRemoveGold(int amount)
    {
        goldAmount = Mathf.Max(goldAmount + amount, 0);
        goldText.text = goldAmount.ToString();
        GoldAmountChanged?.Invoke(goldAmount);
    }

    public bool HasEnoughGold(int amount)
    {
        return goldAmount >= amount;
    }
}