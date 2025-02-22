using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DDAffixVisuals : MonoBehaviour
{
    [SerializeField] private RectTransform rect;
    public RectTransform Rect => rect;
    
    [SerializeField] private RawImage image;
    [SerializeField] private TextMeshProUGUI text;

    private DDAffix affix;
    
    public void SetInfo(DDAffix aff, int value)
    {
        affix = aff;
        image.texture = affix.Image;
        if (value != 0)
        {
            text.enabled = true;
        }
        text.text = value.ToString();
    }

    public bool UpdateInfo(int value)
    {
        if (!affix.ExistsAtZero && value != 0)
        {
            return true;
        }
        
        if (!affix.ExistsNegative && value < 0)
        {
            return true;
        }
        
        text.text = value.ToString();
        return false;
    }

    public void Hovered()
    {
        DDGamePlaySingletonHolder.Instance.Dungeon.SetToolTip(affix.AffixDescription);
    }

    public void Unhovered()
    {
        DDGamePlaySingletonHolder.Instance.Dungeon.SetToolTip("");
    }
}
