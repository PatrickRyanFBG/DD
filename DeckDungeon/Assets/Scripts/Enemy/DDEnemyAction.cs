using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class DDEnemyActionBase
{
    public abstract Texture GetIcon();

    public abstract void DisplayInformation(UnityEngine.UI.RawImage image, TMPro.TextMeshProUGUI text);

    public abstract IEnumerator ExecuteAction(DDEnemyOnBoard enemy);

    public abstract string GetDescription();

    public virtual bool HasLocationBasedEffects(ref Vector2Int target)
    {
        return false;
    }
}

public class DDEnemyAction_Move : DDEnemyActionBase
{
    private EMoveDirection moveDirection;

    public DDEnemyAction_Move(EMoveDirection direction)
    {
        moveDirection = direction;
    }

    public override Texture GetIcon()
    {
        switch (moveDirection)
        {
            case EMoveDirection.Up:
                return DDGamePlaySingletonHolder.Instance.EnemyLibrary.SharedActionIconDictionary.Move_Up;
            case EMoveDirection.Right:
                return DDGamePlaySingletonHolder.Instance.EnemyLibrary.SharedActionIconDictionary.Move_Right;
            case EMoveDirection.Down:
                return DDGamePlaySingletonHolder.Instance.EnemyLibrary.SharedActionIconDictionary.Move_Down;
            case EMoveDirection.Left:
                return DDGamePlaySingletonHolder.Instance.EnemyLibrary.SharedActionIconDictionary.Move_Left;
        }

        return null;
    }

    public override IEnumerator ExecuteAction(DDEnemyOnBoard enemy)
    {
        yield return DDGamePlaySingletonHolder.Instance.Board.MoveEnemy(enemy, moveDirection, 1, false);
    }

    public override void DisplayInformation(RawImage image, TextMeshProUGUI text)
    {
        text.enabled = false;
        image.texture = GetIcon();
        image.enabled = true;
    }

    public override string GetDescription()
    {
        return "This enemy will attempt to move " + moveDirection.ToString();
    }

    private static bool CanMoveToSpot(int turnNumber, Vector2Int checkLoc)
    {
        if (checkLoc.x < 0 || checkLoc.x > DDGamePlaySingletonHolder.Instance.Board.ColumnCountIndex ||
            checkLoc.y < 0 || checkLoc.y > DDGamePlaySingletonHolder.Instance.Board.RowCountIndex)
        {
            return false;
        }

        DDEnemyOnBoard eob = DDGamePlaySingletonHolder.Instance.Board.GetEnemyAtLocation(checkLoc);

        bool canMove = false;

        if (eob != null)
        {
            // If we are after the one we are moving to
            if (turnNumber > eob.TurnNumber && eob.IsPlanningToMove())
            {
                canMove = true;
            }
        }
        else
        {
            // The spot is empty

            // TODO :: We need to check if two enemies are going to move into the same spot?
            // Maybe we put a flag into the location
            canMove = true;
        }

        return canMove;
    }

    private static bool MoveRandomlyLeftOrRight(DDEnemyOnBoard actingEnemy, ref EMoveDirection dir)
    {
        bool canMoveToGoal = false;

        EMoveDirection rand = Random.Range(0, 2) == 0 ? EMoveDirection.Right : EMoveDirection.Left;
        Vector2Int checkLoc = actingEnemy.CurrentLocaton.Coord + (rand == EMoveDirection.Right ? Vector2Int.right : Vector2Int.left);

        if (CanMoveToSpot(actingEnemy.TurnNumber, checkLoc))
        {
            canMoveToGoal = true;
            dir = rand;
        }
        else
        {
            rand = (EMoveDirection)(((int)rand + 2) % 4);
            checkLoc = actingEnemy.CurrentLocaton.Coord + (rand == EMoveDirection.Right ? Vector2Int.right : Vector2Int.left);

            if (CanMoveToSpot(actingEnemy.TurnNumber, checkLoc))
            {
                canMoveToGoal = true;
                dir = rand;
            }
        }

        return canMoveToGoal;
    }

    public static DDEnemyAction_Move CalculateBestMove(DDEnemyOnBoard actingEnemy, EMoveDirection preference, bool attacking)
    {
        bool canMoveToGoal = false;
        EMoveDirection goalDirection = EMoveDirection.Down;

        if (attacking)
        {
            if (DDGamePlaySingletonHolder.Instance.Player.IsLaneArmored(actingEnemy.CurrentLocaton.Coord.x))
            {
                canMoveToGoal = MoveRandomlyLeftOrRight(actingEnemy, ref goalDirection);
            }

            if (canMoveToGoal)
            {
                return new DDEnemyAction_Move(goalDirection);
            }
        }

        switch (preference)
        {
            case EMoveDirection.Up:
                Vector2Int checkLoc = actingEnemy.CurrentLocaton.Coord + Vector2Int.up;
                if (CanMoveToSpot(actingEnemy.TurnNumber, checkLoc))
                {
                    return new DDEnemyAction_Move(EMoveDirection.Up);
                }
                break;
            case EMoveDirection.Right:
                break;
            case EMoveDirection.Down:
                checkLoc = actingEnemy.CurrentLocaton.Coord + Vector2Int.down;
                if (CanMoveToSpot(actingEnemy.TurnNumber, checkLoc))
                {
                    return new DDEnemyAction_Move(EMoveDirection.Down);
                }
                break;
            case EMoveDirection.Left:
                break;
            default:
                break;
        }

        canMoveToGoal = MoveRandomlyLeftOrRight(actingEnemy, ref goalDirection);
        if (canMoveToGoal)
        {
            return new DDEnemyAction_Move(goalDirection);
        }
        else
        {
            return null;
        }

    }
}

public class DDEnemyAction_Attack : DDEnemyActionBase
{
    private int damage;

    public DDEnemyAction_Attack(int dam)
    {
        damage = dam;
    }

    public override void DisplayInformation(UnityEngine.UI.RawImage image, TMPro.TextMeshProUGUI text)
    {
        text.text = damage.ToString();
        text.enabled = true;

        image.texture = GetIcon();
        image.enabled = true;
    }

    public override IEnumerator ExecuteAction(DDEnemyOnBoard enemy)
    {
        GameObject attackPrefab = GameObject.Instantiate(enemy.AttackPrefab, enemy.transform.position + enemy.CurrentLocaton.transform.up * .2f, Quaternion.identity);
        Vector3 goal = attackPrefab.transform.position - (enemy.CurrentLocaton.transform.forward * (1.5f * (enemy.CurrentLocaton.Coord.y + 1)));
        attackPrefab.transform.DOMove(goal, 1);

        yield return new WaitForSeconds(1f);

        int totalDamage = damage;
        totalDamage += enemy.GetAffixValue(EAffixType.Expertise);
        totalDamage += DDGamePlaySingletonHolder.Instance.Board.GetMeleeRangedBonus(enemy.CurrentEnemy.RangeType, enemy.CurrentLocaton.Coord.y);
        int leftOverDamage = DDGamePlaySingletonHolder.Instance.Player.DealDamageInLane(totalDamage, (int)enemy.CurrentLocaton.Coord.x);

        if (leftOverDamage > 0)
        {
            DDGamePlaySingletonHolder.Instance.Dungeon.DoDamage(leftOverDamage);
        }

        GameObject.Destroy(attackPrefab);

        yield return new WaitForSeconds(0.1f);
    }

    public override Texture GetIcon()
    {
        return DDGamePlaySingletonHolder.Instance.EnemyLibrary.SharedActionIconDictionary.Attack_Melee;
    }

    public override string GetDescription()
    {
        return "This enemy will deal " + damage + " to you";
    }
}

public class DDEnemyAction_SpawnEnemy : DDEnemyActionBase
{
    private DDEnemyBase enemyToSpawn;
    private Vector2Int atLocation;
    private Texture icon;

    public DDEnemyAction_SpawnEnemy(DDEnemyBase enemyToSpawn, Vector2Int atLocation, Texture icon)
    {
        this.enemyToSpawn = enemyToSpawn;
        this.atLocation = atLocation;
        this.icon = icon;
    }

    public override void DisplayInformation(RawImage image, TextMeshProUGUI text)
    {
        text.enabled = false;
        image.texture = GetIcon();
        image.enabled = true;
    }

    public override IEnumerator ExecuteAction(DDEnemyOnBoard enemy)
    {
        DDGamePlaySingletonHolder.Instance.Board.SpawnEnemy(atLocation.x, atLocation.y, enemyToSpawn);

        yield return new WaitForSeconds(1f);
    }

    public override Texture GetIcon()
    {
        return icon;
    }

    public override bool HasLocationBasedEffects(ref Vector2Int target)
    {
        target = atLocation;
        return true;
    }

    public override string GetDescription()
    {
        return "This enemy will attempt to spawn a " + enemyToSpawn.EnemyName + " on an empty location";
    }
}

public class DDEnemyAction_HealAlly : DDEnemyActionBase
{
    private int healAmount;
    private Vector2Int targetLocation;

    public DDEnemyAction_HealAlly(int healAmount, Vector2Int targetLocation)
    {
        this.healAmount = healAmount;
        this.targetLocation = targetLocation;
    }

    public override void DisplayInformation(UnityEngine.UI.RawImage image, TMPro.TextMeshProUGUI text)
    {
        text.text = healAmount.ToString();
        text.enabled = true;

        image.texture = GetIcon();
        image.enabled = true;
    }

    public override IEnumerator ExecuteAction(DDEnemyOnBoard enemy)
    {
        DDEnemyOnBoard target = DDGamePlaySingletonHolder.Instance.Board.GetEnemyAtLocation(targetLocation);
        if (target != null)
        {
            target.DoHeal(healAmount);
        }

        yield return new WaitForSeconds(1f);
    }

    public override Texture GetIcon()
    {
        return DDGamePlaySingletonHolder.Instance.EnemyLibrary.SharedActionIconDictionary.Action_Heal;
    }

    public override bool HasLocationBasedEffects(ref Vector2Int target)
    {
        target = targetLocation;
        return true;
    }

    public override string GetDescription()
    {
        return "This enemy will attempt to heal a specific location for " + healAmount;
    }
}

public class DDEnemyAction_ModifyAffix : DDEnemyActionBase
{
    private EAffixType affix;
    private int buffAmount;
    private bool shouldSetValue;
    private Vector2Int? targetLocation;

    public DDEnemyAction_ModifyAffix(EAffixType affix, int buffAmount, bool shouldSetValue, Vector2Int? targetLocation = null)
    {
        this.affix = affix;
        this.buffAmount = buffAmount;
        this.shouldSetValue = shouldSetValue;
        this.targetLocation = targetLocation;
    }

    public override void DisplayInformation(UnityEngine.UI.RawImage image, TMPro.TextMeshProUGUI text)
    {
        if (buffAmount != 0)
        {
            text.text = buffAmount.ToString();
            text.enabled = true;
        }
        else if (text.enabled)
        {
            text.enabled = false;
        }

        image.texture = GetIcon();
        image.enabled = true;
    }

    public override IEnumerator ExecuteAction(DDEnemyOnBoard enemy)
    {
        if (targetLocation == null)
        {
            enemy.ModifyAffix(affix, buffAmount, shouldSetValue);
        }
        else
        {
            DDEnemyOnBoard target = DDGamePlaySingletonHolder.Instance.Board.GetEnemyAtLocation(targetLocation.Value);
            if (target != null)
            {
                target.ModifyAffix(affix, buffAmount, shouldSetValue);
            }
        }

        yield return new WaitForSeconds(1f);
    }

    public override Texture GetIcon()
    {
        return DDGlobalManager.Instance.AffixLibrary.GetAffixByType(affix).Image;
    }

    public override bool HasLocationBasedEffects(ref Vector2Int target)
    {
        if (targetLocation == null)
        {
            return false;
        }

        target = targetLocation.Value;
        return true;
    }

    public override string GetDescription()
    {
        if (targetLocation == null)
        {
            return "This enemy will buff their own " + affix.ToString() + " for " + buffAmount;
        }
        else
        {
            return "This enemy will attempt to buff " + affix.ToString() + " at a location for " + buffAmount;
        }
    }
}

public class DDEnemyAction_AddCardTo : DDEnemyActionBase
{
    private int cardAmount;
    private DDCardBase cardToAdd;
    private ECardLocation location;

    public DDEnemyAction_AddCardTo(int amount, DDCardBase card, ECardLocation toLocation)
    {
        cardAmount = amount;
        cardToAdd = card;
        location = toLocation;
    }

    public override void DisplayInformation(RawImage image, TextMeshProUGUI text)
    {
        text.text = cardAmount.ToString();
        text.enabled = true;

        image.texture = GetIcon();
        image.enabled = true;
    }

    public override IEnumerator ExecuteAction(DDEnemyOnBoard enemy)
    {
        for (int i = 0; i < cardAmount; i++)
        {
            switch (location)
            {
                case ECardLocation.Deck:
                    break;
                case ECardLocation.Hard:
                    break;
                case ECardLocation.Discard:
                    yield return DDGamePlaySingletonHolder.Instance.Player.AddCardToDiscard(cardToAdd);
                    break;
            }
            yield return new WaitForSeconds(0.05f);
        }
    }

    public override string GetDescription()
    {
        return "This enemy will add " + cardAmount + " " + cardToAdd.Name + " to your " + location.ToString() + ".";
    }

    public override Texture GetIcon()
    {
        return DDGamePlaySingletonHolder.Instance.EnemyLibrary.SharedActionIconDictionary.Action_AddCard;
    }
}

public class DDEnemyAction_LockRandomCard : DDEnemyActionBase
{
    public override void DisplayInformation(RawImage image, TextMeshProUGUI text)
    {
        throw new System.NotImplementedException();
    }

    public override IEnumerator ExecuteAction(DDEnemyOnBoard enemy)
    {
        throw new System.NotImplementedException();
    }

    public override string GetDescription()
    {
        throw new System.NotImplementedException();
    }

    public override Texture GetIcon()
    {
        return DDGamePlaySingletonHolder.Instance.EnemyLibrary.SharedActionIconDictionary.Action_LockCard;
    }
}
