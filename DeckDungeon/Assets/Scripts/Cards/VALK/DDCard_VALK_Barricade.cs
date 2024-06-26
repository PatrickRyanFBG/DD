using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDCard_VALK_Barricade : DDCard_VALKBase
{
    [SerializeField]
    private DDEnemyBase barriade;

    public override IEnumerator ExecuteCard(List<DDSelection> selections)
    {
        yield return base.ExecuteCard(selections);

        DDLocation loc = selections[0] as DDLocation;
        if (loc != null)
        {
            SingletonHolder.Instance.Board.SpawnEnemy(loc.Coord.x, loc.Coord.y, barriade);
        }

        yield return null;
    }

    public override bool IsSelectionValid(DDSelection selection, int targetIndex)
    {
        DDLocation loc = selection as DDLocation;
        if(loc != null)
        {
            if(loc.GetEnemy() == null)
            {
                return true;
            }
        }

        return false;
    }
}
