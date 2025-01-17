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

    public virtual bool HasLocationBasedEffects(ref Vector2 target)
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

    private static bool CanMoveToSpot(int turnNumber, Vector2 checkLoc)
    {
        if(checkLoc.x < 0 || checkLoc.x > DDGamePlaySingletonHolder.Instance.Board.ColumnCountIndex ||
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
        Vector2 checkLoc = actingEnemy.CurrentLocaton.Coord + (rand == EMoveDirection.Right ? Vector2.right : Vector2.left);

        if (CanMoveToSpot(actingEnemy.TurnNumber, checkLoc))
        {
            canMoveToGoal = true;
            dir = rand;
        }
        else
        {
            rand = (EMoveDirection)(((int)rand + 2) % 4);
            checkLoc = actingEnemy.CurrentLocaton.Coord + (rand == EMoveDirection.Right ? Vector2.right : Vector2.left);

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

        if(attacking)
        {
            if(DDGamePlaySingletonHolder.Instance.Player.IsLaneArmored(actingEnemy.CurrentLocaton.Coord.x))
            {
                canMoveToGoal = MoveRandomlyLeftOrRight(actingEnemy, ref goalDirection);
            }

            if(canMoveToGoal)
            {
                return new DDEnemyAction_Move(goalDirection);
            }
        }

        switch (preference)
        {
            case EMoveDirection.Up:
                Vector2 checkLoc = actingEnemy.CurrentLocaton.Coord + Vector2.up;
                if (CanMoveToSpot(actingEnemy.TurnNumber, checkLoc))
                {
                    return new DDEnemyAction_Move(EMoveDirection.Up);
                }
                break;
            case EMoveDirection.Right:
                break;
            case EMoveDirection.Down:
                checkLoc = actingEnemy.CurrentLocaton.Coord + Vector2.down;
                if(CanMoveToSpot(actingEnemy.TurnNumber, checkLoc))
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

        int leftOverDamage = DDGamePlaySingletonHolder.Instance.Player.DealDamageInLane(damage + enemy.Dexterity, (int)enemy.CurrentLocaton.Coord.x);

        if(leftOverDamage > 0)
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
    private Vector2 atLocation;
    private Texture icon;

    public DDEnemyAction_SpawnEnemy(DDEnemyBase enemyToSpawn, Vector2 atLocation, Texture icon)
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

    public override bool HasLocationBasedEffects(ref Vector2 target)
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
    private Vector2 targetLocation;

    public DDEnemyAction_HealAlly(int healAmount, Vector2 targetLocation)
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
        if(target != null)
        {
            target.DoHeal(healAmount);
        }

        yield return new WaitForSeconds(1f);
    }

    public override Texture GetIcon()
    {
        return DDGamePlaySingletonHolder.Instance.EnemyLibrary.SharedActionIconDictionary.Action_Heal;
    }

    public override bool HasLocationBasedEffects(ref Vector2 target)
    {
        target = targetLocation;
        return true;
    }

    public override string GetDescription()
    {
        return "This enemy will attempt to heal a specific location for " + healAmount;
    }
}

public class DDEnemyAction_BuffDexterity : DDEnemyActionBase
{
    private int amount;

    public DDEnemyAction_BuffDexterity(int amount)
    {
        this.amount = amount;
    }

    public override void DisplayInformation(UnityEngine.UI.RawImage image, TMPro.TextMeshProUGUI text)
    {
        text.text = amount.ToString();
        text.enabled = true;

        image.texture = GetIcon();
        image.enabled = true;
    }

    public override IEnumerator ExecuteAction(DDEnemyOnBoard enemy)
    {
        // TODO
        // Something with particle effects to show some feedback
        enemy.GainDexterity(amount);
        
        yield return new WaitForSeconds(0.1f);
    }

    public override Texture GetIcon()
    {
        return DDGamePlaySingletonHolder.Instance.EnemyLibrary.SharedActionIconDictionary.Action_GainDexterity;
    }

    public override string GetDescription()
    {
        return "This enemy will gain " + amount + " of dexterity (attacks increased by total dexterity)";
    }
}

public class DDEnemyAction_BuffDexterityAlly : DDEnemyActionBase
{
    private int buffAmount;
    private Vector2 targetLocation;

    public DDEnemyAction_BuffDexterityAlly(int buffAmount, Vector2 targetLocation)
    {
        this.buffAmount = buffAmount;
        this.targetLocation = targetLocation;
    }

    public override void DisplayInformation(UnityEngine.UI.RawImage image, TMPro.TextMeshProUGUI text)
    {
        text.text = buffAmount.ToString();
        text.enabled = true;

        image.texture = GetIcon();
        image.enabled = true;
    }

    public override IEnumerator ExecuteAction(DDEnemyOnBoard enemy)
    {
        DDEnemyOnBoard target = DDGamePlaySingletonHolder.Instance.Board.GetEnemyAtLocation(targetLocation);
        if (target != null)
        {
            target.GainDexterity(buffAmount);
        }

        yield return new WaitForSeconds(1f);
    }

    public override Texture GetIcon()
    {
        return DDGamePlaySingletonHolder.Instance.EnemyLibrary.SharedActionIconDictionary.Action_GainDexterity;
    }

    public override bool HasLocationBasedEffects(ref Vector2 target)
    {
        target = targetLocation;
        return true;
    }

    public override string GetDescription()
    {
        return "This enemy will attempt to buff dexterity a specific location for " + buffAmount;
    }
}

public class DDEnemyAction_GainArmor : DDEnemyActionBase
{
    private int armorAmount;

    public DDEnemyAction_GainArmor(int amount)
    {
        armorAmount = amount;
    }
    
    public override void DisplayInformation(RawImage image, TextMeshProUGUI text)
    {
        text.text = armorAmount.ToString();
        text.enabled = true;

        image.texture = GetIcon();
        image.enabled = true;
    }

    public override IEnumerator ExecuteAction(DDEnemyOnBoard enemy)
    {
        // TODO
        // Something with particle effects to show some feedback
        enemy.SetArmor(armorAmount);

        yield return new WaitForSeconds(0.1f);
    }

    public override string GetDescription()
    {
        return "This enemy will dawn " + armorAmount + " armor";
    }

    public override Texture GetIcon()
    {
        return DDGamePlaySingletonHolder.Instance.EnemyLibrary.SharedActionIconDictionary.Action_GainArmor;
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
