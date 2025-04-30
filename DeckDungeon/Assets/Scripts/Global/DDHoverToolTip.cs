using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DDHoverToolTip : MonoBehaviour
{
    [SerializeField] private RectTransform rect;
    [SerializeField] private GameObject tooltipParent;
    [SerializeField] private TMPro.TextMeshProUGUI tooltip;

    private void Update()
    {
        Vector2 mousePos = Input.mousePosition;
        bool left = mousePos.x < Screen.width / 2f;
        bool bottom = mousePos.y < Screen.height / 2f;
        
        rect.pivot = new Vector2(left ? -.125f : 1.125f, bottom ? 0 : 1);
        rect.anchoredPosition = new Vector2(mousePos.x - Screen.width / 2f, mousePos.y - Screen.height / 2f);
    }

    public void SetText(string text)
    {
        tooltip.text = text;
        tooltipParent.SetActive(!string.IsNullOrEmpty(text));
    }
}
