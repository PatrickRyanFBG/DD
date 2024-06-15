using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDSelection : MonoBehaviour
{
    public virtual void FillSelectionList(ref List<DDSelection> selections)
    {
        selections.Add(this);
    }

    public virtual void FillEnemyList(ref List<DDEnemyOnBoard> enemies)
    {
    }

    public virtual void Hovered()
    {
    }

    public virtual void Unhovered()
    {
    }
}
