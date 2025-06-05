using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDEventDataLibrary : MonoBehaviour
{
    [SerializeField] private List<DDDungeonEventData> genericEventDatas = new List<DDDungeonEventData>();

    [System.NonSerialized] private HashSet<DDDungeonEventData> usedEvents;

    [Header("Debug")] [SerializeField] private int specificEventIndex = -1;
    
    private void Awake()
    {
        usedEvents = new HashSet<DDDungeonEventData>();
    }

    public DDDungeonEventData GetUnusedEvent()
    {
        if (specificEventIndex >= 0)
        {
            return genericEventDatas[specificEventIndex];    
        }

        // Safety for now
        if (usedEvents.Count == genericEventDatas.Count)
        {
            usedEvents.Clear();
        }
        
        DDDungeonEventData data = null;

        do
        {
            data = genericEventDatas.GetRandomElement();
        } while (!data.EventIsValid() || usedEvents.Contains(data));
        
        usedEvents.Add(data);
        
        return data;
    }
}
