using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DDEnemy_GoblinPrince : DDEnemyBase
{
    [SerializeField]
    private int damage;

    [SerializeField]
    private int dexterityBuff;

    [SerializeField]
    private DDEnemyBase meleeGoblin;

    [SerializeField]
    private Texture meleeGoblinIcon;

    public override List<DDEnemyActionBase> CalculateActions(int number, DDEnemyOnBoard actingEnemy)
    {
        List<DDEnemyActionBase> actions = new List<DDEnemyActionBase>(number);

        List<DDEnemyOnBoard> allEnemies = new List<DDEnemyOnBoard>();
        SingletonHolder.Instance.Board.GetAllEnemies(ref allEnemies);

        DDEnemyActionBase summonGoblin = null;

        // Always summon another goblin if prince is there is only one or less other goblin
        if (allEnemies.Count <= 2 || Random.Range(0, 10) < 4)
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
                    summonGoblin = new DDEnemyAction_SpawnEnemy(meleeGoblin, new Vector2(randX, randY), meleeGoblinIcon);
                    break;
                }
            }
        }

        if (summonGoblin != null)
        {
            actions.Add(summonGoblin);
        }
        else
        {
            EMoveDirection randomDirection = (EMoveDirection)Random.Range(0, 4);
            actions.Add(new DDEnemyAction_Move(randomDirection));

            allEnemies.Shuffle();

            DDEnemyActionBase buffAction = null;

            for (int i = 0; i < allEnemies.Count; i++)
            {
                DDEnemyOnBoard eob = allEnemies[i];
                if (eob != null && eob != actingEnemy)
                {
                    buffAction = new DDEnemyAction_BuffDexterityAlly(dexterityBuff, eob.CurrentLocaton.Coord);
                }
            }

            if(buffAction != null)
            {
                actions.Add(buffAction);
            }
            else
            {
                actions.Add(new DDEnemyAction_Attack(damage));
            }
        }

        return actions;
    }
}
