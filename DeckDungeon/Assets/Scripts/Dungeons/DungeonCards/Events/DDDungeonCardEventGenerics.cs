using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDDungeonCardEventGenerics : DDDungeonCardEvent
{
    [SerializeField] private List<DDDungeonEventData> eventDatas;
    
    [System.NonSerialized]
    private DDDungeonEventData currentEvent;
    
    public override void DisplayEvent(DDEventArea area)
    {
        currentEvent = eventDatas.GetRandomElement();
        
        currentEvent.DisplayEvent(area);
    }
}
