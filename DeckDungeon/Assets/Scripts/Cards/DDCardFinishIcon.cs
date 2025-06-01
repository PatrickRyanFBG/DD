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
        DDGlobalManager.Instance.ToolTip.SetText(cardFinish.PlayerCardFinish + "\r\n" + cardFinish.GetDescription());
    }

    public void Unhovered()
    {
        DDGlobalManager.Instance.ToolTip.SetText("");
    }
}
