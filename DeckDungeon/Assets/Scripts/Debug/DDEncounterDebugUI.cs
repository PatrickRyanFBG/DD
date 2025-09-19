using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DDEncounterDebugUI : MonoBehaviour
{
    [SerializeField] private Button buttonPrefab;

    [SerializeField] private GameObject cardParent;
    [SerializeField] private GameObject locationParent;

    [SerializeField] private Transform scrollRectParent;

    [SerializeField] private Toggle skipCostToggle;

    private DDCardBase currentCard;

    private void Awake()
    {
        Dictionary<string, DDCardBase> allCards = DDGlobalManager.Instance.SelectedAdventurer.CardData.AllCards;
        int i = 0;
        Transform twoCardsParent = null;
        foreach (KeyValuePair<string, DDCardBase> kvp in allCards)
        {
            if (i == 0)
            {
                twoCardsParent = new GameObject("TwoCards", typeof(RectTransform)).transform;
                twoCardsParent.AddComponent<HorizontalLayoutGroup>();
                twoCardsParent.SetParent(scrollRectParent.transform);
            }

            DDCardBase card = kvp.Value;
            Button button = Instantiate(buttonPrefab, twoCardsParent);
            button.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = card.CardName;
            button.onClick.AddListener(() =>
            {
                currentCard = card;
                cardParent.SetActive(false);
                locationParent.SetActive(true);
            });
            button.gameObject.SetActive(true);
            
            i = (i + 1) % 2;
        }

        skipCostToggle.SetIsOnWithoutNotify(DDGamePlaySingletonHolder.Instance.DEBUG_SkipCosts);
    }

    public void ClearAndOff()
    {
        if (currentCard)
        {
            currentCard = null;
            cardParent.SetActive(true);
            locationParent.SetActive(false);
        }

        gameObject.SetActive(false);
    }

    public void AddToLocation(ECardLocation location)
    {
        StartCoroutine(DDGamePlaySingletonHolder.Instance.Player.AddCardTo(currentCard, null, location, false));
        currentCard = null;
        cardParent.SetActive(true);
        locationParent.SetActive(false);
    }
    
    public void AddCardToDeck() => AddToLocation(ECardLocation.Deck);
    public void AddCardToHand() => AddToLocation(ECardLocation.Hand);
    public void AddCardToDiscard() => AddToLocation(ECardLocation.Discard);

    public void ToggleSkipCosts()
    {
        DDGamePlaySingletonHolder.Instance.DEBUG_SkipCosts = !DDGamePlaySingletonHolder.Instance.DEBUG_SkipCosts;
    }

    public void ToggleWindow()
    {
        if (gameObject.activeSelf)
        {
            ClearAndOff();
        }
        else
        {
            gameObject.SetActive(true);
        }
    }
}