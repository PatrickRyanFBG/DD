using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDCanvasShowDeckArea : MonoBehaviour
{
    [SerializeField] private DDCardShown playerCardPrefab;
    private Queue<DDCardShown> freePlayerCards = new Queue<DDCardShown>(16);
    private List<DDCardShown> usedPlayerCards = new List<DDCardShown>(16);

    [SerializeField] private DDDungeonCardShown dungeonCardPrefab;
    private Queue<DDDungeonCardShown> freeDungeonCards = new Queue<DDDungeonCardShown>(16);
    private List<DDDungeonCardShown> usedDungeonCards = new List<DDDungeonCardShown>(16);

    [SerializeField] private RectTransform content;

    private DDSelection currentlyHovered;

    [SerializeField] private Vector2 xMinMax = new Vector2(-320, 320);
    [SerializeField] private float yOffset = -250;

    private int numberOfRows;

    public UnityEngine.Events.UnityEvent<DDCardBase> CardSelectedCallback;
    public UnityEngine.Events.UnityEvent<DDDungeonCardBase> DungeonCardSelectedCallback;

    public UnityEngine.Events.UnityEvent OnClose;

    public void CloseArea()
    {
        OnClose?.Invoke();

        gameObject.SetActive(false);
    }

    public void ShowPlayerDeck(List<DDCardBase> cardsToShow)
    {
        ClearArea();

        // We need to only update this scene when the deck is altared or we are now showing the current matchdeck
        // So maybe the deck has a cached verison number that we check compared to the number here.
        // The deck verison number gets updated when it changes and can pass here.

        numberOfRows = 0;

        for (int i = 0; i < cardsToShow.Count; i += 5)
        {
            for (int j = 0; j < 5 && i + j < cardsToShow.Count; j++)
            {
                DDCardShown cardShown = GetAFreePlayerCard();
                cardShown.SetUpCard(cardsToShow[i + j]);
                float percent = Mathf.Lerp(xMinMax.x, xMinMax.y, j / 4f);
                cardShown.RectTransform.anchoredPosition = new Vector2(percent, -25 + numberOfRows * yOffset);
                cardShown.OnCardSelected.AddListener(CardSelected);
            }

            ++numberOfRows;
        }

        content.sizeDelta = new Vector2(content.sizeDelta.x, numberOfRows * 300);
    }

    public void ShowPlayerDeck(List<DDCardInHand> cardsToShow)
    {
        ClearArea();

        // We need to only update this scene when the deck is altared or we are now showing the current matchdeck
        // So maybe the deck has a cached verison number that we check compared to the number here.
        // The deck verison number gets updated when it changes and can pass here.

        numberOfRows = 0;

        for (int i = 0; i < cardsToShow.Count; i += 5)
        {
            for (int j = 0; j < 5 && i + j < cardsToShow.Count; j++)
            {
                DDCardShown cardShown = GetAFreePlayerCard();
                cardShown.SetUpCard(cardsToShow[i + j].CurrentCard);
                float percent = Mathf.Lerp(xMinMax.x, xMinMax.y, j / 4f);
                cardShown.RectTransform.anchoredPosition = new Vector2(percent, -25 + numberOfRows * yOffset);
                cardShown.OnCardSelected.AddListener(CardSelected);
            }

            ++numberOfRows;
        }

        content.sizeDelta = new Vector2(content.sizeDelta.x, numberOfRows * 300);

        gameObject.SetActive(true);
    }

    public void ClearArea()
    {
        for (int i = usedPlayerCards.Count - 1; i >= 0; --i)
        {
            usedPlayerCards[i].gameObject.SetActive(false);
            usedPlayerCards[i].OnCardSelected.RemoveListener(CardSelected);

            freePlayerCards.Enqueue(usedPlayerCards[i]);
            usedPlayerCards.RemoveAt(i);
        }

        for (int i = usedDungeonCards.Count - 1; i >= 0; --i)
        {
            usedDungeonCards[i].gameObject.SetActive(false);
            usedDungeonCards[i].OnCardSelected.RemoveListener(DungeonCardSelected);

            freeDungeonCards.Enqueue(usedDungeonCards[i]);
            usedDungeonCards.RemoveAt(i);
        }

        if (DDGamePlaySingletonHolder.Instance != null)
        {
            DDGamePlaySingletonHolder.Instance.PlayerSelector.BlockClicks(true);
        }
    }

    public void ShowDungeonDeck(List<DDDungeonCardBase> cardsToShow)
    {
        ClearArea();

        // We need to only update this scene when the deck is altared or we are now showing the current matchdeck
        // So maybe the deck has a cached verison number that we check compared to the number here.
        // The deck verison number gets updated when it changes and can pass here.

        numberOfRows = 0;

        for (int i = 0; i < cardsToShow.Count; i += 5)
        {
            for (int j = 0; j < 5 && i + j < cardsToShow.Count; j++)
            {
                DDDungeonCardShown cardShown = GetAFreeDungeonCard();
                cardShown.SetUpDungeonCard(cardsToShow[i + j], -1, true);
                float percent = Mathf.Lerp(xMinMax.x, xMinMax.y, j / 4f);
                cardShown.RectTransform.anchoredPosition = new Vector2(percent, -25 + numberOfRows * yOffset);
                cardShown.OnCardSelected.AddListener(DungeonCardSelected);
            }

            ++numberOfRows;
        }

        content.sizeDelta = new Vector2(content.sizeDelta.x, numberOfRows * 300);

        gameObject.SetActive(true);
    }
    
    public void CardSelected(DDCardShown cardShown)
    {
        CardSelectedCallback?.Invoke(cardShown.CurrentCard);
    }

    private DDCardShown GetAFreePlayerCard()
    {
        DDCardShown cardShown;

        if (freePlayerCards.Count > 0)
        {
            cardShown = freePlayerCards.Dequeue();
        }
        else
        {
            cardShown = Instantiate(playerCardPrefab, content);
        }

        cardShown.gameObject.SetActive(true);
        usedPlayerCards.Add(cardShown);

        cardShown.transform.SetAsFirstSibling();

        return cardShown;
    }

    public void DungeonCardSelected(DDDungeonCardShown dungeonCardShown)
    {
        DungeonCardSelectedCallback?.Invoke(dungeonCardShown.CurrentDungeonCard);
    }

    private DDDungeonCardShown GetAFreeDungeonCard()
    {
        DDDungeonCardShown cardShown;

        if (freeDungeonCards.Count > 0)
        {
            cardShown = freeDungeonCards.Dequeue();
        }
        else
        {
            cardShown = Instantiate(dungeonCardPrefab, content);
        }

        cardShown.gameObject.SetActive(true);
        usedDungeonCards.Add(cardShown);
        
        cardShown.transform.SetAsFirstSibling();
        
        return cardShown;
    }
}