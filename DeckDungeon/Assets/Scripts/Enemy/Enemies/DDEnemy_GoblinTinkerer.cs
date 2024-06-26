using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DDEnemy_GoblinTinkerer : DDEnemyBase
{
    [SerializeField]
    private int damage;

    [SerializeField]
    private DDEnemyBase bombEnemy;

    [SerializeField]
    private Texture bombIcon;

    public override List<DDEnemyActionBase> CalculateActions(int number, DDEnemyOnBoard actingEnemy)
    {
        List<DDEnemyActionBase> actions = new List<DDEnemyActionBase>(number);

        DDEnemyActionBase bombAction = null;

        if (Random.Range(0, 10) < 4)
        {
            Vector2 actingCoords = actingEnemy.CurrentLocaton.Coord;

            for (int i = 0; i < 5; i++)
            {
                int randX = Random.Range(-1, 2);
                int randY = Random.Range(-1, 2);

                if (Random.Range(0, 2) == 0)
                {
                    if (randY != 0)
                    {
                        randX = 0;
                    }
                }
                else
                {
                    if (randX != 0)
                    {
                        randY = 0;
                    }
                }

                if (randX == 0 && randX == 0)
                {
                    continue;
                }

                randX = (int)actingCoords.x + randX;
                randY = (int)actingCoords.y + randY;

                if (randX < 0 ||
                    randX >= SingletonHolder.Instance.Board.ColumnsCount ||
                    randY < 0 ||
                    randY >= SingletonHolder.Instance.Board.RowCount)
                {
                    continue;
                }

                if (SingletonHolder.Instance.Board.GetEnemyAtLocation(randX, randY) == null)
                {
                    bombAction = new DDEnemyAction_SpawnEnemy(bombEnemy, new Vector2(randX, randY), bombIcon);
                    break;
                }
            }
        }

        if (bombAction != null)
        {
            actions.Add(bombAction);
        }
        else
        {
            // We want to move down
            if (actingEnemy.CurrentLocaton.Coord.y < SingletonHolder.Instance.Board.RowCountIndex)
            {
                // checking to see if there is an enemy in the space below
                Vector2 checkLoc = actingEnemy.CurrentLocaton.Coord + Vector2.down;
                DDEnemyOnBoard eob = SingletonHolder.Instance.Board.GetEnemyAtLocation(checkLoc);

                bool canMove = false;

                if (eob != null)
                {
                    // If we are after the one we are moving to
                    if (actingEnemy.TurnNumber > eob.TurnNumber && eob.IsPlanningToMove())
                    {
                        canMove = true;
                    }
                    else
                    {
                        /*
                        if (actingEnemy.CurrentLocaton.Coord.x > 0 && actingEnemy.CurrentLocaton.Coord.x < (SingletonHolder.Instance.Board.ColumnsCount - 1))
                        {

                        }
                        */
                        actions.Add(new DDEnemyAction_Move(Random.Range(0, 2) == 0 ? EMoveDirection.Left : EMoveDirection.Right));
                    }
                }
                else
                {
                    canMove = true;
                }

                if (canMove)
                {
                    actions.Add(new DDEnemyAction_Move(EMoveDirection.Up));
                }
            }
            actions.Add(new DDEnemyAction_Attack(damage));
        }

        return actions;
    }
}
