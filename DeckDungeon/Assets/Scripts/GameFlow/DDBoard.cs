using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDBoard : MonoBehaviour
{
    [SerializeField]
    private DDColumn[] columns;

    [SerializeField, UnityEngine.Serialization.FormerlySerializedAs("meleeRangeBonus")]
    private Vector2Int[] meleeRangedBonus;

    private int columnsCount;
    public int ColumnsCount => columnsCount;
    public int ColumnCountIndex => columnsCount - 1;

    private int rowCount;
    public int RowCount => rowCount;
    public int RowCountIndex => rowCount - 1;

    [Header("Testing")]
    [SerializeField]
    private DDEnemyOnBoard dummyPrefab;

    // Gotta put this somewhere else?
    [SerializeField]
    private int bonkDamage = 5;

    // TODO change this to turn number and not frame
    private List<DDEnemyOnBoard> currentFrameEnemyList = null;
    private int lastFrameEnemyListGathered;

    private void Awake()
    {
        for (int i = 0; i < columns.Length; i++)
        {
            columns[i].SetIndex(i);
        }

        columnsCount = columns.Length;
        rowCount = columns[0].Locations.Length;
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
        Vector2Int currentCoord = enemy.CurrentLocaton.Coord;
        if(direction == EMoveDirection.Up || direction == EMoveDirection.Down)
        {
            int toAdd = direction == EMoveDirection.Up ? 1 : -1;
            int destination = Mathf.Clamp(currentCoord.y + (toAdd * amount), 0, (rowCount - 1));
            while (currentCoord.y != destination)
            {
                int checkCoord = currentCoord.y + toAdd;
                DDEnemyOnBoard occupiedEnemy = columns[currentCoord.x].Locations[checkCoord].GetEnemy();
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
                yield return columns[currentCoord.x].Locations[currentCoord.y].SetEnemy(enemy);
            }
        }
        else if (direction == EMoveDirection.Left || direction == EMoveDirection.Right)
        {
            int toAdd = direction == EMoveDirection.Right ? 1 : -1;
            int destination = Mathf.Clamp(currentCoord.x + (toAdd * amount), 0, (columnsCount - 1));
            while (currentCoord.x != destination)
            {
                int checkCoord = currentCoord.x + toAdd;
                DDEnemyOnBoard occupiedEnemy = columns[checkCoord].Locations[currentCoord.y].GetEnemy();
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
                yield return columns[currentCoord.x].Locations[currentCoord.y].SetEnemy(enemy);
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

    public DDEnemyOnBoard GetEnemyAtLocation(Vector2Int location)
    {
        return GetEnemyAtLocation(location.x, location.y);
    }

    public DDEnemyOnBoard GetEnemyAtLocation(int x, int y)
    {
        return columns[x].Locations[y].GetEnemy();
    }

    public void GetAllEnemies(ref List<DDEnemyOnBoard> enemyList)
    {
        // To avoid the need to gather every enemy each time it is need in the same frame (but will be turn number later)
        if(lastFrameEnemyListGathered == Time.frameCount)
        {
            enemyList = currentFrameEnemyList;
        }
        else
        {
            for (int i = 0; i < columns.Length; i++)
            {
                columns[i].FillEnemyList(ref enemyList);
            }

            currentFrameEnemyList = enemyList;
            lastFrameEnemyListGathered = Time.frameCount;
        }
    }

    public DDLocation GetLocation(Vector2Int location)
    {
        return GetLocation(location.x, location.y);
    }

    public DDLocation GetLocation(int x, int y)
    {
        return columns[x].Locations[y];
    }

    public int GetMeleeRangedBonus(ERangeType type, int row)
    {
        if(type == ERangeType.None)
        {
            return 0;
        }
        return type == ERangeType.Melee ? meleeRangedBonus[row].x : meleeRangedBonus[row].y;
    }
}
