using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DDEnemyActionIcon : MonoBehaviour
{
    [SerializeField] private RawImage icon;
    [SerializeField] private TMPro.TextMeshProUGUI text;

    private DDEnemyActionBase currentAction;

    public void SetUpAction(DDEnemyActionBase action)
    {
        currentAction = action;
        
        currentAction.DisplayInformation(icon, text);

        gameObject.SetActive(true);
    }

    public void Clear()
    {
        currentAction = null;
        icon.texture = null;
        text.text = "";
        
        icon.enabled = false;
        
        gameObject.SetActive(false);
    }

    public void Hovered()
    {
        DDGlobalManager.Instance.ToolTip.SetText(currentAction.GetDescription());
    }

    public void Unhovered()
    {
        DDGlobalManager.Instance.ToolTip.SetText("");
    }
}
