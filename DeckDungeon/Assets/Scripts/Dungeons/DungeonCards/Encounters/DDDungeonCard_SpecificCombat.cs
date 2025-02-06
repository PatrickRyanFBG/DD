using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDDungeonCard_SpecificCombat : DDDungeonCardEncounter
{
    [Header("Generic Combat")]
    [SerializeField]
    private List<DDCombatEnemySetup> enemies;

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

        List<Vector2Int> locs = new List<Vector2Int>();
        for (int i = 0; i < enemies.Count; i++)
        {
            int count = enemies[i].Amount;
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
                board.SpawnEnemy(x, y, enemies[i].Enemy);
            }
        }
    }
}
