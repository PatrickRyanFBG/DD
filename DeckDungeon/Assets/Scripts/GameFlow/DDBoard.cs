using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDBoard : MonoBehaviour
{
    [SerializeField]
    private DDColumn[] columns;

    private int columnsCount;
    public int ColumnsCount { get { return columnsCount; } }
    public int ColumnCountIndex { get { return columnsCount - 1; } }

    private int rowCount;
    public int RowCount { get { return rowCount; } }
    public int RowCountIndex { get { return rowCount - 1; } }

    [Header("Testing")]
    [SerializeField]
    private DDEnemyOnBoard dummyPrefab;

    // Gotta put this somewhere else?
    [SerializeField]
    private int bonkDamage = 5;

    private void Awake()
    {
        for (int i = 0; i < columns.Length; i++)
        {
            columns[i].SetIndex(i);
        }

        columnsCount = columns.Length;
        rowCount = columns[0].Locations.Length;
    }

    public void SpawnEnemy(float x, float y, DDEnemyBase enemy)
    {
        SpawnEnemy((int)x, (int)y, enemy);
    }

    public void SpawnEnemy(int x, int y, DDEnemyBase enemy)
    {
        if(columns[x].Locations[y].GetEnemy() == null)
        {
            DDEnemyOnBoard currentEnemy = Instantiate(dummyPrefab, transform);
            DDGamePlaySingletonHolder.Instance.Encounter.RegisterEnemy(currentEnemy);
            currentEnemy.SetUpEnemy(enemy);
            columns[x].Locations[y].SnapEnemyToHere(currentEnemy);
        }
    }

    public IEnumerator MoveEnemy(DDEnemyOnBoard enemy, EMoveDirection direction, int amount, bool fromPlayer)
    {
        Vector2 currentCoord = enemy.CurrentLocaton.Coord;
        if(direction == EMoveDirection.Up || direction == EMoveDirection.Down)
        {
            int toAdd = direction == EMoveDirection.Up ? 1 : -1;
            int destination = Mathf.Clamp((int)currentCoord.y + (toAdd * amount), 0, (rowCount - 1));
            while (currentCoord.y != destination)
            {
                int checkCoord = (int)currentCoord.y + toAdd;
                DDEnemyOnBoard occupiedEnemy = columns[(int)currentCoord.x].Locations[checkCoord].GetEnemy();
                if (occupiedEnemy != null)
                {
                    // The next position has an enemy so we can't move any more
                    // We bonk though?!
                    if(fromPlayer)
                    {
                        yield return enemy.MoveToLocationAndBack(occupiedEnemy.CurrentLocaton.transform.position);
                        enemy.DoDamage(bonkDamage);
                        occupiedEnemy.DoDamage(bonkDamage);
                    }
                    break;
                }

                currentCoord.y = checkCoord;
            }

            // We have moved any amount
            if(enemy.CurrentLocaton.Coord.y != currentCoord.y)
            {
                yield return enemy.CurrentLocaton.SetEnemy(null);
                yield return columns[(int)currentCoord.x].Locations[(int)currentCoord.y].SetEnemy(enemy);
            }
        }
        else if (direction == EMoveDirection.Left || direction == EMoveDirection.Right)
        {
            int toAdd = direction == EMoveDirection.Right ? 1 : -1;
            int destination = Mathf.Clamp((int)currentCoord.x + (toAdd * amount), 0, (columnsCount - 1));
            while (currentCoord.x != destination)
            {
                int checkCoord = (int)currentCoord.x + toAdd;
                DDEnemyOnBoard occupiedEnemy = columns[checkCoord].Locations[(int)currentCoord.y].GetEnemy();
                if (occupiedEnemy != null)
                {
                    // The next position has an enemy so we can't move any more
                    // We bonk though?!
                    if (fromPlayer)
                    {
                        yield return enemy.MoveToLocationAndBack(occupiedEnemy.CurrentLocaton.transform.position);
                        enemy.DoDamage(bonkDamage);
                        occupiedEnemy.DoDamage(bonkDamage);
                    }
                    break;
                }

                currentCoord.x = checkCoord;
            }

            // We have moved any amount
            if (enemy.CurrentLocaton.Coord.x != currentCoord.x)
            {
                yield return enemy.CurrentLocaton.SetEnemy(null);
                yield return columns[(int)currentCoord.x].Locations[(int)currentCoord.y].SetEnemy(enemy);
            }
        }
    }

    public void DoAllEffects()
    {
        for (int i = 0; i < columns.Length; i++)
        {
            columns[i].DoAllEffects();
        }
    }

    public void ClearAllEffects()
    {
        for (int i = 0; i < columns.Length; i++)
        {
            columns[i].ClearAllEffects();
        }
    }

    public DDEnemyOnBoard GetEnemyAtLocation(Vector2 location)
    {
        return GetEnemyAtLocation((int)location.x, (int)location.y);
    }

    public DDEnemyOnBoard GetEnemyAtLocation(int x, int y)
    {
        return columns[x].Locations[y].GetEnemy();
    }

    public void GetAllEnemies(ref List<DDEnemyOnBoard> enemyList)
    {
        for (int i = 0; i < columns.Length; i++)
        {
            columns[i].FillEnemyList(ref enemyList);
        }
    }

    public DDLocation GetLocation(Vector2 location)
    {
        return GetLocation((int)location.x, (int)location.y);
    }

    public DDLocation GetLocation(int x, int y)
    {
        return columns[x].Locations[y];
    }
}


[System.Serializable]
public class TestingEnemySpawn
{
    public int X;
    public int Y;
    // something something enemy here?!?!
}
