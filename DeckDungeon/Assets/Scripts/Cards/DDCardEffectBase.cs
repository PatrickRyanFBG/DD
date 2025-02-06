using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DDCardEffectBase
{
    [SerializeField] protected ETargetType targetType;
    public abstract IEnumerator ExecuteEffect(List<DDSelection> selections);
}

public class DDCardEffectDamageEnemy : DDCardEffectBase
{
    [SerializeField] protected int damage;

    [SerializeField] protected ERangeType rangeType = ERangeType.None;

    public override IEnumerator ExecuteEffect(List<DDSelection> selections)
    {
        List<DDEnemyOnBoard> allEnemies = new List<DDEnemyOnBoard>();

        for (int i = 0; i < selections.Count; i++)
        {
            selections[i].FillEnemyList(ref allEnemies);
        }

        for (int i = 0; i < allEnemies.Count; i++)
        {
            DDEnemyOnBoard enemy = allEnemies[i];

            if (enemy)
            {
                DDGamePlaySingletonHolder.Instance.Player.DealDamageToEnemy(damage, rangeType, enemy);
            }
        }

        yield return null;
    }
}

public class DDCardEffectApplyAffixToPlayer : DDCardEffectBase
{
    [SerializeField] protected EAffixType affixType;

    [SerializeField] protected int amount;

    [SerializeField] protected bool shouldSetValue;

    public override IEnumerator ExecuteEffect(List<DDSelection> selections)
    {
        yield return null;
        
        DDGamePlaySingletonHolder.Instance.Player.ModifyAffix(affixType, amount, shouldSetValue);
        
        yield return null;
    }
}

public class DDCardEffectApplyAffixToEnemy : DDCardEffectBase
{
    [SerializeField] protected EAffixType affixType;

    [SerializeField] protected int amount;

    [SerializeField] protected bool shouldSetValue;

    public override IEnumerator ExecuteEffect(List<DDSelection> selections)
    {
        List<DDEnemyOnBoard> allEnemies = new List<DDEnemyOnBoard>();

        for (int i = 0; i < selections.Count; i++)
        {
            selections[i].FillEnemyList(ref allEnemies);
        }

        for (int i = 0; i < allEnemies.Count; i++)
        {
            DDEnemyOnBoard enemy = allEnemies[i];

            if (enemy)
            {
                enemy.ModifyAffix(affixType, amount, shouldSetValue);
            }
        }
        
        yield return null;
    }
}