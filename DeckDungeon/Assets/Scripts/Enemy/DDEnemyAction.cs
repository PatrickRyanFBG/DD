using System;
using LitMotion;
using System.Collections;
using System.Collections.Generic;
using LitMotion.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public abstract class DDEnemyActionBase
{
    public bool HiddenAction = false;

    public abstract Texture GetIcon();

    public virtual void DisplayInformation(UnityEngine.UI.RawImage image, TMPro.TextMeshProUGUI text)
    {
        if (text.enabled)
        {
            text.enabled = false;
        }

        image.texture = GetIcon();
        image.enabled = true;
    }

    public abstract IEnumerator ExecuteAction(DDEnemyOnBoard enemy);

    public abstract string GetDescription();

    public virtual void ShowLocationBasedEffects()
    {
    }
    
    public virtual void HideLocationBasedEffects()
    {
    }
}

public class DDEnemyActionEmpty : DDEnemyActionBase
{
    public override Texture GetIcon()
    {
        throw new NotImplementedException();
    }

    public override void DisplayInformation(RawImage image, TextMeshProUGUI text)
    {
        throw new NotImplementedException();
    }

    public override IEnumerator ExecuteAction(DDEnemyOnBoard enemy)
    {
        yield return null;
    }

    public override string GetDescription()
    {
        throw new NotImplementedException();
    }
}

public class DDEnemyActionMove : DDEnemyActionBase
{
    private EMoveDirection moveDirection;

    public DDEnemyActionMove(EMoveDirection direction)
    {
        moveDirection = direction;
    }

    public override Texture GetIcon()
    {
        switch (moveDirection)
        {
            case EMoveDirection.Up:
                return DDGamePlaySingletonHolder.Instance.EnemyLibrary.SharedActionIconDictionary.MoveUp;
            case EMoveDirection.Right:
                return DDGamePlaySingletonHolder.Instance.EnemyLibrary.SharedActionIconDictionary.MoveRight;
            case EMoveDirection.Down:
                return DDGamePlaySingletonHolder.Instance.EnemyLibrary.SharedActionIconDictionary.MoveDown;
            case EMoveDirection.Left:
                return DDGamePlaySingletonHolder.Instance.EnemyLibrary.SharedActionIconDictionary.MoveLeft;
        }

        return null;
    }

    public override IEnumerator ExecuteAction(DDEnemyOnBoard enemy)
    {
        if (enemy.GetAffixValue(EAffixType.Immobile) > 0)
        {
            enemy.ModifyAffix(EAffixType.Immobile, -1, false);

            yield return null;
        }
        else
        {
            yield return DDGamePlaySingletonHolder.Instance.Board.MoveEnemy(enemy, moveDirection, 1, false);
        }
    }

    public override void DisplayInformation(RawImage image, TextMeshProUGUI text)
    {
        text.enabled = false;
        image.texture = GetIcon();
        image.enabled = true;
    }

    public override string GetDescription()
    {
        return "Attempts to [Move] " + moveDirection.ToString() + ".";
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

        if (eob)
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
        Vector2Int checkLoc = actingEnemy.CurrentLocaton.Coord +
                              (rand == EMoveDirection.Right ? Vector2Int.right : Vector2Int.left);

        if (CanMoveToSpot(actingEnemy.TurnNumber, checkLoc))
        {
            canMoveToGoal = true;
            dir = rand;
        }
        else
        {
            rand = (EMoveDirection)(((int)rand + 2) % 4);
            checkLoc = actingEnemy.CurrentLocaton.Coord +
                       (rand == EMoveDirection.Right ? Vector2Int.right : Vector2Int.left);

            if (CanMoveToSpot(actingEnemy.TurnNumber, checkLoc))
            {
                canMoveToGoal = true;
                dir = rand;
            }
        }

        return canMoveToGoal;
    }

    private static bool MoveRandomlyUpOrDown(DDEnemyOnBoard actingEnemy, ref EMoveDirection dir)
    {
        bool canMoveToGoal = false;

        EMoveDirection rand = Random.Range(0, 2) == 0 ? EMoveDirection.Up : EMoveDirection.Down;
        Vector2Int checkLoc = actingEnemy.CurrentLocaton.Coord +
                              (rand == EMoveDirection.Up ? Vector2Int.up : Vector2Int.down);

        if (CanMoveToSpot(actingEnemy.TurnNumber, checkLoc))
        {
            canMoveToGoal = true;
            dir = rand;
        }
        else
        {
            rand = (EMoveDirection)(((int)rand + 2) % 4);
            checkLoc = actingEnemy.CurrentLocaton.Coord + (rand == EMoveDirection.Up ? Vector2Int.up : Vector2Int.down);

            if (CanMoveToSpot(actingEnemy.TurnNumber, checkLoc))
            {
                canMoveToGoal = true;
                dir = rand;
            }
        }

        return canMoveToGoal;
    }

    public static DDEnemyActionMove CalculateBestMove(DDEnemyOnBoard actingEnemy, EMoveDirection preference,
        bool attacking)
    {
        bool canMoveToGoal = false;
        EMoveDirection goalDirection = preference;

        if (attacking)
        {
            if (DDGamePlaySingletonHolder.Instance.Player.IsLaneArmored(actingEnemy.CurrentLocaton.Coord.x))
            {
                canMoveToGoal = MoveRandomlyLeftOrRight(actingEnemy, ref goalDirection);
            }

            if (canMoveToGoal)
            {
                return new DDEnemyActionMove(goalDirection);
            }
        }

        switch (preference)
        {
            case EMoveDirection.Up:
                Vector2Int checkLoc = actingEnemy.CurrentLocaton.Coord + Vector2Int.up;
                if (CanMoveToSpot(actingEnemy.TurnNumber, checkLoc))
                {
                    return new DDEnemyActionMove(EMoveDirection.Up);
                }

                break;
            case EMoveDirection.Right:
                checkLoc = actingEnemy.CurrentLocaton.Coord + Vector2Int.right;
                if (CanMoveToSpot(actingEnemy.TurnNumber, checkLoc))
                {
                    return new DDEnemyActionMove(EMoveDirection.Right);
                }

                break;
            case EMoveDirection.Down:
                checkLoc = actingEnemy.CurrentLocaton.Coord + Vector2Int.down;
                if (CanMoveToSpot(actingEnemy.TurnNumber, checkLoc))
                {
                    return new DDEnemyActionMove(EMoveDirection.Down);
                }

                break;
            case EMoveDirection.Left:
                checkLoc = actingEnemy.CurrentLocaton.Coord + Vector2Int.left;
                if (CanMoveToSpot(actingEnemy.TurnNumber, checkLoc))
                {
                    return new DDEnemyActionMove(EMoveDirection.Left);
                }

                break;
        }

        if (preference == EMoveDirection.Up || preference == EMoveDirection.Down)
        {
            canMoveToGoal = MoveRandomlyLeftOrRight(actingEnemy, ref goalDirection);
        }
        else if (preference == EMoveDirection.Right || preference == EMoveDirection.Left)
        {
            canMoveToGoal = MoveRandomlyUpOrDown(actingEnemy, ref goalDirection);
        }

        if (canMoveToGoal)
        {
            return new DDEnemyActionMove(goalDirection);
        }
        else
        {
            return null;
        }
    }
}

public class DDEnemyActionAttack : DDEnemyActionBase
{
    protected int damage;
    protected ERangeType rangedType = ERangeType.None;

    public DDEnemyActionAttack(int dam, DDEnemyBase enemy)
    {
        damage = dam;
        rangedType = enemy.RangeType;
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
        Vector3 particleStart = enemy.transform.position + enemy.CurrentLocaton.transform.up * .1f;
        Vector3 particleGoal = DDGamePlaySingletonHolder.Instance.Player.GetLaneAffixVisualPosition(enemy.CurrentLocaton.Coord.x) + enemy.CurrentLocaton.transform.up * .1f;
        particleStart = Vector3.Lerp(particleStart, particleGoal, .2f);

        if (rangedType == ERangeType.Ranged || rangedType == ERangeType.Pure)
        {
            DDGlobalManager.Instance.ClipLibrary.Ranged.PlayNow();
            
            DDEnemyRangeParticle rangePart =
                DDGlobalManager.Instance.ParticleLibrary.GetParticle<DDEnemyRangeParticle>();
            
            rangePart.transform.position = particleStart;
            rangePart.GoalPosition = particleGoal;

            yield return rangePart.Play();
        }
        else if (rangedType == ERangeType.Melee)
        {
            DDGlobalManager.Instance.ClipLibrary.Melee.PlayNow();
            
            DDEnemyMeleeParticle meleePart = DDGlobalManager.Instance.ParticleLibrary.GetParticle<DDEnemyMeleeParticle>();
            meleePart.transform.position = particleGoal;
            
            yield return meleePart.Play();
        }

        yield return new WaitForSeconds(.05f);

        int totalDamage = damage;
        totalDamage += enemy.GetAffixValue(EAffixType.Expertise);
        totalDamage +=
            DDGamePlaySingletonHolder.Instance.Board.GetMeleeRangedBonus(enemy.CurrentEnemy.RangeType,
                enemy.CurrentLocaton.Coord.y);

        DDGamePlaySingletonHolder.Instance.Player.DealDamageInLane(totalDamage, (int)enemy.CurrentLocaton.Coord.x);

        // Retaliate
        int? retaliateNumber =
            DDGamePlaySingletonHolder.Instance.Player.GetLaneAffix(EAffixType.Retaliate, enemy.CurrentLocaton.Coord.x);

        if (retaliateNumber != null)
        {
            enemy.TakeDamage(retaliateNumber.Value, ERangeType.Pure, false);
        }

        yield return new WaitForSeconds(0.05f);
    }

    public override Texture GetIcon()
    {
        switch (rangedType)
        {
            case ERangeType.Ranged:
                return DDGamePlaySingletonHolder.Instance.EnemyLibrary.SharedActionIconDictionary.AttackRanged;
            case ERangeType.Pure:
                return DDGamePlaySingletonHolder.Instance.EnemyLibrary.SharedActionIconDictionary.AttackPure;
            default:
                return DDGamePlaySingletonHolder.Instance.EnemyLibrary.SharedActionIconDictionary.AttackMelee;
        }
    }

    public override string GetDescription()
    {
        return "Deals [" + rangedType + "] " + damage + ".";
    }
}

public class DDEnemyActionSpawnEnemy : DDEnemyActionBase
{
    private DDEnemyBase enemyToSpawn;
    private Vector2Int atLocation;
    private Texture icon;

    public DDEnemyActionSpawnEnemy(DDEnemyBase enemyToSpawn, Vector2Int atLocation, Texture icon)
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

    public override void ShowLocationBasedEffects()
    {
        DDLocation loc = DDGamePlaySingletonHolder.Instance.Board.GetLocation(atLocation);
        loc.Highlight();
        loc.ShowSpawnedEntity(enemyToSpawn.Image);
    }

    public override void HideLocationBasedEffects()
    {
        DDLocation loc = DDGamePlaySingletonHolder.Instance.Board.GetLocation(atLocation);
        loc.Unhighlight();
        loc.HideSpawnedEntity();
    }

    public override string GetDescription()
    {
        return "Spawn a " + enemyToSpawn.EntityName + " at location.";
    }
}

public class DDEnemyActionHealAlly : DDEnemyActionBase
{
    private int healAmount;
    private Vector2Int? targetLocation;

    public DDEnemyActionHealAlly(int healAmount, Vector2Int? targetLocation = null)
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
        DDEnemyOnBoard target = enemy;
        if (targetLocation != null)
        {
            target = DDGamePlaySingletonHolder.Instance.Board.GetEnemyAtLocation(targetLocation.Value);
        }

        if (target)
        {
            target.DoHeal(healAmount);
        }

        yield return new WaitForSeconds(1f);
    }

    public override Texture GetIcon()
    {
        return DDGamePlaySingletonHolder.Instance.EnemyLibrary.SharedActionIconDictionary.ActionHeal;
    }

    
    public override void ShowLocationBasedEffects()
    {
        if (targetLocation != null)
        {
            DDGamePlaySingletonHolder.Instance.Board.GetLocation(targetLocation.Value).Highlight();
        }
    }

    public override void HideLocationBasedEffects()
    {
        if (targetLocation != null)
        {
            DDGamePlaySingletonHolder.Instance.Board.GetLocation(targetLocation.Value).Unhighlight();
        }
    }

    public override string GetDescription()
    {
        if (targetLocation == null)
        {
            return "Heal self for " + healAmount + ".";
        }
        else
        {
            return "Heal at location for " + healAmount + ".";
        }
    }
}

public class DDEnemyActionModifyAffix : DDEnemyActionBase
{
    private EAffixType affix;
    private int buffAmount;
    private bool shouldSetValue;
    private Vector2Int? targetLocation;

    public DDEnemyActionModifyAffix(EAffixType affix, int buffAmount, bool shouldSetValue,
        Vector2Int? targetLocation = null)
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
            if (target)
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

    public override void ShowLocationBasedEffects()
    {
        if (targetLocation != null)
        {
            DDGamePlaySingletonHolder.Instance.Board.GetLocation(targetLocation.Value).Highlight();
        }
    }

    public override void HideLocationBasedEffects()
    {
        if (targetLocation != null)
        {
            DDGamePlaySingletonHolder.Instance.Board.GetLocation(targetLocation.Value).Unhighlight();
        }
    }
    
    public override string GetDescription()
    {
        if (targetLocation == null)
        {
            return "Buffs own " + affix.ToString() + " for " + buffAmount + ".";
        }
        else
        {
            return "Buffs ally's " + affix.ToString() + " at location for " + buffAmount + ".";
        }
    }
}

public class DDEnemyActionAddCardTo : DDEnemyActionBase
{
    private int cardAmount;
    private DDCardBase cardToAdd;
    private ECardLocation location;

    public DDEnemyActionAddCardTo(int amount, DDCardBase card, ECardLocation toLocation)
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
        Vector3 position = DDGamePlaySingletonHolder.Instance.MainCamera.WorldToScreenPoint(enemy.transform.position);

        for (int i = 0; i < cardAmount; i++)
        {
            yield return DDGamePlaySingletonHolder.Instance.Player.AddCardTo(cardToAdd, position, location, false);

            yield return new WaitForSeconds(0.05f);
        }
    }

    public override string GetDescription()
    {
        return "Adds " + cardAmount + " " + cardToAdd.CardName + " to " + location.ToString() + ".";
    }

    public override Texture GetIcon()
    {
        return DDGamePlaySingletonHolder.Instance.EnemyLibrary.SharedActionIconDictionary.ActionAddCard;
    }
}

public class DDEnemyActionLockRandomCard : DDEnemyActionBase
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
        return DDGamePlaySingletonHolder.Instance.EnemyLibrary.SharedActionIconDictionary.ActionLockCard;
    }
}

public class DDEnemyActionBleedAttack : DDEnemyActionBase
{
    private int damage;

    private int bleedAmount;

    public DDEnemyActionBleedAttack(int dam, int bleed)
    {
        damage = dam;
        bleedAmount = bleed;
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
        GameObject attackPrefab = GameObject.Instantiate(enemy.AttackPrefab,
            enemy.transform.position + enemy.CurrentLocaton.transform.up * .2f, Quaternion.identity);
        Vector3 goal = attackPrefab.transform.position -
                       (enemy.CurrentLocaton.transform.forward * (1.5f * (enemy.CurrentLocaton.Coord.y + 1)));
        LMotion.Create(attackPrefab.transform.position, goal, 1).BindToPosition(attackPrefab.transform);

        yield return new WaitForSeconds(1f);

        int totalDamage = damage;
        totalDamage += enemy.GetAffixValue(EAffixType.Expertise);
        totalDamage +=
            DDGamePlaySingletonHolder.Instance.Board.GetMeleeRangedBonus(enemy.CurrentEnemy.RangeType,
                enemy.CurrentLocaton.Coord.y);
        DDGamePlaySingletonHolder.Instance.Player.DealDamageInLane(totalDamage, (int)enemy.CurrentLocaton.Coord.x);

        GameObject.Destroy(attackPrefab);

        // Retaliate
        int? retaliateNumber =
            DDGamePlaySingletonHolder.Instance.Player.GetLaneAffix(EAffixType.Retaliate, enemy.CurrentLocaton.Coord.x);

        if (retaliateNumber != null)
        {
            enemy.TakeDamage(retaliateNumber.Value, ERangeType.Pure, false);
        }

        yield return new WaitForSeconds(0.1f);
    }

    public override Texture GetIcon()
    {
        return DDGamePlaySingletonHolder.Instance.EnemyLibrary.SharedActionIconDictionary.AttackMelee;
    }

    public override string GetDescription()
    {
        return "Applies " + damage + " to you";
    }
}

public class DDEnemyActionModifyPlayerAffix : DDEnemyActionBase
{
    private EAffixType affixType;
    private int amount;

    public DDEnemyActionModifyPlayerAffix(EAffixType type, int amt)
    {
        affixType = type;
        amount = amt;
    }

    public override void DisplayInformation(UnityEngine.UI.RawImage image, TMPro.TextMeshProUGUI text)
    {
        if (amount != 0)
        {
            text.text = amount.ToString();
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
        DDGamePlaySingletonHolder.Instance.Player.ModifyAffix(affixType, amount, false);

        yield return new WaitForSeconds(1f);
    }

    public override Texture GetIcon()
    {
        return DDGlobalManager.Instance.AffixLibrary.GetAffixByType(affixType).Image;
    }

    public override string GetDescription()
    {
        return "Applies " + amount + " " + affixType + " to you.";
    }
}