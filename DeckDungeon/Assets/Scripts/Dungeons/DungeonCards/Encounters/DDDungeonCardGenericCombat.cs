using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDDungeonCardGenericCombat : DDDungeonCardEncounter
{
    [Header("Generic Combat")]
    [SerializeField]
    private List<DDDungeonCombatData> introCombats;

    [SerializeField]
    private List<DDDungeonCombatData> fullCombats;

    public override bool CanSelect()
    {
        if(testingHasChest)
        {
            if(!DDGamePlaySingletonHolder.Instance.Dungeon.HasKey)
            {
                return false;
            }
        }

        return true;
    }

    public override void SpawnEnemies()
    {
        DDBoard board = DDGamePlaySingletonHolder.Instance.Board;

        DDDungeonCombatData combatData = null;

        if(DDGamePlaySingletonHolder.Instance.Dungeon.DungeonStats.EncountersCompleted < 3)
        {
            combatData = introCombats[Random.Range(0, introCombats.Count)];
        }
        else
        {
            combatData = fullCombats[Random.Range(0, fullCombats.Count)];
        }

        List<Vector2Int> locs = new List<Vector2Int>();
        for (int i = 0; i < combatData.Enemies.Count; i++)
        {
            int count = combatData.Enemies[i].Amount;
            for (int j = 0; j < count; j++)
            {
                int x = Random.Range(0, board.ColumnsCount);
                int y = Random.Range(0, board.RowCount);

                Vector2Int nextPos = new Vector2Int(x, y);

                while (locs.Contains(nextPos))
                {
                    x = Random.Range(0, board.ColumnsCount);
                    y = Random.Range(0, board.RowCount);

                    nextPos = new Vector2Int(x, y);
                }

                locs.Add(nextPos);
                board.SpawnEnemy(x, y, combatData.Enemies[i].Enemy);
            }
        }
    }
}
