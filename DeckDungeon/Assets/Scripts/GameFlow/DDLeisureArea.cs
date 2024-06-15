using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DDLeisureArea : MonoBehaviour
{
    [SerializeField]
    private TMPro.TextMeshProUGUI leisureName;
    public TMPro.TextMeshProUGUI LeisureName { get { return leisureName; } }

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
    private Button optionPrefab;

    private DDDungeonCardLeisure currentLeisure;

    public void DisplayLeisure(DDDungeonCardLeisure leisureCard)
    {
        currentLeisure = leisureCard;

        CleanUpButtons();
        gameObject.SetActive(true);
        currentLeisure.DisplayLeisure(this);
    }

    public void CleanUpButtons()
    {
        for (int i = 0; i < options.Count; i++)
        {
            Destroy(options[i]);
        }

        options.Clear();
    }

    public Button GenerateButton()
    {
        Button button = Instantiate(optionPrefab, optionParent);
        options.Add(button.gameObject);
        return button;
    }
}
