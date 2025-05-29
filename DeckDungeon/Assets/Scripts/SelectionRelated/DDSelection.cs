using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DDSelection : MonoBehaviour
{
    [SerializeField] private ETargetType targetType;
    public ETargetType TargetType => targetType;

    public virtual void FillSelectionList(ref List<DDSelection> selections)
    {
        selections.Add(this);
    }

    public virtual void FillEnemyList(ref List<DDEnemyOnBoard> enemies)
    {
    }

    public virtual void Hovered(bool fromAnotherSelection = false)
    {
        if (!fromAnotherSelection)
        {
            DDGlobalManager.Instance.ClipLibrary.HoverTarget.PlayNow();
        }
    }

    public virtual void Unhovered(bool fromAnotherSelection = false)
    {
    }
}
