using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DDCard_VALK_ImproviseWeapon : DDCard_VALKBase
{
    [Header("Improvise Weapon")]
    [SerializeField]
    private int damage;

    public override IEnumerator ExecuteCard(List<DDSelection> selections)
    {
        yield return base.ExecuteCard(selections);

        DDCardInHand card = selections[0] as DDCardInHand;
        DDGamePlaySingletonHolder.Instance.Player.DiscardCard(card);

        yield return new WaitForSeconds(.25f);

        DDEnemyOnBoard enemy = selections[1] as DDEnemyOnBoard;
        enemy.DoDamage(damage);

        yield return new WaitForSeconds(.1f);
    }
}
