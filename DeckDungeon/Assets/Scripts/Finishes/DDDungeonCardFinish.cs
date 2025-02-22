using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class DDDungeonCardFinish
{
    public abstract EDungeonCardFinish DungeonCardFinish { get; }
    public abstract EDungeonCardExecutionTime DungeonCardExecutionTime { get; }

    [SerializeField, Multiline] private string description;
    
    // This needs to go from icon to shader/material for actual finish
    [SerializeField] private Texture icon;
    public Texture Icon => icon;
    
    public virtual IEnumerator ExecuteFinish(DDCardBase card)
    {
        yield return null;
    }

    public string GetDescription()
    {
        return description;
    }
}
