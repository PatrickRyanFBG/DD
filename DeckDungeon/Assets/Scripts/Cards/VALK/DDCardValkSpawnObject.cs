using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class DDCardValkSpawnObject : DDCardValkGeneric
{
    [SerializeField] private DDEnemyBase objectToSpawn;

    public override List<ETargetType> GetTargets()
    {
        if (targets != null) return targets;

        targets = new List<ETargetType>() { ETargetType.Location };
        
        return base.GetTargets();
    }

    protected override IEnumerator Execute(List<DDSelection> selections)
    {
        yield return base.Execute(selections);

        DDLocation loc = selections[0] as DDLocation;
        if (loc)
        {
            DDGamePlaySingletonHolder.Instance.Board.SpawnEnemy(loc.Coord.x, loc.Coord.y, objectToSpawn);
        }

        yield return null;
    }

    public override bool IsSelectionValid(DDSelection selection, int targetIndex)
    {
        DDLocation loc = selection as DDLocation;
        if (loc)
        {
            if (!loc.GetEnemy())
            {
                return true;
            }
        }

        return false;
    }
}