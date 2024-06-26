using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDEnemy_GoblinShaman : DDEnemyBase
{
    [SerializeField]
    private int damage;

    [SerializeField]
    private int healAmount;

    public override List<DDEnemyActionBase> CalculateActions(int number, DDEnemyOnBoard actingEnemy)
    {
        List<DDEnemyActionBase> actions = new List<DDEnemyActionBase>(number);

        List<DDEnemyOnBoard> allEnemies = new List<DDEnemyOnBoard>();
        SingletonHolder.Instance.Board.GetAllEnemies(ref allEnemies);
        allEnemies.Shuffle();

        DDEnemyActionBase healAction = null;

        for (int i = 0; i < allEnemies.Count; i++)
        {
            DDEnemyOnBoard eob = allEnemies[i];
            if (eob != null && eob.IsDamaged())
            {
                healAction = new DDEnemyAction_HealAlly(healAmount, eob.CurrentLocaton.Coord);
            }
        }

        if (healAction != null)
        {
            actions.Add(healAction);
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
