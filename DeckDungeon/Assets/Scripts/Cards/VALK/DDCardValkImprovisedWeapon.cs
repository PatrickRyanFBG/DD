using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DDCardValkImprovisedWeapon : DDCardValkGeneric
{
    [Header("Improvised Weapon")] [SerializeField]
    private int damage;

    public override List<ETargetType> GetTargets()
    {
        if (targets != null) return targets;

        targets = new List<ETargetType>() { ETargetType.PlayerCard, ETargetType.Enemy };
        
        return base.GetTargets();
    }
    
    protected override IEnumerator Execute(List<DDSelection> selections)
    {
        yield return base.Execute(selections);

        DDCardInHand card = selections[0] as DDCardInHand;
        
        yield return DDGamePlaySingletonHolder.Instance.Player.DiscardCard(card);

        yield return new WaitForSeconds(.25f);

        DDEnemyOnBoard enemy = selections[1] as DDEnemyOnBoard;
        DDGamePlaySingletonHolder.Instance.Player.DealDamageToEnemy(damage, rangeType, enemy);

        yield return new WaitForSeconds(.1f);
    }
}