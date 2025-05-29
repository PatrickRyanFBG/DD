using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDDungeonEventDataAddFinishes : DDDungeonEventData
{
    [Header("Finishes")] [SerializeField] private string finishesText;

    [SerializeField] private int amountToFinish = 5;
    
    [SerializeField, Multiline] private string afterFinishesDescription;

    public override void DisplayEvent(DDEventArea area)
    {
        base.DisplayEvent(area);

        DDButton buttonOne = area.GenerateButton(finishesText);
        buttonOne.Button.onClick.AddListener(() =>
        {
            List<int> indexes = new List<int>(amountToFinish);
            for (int i = 0; i < amountToFinish; i++)
            {
                int index = Random.Range(0, DDGamePlaySingletonHolder.Instance.Dungeon.PlayerDeck.Count);
                while (indexes.Contains(index))
                {
                    index = Random.Range(0, DDGamePlaySingletonHolder.Instance.Dungeon.PlayerDeck.Count);
                }
                indexes.Add(index);
            }

            for (int i = 0; i < indexes.Count; i++)
            {
                DDGamePlaySingletonHolder.Instance.Dungeon.PlayerDeck[i].AddRandomFinishByImpact(EPlayerCardFinishImpact.Positive);
            }
            
            area.Description.text = afterFinishesDescription;
            area.CleanUpButtons();

            DDButton okayButton = area.GenerateButton("Leave");
            okayButton.Button.onClick.AddListener(() =>
            {
                DDGamePlaySingletonHolder.Instance.Dungeon.EndEvent();
            });
        });

        DDButton buttonTwo = area.GenerateButton("Leave");
        buttonTwo.Button.onClick.AddListener(() =>
        {
            DDGamePlaySingletonHolder.Instance.Dungeon.EndEvent();
        });
    }

    public override bool EventIsValid()
    {
        return DDGamePlaySingletonHolder.Instance.Dungeon.PlayerDeck.Count >= amountToFinish;
    }
}
