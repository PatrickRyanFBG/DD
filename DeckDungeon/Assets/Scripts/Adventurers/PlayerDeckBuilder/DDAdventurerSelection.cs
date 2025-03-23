using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DDAdventurerSelection : MonoBehaviour
{
    [SerializeField]
    private RawImage heroShotImage;
    public RawImage HeroShotImage => heroShotImage;

    [SerializeField]
    private TextMeshProUGUI adventurerName;
    public TextMeshProUGUI AdventurerName => adventurerName;

    [SerializeField]
    private TextMeshProUGUI adventurerDescription;
    public TextMeshProUGUI AdventurerDescription => adventurerDescription;

    [SerializeField]
    private Button selectionButton;
    public Button SelectionButton => selectionButton;

    [Header("Testing")]
    [SerializeField]
    private GameObject comingSoonObject;
}
