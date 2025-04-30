using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDPlayerSelection : DDSelection
{
    // Called from UI
    public virtual void UI_Clicked()
    {
        // Put some check here?
        // Also don't like how we are sort of jacking this callback
        DDGamePlaySingletonHolder.Instance.PlayerSelector.SomethingSelected?.Invoke(this);
    }
}
