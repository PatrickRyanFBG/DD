using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDDungeonCardGenericCombat : DDDungeonCardEncounter
{
    [Header("Generic Combat")] [SerializeField] private DDDungeonCombatData data;

    public override void SpawnEnemies()
    {
        DDBoard board = DDGamePlaySingletonHolder.Instance.Board;

        DDCombatEnemySetup enemySetup = null;

        if (DDGamePlaySingletonHolder.Instance.Dungeon.DungeonStats.EncountersCompleted < 3)
        {
            enemySetup = data.GetRandomEnemySetup(ECombatTier.Intro);
        }
        else
        {
            enemySetup = data.GetRandomEnemySetup(ECombatTier.One);
        }

        List<Vector2Int> locs = new List<Vector2Int>();
        for (int i = 0; i < enemySetup.Enemies.Count; i++)
        {
            int count = enemySetup.Enemies[i].Amount;
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
                board.SpawnEnemy(x, y, enemySetup.Enemies[i].Enemy);
            }
        }
    }
}