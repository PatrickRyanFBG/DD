using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DDCardEffectBase
{
    [SerializeField] protected ETargetType targetType;
    public ETargetType TargetType => targetType;

    [SerializeField] protected bool differentTarget;
    public bool DifferentTarget => differentTarget;

    [SerializeField] protected bool useLastTarget;
    public bool UseLastTarget => useLastTarget;

    [SerializeField] private DDCardTargetInfo targetInfo;
    public DDCardTargetInfo TargetInfo => targetInfo;
    
    public virtual IEnumerator ExecuteEffect(DDSelection selection, DDCardBase card)
    {
        yield return null;
    }

    protected void GetEnemies(DDSelection selection, ref List<DDEnemyOnBoard> allEnemies)
    {
        if (useLastTarget)
        {
            DDEnemyOnBoard eob = selection as DDEnemyOnBoard;

            if (eob)
            {
                switch (targetType)
                {
                    // Need to figure this out. This is like "target the enemy and drop some shit at the ground"
                    case ETargetType.Location:
                        break;
                    case ETargetType.Row:
                        DDRow row = DDGamePlaySingletonHolder.Instance.Board.GetRow(eob.CurrentLocaton.Coord.y);
                        row.FillEnemyList(ref allEnemies);
                        break;
                    case ETargetType.Column:
                        DDColumn column =
                            DDGamePlaySingletonHolder.Instance.Board.GetColumn(eob.CurrentLocaton.Coord.x);
                        column.FillEnemyList(ref allEnemies);
                        break;
                }
            }
        }
        else
        {
            selection.FillEnemyList(ref allEnemies);
        }
    }
}

public class DDCardEffectMoveEnemy : DDCardEffectBase
{
    [SerializeField] private EMoveDirection direction;

    [SerializeField] private int amount;

    public override IEnumerator ExecuteEffect(DDSelection selection, DDCardBase card)
    {
        // Should put this into a helperf unction somewere
        List<DDEnemyOnBoard> allEnemies = new List<DDEnemyOnBoard>();
        GetEnemies(selection, ref allEnemies);

        for (int i = 0; i < allEnemies.Count; i++)
        {
            DDEnemyOnBoard enemy = allEnemies[i];
            if (enemy && !enemy.CurrentEnemy.Immovable)
            {
                yield return DDGamePlaySingletonHolder.Instance.Board.MoveEnemy(enemy, direction, amount, true);
            }
        }

        yield return null;
    }
}

public class DDCardEffectDamageEnemy : DDCardEffectBase
{
    [SerializeField] protected int damage;

    public override IEnumerator ExecuteEffect(DDSelection selection, DDCardBase card)
    {
        List<DDEnemyOnBoard> allEnemies = new List<DDEnemyOnBoard>();
        GetEnemies(selection, ref allEnemies);

        for (int i = 0; i < allEnemies.Count; i++)
        {
            DDEnemyOnBoard enemy = allEnemies[i];

            if (enemy)
            {
                if (card.RangeType == ERangeType.Ranged)
                {
                    DDGlobalManager.Instance.ClipLibrary.Ranged.PlayNow();
                }
                else if (card.RangeType == ERangeType.Melee)
                {
                    DDGlobalManager.Instance.ClipLibrary.Melee.PlayNow();
                }
                else
                {
                    
                }
                
                DDGamePlaySingletonHolder.Instance.Player.DealDamageToEnemy(damage, card.RangeType, enemy, true);
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

    public override IEnumerator ExecuteEffect(DDSelection selection, DDCardBase card)
    {
        DDGamePlaySingletonHolder.Instance.Player.ModifyAffix(affixType, amount, shouldSetValue);

        yield return null;
    }
}

public class DDCardEffectApplyAffixToColumn : DDCardEffectBase
{
    [SerializeField] protected EAffixType affixType;

    [SerializeField] protected int amount;

    [SerializeField] protected bool shouldSetValue;

    public override IEnumerator ExecuteEffect(DDSelection selection, DDCardBase card)
    {
        int laneNumber = 0;

        switch (selection)
        {
            case DDEnemyOnBoard eob:
                laneNumber = eob.CurrentLocaton.Coord.x;
                break;
            case DDLocation loc:
                laneNumber = loc.Coord.x;
                break;
            case DDColumn column:
                laneNumber = column.Index;
                break;
        }
        
        DDGamePlaySingletonHolder.Instance.Player.ModifyLaneAffix(affixType, amount, laneNumber, shouldSetValue);

        yield return null;
    }
}

public class DDCardEffectApplyAffixToEnemy : DDCardEffectBase
{
    [SerializeField] protected EAffixType affixType;

    [SerializeField] protected int amount;

    [SerializeField] protected bool shouldSetValue;

    public override IEnumerator ExecuteEffect(DDSelection selection, DDCardBase card)
    {
        List<DDEnemyOnBoard> allEnemies = new List<DDEnemyOnBoard>();
        GetEnemies(selection, ref allEnemies);

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

public class DDCardEffectDrawCard : DDCardEffectBase
{
    [SerializeField] protected int amount;

    public override IEnumerator ExecuteEffect(DDSelection selection, DDCardBase card)
    {
        for (int i = 0; i < amount; i++)
        {
            yield return DDGamePlaySingletonHolder.Instance.Player.DrawACard();
        }
    }
}