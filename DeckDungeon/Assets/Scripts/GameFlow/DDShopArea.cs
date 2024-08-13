using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DDShopArea : MonoBehaviour
{
    [SerializeField]
    private TMPro.TextMeshProUGUI shopName;
    public TMPro.TextMeshProUGUI ShopName { get { return shopName; } }

    [SerializeField]
    private RawImage image;
    public RawImage Image { get { return image; } }

    [SerializeField]
    private TMPro.TextMeshProUGUI description;
    public TMPro.TextMeshProUGUI Description { get { return description; } }

    [SerializeField]
    private DDCardShown[] cards;

    [SerializeField]
    private DDDungeonCardShown[] dungeonCards;

    [SerializeField]
    private DDArtifactUI[] artifacts;

    [SerializeField]
    private Camera cardAreaCamera;

    private Ray camRay;
    public Ray CamRay { get { return camRay; } }

    private RaycastHit hit;
    private DDSelection currentlyHovered;

    private DDDungeonCardShop currentShop;

    private void Awake()
    {
        
    }

    private void Update()
    {
        bool camHitSomething = false;
        Vector3 pos = Input.mousePosition;
        pos -= new Vector3(Screen.width / 2, (Screen.height / 2), 0);
        pos *= (1 / .65f);
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
                    case DDCardShown:
                        DDCardShown cardShown = currentlyHovered as DDCardShown;
                        cardShown.CardSelected();
                        break;
                    case DDDungeonCardShown:
                        DDDungeonCardShown dungeonCardShown = currentlyHovered as DDDungeonCardShown;
                        dungeonCardShown.DungeonCardSelected();
                        break;
                }
            }
        }
    }

    public void DisplayShop(DDDungeonCardShop shopCard)
    {
        currentShop = shopCard;
        gameObject.SetActive(true);
        currentShop.DisplayShop(this);
    }
}
