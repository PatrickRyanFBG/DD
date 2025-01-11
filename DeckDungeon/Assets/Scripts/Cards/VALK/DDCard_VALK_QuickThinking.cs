using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDCard_VALK_QuickThinking : DDCard_VALKBase
{
    [Header("Quick Thinking")]
    [SerializeField]
    private int drawAmount;

    public override IEnumerator ExecuteCard(List<DDSelection> selections)
    {
        yield return base.ExecuteCard(selections);

        for (int i = 0; i < drawAmount; i++)
        {
            DDGamePlaySingletonHolder.Instance.Player.DrawACard();
            yield return new WaitForSeconds(.25f);
        }
    }
}
