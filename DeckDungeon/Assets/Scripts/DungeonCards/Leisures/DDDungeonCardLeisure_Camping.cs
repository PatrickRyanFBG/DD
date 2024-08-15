using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class DDDungeonCardLeisure_Camping : DDDungeonCardLeisure
{
    [Header("Camping")]
    [SerializeField]
    private int healAmount = 25;
    public override void DisplayLeisure(DDLeisureArea area)
    {
        Button buttonOne = area.GenerateButton();
        buttonOne.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Rest (Heal " + healAmount.ToString() + ")";
        buttonOne.onClick.AddListener(() =>
        {
            SingletonHolder.Instance.Dungeon.HealDamage(healAmount);
            SingletonHolder.Instance.Dungeon.PromptDungeonCard();
        });

        Button buttonTwo = area.GenerateButton();
        buttonTwo.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Purge A Card";
        buttonTwo.onClick.AddListener(() =>
        {
            SingletonHolder.Instance.Dungeon.DisplayPlayerDeck();
            SingletonHolder.Instance.ShowDeckArea.CardSelectedCallback.AddListener(CardSelected);
            buttonTwo.interactable = false;
        });

        Button buttonThree = area.GenerateButton();
        buttonThree.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Other Option ?";
        buttonThree.onClick.AddListener(() =>
        {
        });

        base.DisplayLeisure(area);
    }

    public void CardSelected(DDCardBase selectedCard)
    {
        SingletonHolder.Instance.Dungeon.RemoveCardFromDeck(selectedCard);
        SingletonHolder.Instance.ShowDeckArea.CardSelectedCallback.RemoveListener(CardSelected);
        SingletonHolder.Instance.Dungeon.DisplayDeckClosed();
    }
}
