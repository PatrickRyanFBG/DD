using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDDungeonCardEventGenerics : DDDungeonCardEvent
{
    [System.NonSerialized]
    private DDDungeonEventData currentEvent;
    
    public override void DisplayEvent(DDEventArea area)
    {
        currentEvent = DDGlobalManager.Instance.EventDataLibrary.GetUnusedEvent();
        
        currentEvent.DisplayEvent(area);
    }
}
