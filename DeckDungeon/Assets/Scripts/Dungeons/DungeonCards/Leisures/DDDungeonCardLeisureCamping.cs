using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class DDDungeonCardLeisureCamping : DDDungeonCardLeisure
{
    [Header("Camping")] [SerializeField] private int healAmount = 25;

    public override void DisplayLeisure(DDLeisureArea area)
    {
        Button buttonOne = area.GenerateButton();
        buttonOne.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Rest and Leave" +
                                                                         "\r\n (Heal " + healAmount.ToString() + ")";
        buttonOne.onClick.AddListener(() =>
        {
            DDGamePlaySingletonHolder.Instance.Dungeon.HealDamage(healAmount);
            DDGamePlaySingletonHolder.Instance.Dungeon.PromptDungeonCard();
        });

        Button buttonTwo = area.GenerateButton();
        buttonTwo.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Purge A Card";
        buttonTwo.onClick.AddListener(() =>
        {
            DDGamePlaySingletonHolder.Instance.Dungeon.DisplayPlayerDeck();
            DDGamePlaySingletonHolder.Instance.ShowDeckArea.CardSelectedCallback.AddListener(PurgeCardSelected);
            buttonTwo.interactable = false;
        });

        Button buttonThree = area.GenerateButton();
        buttonThree.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Add Finishes To A Card";
        buttonThree.onClick.AddListener(() =>
        {
            DDGamePlaySingletonHolder.Instance.Dungeon.DisplayPlayerDeck();
            DDGamePlaySingletonHolder.Instance.ShowDeckArea.CardSelectedCallback.AddListener(UpgradeCardSelected);
            buttonThree.interactable = false;
        });

        base.DisplayLeisure(area);
    }

    public void PurgeCardSelected(DDCardBase selectedCard)
    {
        DDGamePlaySingletonHolder.Instance.Dungeon.RemoveCardFromDeck(selectedCard);
        DDGamePlaySingletonHolder.Instance.ShowDeckArea.CardSelectedCallback.RemoveListener(PurgeCardSelected);
        DDGamePlaySingletonHolder.Instance.ShowDeckArea.CloseArea();
    }

    public void UpgradeCardSelected(DDCardBase selectedCard)
    {
        for (int i = 0; i < 2; i++)
        {
            selectedCard.AddRandomFinish();
        }

        DDGamePlaySingletonHolder.Instance.ShowDeckArea.CardSelectedCallback.RemoveListener(UpgradeCardSelected);
        DDGamePlaySingletonHolder.Instance.ShowDeckArea.CloseArea();
    }
}