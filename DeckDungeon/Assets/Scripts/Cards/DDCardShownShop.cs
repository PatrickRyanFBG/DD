using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDCardShownShop : DDCardShown
{
    private int currentPrice;
    public int CurrentPrice { get => currentPrice; }

    [SerializeField]
    private GameObject priceParent;

    [SerializeField]
    private TMPro.TextMeshProUGUI priceText;

    [SerializeField]
    private UnityEngine.UI.Image priceBG;

    private Color canBuyColor;

    private void Awake()
    {
        canBuyColor = priceBG.color;
    }

    private void OnEnable()
    {
        SingletonHolder.Instance.Dungeon.GoldAmountChanged.AddListener(GoldAmountChanged);
        GoldAmountChanged(SingletonHolder.Instance.Dungeon.GoldAmount);
    }

    private void OnDisable()
    {
        SingletonHolder.Instance.Dungeon.GoldAmountChanged.RemoveListener(GoldAmountChanged);
    }

    public void GoldAmountChanged(int amount)
    {
        priceBG.color = amount < currentPrice ? Color.red : canBuyColor;
    }

    public void SetUpShopCard(DDCardBase cardBase)
    {
        base.SetUpCard(cardBase);

        if (!priceParent.activeSelf)
        {
            priceParent.SetActive(true);
        }
        currentPrice = cardBase.Price;
        priceText.text = currentPrice.ToString();
    }
}
