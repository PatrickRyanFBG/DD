using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDEventDataLibrary : MonoBehaviour
{
    [SerializeField] private List<DDDungeonEventData> genericEventDatas = new List<DDDungeonEventData>();

    private HashSet<DDDungeonEventData> usedEvents;

    private void Awake()
    {
        usedEvents = new HashSet<DDDungeonEventData>();
    }

    public DDDungeonEventData GetUnusedEvent()
    {
        DDDungeonEventData data = null;

        do
        {
            data = genericEventDatas.GetRandomElement();
        } while (data.EventIsValid() || usedEvents.Contains(data));
        
        usedEvents.Add(data);
        
        return data;
    }
}
