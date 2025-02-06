using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDRow : DDSelection
{
    [SerializeField]
    private DDLocation[] locations;
    public DDLocation[] Locations => locations;

#if UNITY_EDITOR
    [ContextMenu("Fix Coord")]
    private void FixCoord()
    {
        locations = new DDLocation[transform.childCount];
        for (int i = 0; i < locations.Length; i++)
        {
            locations[i] = transform.GetChild(i).GetComponent<DDLocation>();
        }

        string s = gameObject.name.Substring(3);
        int val;
        if(int.TryParse(s, out val))
        {
            for (int i = 0; i < locations.Length; i++)
            {
                UnityEditor.SerializedObject serObj = new UnityEditor.SerializedObject(locations[i]);
                UnityEditor.SerializedProperty serProp = serObj.FindProperty("coord");
                Vector2Int cur = serProp.vector2IntValue;
                cur.y = val;
                serProp.vector2IntValue = cur;
                serObj.ApplyModifiedProperties();

                //locations[i].FixCoord(val);
                //UnityEditor.EditorUtility.SetDirty(locations[i]);
                //UnityEditor.EditorUtility.SetDirty(locations[i].gameObject);
            }
        }
    }
#endif

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
}
