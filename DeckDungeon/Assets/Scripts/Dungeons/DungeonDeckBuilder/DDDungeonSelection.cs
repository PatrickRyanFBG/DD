using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DDDungeonSelection : MonoBehaviour
{
    [SerializeField]
    private RawImage heroShotImage;
    public RawImage HeroShotImage { get => heroShotImage; }

    [SerializeField]
    private TextMeshProUGUI dungeonName;
    public TextMeshProUGUI DungeonName { get => dungeonName; }

    [SerializeField]
    private TextMeshProUGUI dungeonDescription;
    public TextMeshProUGUI DungeonDescription { get => dungeonDescription; }

    [SerializeField]
    private Button selectionButton;
    public Button SelectionButton { get => selectionButton; }

    [Header("Testing")]
    [SerializeField]
    private GameObject comingSoonObject;
}
