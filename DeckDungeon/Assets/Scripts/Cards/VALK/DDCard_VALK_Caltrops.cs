using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DDCard_VALK_Caltrops : DDCard_VALKBase
{
    [SerializeField]
    private int numberOfTurns;

    [SerializeField]
    private int damage;

    [SerializeField]
    private Texture caltropsImage;

    public override IEnumerator ExecuteCard(List<DDSelection> selections)
    {
        yield return base.ExecuteCard(selections);

        DDLocation loc = selections[0] as DDLocation;
        if (loc != null)
        {
            loc.AddEffect(new DDLocationEffect_Caltrops(numberOfTurns, damage), caltropsImage);
        }

        yield return null;
    }
}

public class DDLocationEffect_Caltrops : DDLocationEffectBase
{
    private int damage;

    public DDLocationEffect_Caltrops(int forTurns, int forDamage) : base(forTurns)
    {
        damage = forDamage;
    }

    public override bool DoEffect(DDLocation loc)
    {
        DDEnemyOnBoard enemy = loc.GetEnemy();

        if (enemy)
        {
            enemy.DoDamage(damage);
        }

        return base.DoEffect(loc);
    }
}