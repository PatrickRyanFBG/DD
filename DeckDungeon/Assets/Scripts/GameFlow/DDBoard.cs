using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDBoard : MonoBehaviour
{
    [SerializeField] private DDColumn[] columns;

    [SerializeField] private DDRow[] rows;

    [SerializeField, UnityEngine.Serialization.FormerlySerializedAs("meleeRangeBonus")]
    private Vector2Int[] meleeRangedBonus;

    private int columnsCount;
    public int ColumnsCount => columnsCount;
    public int ColumnCountIndex => columnsCount - 1;

    private int rowCount;
    public int RowCount => rowCount;
    public int RowCountIndex => rowCount - 1;

    [Header("Testing")] [SerializeField] private DDEnemyOnBoard dummyPrefab;

    // Gotta put this somewhere else?
    [SerializeField] private int bonkDamage = 5;

    private void Awake()
    {
        for (int i = 0; i < columns.Length; i++)
        {
            columns[i].SetIndex(i);
        }

        for (int i = 0; i < rows.Length; i++)
        {
            rows[i].SetIndex(i, meleeRangedBonus[i]);
        }
        
        columnsCount = columns.Length;
        rowCount = rows.Length;
    }

    public void SpawnEnemy(Vector2Int loc, DDEnemyBase enemy)
    {
        SpawnEnemy(loc.x, loc.y, enemy);
    }
    
    public void SpawnEnemy(int x, int y, DDEnemyBase enemy)
    {
        if (!columns[x].Locations[y].GetEnemy())
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
        if (direction == EMoveDirection.Up || direction == EMoveDirection.Down)
        {
            int toAdd = direction == EMoveDirection.Up ? 1 : -1;
            int destination = Mathf.Clamp(currentCoord.y + (toAdd * amount), 0, (rowCount - 1));
            while (currentCoord.y != destination)
            {
                int checkCoord = currentCoord.y + toAdd;
                DDEnemyOnBoard occupiedEnemy = columns[currentCoord.x].Locations[checkCoord].GetEnemy();
                if (occupiedEnemy)
                {
                    // The next position has an enemy so we can't move any more
                    // We bonk though?!
                    if (fromPlayer)
                    {
                        yield return enemy.MoveToLocationAndBack(occupiedEnemy.CurrentLocaton.transform.position,
                            () =>
                            {
                                DDGlobalManager.Instance.ClipLibrary.Bonk.PlayNow();
                                enemy.TakeDamage(GetTotalBonkDamage(), ERangeType.Pure, false);
                                occupiedEnemy.TakeDamage(GetTotalBonkDamage(), ERangeType.Pure, false);
                            });
                    }

                    break;
                }

                currentCoord.y = checkCoord;
            }

            // We have moved any amount
            if (enemy.CurrentLocaton.Coord.y != currentCoord.y)
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
                if (occupiedEnemy)
                {
                    // The next position has an enemy so we can't move any more
                    // We bonk though?!
                    if (fromPlayer)
                    {
                        yield return enemy.MoveToLocationAndBack(occupiedEnemy.CurrentLocaton.transform.position,
                            () =>
                            {
                                DDGlobalManager.Instance.ClipLibrary.Bonk.PlayNow();
                                enemy.TakeDamage(GetTotalBonkDamage(), ERangeType.Pure, false);
                                occupiedEnemy.TakeDamage(GetTotalBonkDamage(), ERangeType.Pure, false);
                            });
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

    public List<DDLocation> GetSurroundingLocations(Vector2Int location)
    {
        List<DDLocation> locations = new();

        for (int x = location.x - 1; x <= location.x + 1; x++)
        {
            for (int y = location.y - 1; y <= location.y + 1; y++)
            {
                if (x == location.x && y == location.y)
                {
                    continue;
                }

                if (x < 0 || x >= columnsCount || y < 0 || y >= rowCount)
                {
                    continue;
                }
                
                locations.Add(columns[x].Locations[y]);
            }
        }
        
        return locations;
    }
    
    public DDLocation GetLocation(Vector2Int location)
    {
        return GetLocation(location.x, location.y);
    }

    public DDLocation GetLocation(int x, int y)
    {
        return columns[x].Locations[y];
    }

    public DDRow GetRow(int index)
    {
        return rows[index];
    }

    public DDColumn GetColumn(int index)
    {
        return columns[index];
    }

    public int GetMeleeRangedBonus(ERangeType type, int row)
    {
        if (type is ERangeType.Pure or ERangeType.None)
        {
            return 0;
        }

        return type == ERangeType.Melee ? meleeRangedBonus[row].x : meleeRangedBonus[row].y;
    }

    public int GetTotalBonkDamage()
    {
        int playerVigor = DDGamePlaySingletonHolder.Instance.Player.GetAffixValue(EAffixType.Vigor);
        int handVigor = DDGamePlaySingletonHolder.Instance.Player.GetFinishCountByType(EPlayerCardFinish.Weighty);
        int handFragile = DDGamePlaySingletonHolder.Instance.Player.GetFinishCountByType(EPlayerCardFinish.Fragile);
        return bonkDamage + playerVigor + handVigor - handFragile;
    }

    public int GetRandomRowIndex()
    {
        return Random.Range(0, rows.Length);
    }

    public int GetRandomColumnIndex()
    {
        return Random.Range(0, columns.Length);
    }

    public Vector2Int GetRandomLocation()
    {
        return new Vector2Int(GetRandomRowIndex(), GetRandomColumnIndex());
    }
}