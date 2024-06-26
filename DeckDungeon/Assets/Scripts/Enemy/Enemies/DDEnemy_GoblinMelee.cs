using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DDEnemy_GoblinMelee : DDEnemyBase
{
    [SerializeField]
    private int damage;

    [SerializeField]
    private int dexGain;

    public override List<DDEnemyActionBase> CalculateActions(int number, DDEnemyOnBoard actingEnemy)
    {
        List<DDEnemyActionBase> actions = new List<DDEnemyActionBase>(number);

        DDEnemyActionBase buffAction = null;

        // If no dex higher chance to buff
        if ((actingEnemy.Dexterity <= 0 && Random.Range(0, 10) < 5) ||
            (actingEnemy.Dexterity > 0 && Random.Range(0, 10) < 3))
        {
            buffAction = new DDEnemyAction_BuffDexterity(dexGain);
        }

        if (buffAction != null)
        {
            EMoveDirection randomDirection = (EMoveDirection)Random.Range(0, 4);
            actions.Add(new DDEnemyAction_Move(randomDirection));

            actions.Add(buffAction);
        }
        else
        {
            if (Random.Range(0, 4) <= 2)
            {
                // We want to move down
                if (actingEnemy.CurrentLocaton.Coord.y > 0)
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
                        actions.Add(new DDEnemyAction_Move(EMoveDirection.Down));
                    }
                }
                //EMoveDirection randomDirection = (EMoveDirection)Random.Range(0, 4);
                //actions.Add(new DDEnemyAction_Move(randomDirection));
            }
            else
            {
                actions.Add(new DDEnemyAction_Attack(damage));
            }

            actions.Add(new DDEnemyAction_Attack(damage));
        }

        return actions;
    }
}