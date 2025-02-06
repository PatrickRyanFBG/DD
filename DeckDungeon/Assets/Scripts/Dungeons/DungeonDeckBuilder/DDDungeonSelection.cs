using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class DDDungeonSelection : MonoBehaviour
{
    [SerializeField]
    private RawImage heroShotImage;
    public RawImage HeroShotImage => heroShotImage;

    [SerializeField]
    private TextMeshProUGUI dungeonName;
    public TextMeshProUGUI DungeonName => dungeonName;

    [SerializeField]
    private TextMeshProUGUI dungeonDescription;
    public TextMeshProUGUI DungeonDescription => dungeonDescription;

    [SerializeField]
    private EventTrigger events;
    public EventTrigger Events => events;

    public void PointerEnter()
    {
        dungeonName.gameObject.SetActive(false);
        dungeonDescription.gameObject.SetActive(true);
    }

    public void PointerExit()
    {
        dungeonName.gameObject.SetActive(true);
        dungeonDescription.gameObject.SetActive(false);
    }
}
