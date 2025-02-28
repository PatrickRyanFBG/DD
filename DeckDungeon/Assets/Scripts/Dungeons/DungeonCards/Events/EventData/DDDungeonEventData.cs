using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class DDDungeonEventData : DDScriptableObject
{
    [Header("Event")] [SerializeField] protected string eventName;

    [SerializeField, Multiline] protected string eventDescription;

    [SerializeField] protected Texture eventImage;

    public virtual void DisplayEvent(DDEventArea area)
    {
        area.EventName.text = eventName;
        area.Description.text = eventDescription;
        area.Image.texture = eventImage;
    }

    public virtual bool EventIsValid()
    {
        return true;
    }
}
