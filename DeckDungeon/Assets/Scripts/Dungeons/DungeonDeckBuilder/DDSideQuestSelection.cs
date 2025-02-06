using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DDSideQuestSelection : MonoBehaviour
{
    [SerializeField]
    private RawImage heroShotImage;
    public RawImage HeroShotImage => heroShotImage;

    [SerializeField]
    private TextMeshProUGUI sideQuestName;
    public TextMeshProUGUI SideQuestName => sideQuestName;

    [SerializeField]
    private TextMeshProUGUI sideQuestDescription;
    public TextMeshProUGUI SideQuestDescription => sideQuestDescription;

    [SerializeField]
    private Toggle selectionButton;
    public Toggle SelectionButton => selectionButton;
}
