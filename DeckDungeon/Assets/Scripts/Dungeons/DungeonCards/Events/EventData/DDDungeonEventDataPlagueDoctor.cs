using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDDungeonEventDataPlagueDoctor : DDDungeonEventData
{
    [Header("Pleague Doctor")] [SerializeField] private string[] healTexts;

    [SerializeField] private float[] healValues;

    [SerializeField] private int[] costValues;

    [SerializeField, Multiline] private string afterHealDescription;

    [SerializeField] private DDCardBase wound;
    
    public override void DisplayEvent(DDEventArea area)
    {
        base.DisplayEvent(area);

        for (int i = 0; i < healTexts.Length; i++)
        {
            DDButton button = area.GenerateButton(healTexts[i]);
            if (!DDGamePlaySingletonHolder.Instance.Dungeon.HasEnoughGold(costValues[i]))
            {
                button.Button.interactable = false;
                continue;
            }
            
            float healPercent = healValues[i];
            bool addWound = i == 0;
            button.Button.onClick.AddListener(() =>
            {
                DDGamePlaySingletonHolder.Instance.Dungeon.HealDamage(
                    (int)(DDGamePlaySingletonHolder.Instance.Dungeon.MaxHealth * healPercent));
                if (addWound)
                {
                    DDGamePlaySingletonHolder.Instance.Dungeon.AddCardToDeck(wound, DDGamePlaySingletonHolder.Instance.Dungeon.DungeonCardStartPosition);
                }
                AfterSelection(area);
            });
        }

        DDButton leaveButton = area.GenerateButton("Leave");
        leaveButton.Button.onClick.AddListener(() => { DDGamePlaySingletonHolder.Instance.Dungeon.EndEvent(); });
    }

    private void AfterSelection(DDEventArea area)
    {
        area.CleanUpButtons();
        area.Description.text = afterHealDescription;

        DDButton buttonOne = area.GenerateButton("Leave");
        buttonOne.Button.onClick.AddListener(() => { DDGamePlaySingletonHolder.Instance.Dungeon.EndEvent(); });
    }
}