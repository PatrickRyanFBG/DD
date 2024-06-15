using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DDLocationEffectVisuals : MonoBehaviour
{
    [SerializeField]
    private RawImage img;

    [SerializeField]
    private TMPro.TextMeshProUGUI text;

    public void Init(Texture image, int turns)
    {
        img.texture = image;
        text.text = turns.ToString();
    }

    public void UpdateTurns(int turns)
    {
        text.text = turns.ToString();
    }
}
