using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDDungeonCardEventSpecific : DDDungeonCardEvent
{
    [SerializeField] private DDDungeonEventData eventData;
    
    public override void DisplayEvent(DDEventArea area)
    {
        eventData.DisplayEvent(area);
    }
}