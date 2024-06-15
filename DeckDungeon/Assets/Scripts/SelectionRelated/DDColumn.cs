using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDColumn : DDSelection
{
    [SerializeField]
    private DDLocation[] locations;
    public DDLocation[] Locations { get { return locations; } }
    
    /*
    [SerializeField]
    private DDRow[] rows;

#if UNITY_EDITOR
    [ContextMenu("Fix Location")]
    private void FixCoord()
    {
        locations = new DDLocation[rows.Length];
        string s = gameObject.name.Substring(6);
        int val;
        if (int.TryParse(s, out val))
        {
            for (int i = 0; i < rows.Length; i++)
            {
                locations[i] = rows[i].Locations[val];
            }
        }
    }
#endif
    */

    public override void FillSelectionList(ref List<DDSelection> selections)
    {
        for (int i = 0; i < locations.Length; i++)
        {
            locations[i].FillSelectionList(ref selections);
        }
    }

    public override void FillEnemyList(ref List<DDEnemyOnBoard> enemies)
    {
        for (int i = 0; i < locations.Length; i++)
        {
            locations[i].FillEnemyList(ref enemies);
        }
    }

    public override void Hovered()
    {
        for (int i = 0; i < locations.Length; i++)
        {
            locations[i].Hovered();
        }
    }

    public override void Unhovered()
    {
        for (int i = 0; i < locations.Length; i++)
        {
            locations[i].Unhovered();
        }
    }

    public void DoAllEffects()
    {
        for (int i = 0; i < locations.Length; i++)
        {
            locations[i].DoEffects();
        }
    }

    public void ClearAllEffects()
    {
        for (int i = 0; i < locations.Length; i++)
        {
            locations[i].ClearEffects();
        }
    }
}
