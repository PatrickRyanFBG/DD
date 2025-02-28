using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDDungeonEventDataPlagueDoctor : DDDungeonEventData
{
    [Header("Pleague Doctor")] [SerializeField] private string[] healTexts;

    [SerializeField] private float[] healValues;

    [SerializeField, Multiline] private string afterHealDescription;

    public override void DisplayEvent(DDEventArea area)
    {
        base.DisplayEvent(area);

        for (int i = 0; i < healTexts.Length; i++)
        {
            DDButton button = area.GenerateButton(healTexts[i]);
            float healPercent = healValues[i];
            button.Button.onClick.AddListener(() =>
            {
                // Maybe something here add a wound card to deck?
                DDGamePlaySingletonHolder.Instance.Dungeon.HealDamage(
                    (int)(DDGamePlaySingletonHolder.Instance.Dungeon.MaxHealth * healPercent));
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