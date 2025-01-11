using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class DDQuestEventOption
{
    [SerializeField]
    private string optionButtonText;
    public string OptionButtonText { get => optionButtonText; }

    [SerializeField, Multiline]
    private string optionButtonDescription;
    public string OptionButtonDescription { get => optionButtonDescription; }

    [SerializeField]
    private string optionButtonTextExit;
    public string OptionButtonTextExit { get => optionButtonTextExit; }

    // 
}

public class DDDungeonCardEvent_Quest : DDDungeonCardEvent
{
    [Header("Quest")]
    // This has got to be an enum I think just to avoid user error for typing in the same Id a bunch.
    [SerializeField]
    private string questId;

    [SerializeField]
    private List<DDQuestEventOption> options;

    // For a general quest I need a variable amount of options and to some how be able to add individual out comes

    public override void DisplayEvent(DDEventArea area)
    {
        base.DisplayEvent(area);

        for (int i = 0; i < options.Count; i++)
        {
            DDButton button = area.GenerateButton();
            button.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = options[i].OptionButtonText;
            button.Button.onClick.AddListener(() =>
            {
                area.Description.text = options[i].OptionButtonDescription;
                area.CleanUpButtons();

                DDButton okayButton = area.GenerateButton();
                okayButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = options[i].OptionButtonTextExit;
                okayButton.Button.onClick.AddListener(() =>
                {
                    // Here is where I gotta figure out what to do after
                    DDGamePlaySingletonHolder.Instance.Dungeon.PromptDungeonCard();
                });
            });
        }
        /*
        DDButton buttonOne = area.GenerateButton();
        buttonOne.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Follow the mysterious figure.";
        buttonOne.Button.onClick.AddListener(() =>
        {
            area.Description.text = secondDescription;
            area.CleanUpButtons();

            DDButton okayButton = area.GenerateButton();
            okayButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Wow that is cool!";
            okayButton.Button.onClick.AddListener(() =>
            {
                DDGamePlaySingletonHolder.Instance.Dungeon.PromptDungeonCard();
            });
        });

        DDButton buttonTwo = area.GenerateButton();
        buttonTwo.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Loot their camp. (100 gold)";
        buttonTwo.Button.onClick.AddListener(() =>
        {
            area.Description.text = secondDescription;//"Well gold isn't added into this demo, so that stinks, but secret is the other button didn't do anything either";
            area.CleanUpButtons();

            DDButton okayButton = area.GenerateButton();
            okayButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Wow that is cool!";
            okayButton.Button.onClick.AddListener(() =>
            {
                DDGamePlaySingletonHolder.Instance.Dungeon.PromptDungeonCard();
            });
        });
        */
    }
}
