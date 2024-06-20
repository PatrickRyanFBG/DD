using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DDEventArea : MonoBehaviour
{
    [SerializeField]
    private TMPro.TextMeshProUGUI eventName;
    public TMPro.TextMeshProUGUI EventName { get { return eventName; } }

    [SerializeField]
    private RawImage image;
    public RawImage Image { get { return image; } }

    [SerializeField]
    private TMPro.TextMeshProUGUI description;
    public TMPro.TextMeshProUGUI Description { get { return description; } }

    [SerializeField]
    private Transform optionParent;

    private List<GameObject> options = new List<GameObject>();

    [SerializeField]
    private DDButton optionPrefab;

    private DDDungeonCardEvent currentEvent;

    public void DisplayEvent(DDDungeonCardEvent eventCard)
    {
        currentEvent = eventCard;

        CleanUpButtons();
        gameObject.SetActive(true);
        currentEvent.DisplayEvent(this);
    }

    public void CleanUpButtons()
    {
        for (int i = 0; i < options.Count; i++)
        {
            Destroy(options[i]);
        }

        options.Clear();
    }

    public DDButton GenerateButton()
    {
        DDButton button = Instantiate(optionPrefab, optionParent);
        options.Add(button.gameObject);
        return button;
    }
}
