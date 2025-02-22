using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDEntireOrEmpty : DDSelection
{
    [SerializeField]
    private DDRow[] rows;

    public override void FillSelectionList(ref List<DDSelection> selections)
    {
        for (int i = 0; i < rows.Length; i++)
        {
            rows[i].FillSelectionList(ref selections);
        }
    }

    public override void FillEnemyList(ref List<DDEnemyOnBoard> enemies)
    {
        for (int i = 0; i < rows.Length; i++)
        {
            rows[i].FillEnemyList(ref enemies);
        }
    }

    public override bool Hovered()
    {
        for (int i = 0; i < rows.Length; i++)
        {
            rows[i].Hovered();
        }

        return true;
    }

    public override void Unhovered()
    {
        for (int i = 0; i < rows.Length; i++)
        {
            rows[i].Unhovered();
        }
    }
}
