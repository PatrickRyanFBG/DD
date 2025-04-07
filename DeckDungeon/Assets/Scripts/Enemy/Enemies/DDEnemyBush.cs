using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDEnemyBush : DDEnemyBase
{
    [Header("Bush")]
    [SerializeField] private int retaliateGain = 1;
    
    public override List<DDEnemyActionBase> CalculateActions(int number, DDEnemyOnBoard actingEnemy)
    {
        List<DDEnemyActionBase> actions = new List<DDEnemyActionBase>();

        List<DDLocation> surroundingLocations =
            DDGamePlaySingletonHolder.Instance.Board.GetSurroundingLocations(actingEnemy.CurrentLocaton.Coord);
        
        List<Vector2Int> emptyLocations = new List<Vector2Int>();

        foreach (DDLocation location in surroundingLocations)
        {
            if (!location.HasEnemy())
            {
                emptyLocations.Add(location.Coord);
            }
        }
        
        // If all locations are filled, or 50% chance, we gain 1 retaliate
        if (RandomHelpers.GetRandomBool(25))
        {
            actions.Add(new DDEnemyActionModifyAffix(EAffixType.Retaliate, retaliateGain, false));
        }
        else if(emptyLocations.Count > 0 && RandomHelpers.GetRandomBool(25))
        {
            // if there is empty locations 50% chance we propigate
            actions.Add(new DDEnemyActionSpawnEnemy(this, emptyLocations.GetRandomElement(), Image));
        }

        return actions;
    }

    public override IEnumerator OnDeath()
    {
        // Spawn
        yield return null;
    }
}
