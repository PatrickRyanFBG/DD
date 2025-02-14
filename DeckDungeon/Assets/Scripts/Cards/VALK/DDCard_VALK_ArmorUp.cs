using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DDCard_VALK_ArmorUp : DDCard_VALKBase
{
    [Header("Armor Up")]
    [SerializeField]
    private int armorAmount;

    protected override IEnumerator Execute(List<DDSelection> selections)
    {
        yield return base.Execute(selections);

        DDColumn col = selections[0] as DDColumn;

        //DDGamePlaySingletonHolder.Instance.Player.AddArmorToLane(armorAmount, col.Index);

        yield return null;
    }
}
