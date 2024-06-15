using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DDEnemy_GoblinBomb : DDEnemyBase
{
    [SerializeField]
    private int damage;

    public override List<DDEnemyActionBase> CalculateActions(int number, DDEnemyOnBoard actingEnemy)
    {
        List<DDEnemyActionBase> actions = new List<DDEnemyActionBase>(1);

        actions.Add(new DDEnemyAction_Explode(damage));

        return actions;
    }
}

public class DDEnemyAction_Explode : DDEnemyActionBase
{
    private int damage;

    public DDEnemyAction_Explode(int damage)
    {
        this.damage = damage;
    }

    public override void DisplayInformation(RawImage image, TextMeshProUGUI text)
    {
        text.text = damage.ToString();
        text.enabled = true;

        image.texture = GetIcon();
        image.enabled = true;
    }

    public override IEnumerator ExecuteAction(DDEnemyOnBoard enemy)
    {
        SingletonHolder.Instance.Dungeon.DoDamage(damage);
        enemy.DoDamage(damage);

        yield return new WaitForSeconds(1f);
    }

    public override string GetDescription()
    {
        return "This enemy will destroy it self and deal " + damage + " to you";
    }

    public override Texture GetIcon()
    {
        return SingletonHolder.Instance.EnemyLibrary.SharedActionIconDictionary.Attack_Explode;
    }
}