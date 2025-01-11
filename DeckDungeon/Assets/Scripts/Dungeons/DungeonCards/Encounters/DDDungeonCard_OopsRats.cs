using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDDungeonCard_OopsRats : DDDungeonCardEncounter
{
    [SerializeField]
    private int numberOfRats = 3;

    [SerializeField]
    private DDEnemyBase rat;

    public override void SpawnEnemies()
    {
        DDBoard board = DDGamePlaySingletonHolder.Instance.Board;

        List<Vector2> locs = new List<Vector2>();
        for (int i = 0; i < numberOfRats; i++)
        {
            int x = Random.Range(0, board.ColumnsCount);
            int y = Random.Range(0, board.RowCount);

            Vector2 nextPos = new Vector2(x, y);

            while (locs.Contains(nextPos))
            {
                x = Random.Range(0, board.ColumnsCount);
                y = Random.Range(0, board.RowCount);

                nextPos = new Vector2(x, y);
            }

            locs.Add(nextPos);
            board.SpawnEnemy(x, y, rat);
        }
    }
}
