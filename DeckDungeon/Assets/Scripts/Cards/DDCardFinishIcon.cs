using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DDCardFinishIcon : MonoBehaviour
{
    [SerializeField] private RawImage finishImage;
    
    private DDPlayerCardFinish cardFinish;

    public void SetUp(DDPlayerCardFinish finish)
    {
        cardFinish = finish;
        finishImage.texture = cardFinish.Icon;
    }

    public void Hovered()
    {
        DDGamePlaySingletonHolder.Instance.Dungeon.SetToolTip(cardFinish.GetDescription());
    }

    public void Unhovered()
    {
        DDGamePlaySingletonHolder.Instance.Dungeon.SetToolTip("");
    }
}
