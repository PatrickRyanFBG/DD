using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DDShopArea : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI shopName;
    public TMPro.TextMeshProUGUI ShopName => shopName;

    [SerializeField] private RawImage image;
    public RawImage Image => image;

    [SerializeField] private TMPro.TextMeshProUGUI description;
    public TMPro.TextMeshProUGUI Description => description;

    [SerializeField] private DDCardShownShop[] cards;

    // Shop to buy DungeonCards
    //[SerializeField] private DDDungeonCardShown[] dungeonCards;

    [SerializeField] private DDArtifactUI[] artifacts;

    private DDDungeonCardShop currentShop;

    [SerializeField] private Camera mainUICamera;

    /*
    private void Update()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(renderTexture.rectTransform, Input.mousePosition, null,
            out var localPoint);
        bool camHitSomething = false;
        localPoint.x = localPoint.x / renderTexture.rectTransform.sizeDelta.x;
        localPoint.x *= Screen.width;
        localPoint.y = localPoint.y / renderTexture.rectTransform.sizeDelta.y;
        localPoint.y *= Screen.height;

        camRay = cardAreaCamera.ScreenPointToRay(new Vector3(localPoint.x, localPoint.y));
        LayerMask currentAndCameraMask = cardAreaCamera.cullingMask;

        Debug.DrawLine(camRay.origin, camRay.origin + camRay.direction * 1000, Color.green);

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

                if (currentlyHovered)
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
                    case DDCardShownShop:
                        DDCardShownShop cardShown = currentlyHovered as DDCardShownShop;
                        if (DDGamePlaySingletonHolder.Instance.Dungeon.HasEnoughGold(cardShown.CurrentPrice))
                        {
                            DDGamePlaySingletonHolder.Instance.Dungeon.AddOrRemoveGold(-cardShown.CurrentPrice);

                            Debug.Log(cardShown.CurrentCard.name);
                            cardShown.gameObject.SetActive(false);
                            // Okay this is WILD but I need to go from world space of the actual card Object to screenSpace of the cardShown camera
                            Vector3 screenSpace = cardAreaCamera.WorldToScreenPoint(cardShown.transform.position);
                            // and then find the percentage of the pos
                            screenSpace.x /= Screen.width;
                            // scale it to the renderTexture Width
                            screenSpace.x *= renderTexture.rectTransform.sizeDelta.x;
                            screenSpace.y /= Screen.height;
                            screenSpace.y *= renderTexture.rectTransform.sizeDelta.y;

                            // then add it to the bottom left corner of the renderTexture
                            screenSpace.x += corners[0].x;
                            screenSpace.y += corners[0].y;

                            // then go back to worldSpace, but this time with the mainUI camera so it is in the correct space.
                            // xdd
                            Vector3 worldSpace = mainUICamera.ScreenToWorldPoint(screenSpace);

                            DDGamePlaySingletonHolder.Instance.Dungeon.AddCardToDeck(cardShown.CurrentCard, worldSpace);
                        }

                        break;
                    //case DDDungeonCardShown:
                    //    DDDungeonCardShown dungeonCardShown = currentlyHovered as DDDungeonCardShown;
                    //    dungeonCardShown.DungeonCardSelected();
                    //    break;
                }
            }
        }
    }
    */

    public void CardSelected(DDCardShown cardShownBase)
    {
        DDCardShownShop cardShown = cardShownBase as DDCardShownShop;
        if(!cardShown)
        {
            // This shouldn't happen
            return;
        }
        
        //if (DDGamePlaySingletonHolder.Instance.Dungeon.HasEnoughGold(cardShown.CurrentPrice))
        {
            DDGamePlaySingletonHolder.Instance.Dungeon.AddOrRemoveGold(-cardShown.CurrentPrice);

            cardShown.gameObject.SetActive(false);

            Vector3 worldSpace = mainUICamera.ScreenToWorldPoint(cardShown.transform.position);

            DDGamePlaySingletonHolder.Instance.Dungeon.AddCardToDeck(cardShown.CurrentCard, worldSpace);
        }
    }

    public void DisplayShop(DDDungeonCardShop shopCard)
    {
        currentShop = shopCard;
        gameObject.SetActive(true);
        currentShop.DisplayShop(this);

        //int amountOfCards = Random.Range(0, cards.Length);
        List<DDCardBase> generatedCards =
            DDGlobalManager.Instance.SelectedAdventurer.CardData.GenerateCards(cards.Length);
        for (int i = 0; i < generatedCards.Count; i++)
        {
            cards[i].SetUpShopCard(generatedCards[i]);
            //cards[i].OnCardSelected.AddListener(CardSelected);
        }
    }

    public void CloseShop()
    {
        DDGamePlaySingletonHolder.Instance.Dungeon.PromptDungeonCard();
    }
}