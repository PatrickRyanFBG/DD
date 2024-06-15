using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDDungeonCard_GenericCombat : DDDungeonCardEncounter
{
    [Header("Generic Combat")]
    [SerializeField]
    private List<DDCombatEnemy> enemies;

    public override bool CanSelect()
    {
        if(testingHasChest)
        {
            if(!SingletonHolder.Instance.Dungeon.HasKey)
            {
                return false;
            }
        }

        return true;
    }

    public override void SpawnEnemies()
    {
        DDBoard board = SingletonHolder.Instance.Board;

        List<Vector2> locs = new List<Vector2>();
        for (int i = 0; i < enemies.Count; i++)
        {
            int count = enemies[i].Amount;
            for (int j = 0; j < count; j++)
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
                board.SpawnEnemy(x, y, enemies[i].Enemy);
            }
        }
    }
}

[System.Serializable]
public class DDCombatEnemy
{
    [SerializeField]
    private int amount;
    public int Amount { get => amount; }

    [SerializeField]
    private DDEnemyBase enemy;
    public DDEnemyBase Enemy { get => enemy; }
}
