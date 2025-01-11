using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DDAdventurerSelection : MonoBehaviour
{
    [SerializeField]
    private RawImage heroShotImage;
    public RawImage HeroShotImage { get => heroShotImage; }

    [SerializeField]
    private TextMeshProUGUI adventurerName;
    public TextMeshProUGUI AdventurerName { get => adventurerName; }

    [SerializeField]
    private TextMeshProUGUI adventurerDescription;
    public TextMeshProUGUI AdventurerDescription { get => adventurerDescription; }

    [SerializeField]
    private Button selectionButton;
    public Button SelectionButton { get => selectionButton; }

    [Header("Testing")]
    [SerializeField]
    private GameObject comingSoonObject;
}
