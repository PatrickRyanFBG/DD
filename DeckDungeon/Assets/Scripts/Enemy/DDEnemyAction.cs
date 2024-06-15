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
                return SingletonHolder.Instance.EnemyLibrary.SharedActionIconDictionary.Move_Up;
            case EMoveDirection.Right:
                return SingletonHolder.Instance.EnemyLibrary.SharedActionIconDictionary.Move_Right;
            case EMoveDirection.Down:
                return SingletonHolder.Instance.EnemyLibrary.SharedActionIconDictionary.Move_Down;
            case EMoveDirection.Left:
                return SingletonHolder.Instance.EnemyLibrary.SharedActionIconDictionary.Move_Left;
        }

        return null;
    }

    public override IEnumerator ExecuteAction(DDEnemyOnBoard enemy)
    {
        SingletonHolder.Instance.Board.MoveEnemy(enemy, moveDirection, 1, false);

        yield return new WaitForSeconds(1f);
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
        GameObject attackPrefab = GameObject.Instantiate(enemy.AttackPrefab, enemy.transform.position, Quaternion.identity);
        Vector3 goal = attackPrefab.transform.position;
        goal.z = -4;
        attackPrefab.transform.DOMove(goal, 1);

        yield return new WaitForSeconds(1f);

        SingletonHolder.Instance.Dungeon.DoDamage(damage);

        GameObject.Destroy(attackPrefab);

        yield return new WaitForSeconds(0.1f);
    }

    public override Texture GetIcon()
    {
        return SingletonHolder.Instance.EnemyLibrary.SharedActionIconDictionary.Attack_Melee;
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
        SingletonHolder.Instance.Board.SpawnEnemy(atLocation.x, atLocation.y, enemyToSpawn);

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
        DDEnemyOnBoard target = SingletonHolder.Instance.Board.GetEnemyAtLocation(targetLocation);
        if(target != null)
        {
            target.DoHeal(healAmount);
        }

        yield return new WaitForSeconds(1f);
    }

    public override Texture GetIcon()
    {
        return SingletonHolder.Instance.EnemyLibrary.SharedActionIconDictionary.Attack_Heal;
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
