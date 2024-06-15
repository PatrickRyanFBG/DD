using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDDungeon : MonoBehaviour
{
    // Dungeon Deck
    private List<DDDungeonCardBase> dungeonDeck = new List<DDDungeonCardBase>();
    // Dungeon Discard
    private List<DDDungeonCardBase> dungeonDiscard = new List<DDDungeonCardBase>();

    [Header("Player")]
    // Players Deck
    private List<DDCardBase> playerDeck = new List<DDCardBase>();
    public List<DDCardBase> PlayerDeck { get { return playerDeck; } }

    // Players Health
    [SerializeField]
    private int startingHealth;

    private int maxHealth;
    private int currentHealth;

    private EDungeonPhase currentDungeonPhase = EDungeonPhase.DungeonStart;

    public bool HasKey;

    [SerializeField]
    private List<DDArtifactBase> artifacts = new List<DDArtifactBase>();

    [Header("States")]
    [SerializeField]
    private DDEncounter encounter;
    [SerializeField]
    private DDDungeonCardSelection dungeonCardSelection;
    [SerializeField]
    private DDCardSelection playerCardSelection;
    [SerializeField]
    private DDEventArea eventArea;
    [SerializeField]
    private DDLeisureArea leisureArea;

    public UnityEngine.Events.UnityEvent<EDungeonPhase> PhaseChanged;

    // Artifacts/Equipment
    [Header("Testing")]
    [SerializeField]
    private TMPro.TextMeshProUGUI healthText;
    [SerializeField]
    private TMPro.TextMeshProUGUI dungeonDeckCount;
    [SerializeField]
    private TMPro.TextMeshProUGUI dungeonDiscardCount;

    [SerializeField]
    private List<DDDungeonCardBase> startingDungeonDeck;
    [SerializeField]
    private List<DDCardBase> startingPlayerDeck;

    [SerializeField]
    private GameObject keyImage;

    [SerializeField]
    private Transform artifactUIParent;

    [SerializeField]
    private DDArtifactUI artifactUIPrefab;

    private void Awake()
    {
        maxHealth = startingHealth;
        currentHealth = maxHealth;

        UpdateHealthText();

        dungeonDeck.AddRange(startingDungeonDeck);
        dungeonDeckCount.text = dungeonDeck.Count.ToString();
        playerDeck.AddRange(startingPlayerDeck);

        PromptDungeonCard();
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

        if(playerCardSelection.gameObject.activeInHierarchy)
        {
            playerCardSelection.gameObject.SetActive(false);
        }

        if (dungeonCardSelection.gameObject.activeInHierarchy)
        {
            dungeonCardSelection.gameObject.SetActive(false);
        }
    }

    #region Dungeon Deck Related
    public void AddCardToDungeonDeck(DDDungeonCardBase card)
    {
        dungeonDeck.Add(card);
        dungeonDeckCount.text = dungeonDeck.Count.ToString();
    }

    public void PromptDungeonCard()
    {
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
        dungeonDiscard.Add(card);
        dungeonDiscardCount.text = dungeonDiscard.Count.ToString();

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
                if(eventCard != null)
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
        }
    }
    #endregion

    #region Player Card Related
    public void PromptPlayerCard(DDDungeonCardEncounter encounterCard)
    {
        TurnOffAreas();
        playerCardSelection.DisplayPlayerCards(encounterCard);
        ChangeDungeonPhase(EDungeonPhase.PlayerCardSelection);
    }

    public void PlayerCardSelect(DDCardBase card)
    {
        playerDeck.Add(card);
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

    public void StartLeisure(DDDungeonCardLeisure leisureCard)
    {
        TurnOffAreas();
        leisureArea.DisplayLeisure(leisureCard);
        ChangeDungeonPhase(EDungeonPhase.Leisure);
    }

    public void StartEncounter(DDDungeonCardEncounter encounterCard)
    {
        TurnOffAreas();
        encounter.SetUpEncounter(encounterCard);
        ChangeDungeonPhase(EDungeonPhase.Encounter);
    }

    private void ChangeDungeonPhase(EDungeonPhase toPhase)
    {
        keyImage.gameObject.SetActive(HasKey);
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
        currentHealth += value;
        UpdateHealthText();
    }
}
