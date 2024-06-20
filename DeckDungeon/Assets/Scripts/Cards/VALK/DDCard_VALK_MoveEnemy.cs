using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DDCard_VALK_MoveEnemy : DDCard_VALKBase
{
    [Header("Valkyrie")]
    [SerializeField]
    private EMoveDirection direction;

    [SerializeField]
    private int amount;

    public override IEnumerator ExecuteCard(List<DDSelection> selections)
    {
        // Should put this into a helperf unction somewere
        List<DDEnemyOnBoard> allEnemies = new List<DDEnemyOnBoard>();

        for (int i = 0; i < selections.Count; i++)
        {
            selections[i].FillEnemyList(ref allEnemies);
        }

        for (int i = 0; i < allEnemies.Count; i++)
        {
            DDEnemyOnBoard enemy = allEnemies[i];
            if (enemy != null && !enemy.CurrentEnemy.Immovable)
            {
                SingletonHolder.Instance.Board.MoveEnemy(enemy, direction, amount, true);
            }

            yield return null;
        }

        SingletonHolder.Instance.Player.AddToMomentum(momentumGain);

        yield return null;
    }

    public override bool IsSelectionValid(DDSelection selection, int targetIndex)
    {
        if(targets[targetIndex].TargetType == Target.ETargetType.Enemy)
        {
            DDEnemyOnBoard eob = selection as DDEnemyOnBoard;
            if (eob)
            {
                return !eob.CurrentEnemy.Immovable;   
            }
        }
        return true;
    }
}
