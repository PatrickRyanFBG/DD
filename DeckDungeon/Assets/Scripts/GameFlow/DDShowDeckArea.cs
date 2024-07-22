using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDShowDeckArea : MonoBehaviour
{
    [SerializeField]
    private DDCardShown playerCardPrefab;
    private Queue<DDCardShown> freePlayerCards = new Queue<DDCardShown>(16);
    private List<DDCardShown> usedPlayerCards = new List<DDCardShown>(16);

    [SerializeField]
    private Transform playerCardParent;

    [SerializeField]
    private DDDungeonCardShown dungeonCardPrefab;
    private Queue<DDDungeonCardShown> freeDungeonCards = new Queue<DDDungeonCardShown>(16);
    private List<DDDungeonCardShown> usedDungeonCards = new List<DDDungeonCardShown>(16);

    [SerializeField]
    private Transform[] cardPositions;

    [SerializeField]
    private Camera cardAreaCamera;

    [SerializeField]
    private DDFakeUIScrollbar scrollbar;

    private Ray camRay;
    public Ray CamRay { get { return camRay; } }

    private RaycastHit hit;
    private DDSelection currentlyHovered;

    private int numberOfRows;

    public void CloseArea()
    {
        SingletonHolder.Instance.Dungeon.DisplayDeckClosed();
        SingletonHolder.Instance.PlayerSelector.BlockClicks(false);
    }

    private void Update()
    {
        bool camHitSomething = false;
        Vector3 pos = Input.mousePosition;
        pos -= new Vector3(Screen.width / 2, (Screen.height / 2), 0);
        pos *= (1/.65f);
        pos += new Vector3(Screen.width / 2, (Screen.height / 2), 0);
        camRay = cardAreaCamera.ScreenPointToRay(pos);
        LayerMask currentAndCameraMask = cardAreaCamera.cullingMask;

        Debug.DrawLine(camRay.origin, camRay.origin + camRay.direction * 1000, Color.yellow);

        if (Physics.Raycast(camRay, out hit, 1000, currentAndCameraMask))
        {
            DDSelection mousedOver = hit.transform.GetComponent<DDSelection>();
            camHitSomething = true;

            if (currentlyHovered != mousedOver)
            {
                if (currentlyHovered)
                {
                    currentlyHovered.Unhovered();
                }

                currentlyHovered = mousedOver;

                if(currentlyHovered)
                {
                    currentlyHovered.Hovered();
                }
            }
        }

        if (!camHitSomething)
        {
            if (currentlyHovered)
            {
                currentlyHovered.Unhovered();
                currentlyHovered = null;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (currentlyHovered)
            {
                switch (currentlyHovered)
                {
                    case DDCardShown:
                        DDCardShown cardShown = currentlyHovered as DDCardShown;
                        cardShown.CardSelected();
                        break;
                    case DDScroller:
                        scrollbar.GrabbedBar(true);
                        break;
                }
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            scrollbar.GrabbedBar(false);
        }

        playerCardParent.localPosition = Vector3.forward * (Mathf.Lerp(2f * numberOfRows + .5f, 2f, scrollbar.Value));
    }

    public void ShowPlayerDeck(List<DDCardBase> cardsToShow)
    {
        ClearArea();

        // We need to only update this scene when the deck is altared or we are now showing the current matchdeck
        // So maybe the deck has a cached verison number that we check compared to the number here.
        // The deck verison number gets updated when it changes and can pass here.

        playerCardParent.localPosition = Vector3.zero;
        numberOfRows = 0;

        for (int i = 0; i < cardsToShow.Count; i += cardPositions.Length)
        {
            ++numberOfRows;
            for (int j = 0; j < cardPositions.Length && i + j < cardsToShow.Count; j++)
            {
                DDCardShown cardShown = GetAFreePlayerCard();
                cardShown.SetUpCard(cardsToShow[i + j]);
                cardShown.transform.position = cardPositions[j].position;
                cardShown.OnCardSelected.AddListener(CardSelected);
            }

            playerCardParent.position = playerCardParent.position + Vector3.forward * 3.5f;
        }

        playerCardParent.localPosition = Vector3.forward * 2f;

        scrollbar.AreaOpened();
    }

    public void ShowPlayerDeck(List<DDCardInHand> cardsToShow)
    {
        ClearArea();

        // We need to only update this scene when the deck is altared or we are now showing the current matchdeck
        // So maybe the deck has a cached verison number that we check compared to the number here.
        // The deck verison number gets updated when it changes and can pass here.

        playerCardParent.localPosition = Vector3.zero;
        numberOfRows = 0;

        for (int i = 0; i < cardsToShow.Count; i += cardPositions.Length)
        {
            ++numberOfRows;
            for (int j = 0; j < cardPositions.Length && i + j < cardsToShow.Count; j++)
            {
                DDCardShown cardShown = GetAFreePlayerCard();
                cardShown.SetUpCard(cardsToShow[i + j].CurrentCard);
                cardShown.transform.position = cardPositions[j].position;
                cardShown.OnCardSelected.AddListener(CardSelected);
            }

            playerCardParent.position = playerCardParent.position + Vector3.forward * 3.5f;
        }

        playerCardParent.localPosition = Vector3.forward * 2f;

        scrollbar.AreaOpened();
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

        SingletonHolder.Instance.PlayerSelector.BlockClicks(true);
    }

    public void ShowDungeonDeck(List<DDDungeonCardBase> cardsToShow)
    {
        ClearArea();

        // We need to only update this scene when the deck is altared or we are now showing the current matchdeck
        // So maybe the deck has a cached verison number that we check compared to the number here.
        // The deck verison number gets updated when it changes and can pass here.

        playerCardParent.localPosition = Vector3.zero;
        numberOfRows = 0;

        for (int i = 0; i < cardsToShow.Count; i += cardPositions.Length)
        {
            ++numberOfRows;
            for (int j = 0; j < cardPositions.Length && i + j < cardsToShow.Count; j++)
            {
                DDDungeonCardShown cardShown = GetAFreeDungeonCard();
                cardShown.SetUpDungeonCard(cardsToShow[i + j], -1, true);
                cardShown.transform.position = cardPositions[j].position;
                cardShown.OnCardSelected.AddListener(DungeonCardSelected);
            }

            playerCardParent.position = playerCardParent.position + Vector3.forward * 3.5f;
        }

        playerCardParent.localPosition = Vector3.forward * 2f;

        scrollbar.AreaOpened();
    }

    public void CardSelected(DDCardShown cardShown)
    {
        Debug.Log(cardShown.CurrentCard.name);
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
            cardShown = Instantiate(playerCardPrefab, playerCardParent);
        }

        cardShown.gameObject.SetActive(true);
        usedPlayerCards.Add(cardShown);

        return cardShown;
    }

    public void DungeonCardSelected(DDDungeonCardShown cardShown)
    {
        Debug.Log(cardShown.CurrentDungeonCard.name);
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
            cardShown = Instantiate(dungeonCardPrefab, playerCardParent);
        }

        cardShown.gameObject.SetActive(true);
        usedDungeonCards.Add(cardShown);

        return cardShown;
    }
}
