using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DDDungeonCardEvent_Crypt : DDDungeonCardEvent
{
    [Header("Crypt")]
    [SerializeField]
    private string lootGoldText;

    [SerializeField, Multiline]
    private string afterLootGoldDescription;

    [SerializeField]
    private int goldAmount = 200;

    [SerializeField]
    private string returnCardText;

    [SerializeField, Multiline]
    private string afterReturnCardDescription;

    public override void DisplayEvent(DDEventArea area)
    {
        base.DisplayEvent(area);

        DDButton buttonOne = area.GenerateButton();
        buttonOne.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = lootGoldText;
        buttonOne.Button.onClick.AddListener(() =>
        {
            SingletonHolder.Instance.Dungeon.AddOrRemoveGold(goldAmount);

            area.Description.text = afterLootGoldDescription;
            area.CleanUpButtons();

            DDButton okayButton = area.GenerateButton();
            okayButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Oops!";
            okayButton.Button.onClick.AddListener(() =>
            {
                SingletonHolder.Instance.Dungeon.PromptDungeonCard();
            });
        });

        DDButton buttonTwo = area.GenerateButton();
        buttonTwo.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = returnCardText;
        buttonTwo.Button.onClick.AddListener(() =>
        {
            SingletonHolder.Instance.Dungeon.DisplayDungeonDiscard();
            SingletonHolder.Instance.ShowDeckArea.DungeonCardSelectedCallback.AddListener(DungeonCardSelected);

            area.Description.text = afterReturnCardDescription;
            area.CleanUpButtons();

            DDButton okayButton = area.GenerateButton();
            okayButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Thoughts and Prayers.";
            okayButton.Button.onClick.AddListener(() =>
            {
                SingletonHolder.Instance.ShowDeckArea.DungeonCardSelectedCallback.RemoveListener(DungeonCardSelected);
                SingletonHolder.Instance.Dungeon.PromptDungeonCard();
            });
        });
    }

    public void DungeonCardSelected(DDDungeonCardBase selectedCard)
    {
        SingletonHolder.Instance.Dungeon.AddCardToDungeonDeck(selectedCard);
        SingletonHolder.Instance.Dungeon.RemoveCardFromDungeonDiscard(selectedCard);
        SingletonHolder.Instance.ShowDeckArea.DungeonCardSelectedCallback.RemoveListener(DungeonCardSelected);
        SingletonHolder.Instance.Dungeon.DisplayDeckClosed();
    }
}
